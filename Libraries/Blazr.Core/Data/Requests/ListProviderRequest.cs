/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public readonly struct ListProviderRequest<TRecord>
    where TRecord : class, new()
{
    public int StartIndex { get; }

    public int PageSize { get; }

    public bool SortDescending { get; } = false;

    public Expression<Func<TRecord, bool>>? FilterExpression { get; }

    public Expression<Func<TRecord, object>>? SortExpression { get; }

    public CancellationToken CancellationToken { get; }

    public ListProviderRequest()
    {
        StartIndex = 0;
        PageSize = 10000;
        CancellationToken = new CancellationToken();
        SortExpression = null;
        FilterExpression = null;
    }
    public ListProviderRequest(int startIndex, int pageSize)
    {
        StartIndex = startIndex;
        PageSize = pageSize;
        CancellationToken = new CancellationToken();
        SortExpression = null;
        FilterExpression = null;
    }

    public ListProviderRequest(int startIndex, int pageSize, bool sortDescending = false, Expression<Func<TRecord, object>>? sortExpression = null, Expression<Func<TRecord, bool>>? filterExpression = null)
    {
        StartIndex = startIndex;
        PageSize = pageSize;
        CancellationToken = new CancellationToken();
        SortDescending = sortDescending;
        SortExpression = sortExpression;
        FilterExpression = filterExpression;
    }
}
