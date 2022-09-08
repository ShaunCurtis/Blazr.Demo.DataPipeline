/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class CQSListQueryBrokerTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public CQSListQueryBrokerTests()
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
        services.AddScoped<IListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();
        // Creates a Service Provider from the Services collection
        // This is our DI container
        var provider = services.BuildServiceProvider();

        // Adds the test data to the in memory database
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>()!;
        WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherDbContext>(factory);

        return provider!;
    }

    [Fact]
    public async void TestWeatherLocationListCQSDataBroker()
    {
        // Build our DI container
        var provider = GetServiceProvider();
        //Get the Data Broker
        var broker = provider.GetService<ICQSDataBroker>()!;

        // Get the control record count from the Test Data Provider
        var testRecordCount = _weatherTestDataProvider.WeatherLocations.Count();
        int pageSize = 10;
        // Get the expected recordset count.
        // It should be either the page size or the total record count if that's smaller
        var testCount = testRecordCount > pageSize ? pageSize : testRecordCount;

        // Create a list request
        var listRequest = new ListProviderRequest<DboWeatherLocation>(0, pageSize);

        // Create a ListQuery and execute the query on the Data Broker against the DboWeatherLocation recordset
        var query = ListQuery<DboWeatherLocation>.GetQuery(listRequest);
        var result = await broker.ExecuteAsync<DboWeatherLocation>(query);

        // Check we have success
        Assert.True(result.Success);
        // Check the recordset count
        Assert.Equal(testCount, result.Items.Count());
        // Check the total count os correct against the test provider
        Assert.True(result.TotalItemCount == testRecordCount);
    }

    [Fact]
    public async void TestDboWeatherForecastListCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;
        var testRecordCount = _weatherTestDataProvider.WeatherForecasts.Count();
        int pageSize = 10;
        var testCount = testRecordCount > pageSize ? pageSize : testRecordCount;

        var listRequest = new ListProviderRequest<DboWeatherForecast>(0, pageSize);

        var query = ListQuery<DboWeatherForecast>.GetQuery(listRequest);
        var result = await broker.ExecuteAsync<DboWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(testCount, result.Items.Count());
        Assert.True(result.TotalItemCount == testRecordCount);
    }

    [Fact]
    public async void TestDvoWeatherForecastListCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;
        var testRecordCount = _weatherTestDataProvider.WeatherForecasts.Count();
        int pageSize = 10;
        var testCount = testRecordCount > pageSize ? pageSize : testRecordCount;

        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, pageSize);

        var query = ListQuery<DvoWeatherForecast>.GetQuery(listRequest);
        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(testCount, result.Items.Count());
        Assert.True(result.TotalItemCount == testRecordCount);
    }

    [Fact]
    public async void TestFKCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var query = new FKListQuery<FkWeatherSummary>();
        var result = await broker.ExecuteAsync<FkWeatherSummary>(query);

        Assert.True(result.Success);
        Assert.Equal(_weatherTestDataProvider.WeatherSummaries.Count(), result.Items.Count());
    }


    [Fact]
    public async void TestFilteredDvoWeatherForecastListCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;
        var locationId = _weatherTestDataProvider.WeatherLocations.First().Uid;

        var testRecordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherLocationId == locationId).Count();
        int pageSize = 10;
        var testCount = testRecordCount > pageSize ? pageSize : testRecordCount;

        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, pageSize, null, $"WeatherLocationId.Equals({locationId})");

        var query = ListQuery<DvoWeatherForecast>.GetQuery(listRequest);
        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(testCount, result.Items.Count());
        Assert.True(result.TotalItemCount == testRecordCount);
    }

    [Fact]
    public async void TestCustomDvoWeatherForecastListCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;
        var locationId = _weatherTestDataProvider.WeatherLocations.First().Uid;

        var testRecordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherLocationId == locationId).Count();
        int pageSize = 10;
        var testCount = testRecordCount > pageSize ? pageSize : testRecordCount;

        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, pageSize);

        var query = WeatherForecastListQuery.GetQuery(locationId, listRequest);
        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(testCount, result.Items.Count());
        Assert.True(result.TotalItemCount == testRecordCount);
    }

}
