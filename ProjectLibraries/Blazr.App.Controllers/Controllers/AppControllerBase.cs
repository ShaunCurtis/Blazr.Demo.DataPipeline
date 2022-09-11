/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Controllers;

[TypeFilter(typeof(OperationCanceledException))]
public abstract class AppControllerBase<TRecord>
    : ControllerBase
    where TRecord : class, new()
{
    protected ICQSDataBroker _dataBroker;

    public AppControllerBase(ICQSDataBroker dataBroker)
        => _dataBroker = dataBroker;

    [Mvc.Route("/api/[controller]/listquery")]
    [Mvc.HttpPost]
    public async Task<ListProviderResult<TRecord>> ListQuery([FromBody] APIListProviderRequest<TRecord> request, CancellationToken cancellationToken)
    {
        try
        {
            ListQuery<TRecord> query = ListQuery<TRecord>.GetQuery(request, cancellationToken);
            return await _dataBroker.ExecuteAsync<TRecord>(query);
        }
        catch (Exception e)
        {
            return ListProviderResult<TRecord>.Failure($"Something went seriously wrong - unique reference no: {request.TransactionId} - error detail: {e.Message}");
        }
    }

    [Mvc.Route("/api/[controller]/recordquery")]
    [Mvc.HttpPost]
    public async Task<RecordProviderResult<TRecord>> RecordQuery([FromBody] APIRecordProviderRequest<TRecord> request, CancellationToken cancellationToken)
    {
        try
        {
            RecordQuery<TRecord> query = RecordQuery<TRecord>.GetQuery(request, cancellationToken);
            return await _dataBroker.ExecuteAsync<TRecord>(query);
        }
        catch (Exception e)
        {
            return RecordProviderResult<TRecord>.Failure($"Something went seriously wrong - unique reference no: {request.TransactionId} - error detail: {e.Message}");
        }
    }

    [Mvc.Route("/api/[controller]/addrecordcommand")]
    [Mvc.HttpPost]
    public async Task<CommandResult> AddRecordCommand([FromBody] APICommandProviderRequest<TRecord> request, CancellationToken cancellationToken)
    {
        try
        {
            AddRecordCommand<TRecord> command = AddRecordCommand<TRecord>.GetCommand(request, cancellationToken);
            return await _dataBroker.ExecuteAsync<TRecord>(command);
        }
        catch (Exception e)
        {
            return CommandResult.Failure($"Something went seriously wrong - unique reference no: {request.TransactionId} - error detail: {e.Message}");
        }
    }

    [Mvc.Route("/api/[controller]/updaterecordcommand")]
    [Mvc.HttpPost]
    public async Task<CommandResult> UpdateRecordCommand([FromBody] APICommandProviderRequest<TRecord> request, CancellationToken cancellationToken)
    {
        try
        {
            UpdateRecordCommand<TRecord> command = UpdateRecordCommand<TRecord>.GetCommand(request, cancellationToken);
            return await _dataBroker.ExecuteAsync<TRecord>(command);
        }
        catch (Exception e)
        {
            return CommandResult.Failure($"Something went seriously wrong - unique reference no: {request.TransactionId} - error detail: {e.Message}");
        }
    }

    [Mvc.Route("/api/[controller]/deleterecordcommand")]
    [Mvc.HttpPost]
    public async Task<CommandResult> DeleteRecordCommand([FromBody] APICommandProviderRequest<TRecord> request, CancellationToken cancellationToken)
    {
        try
        {
            DeleteRecordCommand<TRecord> command = DeleteRecordCommand<TRecord>.GetCommand(request, cancellationToken);
            return await _dataBroker.ExecuteAsync<TRecord>(command);
        }
        catch (Exception e)
        {
            return CommandResult.Failure($"Something went seriously wrong - unique reference no: {request.TransactionId} - error detail: {e.Message}");
        }
    }
}