#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
# Semantic version to stamp on the assemblies. The build context has no git history, so MinVer
# can't calculate the version from tags here; CI passes it in with
#   docker build --build-arg APP_VERSION=1.4.0 ...
# MSBuild reads environment variables as properties, so this becomes MinVerVersionOverride for
# both the build and publish stages. If it's omitted, MinVer warns (MINVER1001) and falls back to
# 1.0.0-alpha.0.
ARG APP_VERSION=
ENV MinVerVersionOverride=${APP_VERSION}
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["ONIONARCH.Presentation.API/ONIONARCH.Presentation.API.csproj", "ONIONARCH.Presentation.API/"]
RUN dotnet restore "./ONIONARCH.Presentation.API/ONIONARCH.Presentation.API.csproj"
COPY . .
WORKDIR "/src/ONIONARCH.Presentation.API"
RUN dotnet build "./ONIONARCH.Presentation.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ONIONARCH.Presentation.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG APP_VERSION=
LABEL org.opencontainers.image.version=${APP_VERSION}
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ONIONARCH.Presentation.API.dll"]