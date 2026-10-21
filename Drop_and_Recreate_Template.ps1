dotnet new uninstall          # lists installed packages; note any Vercuski.OnionArch.Templates or ...\source entries
pause
dotnet new uninstall Vercuski.OnionArch.Templates
pause
Get-ChildItem "$env:USERPROFILE\.templateengine" -Directory | Remove-Item -Recurse -Force
pause
dotnet new uninstall "$PWD\source"   # only if a folder install is listed
pause
dotnet pack template/Vercuski.OnionArch.Templates.csproj -c Release -o artifacts
pause
dotnet new install (Get-ChildItem artifacts\Vercuski.OnionArch.Templates.*.nupkg | Sort-Object LastWriteTime | Select-Object -Last 1).FullName
pause
dotnet new onionarch --help   # should list:  -da, --data-access <Both|Dapper|EfCore>
pause