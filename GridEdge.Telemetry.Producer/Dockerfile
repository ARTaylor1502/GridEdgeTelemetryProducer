FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GridEdge.Telemetry.Producer/GridEdge.Telemetry.Producer.csproj", "GridEdge.Telemetry.Producer/"]
COPY ["GridEdge.Telemetry.Shared/GridEdge.Telemetry.Shared.csproj", "GridEdge.Telemetry.Shared/"]

RUN dotnet restore "GridEdge.Telemetry.Producer/GridEdge.Telemetry.Producer.csproj"

COPY . .
WORKDIR "/src/GridEdge.Telemetry.Producer"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GridEdge.Telemetry.Producer.dll"]