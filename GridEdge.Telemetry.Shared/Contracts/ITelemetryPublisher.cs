namespace GridEdge.Telemetry.Shared.Contracts;

public interface ITelemetryPublisher
{
    Task PublishAsync(MeterReadingDto reading);
}