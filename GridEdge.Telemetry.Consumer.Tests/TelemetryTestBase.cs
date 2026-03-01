using GridEdge.Telemetry.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace GridEdge.Telemetry.Consumer.Tests;

public abstract class TelemetryTestBase : IDisposable
{
    protected readonly TelemetryDbContext _dbContext;
    protected readonly Mock<IServiceScopeFactory> _mockScopeFactory;
    private readonly Microsoft.Data.Sqlite.SqliteConnection _connection;

    protected TelemetryTestBase()
    {
        //Setup in memory db
        _connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TelemetryDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new TelemetryDbContext(options);
        _dbContext.Database.EnsureCreated();

        //mock scoped db context
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(x => x.GetService(typeof(TelemetryDbContext)))
            .Returns(_dbContext);

        var mockScope = new Mock<IServiceScope>();
        mockScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);

        _mockScopeFactory = new Mock<IServiceScopeFactory>();
        _mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
    }

    public void Dispose()
    {
        _connection.Close();
        _dbContext.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
