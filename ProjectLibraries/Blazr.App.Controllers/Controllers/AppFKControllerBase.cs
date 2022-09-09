/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Controllers;

public class FKControllerBase<TFKRecord>
    : ControllerBase
    where TFKRecord : class, IFkListItem, new()
{
    protected ICQSDataBroker _dataBroker;

    public FKControllerBase(ICQSDataBroker dataBroker)
        => _dataBroker = dataBroker;

    [Mvc.Route("/api/[controller]/query")]
    [Mvc.HttpPost]
    public async Task<FKListProviderResult> FKListQuery([FromBody] APIFKListQueryProviderRequest<TFKRecord> request)
    {
        FKListQuery<TFKRecord> query = FKListQuery<TFKRecord>.GetQuery(request);

        return await _dataBroker.ExecuteAsync<TFKRecord>(query);
    }
}