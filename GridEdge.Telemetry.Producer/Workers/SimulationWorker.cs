
using System.Text.Json;

using GridEdge.Telemetry.Producer.Services.MeterReadingGenerator;
using GridEdge.Telemetry.Shared.Contracts;

namespace GridEdge.Telemetry.Producer.Workers;

public class SimulationWorker(
    IMeterReadingGenerator meterReadingGenerator,
    IOptions<TelemetrySettings> settings,
    ILogger<SimulationWorker> logger,
    ITelemetryPublisher publisher
) : BackgroundService
{
    private readonly string _meterId = settings.Value.MeterId ?? $"METER-{Guid.NewGuid()}";

    private readonly int _transmissionIntervalMilliSeconds = settings.Value.TransmissionIntervalMilliSeconds;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker started for Meter: {Id}", _meterId);

            MeterReadingDto meterReading = meterReadingGenerator.GenerateReading(_meterId);
            try
            {
                await publisher.PublishAsync(meterReading);

                logger.LogInformation("Reading generated: {Usage}", JsonSerializer.Serialize(meterReading));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish telemetry reading");
            }

            await Task.Delay(_transmissionIntervalMilliSeconds, stoppingToken);
        }
    }
}
