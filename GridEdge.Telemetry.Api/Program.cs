using GridEdge.Telemetry.Infrastructure.Configuration;
using GridEdge.Telemetry.Infrastructure.Persistence;

using GridEdge.Telemetry.Api.Models;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

using Swashbuckle.AspNetCore.ReDoc;
using System.Runtime.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddDbContext<TelemetryDbContext>((serviceProvider, options) =>
{
    var dbSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    options.UseNpgsql(dbSettings.Postgres.ConnectionString);
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<TelemetryDbContext>("PostgreSQL");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Redoc setup
app.UseSwagger();
app.UseReDoc(c =>
{
    c.DocumentTitle = "GridEdge Telemetry API";
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.RoutePrefix = "api-docs";
});

//Health check end point
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
})
.WithGroupName("v1")
.WithSummary("Check API and Database Health")
.WithDescription("Returns the status of the PostgreSQL connection.")
.WithTags("Monitoring");

//Endpoints
app.MapGet("/api/meter-readings", async Task<IResult> (
    TelemetryDbContext db,
    [AsParameters] ReadingQuery query) =>
{
    const int MaxLimit = 100;
    var actualLimit = Math.Min(query.Limit, MaxLimit);

    var dbQuery = db.MeterReadings.AsNoTracking().AsQueryable();

    if (query.From.HasValue)
        dbQuery = dbQuery.Where(r => r.Timestamp >= query.From.Value.ToUniversalTime());

    if (query.To.HasValue)
        dbQuery = dbQuery.Where(r => r.Timestamp <= query.To.Value.ToUniversalTime());

    var results = await dbQuery
        .OrderByDescending(r => r.Timestamp)
        .Skip(query.Offset)
        .Take(actualLimit)
        .ToListAsync();

    return TypedResults.Ok(results);
})
.WithSummary("Fetch paginated telemetry readings")
.WithDescription("Filter telemetry readings by date range and standard offset pagination.")
.WithTags("Telemetry");

app.MapGet("/api/meter-readings/{meterId}", async Task<IResult> (
    TelemetryDbContext db,
    string meterId,
    [AsParameters] ReadingQuery query) =>
{
    const int MaxLimit = 100;
    var actualLimit = Math.Min(query.Limit, MaxLimit);

    var dbQuery = db.MeterReadings
        .AsNoTracking()
        .Where(r => r.MeterId == meterId);

    if (query.From.HasValue)
        dbQuery = dbQuery.Where(r => r.Timestamp >= query.From.Value.ToUniversalTime());

    if (query.To.HasValue)
        dbQuery = dbQuery.Where(r => r.Timestamp <= query.To.Value.ToUniversalTime());

    var results = await dbQuery
        .OrderByDescending(r => r.Timestamp)
        .Skip(query.Offset)
        .Take(actualLimit)
        .ToListAsync();

    return results.Any()
        ? TypedResults.Ok(results)
        : TypedResults.NotFound(new { message = $"No readings found for meter {meterId} with the specified filters." });
})
.WithSummary("Get filtered readings for a specific meter")
.WithTags("Telemetry");

app.Run();
