namespace GridEdge.Telemetry.Consumer.Configuration;

public class DatabaseSettings
{
    public required PostgresDBSettings Postgres { get; set; }
    public bool EnableLogging { get; set; }
    public int CommandTimeout { get; set; }
}
