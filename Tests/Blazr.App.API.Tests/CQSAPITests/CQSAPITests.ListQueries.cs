using Blazr.App.Core;
using Blazr.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Blazr.App.API.Tests;

public partial class CQSAPITests
{
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
    public async void TestWeatherSummaryFKCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var query = new FKListQuery<FkWeatherSummary>();
        var result = await broker.ExecuteAsync<FkWeatherSummary>(query);

        Assert.True(result.Success);
        Assert.Equal(_weatherTestDataProvider.WeatherSummaries.Count(), result.Items.Count());
    }

    [Fact]
    public async void TestWeatherLocationFKCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var query = new FKListQuery<FkWeatherLocation>();
        var result = await broker.ExecuteAsync<FkWeatherLocation>(query);

        Assert.True(result.Success);
        Assert.Equal(_weatherTestDataProvider.WeatherLocations.Count(), result.Items.Count());
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

        Expression<Func<DvoWeatherForecast, object>> SorterExpression = (DvoWeatherForecast item) => item.TemperatureC;
        Expression<Func<DvoWeatherForecast, bool>> filterExpression = (DvoWeatherForecast item) => item.WeatherLocationId == locationId;

        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, pageSize, false, SorterExpression, filterExpression);

        var query = ListQuery<DvoWeatherForecast>.GetQuery(listRequest);

        var x = APIListProviderRequest<DvoWeatherForecast>.GetRequest(query);

        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(testCount, result.Items.Count());
        Assert.True(result.TotalItemCount == testRecordCount);
    }
}