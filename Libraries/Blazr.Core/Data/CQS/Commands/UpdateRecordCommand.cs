/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record UpdateRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
    where TRecord : class, new()
{
    private UpdateRecordCommand() { }

    public static UpdateRecordCommand<TRecord> GetCommand(TRecord record)
        => new UpdateRecordCommand<TRecord> { Record = record };

    public static UpdateRecordCommand<TRecord> GetCommand(APICommandProviderRequest<TRecord> request, CancellationToken cancellationToken = default)
        => new UpdateRecordCommand<TRecord> { TransactionId = request.TransactionId, Record = request.Record, CancellationToken= cancellationToken };

}
