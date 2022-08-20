/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record ListQuery<TRecord>
    :ListQueryBase<TRecord>
    where TRecord : class, new()
{
    public static ListQuery<TRecord> GetQuery(ListProviderRequest<TRecord> request)
        => new ListQuery<TRecord>
        {
            StartIndex = request.StartIndex,
            PageSize = request.PageSize,
            SortExpressionString = request.SortExpressionString,
            FilterExpressionString = request.FilterExpressionString
        };
}
