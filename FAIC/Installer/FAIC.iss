#define AppName "Fast Animated Image Converter"
#define AppNameShort "FAIC"
#define AppExe "FAIC.exe"
#define AppDir "../bin/Release/net8.0-windows/publish/win-x64/"
#define AppVersion GetFileVersion(AppDir + AppExe)
#define ContextMenuText "Convert to Animated Image"

[Setup]
AppId={{CBC3F5A1-D854-4067-A1BA-EE1703D95FF9}}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=Jaime Shirazi
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=..\Installer\Build
OutputBaseFilename=FAIC-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\{#AppExe}
UninstallDisplayName={#AppNameShort}
ChangesAssociations=yes

[Files]
Source: "{#AppDir}*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion; Excludes: "*.pdb"

[Tasks]
Name: "contextmenu"; Description: "Add ""{#ContextMenuText}"" to right-click menu for videos"; Flags: checkedonce
Name: "startmenu"; Description: "Create Start Menu shortcut"; Flags: checkedonce

[Registry]
#define SubkeyPathPre "Software\Classes\"
#define SubkeyPathPost "\shell\" + ContextMenuText
;Context menu for all videos
Root: HKCU; Subkey: "{#SubkeyPathPre}*{#SubkeyPathPost}"; ValueType: string; ValueName: ""; ValueData: "{#ContextMenuText}"; Flags: uninsdeletekey; Tasks: contextmenu   
Root: HKCU; Subkey: "{#SubkeyPathPre}*{#SubkeyPathPost}"; ValueType: string; ValueName: "Icon"; ValueData: "{app}\{#AppExe}"; Tasks: contextmenu
Root: HKCU; Subkey: "{#SubkeyPathPre}*{#SubkeyPathPost}"; ValueType: string; ValueName: "MultiSelectModel"; ValueData: "Player"; Tasks: contextmenu
Root: HKCU; Subkey: "{#SubkeyPathPre}*{#SubkeyPathPost}"; ValueType: string; ValueName: "AppliesTo"; ValueData: "System.Kind:=video"; Tasks: contextmenu
Root: HKCU; Subkey: "{#SubkeyPathPre}*{#SubkeyPathPost}\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExe}"" ""%1"""; Tasks: contextmenu
;webm for some reason isn't a kind of video, at least on my computer
Root: HKCU; Subkey: "{#SubkeyPathPre}SystemFileAssociations\.webm{#SubkeyPathPost}"; ValueType: string; ValueName: ""; ValueData: "{#ContextMenuText}"; Flags: uninsdeletekey; Tasks: contextmenu   
Root: HKCU; Subkey: "{#SubkeyPathPre}SystemFileAssociations\.webm{#SubkeyPathPost}"; ValueType: string; ValueName: "Icon"; ValueData: "{app}\{#AppExe}"; Tasks: contextmenu
Root: HKCU; Subkey: "{#SubkeyPathPre}SystemFileAssociations\.webm{#SubkeyPathPost}"; ValueType: string; ValueName: "MultiSelectModel"; ValueData: "Player"; Tasks: contextmenu
Root: HKCU; Subkey: "{#SubkeyPathPre}SystemFileAssociations\.webm{#SubkeyPathPost}\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExe}"" ""%1"""; Tasks: contextmenu

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppName}"; Tasks: startmenu
Name: "{group}\Uninstall {#AppNameShort}"; Filename: "{uninstallexe}"; Tasks: startmenu
  
[Run]
Filename: "{app}\{#AppExe}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent