using Microsoft.Extensions.Logging;

using Moq;

using System;

namespace GridEdge.Telemetry.Consumer.Tests;

public static class MockLogger
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, string messagePart, Times times)
    {
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == level),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(messagePart)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}