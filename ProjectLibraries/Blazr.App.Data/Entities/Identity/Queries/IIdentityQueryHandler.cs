using System.Security.Claims;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Data;

public interface IIdentityQueryHandler
    : ICQSHandler<IdentityQuery, ValueTask<IdentityRequestResult>>
{}
