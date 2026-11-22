dotnet sonarscanner begin /k:"Onion-Architecture-Template" /d:sonar.host.url="http://localhost:9000"  /d:sonar.token="sqp_c76464d0136b0903a9f6b8e34ed7185392e17998"
dotnet build .\source\ONIONARCH.sln
dotnet sonarscanner end /d:sonar.token="sqp_c76464d0136b0903a9f6b8e34ed7185392e17998"