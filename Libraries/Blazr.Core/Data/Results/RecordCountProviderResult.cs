/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public record RecordCountProviderResult
{
    public int Count { get; init; } = 0;

    public bool Success { get; init; } = false;

    public string Message { get; init; } = string.Empty;

    private RecordCountProviderResult() { }

    public static RecordCountProviderResult Failure(string message)
    => new RecordCountProviderResult { Message = message };

    public static RecordCountProviderResult Successful(int count, string? message = null)
        => new RecordCountProviderResult { Count = count, Success = true, Message = message ?? "The query completed successfully" };
}
