namespace GridEdge.Telemetry.Consumer.Configuration;

public class RabbitMQSettings
{
    public string HostName { get; set; } = "localhost";
    public string QueueName { get; set; } = "telemetry-queue";
}
