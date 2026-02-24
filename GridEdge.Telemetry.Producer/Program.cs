using GridEdge.Telemetry.Producer.Infrastructure;
using GridEdge.Telemetry.Producer.Services.MeterReadingGenerator;
using GridEdge.Telemetry.Shared.Contracts;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<ITelemetryPublisher, RabbitMQPublisher>();
builder.Services.Configure<TelemetrySettings>(builder.Configuration.GetSection(TelemetrySettings.SectionName));
builder.Services.AddHostedService<SimulationWorker>();
builder.Services.AddSingleton<IMeterReadingGenerator, MeterReadingGenerator>();

var host = builder.Build();
host.Run();
