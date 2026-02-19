namespace GridEdge.Telemetry.Producer.Configuration;

public class TelemetrySettings
{
    public const string SectionName = "TelemetrySettings";

    public string MeterId { get; set; } = "Default-Meter-001";

    public int TransmissionIntervalMilliSeconds { get; set; } = 1000;
}