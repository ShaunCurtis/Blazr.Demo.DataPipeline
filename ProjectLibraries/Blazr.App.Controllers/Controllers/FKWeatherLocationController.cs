/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Controllers;

[ApiController]
public class FKWeatherLocationController : FKControllerBase<FkWeatherLocation>
{
    public FKWeatherLocationController(ICQSDataBroker dataBroker)
        : base(dataBroker)
    { }
}
