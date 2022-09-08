/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Linq.Dynamic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        if (listQuery.FilterExpressionString is not null)
            query = query
                .Where(listQuery.FilterExpressionString)
                .AsQueryable();

        if (listQuery.SortExpressionString is not null)
            query = query.OrderBy(listQuery.SortExpressionString);

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

        if (listQuery.FilterExpressionString is null)
        {
            IQueryable<TRecord> query = dbContext.Set<TRecord>();

            count = query is IAsyncEnumerable<TRecord>
                ? await query.CountAsync(listQuery.CancellationToken)
                : query.Count();
            return count > 0;
        }

        count = dbContext.Set<TRecord>().Count(listQuery.FilterExpressionString);
        return count > 0;
    }
}
