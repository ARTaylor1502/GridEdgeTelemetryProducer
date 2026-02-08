namespace GridEdgeTelemetryProducer.Services.MeterReadingGenerator;

public interface IMeterReadingGenerator
{
    MeterReadingDto GenerateReading(string meterId);
}