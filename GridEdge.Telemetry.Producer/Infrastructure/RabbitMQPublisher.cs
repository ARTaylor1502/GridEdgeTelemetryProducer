using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using GridEdge.Telemetry.Shared.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace GridEdge.Telemetry.Producer.Infrastructure;

public class RabbitMQPublisher : ITelemetryPublisher, IAsyncDisposable
{
    private readonly ConnectionFactory factory;
    private readonly string queueName;
    private readonly ILogger<RabbitMQPublisher> logger;
    private IConnection? connection;
    private IChannel? channel;


    public RabbitMQPublisher(
        IOptions<RabbitMQSettings> options,
        ILogger<RabbitMQPublisher> _logger
    )
    {
        factory = new ConnectionFactory { HostName = options.Value.HostName };
        queueName = options.Value.QueueName;
        logger = _logger;
    }

    public async Task PublishAsync(MeterReadingDto reading)
    {
        if (connection == null)
        {
            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName, 
                durable: true, 
                exclusive: false, 
                autoDelete: false
            );
        }

        var json = JsonSerializer.Serialize(reading);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            body: body
        );

        logger.LogInformation("Message sent to {Queue}", queueName);
    }

    public async ValueTask DisposeAsync()
    {
        if (channel is not null)
        {
            await channel.CloseAsync();
        }

        if (connection is not null)
        {
            await connection.CloseAsync();
        }
        
        // Suppress finalization to be memory efficient
        GC.SuppressFinalize(this);
    }
}