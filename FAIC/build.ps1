dotnet publish .\FAIC.csproj -c Release -p:PublishProfile="Inno Setup Input"

& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" .\Installer\FAIC.iss