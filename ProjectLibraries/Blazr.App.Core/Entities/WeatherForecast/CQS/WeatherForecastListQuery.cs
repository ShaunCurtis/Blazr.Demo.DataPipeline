
using Blazr.Core;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record WeatherForecastListQuery
    : ListQueryBase<DvoWeatherForecast>
{
    public Guid WeatherLocationId { get; init; }

    private WeatherForecastListQuery() { }

    public static WeatherForecastListQuery GetQuery(Guid weatherLocationId, ListProviderRequest<DvoWeatherForecast> request)
        => new WeatherForecastListQuery
        {
            StartIndex = request.StartIndex,
            PageSize = request.PageSize,
            SortExpressionString = request.SortExpressionString,
            FilterExpressionString = request.FilterExpressionString,
            WeatherLocationId = weatherLocationId,
        };
}
