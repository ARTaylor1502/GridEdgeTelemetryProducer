namespace GridEdge.Telemetry.Shared.Contracts;

public interface IMeterReadingGenerator
{
    MeterReadingDto GenerateReading(string meterId);
}
