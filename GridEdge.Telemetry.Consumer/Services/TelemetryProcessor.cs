using GridEdge.Telemetry.Consumer.Entities;
using GridEdge.Telemetry.Consumer.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace GridEdge.Telemetry.Consumer.Services;

public class TelemetryProcessor(
    ILogger<TelemetryProcessor> logger,
    IServiceScopeFactory scopeFactory) : ITelemetryProcessor
{
    public async Task ProcessTelemetryDataAsync(MeterReadingDto reading, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();

        var exists = await dbContext.MeterReadings.AnyAsync(x => x.Id == reading.Id, cancellationToken);
        if (exists)
        {
            logger.LogInformation("Skipping storage of MeterReading #{Id}, already exists: ", reading.Id);
            return;
        }

        var meterReading = new MeterReading
        {
            Id = reading.Id,
            MeterId = reading.MeterId,
            UsageKwh = reading.UsageKwh,
            Timestamp = reading.Timestamp
        };

        dbContext.MeterReadings.Add(meterReading);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Stored MeterReading: {Id}", reading.Id);
    }
}
