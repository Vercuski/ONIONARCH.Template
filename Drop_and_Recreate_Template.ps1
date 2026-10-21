dotnet new uninstall          # lists installed packages; note any Vercuski.OnionArch.Templates or ...\source entries
dotnet new uninstall Vercuski.OnionArch.Templates
dotnet new uninstall "$PWD\source"   # only if a folder install is listed

dotnet pack template/Vercuski.OnionArch.Templates.csproj -c Release -o artifacts
dotnet new install (Get-ChildItem artifacts\Vercuski.OnionArch.Templates.*.nupkg | Sort-Object LastWriteTime | Select-Object -Last 1).FullName

dotnet new onionarch --help   # should list:  -da, --data-access <Both|Dapper|EfCore>