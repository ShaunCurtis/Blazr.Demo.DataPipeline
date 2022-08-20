/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record FKListProviderResult
{
    public IEnumerable<IFkListItem> Items { get; init; } = Enumerable.Empty<IFkListItem>();

    public bool Success { get; init; }

    public string? Message { get; init; }

    protected FKListProviderResult() { }

    public static FKListProviderResult Failure(string message)
        => new FKListProviderResult { Message = message };

    public static FKListProviderResult Successful(IEnumerable<IFkListItem> items, string? message = null)
        => new FKListProviderResult { Items = items, Success = true, Message = message ?? "The query completed successfully" };
}
