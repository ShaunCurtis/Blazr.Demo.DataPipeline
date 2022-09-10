using Blazr.App.Data;
using Blazr.Core;
using Blazr.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blazr.App.API.Tests;

public partial class CQSAPITests
{
    public const string APIUrl = "https://localhost:7161/";
    private WeatherTestDataProvider _weatherTestDataProvider;

    public CQSAPITests()
    // Creates an instance of the Test Data provider
    => _weatherTestDataProvider = WeatherTestDataProvider.Instance();


    private ServiceProvider GetServiceProvider()
    {
        // Creates a Service Collection
        var services = new ServiceCollection();
        // Adds the application services to the collection
        services.AddScoped<ICQSDataBroker, CQSAPIDataBroker>();
        // Add the Http Client with the correct Url
        services.AddScoped<HttpClient>(s =>
        {
            return new HttpClient
            {
                BaseAddress = new Uri(APIUrl)
            };
        });
        // Creates a Service Provider from the Services collection
        // This is our DI container
        var provider = services.BuildServiceProvider();

        return provider!;
    }
}