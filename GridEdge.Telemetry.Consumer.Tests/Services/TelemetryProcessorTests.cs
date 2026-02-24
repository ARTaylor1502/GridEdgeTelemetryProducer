using Moq;

using Xunit;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using GridEdge.Telemetry.Consumer.Services;
using GridEdge.Telemetry.Consumer.Infrastructure.Persistence;
using GridEdge.Telemetry.Shared.Contracts;
using GridEdge.Telemetry.Consumer.Entities;

using GridEdge.Telemetry.Consumer.Tests;

public class TelemetryProcessorTests : TelemetryTestBase
{
    private readonly Mock<ILogger<TelemetryProcessor>> _mockLogger;
    private readonly TelemetryProcessor _processor;

    public TelemetryProcessorTests()
    {
        _mockLogger = new Mock<ILogger<TelemetryProcessor>>();
        _processor = new TelemetryProcessor(_mockLogger.Object, _mockScopeFactory.Object);
    }

    [Fact]
    public async Task Process_ShouldAddReading_WhenDataIsValid()
    {
        var readingId = Guid.NewGuid();

        var reading = new MeterReadingDto
        {
            Id = readingId,
            MeterId = "METER-1",
            UsageKwh = 12.3,
            Timestamp = DateTime.UtcNow
        };
        await _processor.ProcessTelemetryDataAsync(reading, CancellationToken.None);

        _dbContext.MeterReadings.Should().Contain(x => x.Id == reading.Id);
        _mockLogger.VerifyLog(LogLevel.Information, $"Stored MeterReading: {readingId}", Times.Once());
    }

    [Fact]
    public async Task Process_ShouldSkipProcessing_WhenDuplicateReadingFound()
    {
        var existingId = Guid.NewGuid();
        _dbContext.MeterReadings.Add(new MeterReading
        {
            Id = existingId,
            MeterId = "METER-1",
            UsageKwh = 12.3,
            Timestamp = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        var duplicateDto = new MeterReadingDto
        {
            Id = existingId,
            MeterId = "METER-1",
            UsageKwh = 12.3,
            Timestamp = DateTime.UtcNow
        };

        await _processor.ProcessTelemetryDataAsync(duplicateDto, CancellationToken.None);

        _mockLogger.VerifyLog(LogLevel.Information, "Skipping storage", Times.Once());
    }
}