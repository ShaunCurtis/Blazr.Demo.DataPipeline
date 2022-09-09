/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Controllers;

[ApiController]
public class DboIdentityController : AppControllerBase<DboIdentity>
{
    public DboIdentityController(ICQSDataBroker dataBroker)
        : base(dataBroker)
    { }
}
