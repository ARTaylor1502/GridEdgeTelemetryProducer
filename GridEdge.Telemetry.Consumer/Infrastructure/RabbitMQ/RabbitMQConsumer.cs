using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace GridEdge.Telemetry.Consumer.Infrastructure.RabbitMQ;

public class RabbitMQConsumer(
    IOptions<RabbitMQSettings> options,
    ILogger<RabbitMQConsumer> logger,
    ITelemetryProcessor processor

) : ITelemetryConsumer, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private CancellationToken _cancellationToken;

    public async Task StartConsumingTelemetryDataAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        var factory = new ConnectionFactory { HostName = options.Value.HostName };

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                if (_channel is not { IsOpen: true })
                {
                    logger.LogCritical("RabbitMQ Channel is closed or null. Processing aborted.");
                    return;
                }

                await _channel.QueueDeclareAsync(
                    queue: options.Value.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += HandleMessageReceieved;

                await _channel.BasicConsumeAsync(options.Value.QueueName, autoAck: false, consumer);

                break;
            }
            catch (BrokerUnreachableException ex)
            {
                logger.LogWarning("RabbitMQ connection failed. Retrying in 5s... Error: {Msg}", ex.Message);
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async Task HandleMessageReceieved(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var reading = JsonSerializer.Deserialize<MeterReadingDto>(message);

            if (reading != null)
            {
                await processor.ProcessTelemetryDataAsync(reading, _cancellationToken);
            }

            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, _cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process message. Sending Nack.");
            if (_channel is { IsOpen: true })
            {
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, _cancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.CloseAsync();
        if (_connection is not null) await _connection.CloseAsync();

        // Suppress finalization to be memory efficient
        GC.SuppressFinalize(this);
    }
}
