
using System.Linq.Expressions;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public abstract record ListQueryBase<TRecord>
    :IListQuery<TRecord>
    where TRecord : class, new()
{
    public int StartIndex { get; }

    public int PageSize { get; }

    public bool SortDescending { get; }

    public Expression<Func<TRecord, bool>>? FilterExpression { get; }

    public Expression<Func<TRecord, object>>? SortExpression { get; }

    public Guid TransactionId { get; init; } = Guid.NewGuid();

    public CancellationToken CancellationToken { get; }

    protected ListQueryBase()
        => this.CancellationToken = new CancellationToken();

    protected ListQueryBase(ListProviderRequest<TRecord> request)
    {
        this.StartIndex = request.StartIndex;
        this.PageSize = request.PageSize;
        this.SortExpressionString = request.SortExpressionString;
        this.FilterExpressionString = request.FilterExpressionString;
        this.CancellationToken = request.CancellationToken;
    }
}
