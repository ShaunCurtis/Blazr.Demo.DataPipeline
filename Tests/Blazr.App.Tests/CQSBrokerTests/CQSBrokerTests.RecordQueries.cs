/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Tests;

public partial class CQSBrokerTests
{
    [Fact]
    public async void TestRecordCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;

        var testRecord = _weatherTestDataProvider.GetRandomRecord()!;
        var CompareRecord = _weatherTestDataProvider.GetDvoWeatherForecast(testRecord);

        var query = RecordQuery<DvoWeatherForecast>.GetQuery(testRecord.Uid);
        var result = await broker.ExecuteAsync(query);

        Assert.True(result.Success);
        Assert.NotNull(result.Record);
        Assert.Equal(CompareRecord, result.Record!);
    }
}
