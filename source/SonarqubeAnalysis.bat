dotnet sonarscanner begin /k:"OnionArch" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="" /d:sonar.cs.dotcover.reportsPaths=dotCover.Output.html
dotnet build --no-incremental
dotcover.exe analyse .\coverConfig.xml
dotnet sonarscanner end /d:sonar.token=""
pause
