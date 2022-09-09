
using System.Linq.Expressions;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Tests;

public partial class CQSBrokerTests
{
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

    private object sssss(DvoWeatherForecast item)
        => item.TemperatureC;

    [Fact]
    public async void TestFilteredDvoWeatherForecastListCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;
        var locationId = _weatherTestDataProvider.WeatherLocations.First().Uid;

        var testRecordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherLocationId == locationId).Count();
        int pageSize = 10;
        var testCount = testRecordCount > pageSize ? pageSize : testRecordCount;

        Expression<Func<DvoWeatherForecast,object>> SorterExpression = (DvoWeatherForecast item) => item.TemperatureC;
        Expression<Func<DvoWeatherForecast, bool>> filterExpression = (DvoWeatherForecast item) => item.WeatherLocationId == locationId;

        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, pageSize, false, SorterExpression, filterExpression);

        var query = ListQuery<DvoWeatherForecast>.GetQuery(listRequest);
        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(testCount, result.Items.Count());
        Assert.True(result.TotalItemCount == testRecordCount);
    }

    //[Fact]
    //public async void TestCustomDvoWeatherForecastListCQSDataBroker()
    //{
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<ICQSDataBroker>()!;
    //    var locationId = _weatherTestDataProvider.WeatherLocations.First().Uid;

    //    var testRecordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherLocationId == locationId).Count();
    //    int pageSize = 10;
    //    var testCount = testRecordCount > pageSize ? pageSize : testRecordCount;

    //    var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, pageSize);

    //    var query = WeatherForecastListQuery.GetQuery(locationId, listRequest);
    //    var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

    //    Assert.True(result.Success);
    //    Assert.Equal(testCount, result.Items.Count());
    //    Assert.True(result.TotalItemCount == testRecordCount);
    //}

}
