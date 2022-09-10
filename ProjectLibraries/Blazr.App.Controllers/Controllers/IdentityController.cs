/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Controllers;

[ApiController]
public class IdentityController
    : ControllerBase
{
    protected IIdentityQueryHandler _identityCQSHandler;

    public IdentityController(IIdentityQueryHandler identityCQSHandler)
        => _identityCQSHandler = identityCQSHandler;

    [Mvc.HttpPost]
    [Mvc.Route("/api/[controller]/authenicate")]
    public async Task<IdentityRequestResult> GetIdentity([FromBody] APIIdentityProviderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            IdentityQuery query = IdentityQuery.GetQuery(request, cancellationToken);
            return await _identityCQSHandler.ExecuteAsync(query);
        }
        catch (Exception e)
        {
            return IdentityRequestResult.Failure($"Something went seriously wrong - unique reference no: {request.TransactionId} - error detail: {e.Message}");
        }
    }
}
