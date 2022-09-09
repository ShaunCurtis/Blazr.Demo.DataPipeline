
using System.Net;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Controllers;

public abstract class AppControllerBase<TRecord>
    : ControllerBase
    where TRecord : class, new()
{
    protected ICQSDataBroker _dataBroker;

    public AppControllerBase(ICQSDataBroker dataBroker)
        => _dataBroker = dataBroker;

    [Mvc.Route("/api/[controller]/listquery")]
    [Mvc.HttpPost]
    public async Task<ListProviderResult<TRecord>> ListQuery([FromBody] APIListProviderRequest<TRecord> request)
    {
        ListQuery<TRecord> query = ListQuery<TRecord>.GetQuery(request);
        return await _dataBroker.ExecuteAsync<TRecord>(query);

    }

    [Mvc.Route("/api/[controller]/recordquery")]
    [Mvc.HttpPost]
    public async Task<RecordProviderResult<TRecord>> RecordQuery([FromBody] APIRecordProviderRequest<TRecord> request)
    {
        RecordQuery<TRecord> query = RecordQuery<TRecord>.GetQuery(request);
        return await _dataBroker.ExecuteAsync<TRecord>(query);
    }

    [Mvc.Route("/api/[controller]/addrecordcommand")]
    [Mvc.HttpPost]
    public async Task<CommandResult> AddRecordCommand([FromBody] APICommandProviderRequest<TRecord> request)
    {
        AddRecordCommand<TRecord> command = AddRecordCommand<TRecord>.GetCommand(request);
        return await _dataBroker.ExecuteAsync<TRecord>(command);
    }

    [Mvc.Route("/api/[controller]/updaterecordcommand")]
    [Mvc.HttpPost]
    public async Task<CommandResult> UpdateRecordCommand([FromBody] APICommandProviderRequest<TRecord> request)
    {
        UpdateRecordCommand<TRecord> command = UpdateRecordCommand<TRecord>.GetCommand(request);
        return await _dataBroker.ExecuteAsync<TRecord>(command);
    }

    [Mvc.Route("/api/[controller]/deleterecordcommand")]
    [Mvc.HttpPost]
    public async Task<CommandResult> DeleteRecordCommand([FromBody] APICommandProviderRequest<TRecord> request)
    {
        try
        {
            DeleteRecordCommand<TRecord> command = DeleteRecordCommand<TRecord>.GetCommand(request);
            return await _dataBroker.ExecuteAsync<TRecord>(command);
        }
        catch (Exception e)
        {
            return ErrorReponse(request.TransactionId, e.Message);
        }
    }

    private HttpResponseMessage ErrorReponse(Guid id, string message)
    {
        HttpResponseMessage response = new HttpResponseMessage();
        response.Content = new StringContent($"<html><body><div>SometHing went seriously wrong with your requeast and has been logged with the Unique code of {id}</div><div>{message}</div></body></html>");
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
        response.StatusCode = HttpStatusCode.InternalServerError;
        return response;

    }
}