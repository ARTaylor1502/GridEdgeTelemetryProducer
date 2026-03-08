using GridEdge.Telemetry.UI.Client.Pages;
using GridEdge.Telemetry.UI.Components;
using GridEdge.Telemetry.Infrastructure.Configuration;
using ApexCharts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();
builder.Services.AddSingleton(apiSettings!);

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiSettings!.InternalBaseUrl)
});

builder.Services.AddCascadingValue("ApiSettings", sp =>
    builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddApexCharts();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.MapGet("/", () => Results.Redirect("/dashboard"));
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GridEdge.Telemetry.UI.Client._Imports).Assembly);

app.Run();
