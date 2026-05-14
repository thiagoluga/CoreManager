# syntax=docker/dockerfile:1
#
# Multi-stage build for Luga.Server.Host.
# Build context is the repo root (see .github/workflows/deploy-staging.yml).
#
# Stages:
#   - sdk: restores + publishes the API together with the Blazor WASM bundle.
#   - runtime: minimal aspnet image with the published output.

ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS sdk
WORKDIR /src

# Layer-friendly restore: copy the project graph + central package management files first.
COPY ["global.json", "Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props", "./"]
COPY ["src/Luga.CoreManager.slnx", "./src/Luga.CoreManager.slnx"]

COPY ["src/BuildingBlocks/", "./src/BuildingBlocks/"]
COPY ["src/Modules/", "./src/Modules/"]
COPY ["src/Hosts/", "./src/Hosts/"]

RUN dotnet restore "src/Hosts/Luga.Server.Host/Luga.Server.Host.csproj"

# Publish — emits the API plus wwwroot/_framework/* from the Blazor WASM client.
RUN dotnet publish "src/Hosts/Luga.Server.Host/Luga.Server.Host.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app
COPY --from=sdk /app/publish .

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

ENTRYPOINT ["dotnet", "Luga.Server.Host.dll"]
