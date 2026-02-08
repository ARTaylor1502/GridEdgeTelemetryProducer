using GridEdgeTelemetryProducer.Services.MeterReadingGenerator; 

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<TelemetrySettings>(builder.Configuration.GetSection(TelemetrySettings.SectionName));
builder.Services.AddHostedService<SimulationWorker>();
builder.Services.AddSingleton<IMeterReadingGenerator, MeterReadingGenerator>();

var host = builder.Build();
host.Run();