using Microsoft.EntityFrameworkCore;
using GridEdge.Telemetry.Consumer.Entities;

namespace GridEdge.Telemetry.Consumer.Infrastructure.Persistence;

public class TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : DbContext(options)
{
    public DbSet<MeterReading> MeterReadings => Set<MeterReading>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TelemetryDbContext).Assembly);
    }
}