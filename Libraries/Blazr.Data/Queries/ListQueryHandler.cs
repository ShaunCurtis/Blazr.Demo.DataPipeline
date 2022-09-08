/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Data;

public class ListQueryHandler<TRecord, TDbContext>
    : IListQueryHandler<TRecord>
        where TDbContext : DbContext
        where TRecord : class, new()
{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected int count = 0;

    protected IDbContextFactory<TDbContext> factory;
    protected IListQuery<TRecord> listQuery = default!;

    public ListQueryHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync(IListQuery<TRecord> query)
    {
        if (query is null)
            return ListProviderResult<TRecord>.Failure("No Query Defined");

        listQuery = query;

        if (await this.GetCountAsync())
            await this.GetItemsAsync();

        return ListProviderResult<TRecord>.Successful(this.items, this.count);
    }

    protected virtual async ValueTask<bool> GetItemsAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        if (listQuery.FilterExpression is not null)
            query = query
                .Where(listQuery.FilterExpression)
                .AsQueryable();

        if (listQuery.SortExpression is not null)

            query = listQuery.SortDescending
                ? query.OrderByDescending(listQuery.SortExpression)
                : query.OrderBy(listQuery.SortExpression);

        if (listQuery.PageSize > 0)
            query = query
                .Skip(listQuery.StartIndex)
                .Take(listQuery.PageSize);

        this.items = query is IAsyncEnumerable<TRecord>
            ? await query.ToListAsync(listQuery.CancellationToken)
            : query.ToList();

        return true;
    }

    protected virtual async ValueTask<bool> GetCountAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        if (listQuery.FilterExpression is not null)
            query = query
                .Where(listQuery.FilterExpression)
                .AsQueryable();

        count = query is IAsyncEnumerable<TRecord>
            ? await query.CountAsync(listQuery.CancellationToken)
            : query.Count();
        return count > 0;
    }
}
