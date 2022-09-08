/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public class CQSCommandBrokerTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public CQSCommandBrokerTests()
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

    [Fact]
    public async void TestAddCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var newRecord = _weatherTestDataProvider.GetForecast();
        var id = newRecord!.Uid;

        var command = AddRecordCommand<DboWeatherForecast>.GetCommand(newRecord);
        var result = await broker.ExecuteAsync(command);

        var query = RecordQuery<DboWeatherForecast>.GetQuery(id);
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
        var query = ListQuery<DvoWeatherForecast>.GetQuery(listRequest);

        var startRecords = await broker.ExecuteAsync(query);

        var deleteRecord = _weatherTestDataProvider.GetRandomRecord()!;
        var id = deleteRecord.Uid;

        var command = DeleteRecordCommand<DboWeatherForecast>.GetCommand(deleteRecord);
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
        var query = ListQuery<DvoWeatherForecast>.GetQuery(listRequest);

        var startRecords = await broker.ExecuteAsync(query);

        var editedRecord = _weatherTestDataProvider.GetRandomRecord()! with { Date = DateTime.Now.AddDays(10) };
        var editedDvoRecord = _weatherTestDataProvider.GetDvoWeatherForecast(editedRecord);
        var id = editedRecord.Uid;

        var command = UpdateRecordCommand<DboWeatherForecast>.GetCommand(editedRecord);
        var result = await broker.ExecuteAsync(command);

        var recordQuery = RecordQuery<DvoWeatherForecast>.GetQuery(id);
        var updatedRecord = await broker.ExecuteAsync(recordQuery);

        Assert.True(result.Success);
        Assert.Equal(editedDvoRecord, updatedRecord.Record);
    }

}
