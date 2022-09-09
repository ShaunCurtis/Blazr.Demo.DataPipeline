﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record DeleteRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
    where TRecord : class, new()
{
    public DeleteRecordCommand() { }

    public static DeleteRecordCommand<TRecord> GetCommand(TRecord record)
        => new DeleteRecordCommand<TRecord> { Record = record};

    public static DeleteRecordCommand<TRecord> GetCommand(APICommandProviderRequest<TRecord> request)
        => new DeleteRecordCommand<TRecord> { TransactionId = request.TransactionId, Record = request.Record };

}
