namespace GridEdgeTelemetryProducer.Contracts;

public record MeterReadingDto(
    string MeterId,
    double UsageKw,
    DateTimeOffset Timestamp
);