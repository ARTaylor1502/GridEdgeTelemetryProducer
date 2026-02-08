using FluentAssertions;
using GridEdgeTelemetryProducer.Services.MeterReadingGenerator;

namespace GridEdgeTelemetryProducer.Tests.Services;

public class MeterReadingGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnPopulatedDto()
    {
        var generator = new MeterReadingGenerator();
        var meterId = "TEST_METER_1";

        var result = generator.GenerateReading(meterId);

        result.MeterId.Should().Be(meterId);
        result.UsageKw.Should().BeGreaterThanOrEqualTo(0);
        result.Timestamp.Should().BeWithin(TimeSpan.FromSeconds(1)).Before(DateTime.UtcNow);
    }

    [Theory]
    [InlineData(10, 0.5, 3.5)]
    [InlineData(19, 5.5, 8.5)]
    public void Generate_ShouldReturnCorrectUsage_ForDifferentTimes(int hour, double expectedMinUsage, double expectedMaxUsage)
    {
        var fakeTime = new FakeTimeProvider();
        fakeTime.SetUtcNow(new DateTimeOffset(2026, 2, 8, hour, 0, 0, TimeSpan.Zero));

        var generator = new MeterReadingGenerator(fakeTime);
        var result = generator.GenerateReading("TEST_METER_1");

        result.UsageKw.Should().BeInRange(expectedMinUsage, expectedMaxUsage);
    }
}