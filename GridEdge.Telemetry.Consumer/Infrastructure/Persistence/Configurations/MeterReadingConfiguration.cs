using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GridEdge.Telemetry.Consumer.Entities;

namespace GridEdge.Telemetry.Consumer.Infrastructure.Persistence.Configurations;

public class MeterReadingConfiguration : IEntityTypeConfiguration<MeterReading>
{
    public void Configure(EntityTypeBuilder<MeterReading> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MeterId).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Timestamp);
    }
}