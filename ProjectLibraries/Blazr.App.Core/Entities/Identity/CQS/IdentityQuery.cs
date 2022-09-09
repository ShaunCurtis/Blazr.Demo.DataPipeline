/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record IdentityQuery
    : IRequestAsync<ValueTask<IdentityRequestResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();

    public CancellationToken CancellationToken { get; init; } = new CancellationToken();

    public Guid IdentityId { get; init; } = Guid.Empty;

    public static IdentityQuery GetQuery(Guid Uid)
        => new IdentityQuery { IdentityId = Uid };
}
