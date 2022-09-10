/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record FKListQuery<TFKRecord>
    : IRequestAsync<ValueTask<FKListProviderResult>>
    where TFKRecord : class, IFkListItem, new()
{
    public Guid TransactionId { get; init; } = Guid.NewGuid();

    public CancellationToken CancellationToken { get; init; } = default;

    public static FKListQuery<TFKRecord> GetQuery(APIFKListQueryProviderRequest<TFKRecord> request, CancellationToken cancellationToken = default)
        => new FKListQuery<TFKRecord> { TransactionId = request.TransactionId, CancellationToken = cancellationToken };
}
