FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GridEdge.Telemetry.Consumer/GridEdge.Telemetry.Consumer.csproj", "GridEdge.Telemetry.Consumer/"]
COPY ["GridEdge.Telemetry.Shared/GridEdge.Telemetry.Shared.csproj", "GridEdge.Telemetry.Shared/"]
COPY ["GridEdge.Telemetry.Infrastructure/GridEdge.Telemetry.Infrastructure.csproj", "GridEdge.Telemetry.Infrastructure/"]

RUN dotnet restore "GridEdge.Telemetry.Consumer/GridEdge.Telemetry.Consumer.csproj"

COPY . .
WORKDIR "/src/GridEdge.Telemetry.Consumer"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GridEdge.Telemetry.Consumer.dll"]