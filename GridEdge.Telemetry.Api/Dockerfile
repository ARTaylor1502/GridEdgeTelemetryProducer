FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GridEdge.Telemetry.Api/GridEdge.Telemetry.Api.csproj", "GridEdge.Telemetry.Api/"]
COPY ["GridEdge.Telemetry.Shared/GridEdge.Telemetry.Shared.csproj", "GridEdge.Telemetry.Shared/"]
COPY ["GridEdge.Telemetry.Infrastructure/GridEdge.Telemetry.Infrastructure.csproj", "GridEdge.Telemetry.Infrastructure/"]

RUN dotnet restore "GridEdge.Telemetry.Api/GridEdge.Telemetry.Api.csproj"

COPY . .
WORKDIR "/src/GridEdge.Telemetry.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GridEdge.Telemetry.Api.dll"]