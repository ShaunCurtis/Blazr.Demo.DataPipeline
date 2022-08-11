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

    public string? SortExpressionString { get; }

    public string? FilterExpressionString { get; }

    public ListProviderRequest()
    {
        StartIndex = 0;
        PageSize = 10000;
        SortExpressionString = null;
        FilterExpressionString = null;
    }
    public ListProviderRequest(int startIndex, int pageSize)
    {
        StartIndex = startIndex;
        PageSize = pageSize;
        SortExpressionString = null;
        FilterExpressionString = null;
    }

    public ListProviderRequest(int startIndex, int pageSize, string? sortExpressionString = null, string? filterExpressionString = null)
    {
        StartIndex = startIndex;
        PageSize = pageSize;
        SortExpressionString = sortExpressionString;
        FilterExpressionString = filterExpressionString;
    }
}
