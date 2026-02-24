namespace GridEdge.Telemetry.Shared.Contracts;

public record MeterReadingDto
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string MeterId { get; init; }
    public required double UsageKwh { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
};
