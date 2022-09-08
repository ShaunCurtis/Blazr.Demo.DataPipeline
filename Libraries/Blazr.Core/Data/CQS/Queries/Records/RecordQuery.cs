/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record RecordQuery<TRecord>
    : IRequest<ValueTask<RecordProviderResult<TRecord>>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();

    public Guid GuidId { get; init; }

    public CancellationToken CancellationToken { get; } = new CancellationToken();

    protected RecordQuery() { }

    public static RecordQuery<TRecord> GetQuery(Guid recordId)
        => new RecordQuery<TRecord> { GuidId = recordId };
}
