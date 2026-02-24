namespace GridEdge.Telemetry.Consumer.Configuration;

public class PostgresDBSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public required string Database { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string ConnectionString =>
        $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};Include Error Detail=true";
}
