; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{C6E49E8A-37DE-4CC1-8BD2-74EC835513DA}
AppName=KENSSharp
AppVersion=1.3
;AppVerName=KENSSharp 1.0
AppPublisher=Sonic Retro
AppPublisherURL=http://sonicretro.org/
AppSupportURL=http://sonicretro.org/
AppUpdatesURL=http://sonicretro.org/
DefaultDirName={pf}\KensSharp
AllowNoIcons=yes
OutputBaseFilename=KENSSharpSetup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "KensSharp\bin\Release\KensSharp.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\KensSharp.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\KensSharp.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "Frontend\bin\Release\KensSharpFrontend.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Frontend\bin\Release\KensSharpFrontend.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "Frontend\bin\Release\KensSharpFrontend.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Common.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Enigma.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Enigma.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Kosinski.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Kosinski.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Kosinski+.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Kosinski+.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Nemesis.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Nemesis.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Saxman.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Saxman.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Comper.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "KensSharp\bin\Release\SonicRetro.KensSharp.Comper.pdb"; DestDir: "{app}"; Flags: ignoreversion
Check: Is64BitInstallMode; Source: "x64\Release\KensSharpShellExt.dll"; DestDir: "{app}"; DestName: "KensSharpShellExt64.dll"; Flags: ignoreversion
Check: Is64BitInstallMode; Source: "x64\Release\KensSharpShellExt.pdb"; DestDir: "{app}"; DestName: "KensSharpShellExt64.pdb"; Flags: ignoreversion
Source: "Release\KensSharpShellExt.dll"; DestDir: "{app}"; DestName: "KensSharpShellExt.dll"; Flags: ignoreversion
Source: "Release\KensSharpShellExt.pdb"; DestDir: "{app}"; DestName: "KensSharpShellExt.pdb"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Registry]
Root: HKCR; Subkey: "*\shellex\ContextMenuHandlers\KensSharp"; ValueType: string; ValueData: "{{40376849-AF26-439e-AA72-E3B5E7298301}"; Flags: uninsdeletekey
Root: HKCR32; Subkey: "CLSID\{{40376849-AF26-439e-AA72-E3B5E7298301}"; ValueType: string; ValueData: "KensSharp"; Flags: uninsdeletekey
Root: HKCR32; Subkey: "CLSID\{{40376849-AF26-439e-AA72-E3B5E7298301}\InProcServer32"; ValueType: string; ValueData: "{app}\KensSharpShellExt.dll"
Root: HKCR32; Subkey: "CLSID\{{40376849-AF26-439e-AA72-E3B5E7298301}\InProcServer32"; ValueType: string; ValueName: "ThreadingModel"; ValueData: "Apartment"
Check: Is64BitInstallMode; Root: HKCR64; Subkey: "CLSID\{{40376849-AF26-439e-AA72-E3B5E7298301}"; ValueType: string; ValueData: "KensSharp"; Flags: uninsdeletekey
Check: Is64BitInstallMode; Root: HKCR64; Subkey: "CLSID\{{40376849-AF26-439e-AA72-E3B5E7298301}\InProcServer32"; ValueType: string; ValueData: "{app}\KensSharpShellExt64.dll"
Check: Is64BitInstallMode; Root: HKCR64; Subkey: "CLSID\{{40376849-AF26-439e-AA72-E3B5E7298301}\InProcServer32"; ValueType: string; ValueName: "ThreadingModel"; ValueData: "Apartment"

[Tasks]
Name: starticon; Description: "Create a &start menu icon"
Name: desktopicon; Description: "Create a &desktop icon"

[Icons]
Name: "{userprograms}\KENSSharp Frontend"; Filename: "{app}\KensSharpFrontend.exe"; Tasks: starticon
Name: "{userdesktop}\KENSSharp Frontend"; Filename: "{app}\KensSharpFrontend.exe"; Tasks: desktopicon
