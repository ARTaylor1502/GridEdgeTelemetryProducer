namespace GridEdge.Telemetry.Api.Models;

public record ReadingQuery(
    DateTime? From = null,
    DateTime? To = null,
    int Offset = 0,
    int Limit = 50
);