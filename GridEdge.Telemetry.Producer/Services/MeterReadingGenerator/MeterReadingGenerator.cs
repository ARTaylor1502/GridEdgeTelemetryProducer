
using GridEdge.Telemetry.Shared.Contracts;

namespace GridEdge.Telemetry.Producer.Services.MeterReadingGenerator;

public class MeterReadingGenerator(TimeProvider? timeProvider = null) : IMeterReadingGenerator
{
    private static readonly double _averageHourlyPeakUsageKwh = 7;
    private static readonly double _averageHourlyOffPeakUsageKwh = 2;
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    public MeterReadingDto GenerateReading(string meterId)
    {
        var currentTime = _timeProvider.GetUtcNow();

        var IsPeakTime = currentTime.Hour >= 17 && currentTime.Hour <= 22;

        double currentUsageKwh;

        // Create a +/- 1.5kW fluctuation from average usage
        double range = 3.0;
        double usageFluctuationKwh = (Random.Shared.NextDouble() * range) - (range / 2);

        currentUsageKwh = IsPeakTime
            ? Math.Round(Math.Max(0.0, _averageHourlyPeakUsageKwh + usageFluctuationKwh), 2)
            : Math.Round(Math.Max(0.0, _averageHourlyOffPeakUsageKwh + usageFluctuationKwh), 2);

        return new MeterReadingDto
        {
            MeterId = meterId,
            UsageKwh = currentUsageKwh,
            Timestamp = currentTime
        };
    }
}
