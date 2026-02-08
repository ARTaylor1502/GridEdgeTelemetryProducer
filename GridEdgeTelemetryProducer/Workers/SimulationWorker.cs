namespace GridEdgeTelemetryProducer.Workers;

using System.Text.Json;
using GridEdgeTelemetryProducer.Services.MeterReadingGenerator;

public class SimulationWorker(
    IMeterReadingGenerator meterReadingGenerator,
    IOptions<TelemetrySettings> settings,
    ILogger<SimulationWorker> logger
) : BackgroundService
{
    private readonly string meterId = settings.Value.MeterId ?? $"METER-{Guid.NewGuid()}";

    private readonly int TransmissionIntervalMilliSeconds = settings.Value.TransmissionIntervalMilliSeconds;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker started for Meter: {Id}", meterId);

            MeterReadingDto meterReading = meterReadingGenerator.GenerateReading(meterId);

            string jsonPayload = JsonSerializer.Serialize(meterReading);
            
            logger.LogInformation("Reading generated: {Usage}", jsonPayload);

            await Task.Delay(TransmissionIntervalMilliSeconds, stoppingToken);
        }
    }
}
