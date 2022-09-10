/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record AddRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
    where TRecord : class, new()
{
    private AddRecordCommand() { }

    public static AddRecordCommand<TRecord> GetCommand(TRecord record)
        => new AddRecordCommand<TRecord> { Record=record};

    public static AddRecordCommand<TRecord> GetCommand(APICommandProviderRequest<TRecord> request, CancellationToken cancellationToken = default)
        => new AddRecordCommand<TRecord>{TransactionId= request.TransactionId, Record=request.Record, CancellationToken = cancellationToken};
}
