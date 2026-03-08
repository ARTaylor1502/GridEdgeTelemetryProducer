FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GridEdge.Telemetry.Infrastructure/GridEdge.Telemetry.Infrastructure.csproj", "GridEdge.Telemetry.Infrastructure/"]
COPY ["GridEdge.Telemetry.UI/GridEdge.Telemetry.UI/GridEdge.Telemetry.UI.csproj", "GridEdge.Telemetry.UI/GridEdge.Telemetry.UI/"]
COPY ["GridEdge.Telemetry.Shared/GridEdge.Telemetry.Shared.csproj", "GridEdge.Telemetry.Shared/"]
COPY ["GridEdge.Telemetry.UI/GridEdge.Telemetry.UI.Client/GridEdge.Telemetry.UI.Client.csproj", "GridEdge.Telemetry.UI/GridEdge.Telemetry.UI.Client/"]

RUN dotnet restore "GridEdge.Telemetry.UI/GridEdge.Telemetry.UI/GridEdge.Telemetry.UI.csproj"

COPY . .

WORKDIR "/src/GridEdge.Telemetry.UI/GridEdge.Telemetry.UI"
RUN dotnet publish "GridEdge.Telemetry.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5070

ENTRYPOINT ["dotnet", "GridEdge.Telemetry.UI.dll"]