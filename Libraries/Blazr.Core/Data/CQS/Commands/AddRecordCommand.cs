/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class AddRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
    where TRecord : class, new()
{
    private AddRecordCommand(TRecord record) : base(record) { }

    public static AddRecordCommand<TRecord> GetCommand(TRecord record)
        => new AddRecordCommand<TRecord>(record);
}
