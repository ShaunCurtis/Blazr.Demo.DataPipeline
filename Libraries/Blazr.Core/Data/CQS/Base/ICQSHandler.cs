﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface ICQSHandler<in TRequest, out TResult>
    where TRequest : ICQSRequest<TResult>
{
    TResult ExecuteAsync();
}
