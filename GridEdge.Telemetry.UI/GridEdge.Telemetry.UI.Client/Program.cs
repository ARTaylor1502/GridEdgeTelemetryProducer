using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GridEdge.Telemetry.Infrastructure.Configuration;
using Microsoft.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var state = builder.Services.BuildServiceProvider().GetRequiredService<PersistentComponentState>();
string finalUrl;

if (state.TryTakeFromJson<ApiSettings>("ApiSettings", out var persisted))
{
    finalUrl = persisted!.ExternalBaseUrl;
}
else
{
    finalUrl = builder.Configuration["ApiSettings:ExternalBaseUrl"] ?? "http://localhost:5050";
}

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(finalUrl)
});

await builder.Build().RunAsync();
