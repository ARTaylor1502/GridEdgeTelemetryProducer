namespace GridEdge.Telemetry.Shared.Contracts;

public interface ITelemetryConsumer
{
    Task StartConsumingTelemetryDataAsync(CancellationToken cancellationToken);
}