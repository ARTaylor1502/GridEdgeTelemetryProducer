using GridEdge.Telemetry.Consumer;
using GridEdge.Telemetry.Consumer.Configuration;
using GridEdge.Telemetry.Consumer.Infrastructure.Persistence;
using GridEdge.Telemetry.Consumer.Infrastructure.RabbitMQ;
using GridEdge.Telemetry.Consumer.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddDbContext<TelemetryDbContext>((serviceProvider, options) =>
{
    var dbSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;

    options.UseNpgsql(dbSettings.Postgres.ConnectionString);
});

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<ITelemetryProcessor, TelemetryProcessor>();
builder.Services.AddSingleton<ITelemetryConsumer, RabbitMQConsumer>();
builder.Services.AddHostedService<TelemetryConsumerWorker>();

var host = builder.Build();
host.Run();
