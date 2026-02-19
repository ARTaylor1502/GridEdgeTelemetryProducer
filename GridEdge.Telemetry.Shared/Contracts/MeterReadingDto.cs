namespace GridEdge.Telemetry.Shared.Contracts;

public record MeterReadingDto(
    string MeterId,
    double UsageKw,
    DateTimeOffset Timestamp
);