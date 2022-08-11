/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class CQSBrokerTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public CQSBrokerTests()
        // Creates an instanc of the Test Data provider
        => _weatherTestDataProvider = WeatherTestDataProvider.Instance();

    private ServiceProvider GetServiceProvider()
    {
        // Creates a Service Collection
        var services = new ServiceCollection();
        // Adds the application services to the collection
        services.AddInMemoryAppServerDataServices();
        // Creates a Service Provider from the Services collection
        // This is our DI container
        var provider = services.BuildServiceProvider();

        // Adds the test data to the in memory database
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>();
        if (factory is not null)
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
        var testCount = testRecordCount > pageSize ? pageSize : testRecordCount ;

        // Create a list request
        var listRequest = new ListProviderRequest<DboWeatherLocation>(0, pageSize);

        // Create a ListQuery and execute the query on the Data Broker against the DboWeatherLocation recordset
        var query = new ListQuery<DboWeatherLocation>(listRequest);
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

        var query = new ListQuery<DboWeatherForecast>(listRequest);
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

        var query = new ListQuery<DvoWeatherForecast>(listRequest);
        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(testCount, result.Items.Count());
        Assert.True(result.TotalItemCount == testRecordCount);
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

        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, pageSize);

        var query = new WeatherForecastListQuery(locationId,listRequest);
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
    public async void TestRecordCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var testRecord = _weatherTestDataProvider.GetRandomRecord()!;
        var CompareRecord = _weatherTestDataProvider.GetDvoWeatherForecast(testRecord);

        var query = new RecordQuery<DvoWeatherForecast>(testRecord.Uid);
        var result = await broker.ExecuteAsync(query);

        Assert.True(result.Success);
        Assert.NotNull(result.Record);
        Assert.Equal(CompareRecord, result.Record!);
    }

    [Fact]
    public async void TestAddCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var newRecord = _weatherTestDataProvider.GetForecast();
        var id = newRecord!.Uid;

        var command = new AddRecordCommand<DboWeatherForecast>(newRecord);
        var result = await broker.ExecuteAsync(command);

        var query = new RecordQuery<DboWeatherForecast>(id);
        var checkResult = await broker.ExecuteAsync(query);

        Assert.True(result.Success);
        Assert.Equal(newRecord, checkResult.Record);
    }

    [Fact]
    public async void TestDeleteCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, 10000);
        var query = new ListQuery<DvoWeatherForecast>(listRequest);

        var startRecords = await broker.ExecuteAsync(query);

        var deleteRecord = _weatherTestDataProvider.GetRandomRecord()!;
        var id = deleteRecord.Uid;

        var command = new DeleteRecordCommand<DboWeatherForecast>(deleteRecord);
        var result = await broker.ExecuteAsync(command);

        var endRecords = await broker.ExecuteAsync(query);

        Assert.True(result.Success);
        Assert.Equal(startRecords.TotalItemCount - 1, endRecords.TotalItemCount);
    }

    [Fact]
    public async void TestUpdateCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, 10000);
        var query = new ListQuery<DvoWeatherForecast>(listRequest);

        var startRecords = await broker.ExecuteAsync(query);

        var editedRecord = _weatherTestDataProvider.GetRandomRecord()! with { Date = DateTime.Now.AddDays(10) };
        var editedDvoRecord = _weatherTestDataProvider.GetDvoWeatherForecast(editedRecord);
        var id = editedRecord.Uid;

        var command = new UpdateRecordCommand<DboWeatherForecast>(editedRecord);
        var result = await broker.ExecuteAsync(command);

        var recordQuery = new RecordQuery<DvoWeatherForecast>(id);
        var updatedRecord = await broker.ExecuteAsync(recordQuery);

        Assert.True(result.Success);
        Assert.Equal(editedDvoRecord, updatedRecord.Record);
    }
}
