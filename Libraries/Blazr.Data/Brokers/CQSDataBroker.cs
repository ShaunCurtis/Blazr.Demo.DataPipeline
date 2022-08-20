/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Data;

public class CQSDataBroker<TDbContext>
    :ICQSDataBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;
    private readonly IServiceProvider _serviceProvider;

    public CQSDataBroker(IDbContextFactory<TDbContext> factory, IServiceProvider serviceProvider)
    { 
        _factory = factory;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(ListQuery<TRecord> query) where TRecord : class, new()
    {
        var handler = new ListQueryHandler<TRecord, TDbContext>(_factory);
        return await handler.ExecuteAsync(query);
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(IListQuery<TRecord> query) where TRecord : class, new()
    {
        var queryType = query.GetType();
        var handler = _serviceProvider.GetService<IListQueryHandler<TRecord>>();
        if (handler == null)
            throw new NullReferenceException("No Handler service registed for the List Query");

        return await handler.ExecuteAsync(query);
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordQuery<TRecord> query) where TRecord : class, new()
    {
        var handler = new RecordQueryHandler<TRecord, TDbContext>(_factory);
        return await handler.ExecuteAsync(query);
    }

    public async ValueTask<FKListProviderResult> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new()
    {
        var handler = new FKListQueryHandler<TRecord, TDbContext>(_factory);
        return await handler.ExecuteAsync(query);
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new()
    {
        var handler = new AddRecordCommandHandler<TRecord, TDbContext>(_factory);
        return await handler.ExecuteAsync(command);
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(UpdateRecordCommand<TRecord> command) where TRecord : class, new()
    {
        var handler = new UpdateRecordCommandHandler<TRecord, TDbContext>(_factory);
        return await handler.ExecuteAsync(command);
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(DeleteRecordCommand<TRecord> command) where TRecord : class, new()
    {
        var handler = new DeleteRecordCommandHandler<TRecord, TDbContext>(_factory);
        return await handler.ExecuteAsync(command);
    }

    public ValueTask<object> ExecuteAsync<TRecord>(object query)
        => throw new NotImplementedException();
}
