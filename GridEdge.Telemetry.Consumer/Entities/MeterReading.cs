namespace GridEdge.Telemetry.Consumer.Entities;

public class MeterReading
{
    public Guid Id { get; set; }
    public required string MeterId { get; set; }
    public double UsageKwh { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
