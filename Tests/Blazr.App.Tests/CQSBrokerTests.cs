/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public partial class CQSBrokerTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public CQSBrokerTests()
        // Creates an instance of the Test Data provider
        => _weatherTestDataProvider = WeatherTestDataProvider.Instance();

    private ServiceProvider GetServiceProvider()
    {
        // Creates a Service Collection
        var services = new ServiceCollection();
        // Adds the application services to the collection
        Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
        services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(dbOptions);
        services.AddSingleton<ICQSDataBroker, CQSDataBroker<InMemoryWeatherDbContext>>();
        // Creates a Service Provider from the Services collection
        // This is our DI container
        var provider = services.BuildServiceProvider();

        // Adds the test data to the in memory database
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>()!;
        WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherDbContext>(factory);

        return provider!;
    }
}
