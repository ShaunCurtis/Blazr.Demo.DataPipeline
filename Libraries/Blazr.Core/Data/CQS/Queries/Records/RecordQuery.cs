/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public sealed record RecordQuery<TRecord>
    : IRequestAsync<ValueTask<RecordProviderResult<TRecord>>>
    where TRecord : class, new()
{
    private RecordQuery() { }

    public Guid TransactionId { get; init; } = Guid.NewGuid();

    public Guid Uid { get; init; }

    public CancellationToken CancellationToken { get; init; } = default;
 
    public static RecordQuery<TRecord> GetQuery(Guid recordId)
        => new RecordQuery<TRecord> { Uid = recordId };

    public static RecordQuery<TRecord> GetQuery(APIRecordProviderRequest<TRecord> request, CancellationToken cancellationToken = default)
        => new RecordQuery<TRecord> { TransactionId = request.TransactionId, Uid = request.Uid, CancellationToken = cancellationToken };
}
