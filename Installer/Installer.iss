[Setup]
AppId=ARZVP
AppName=AutoRedZone for VEGAS Pro
AppVersion=2.2.0
AppPublisher=Demiomad
DefaultGroupName=ARZVP
Compression=lzma2
WizardImageFile=.\Resources\wizard.png
WizardImageFileDynamicDark=.\Resources\wizard.png
SolidCompression=yes
OutputDir=.\Output
OutputBaseFilename=ARZVPv2-setup
WizardStyle=modern dynamic
PrivilegesRequired=admin
DefaultDirName=C:\ARZVP
DisableDirPage=yes
CreateAppDir=no
SetupIconFile=.\Resources\Icon.ico
Uninstallable=yes
CreateUninstallRegKey=yes
AppPublisherURL=https://github.com/Demiomad/ARZVP
AppSupportURL=https://github.com/Demiomad/ARZVP/issues
AppUpdatesURL=https://github.com/Demiomad/ARZVP/releases
DisableWelcomePage=no
LicenseFile=..\LICENSE.txt

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "pl"; MessagesFile: "compiler:Languages\Polish.isl"

[Files]
Source: ".\DLL\ARZVPRewrite.dll"; DestDir: "{userdocs}\Vegas Script Menu\AutoRedZone for VEGAS Pro"; Flags: ignoreversion
Source: ".\Resources\Templates\*"; DestDir: "{localappdata}\ARZVPv2\templates"; Flags: recursesubdirs
Source: ".\Resources\Languages\*"; DestDir: "{localappdata}\ARZVPv2\langs"; Flags: recursesubdirs

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\ARZVPv2"
Type: filesandordirs; Name: "{userdocs}\Vegas Script Menu\AutoRedZone for VEGAS Pro"

[Code]
function NextButtonClick(CurPageID: Integer): Boolean;
var
  UserDocsPath: string;
begin
  Result := True;
  if CurPageID = wpFinished then
  begin
    UserDocsPath := ExpandConstant('{userdocs}\Vegas Script Menu\AutoRedZone for VEGAS Pro');
    MsgBox('Script installed in ' + UserDocsPath, mbInformation, MB_OK);
  end;
end;