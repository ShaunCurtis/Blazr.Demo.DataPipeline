/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public abstract record RecordCommandBase<TRecord>
     : IRecordCommand<TRecord>
    where TRecord : class, new()
{
    public Guid TransactionId { get; init; } = Guid.NewGuid();

    public TRecord Record { get; protected init; } = default!;

    public CancellationToken CancellationToken { get; } = new CancellationToken();

    protected RecordCommandBase() { }

    protected RecordCommandBase(TRecord record)
        => this.Record = record;
}
