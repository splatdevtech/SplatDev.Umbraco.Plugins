FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

WORKDIR /src

COPY Umbraco13.Baseline.csproj .
RUN dotnet restore Umbraco13.Baseline.csproj

COPY . .
RUN dotnet publish Umbraco13.Baseline.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

ENV ASPNETCORE_URLS=http://0.0.0.0:5001 \
    ASPNETCORE_ENVIRONMENT=Development

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/umbraco/Data

EXPOSE 5001

HEALTHCHECK --interval=20s --timeout=5s --start-period=90s --retries=5 \
    CMD curl -sf http://127.0.0.1:5001/umbraco || exit 1

ENTRYPOINT ["dotnet", "Umbraco13.Baseline.dll"]
