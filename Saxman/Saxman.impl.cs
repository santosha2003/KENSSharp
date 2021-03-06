﻿namespace SonicRetro.KensSharp
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    public static partial class Saxman
    {
        private struct LZSSGraphEdge
        {
            public int cost;
            public int next_node_index;
            public int previous_node_index;
            public int match_length;
            public int match_offset;
        };

        private static void Encode(Stream input, Stream output, bool with_size)
        {
            int input_size = (int)(input.Length - input.Position);
            byte[] input_buffer = new byte[input_size];
            input.Read(input_buffer, 0, input_size);

            long outputInitialPosition = output.Position;
            if (with_size)
            {
                output.Seek(2, SeekOrigin.Current);
            }

            /*
             * Here we create and populate the "LZSS graph":
             * 
             * Each value in the uncompressed file forms a node in this graph.
             * The various edges between these nodes represent LZSS matches.
             * 
             * Using a shortest-path algorithm, these edges can be used to
             * find the optimal combination of matches needed to produce the
             * smallest possible file.
             * 
             * The outputted array only contains one edge per node: the optimal
             * one. This means, in order to produce the smallest file, you just
             * have to traverse the graph from one edge to the next, encoding
             * each match as you go along.
            */

            LZSSGraphEdge[] node_meta_array = new LZSSGraphEdge[input_size + 1];

            // Initialise the array
            node_meta_array[0].cost = 0;
            for (int i = 1; i < input_size + 1; ++i)
                node_meta_array[i].cost = int.MaxValue;

            // Find matches
            for (int i = 0; i < input_size; ++i)
            {
                int max_read_ahead = Math.Min(0xF + 3, input_size - i);
                int max_read_behind = Math.Max(0, i - 0x1000);

                // Search for zero-fill matches
                if (i < 0x1000)
                {
                    for (int k = 0; k < 0xF + 3; ++k)
                    {
                        if (input_buffer[i + k] == 0)
                        {
                            int length = k + 1;

                            // Update this node's optimal edge if this one is better
                            if (length >= 3 && node_meta_array[i + k + 1].cost > node_meta_array[i].cost + 1 + 16)
                            {
                                node_meta_array[i + k + 1].cost = node_meta_array[i].cost + 1 + 16;
                                node_meta_array[i + k + 1].previous_node_index = i;
                                node_meta_array[i + k + 1].match_length = k + 1;
                                node_meta_array[i + k + 1].match_offset = 0xFFF;
                            }
                        }
                        else
                            break;
                    }
                }

                // Search for dictionary matches
                for (int j = i; j-- > max_read_behind;)
                {
                    for (int k = 0; k < max_read_ahead; ++k)
                    {
                        if (input_buffer[i + k] == input_buffer[j + k])
                        {
                            int distance = i - j;
                            int length = k + 1;

                            // Update this node's optimal edge if this one is better
                            if (length >= 3 && node_meta_array[i + k + 1].cost > node_meta_array[i].cost + 1 + 16)
                            {
                                node_meta_array[i + k + 1].cost = node_meta_array[i].cost + 1 + 16;
                                node_meta_array[i + k + 1].previous_node_index = i;
                                node_meta_array[i + k + 1].match_length = k + 1;
                                node_meta_array[i + k + 1].match_offset = j;
                            }
                        }
                        else
                            break;
                    }
                }

                // Do literal match
                // Update this node's optimal edge if this one is better (or the same, since literal matches usually decode faster)
                if (node_meta_array[i + 1].cost >= node_meta_array[i].cost + 1 + 8)
                {
                    node_meta_array[i + 1].cost = node_meta_array[i].cost + 1 + 8;
                    node_meta_array[i + 1].previous_node_index = i;
                    node_meta_array[i + 1].match_length = 0;
                }
            }

            // Reverse the edge link order, so the array can be traversed from start to end, rather than vice versa
            node_meta_array[0].previous_node_index = int.MaxValue;
            node_meta_array[input_size].next_node_index = int.MaxValue;
            for (int node_index = input_size; node_meta_array[node_index].previous_node_index != int.MaxValue; node_index = node_meta_array[node_index].previous_node_index)
                node_meta_array[node_meta_array[node_index].previous_node_index].next_node_index = node_index;

            /*
             * LZSS graph complete
             */

            UInt8_NE_L_OutputBitStream bitStream = new UInt8_NE_L_OutputBitStream(output);
            MemoryStream data = new MemoryStream();

            for (int node_index = 0; node_meta_array[node_index].next_node_index != int.MaxValue; node_index = node_meta_array[node_index].next_node_index)
            {
                int next_index = node_meta_array[node_index].next_node_index;

                if (node_meta_array[next_index].match_length != 0)
                {
                    // Compressed
                    Push(bitStream, false, output, data);
                    int match_offset_adjusted = node_meta_array[next_index].match_offset - 0x12;   // I don't think there's any reason for this, the format's just stupid
                    NeutralEndian.Write1(data, (byte)(match_offset_adjusted & 0xFF));
                    NeutralEndian.Write1(data, (byte)(((match_offset_adjusted & 0xF00) >> 4) | ((node_meta_array[next_index].match_length - 3) & 0x0F)));
                }
                else
                {
                    // Uncompressed
                    Push(bitStream, true, output, data);
                    NeutralEndian.Write1(data, input_buffer[node_index]);
                }
            }

            // Write remaining data (normally we don't flush until we have a full descriptor byte)
            bitStream.Flush(true);
            byte[] dataArray = data.ToArray();
            output.Write(dataArray, 0, dataArray.Length);

            if (with_size)
            {
                ushort size = (ushort)(outputInitialPosition - output.Position - 2);
                output.Seek(outputInitialPosition, SeekOrigin.Begin);
                LittleEndian.Write2(output, size);
            }
        }

        private static void Push(UInt8_NE_L_OutputBitStream bitStream, bool bit, Stream destination, MemoryStream data)
        {
            if (bitStream.Push(bit))
            {
                byte[] bytes = data.ToArray();
                destination.Write(bytes, 0, bytes.Length);
                data.SetLength(0);
            }
        }


        private static void Decode(Stream input, Stream output)
        {
            ushort size = LittleEndian.Read2(input);
            Decode(input, output, size);
        }

        private static void Decode(Stream input, Stream output, long size)
        {
            long end = input.Position + size;
            UInt8_NE_L_InputBitStream bitStream = new UInt8_NE_L_InputBitStream(input);
            List<byte> outputBuffer = new List<byte>();
            while (input.Position < end)
            {
                if (bitStream.Pop())
                {
                    if (input.Position >= end)
                    {
                        break;
                    }

                    outputBuffer.Add(NeutralEndian.Read1(input));
                }
                else
                {
                    if (input.Position >= end)
                    {
                        break;
                    }

                    int offset = NeutralEndian.Read1(input);

                    if (input.Position >= end)
                    {
                        break;
                    }

                    byte count = NeutralEndian.Read1(input);

                    // We've just read 2 bytes: %llllllll %hhhhcccc
                    // offset = %hhhhllllllll + 0x12, count = %cccc + 3
                    offset |= (ushort)((count & 0xF0) << 4);
                    offset += 0x12;
                    offset &= 0xFFF;
                    offset |= (ushort)(outputBuffer.Count & 0xF000);
                    count &= 0xF;
                    count += 3;

                    if (offset >= outputBuffer.Count)
                    {
                        offset -= 0x1000;
                    }

                    outputBuffer.AddRange(new byte[count]);

                    if (offset < 0)
                    {
                        // Zero-fill
                        for (int destinationIndex = outputBuffer.Count - count; destinationIndex < outputBuffer.Count; ++destinationIndex)
                        {
                            outputBuffer[destinationIndex] = 0;
                        }
                    }
                    else
                    {
                        // Dictionary reference
                        if (offset < outputBuffer.Count)
                        {
                            for (int sourceIndex = offset, destinationIndex = outputBuffer.Count - count;
                                destinationIndex < outputBuffer.Count;
                                sourceIndex++, destinationIndex++)
                            {
                                outputBuffer[destinationIndex] = outputBuffer[sourceIndex];
                            }
                        }
                    }
                }
            }

            byte[] bytes = outputBuffer.ToArray();
            output.Write(bytes, 0, bytes.Length);
        }
    }
}
