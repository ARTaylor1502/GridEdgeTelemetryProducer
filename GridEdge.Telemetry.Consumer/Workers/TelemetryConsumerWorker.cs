namespace GridEdge.Telemetry.Consumer.Workers;

public class TelemetryConsumerWorker(ITelemetryConsumer consumer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await consumer.StartConsumingTelemetryDataAsync(cancellationToken);
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}
