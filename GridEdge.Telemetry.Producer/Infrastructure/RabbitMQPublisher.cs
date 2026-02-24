using System.Text;
using System.Text.Json;

using GridEdge.Telemetry.Shared.Contracts;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace GridEdge.Telemetry.Producer.Infrastructure;

public class RabbitMQPublisher : ITelemetryPublisher, IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly string _queueName;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private IConnection? _connection;
    private IChannel? _channel;


    public RabbitMQPublisher(
        IOptions<RabbitMQSettings> options,
        ILogger<RabbitMQPublisher> logger
    )
    {
        _factory = new ConnectionFactory { HostName = options.Value.HostName };
        _queueName = options.Value.QueueName;
        _logger = logger;
    }

    public async Task PublishAsync(MeterReadingDto reading)
    {
        if (_channel == null)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
        }

        var json = JsonSerializer.Serialize(reading);
        var body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _queueName,
            body: body
        );

        _logger.LogInformation("Message sent to {Queue}", _queueName);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }

        // Suppress finalization to be memory efficient
        GC.SuppressFinalize(this);
    }
}
