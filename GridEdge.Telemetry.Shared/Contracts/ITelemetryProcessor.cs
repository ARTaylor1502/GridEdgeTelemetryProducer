namespace GridEdge.Telemetry.Shared.Contracts;

public interface ITelemetryProcessor
{
    Task ProcessTelemetryDataAsync(MeterReadingDto reading, CancellationToken cancellationToken = default);
}