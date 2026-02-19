namespace GridEdge.Telemetry.Producer.Services.MeterReadingGenerator;

using GridEdge.Telemetry.Shared.Contracts;

public class MeterReadingGenerator: IMeterReadingGenerator
{
    private static readonly double averageHourlyPeakUsageKw = 7;
    private static readonly double averageHourlyOffPeakUsageKw = 2;
    private readonly TimeProvider _timeProvider;

    public MeterReadingGenerator(TimeProvider? timeProvider = null)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public MeterReadingDto GenerateReading(string meterId)
    {
        var currentTime = _timeProvider.GetUtcNow();

        var IsPeakTime = currentTime.Hour >= 17 && currentTime.Hour <= 22;

        double currentUsageKw;

        // Create a +/- 1.5kW fluctuation from average usage
        double range = 3.0;
        double usageFluctuationKw = (Random.Shared.NextDouble() * range) - (range / 2);
 
        if (IsPeakTime)
        { 
            currentUsageKw = Math.Round(Math.Max(0.0, averageHourlyPeakUsageKw + usageFluctuationKw), 2);
        } else
        {
            currentUsageKw = Math.Round(Math.Max(0.0, averageHourlyOffPeakUsageKw + usageFluctuationKw), 2);
        }

        return new MeterReadingDto(
            meterId, 
            currentUsageKw,
            currentTime
        );
    }
}