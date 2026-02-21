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
    private IConnection? connection;
    private IChannel? channel;
    private CancellationToken _cancellationToken;

    public async Task StartConsumingTelemetryDataAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        var factory = new ConnectionFactory { HostName = options.Value.HostName };

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                connection = await factory.CreateConnectionAsync();
                channel = await connection.CreateChannelAsync();

                if (channel is not { IsOpen: true })
                {
                    logger.LogCritical("RabbitMQ Channel is closed or null. Processing aborted.");
                    return;
                }

                await channel.QueueDeclareAsync(
                    queue: options.Value.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                var consumer = new AsyncEventingBasicConsumer(channel);
        
                consumer.ReceivedAsync += HandleMessageReceieved;

                await channel.BasicConsumeAsync(options.Value.QueueName, autoAck: false, consumer);
            
                logger.LogInformation("Waiting for messages in queue: {Queue}", options.Value.QueueName);  

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

            await channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, _cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process message. Sending Nack.");
            if (channel is { IsOpen: true })
            {
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, _cancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (channel is not null) await channel.CloseAsync();
        if (connection is not null) await connection.CloseAsync();

        // Suppress finalization to be memory efficient
        GC.SuppressFinalize(this);
    }
}