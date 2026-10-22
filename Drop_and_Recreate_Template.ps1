echo ""
echo ""
echo "***************************"
echo "* List installed packages *"
echo "***************************"
dotnet new uninstall          # lists installed packages; note any Vercuski.OnionArch.Templates or ...\source entries

echo ""
echo ""
echo "*******************************************"
echo "* Uninstall previous Onionarch Template *"
echo "*******************************************"
dotnet new uninstall Vercuski.OnionArch.Templates

echo ""
echo ""
echo "************************************************"
echo "* Empty $env:USERPROFILE\.templateengine *"
echo "************************************************"
Get-ChildItem "$env:USERPROFILE\.templateengine" -Directory | Remove-Item -Recurse -Force

echo ""
echo ""
echo "******************************************"
echo "* Uninstall any 'folder install' templates *"
echo "******************************************"
dotnet new uninstall "$PWD\source"   # only if a folder install is listed

echo ""
echo ""
echo "*********************************"
echo "* Packing Onionarch Template *"
echo "*********************************"
dotnet pack template/Vercuski.OnionArch.Templates.csproj -c Release -o artifacts

echo ""
echo ""
echo "************************"
echo "* Install new template *"
echo "************************"
dotnet new install (Get-ChildItem artifacts\Vercuski.OnionArch.Templates.*.nupkg | Sort-Object LastWriteTime | Select-Object -Last 1).FullName

echo ""
echo ""
echo "*********************************************************************************************************"
echo "* Test if new template s installed you should see '-da, --data-access <Both|Dapper|EfCore>' in the text *"
echo "*********************************************************************************************************"
dotnet new onionarch --help   # should list:  -da, --data-access <Both|Dapper|EfCore>