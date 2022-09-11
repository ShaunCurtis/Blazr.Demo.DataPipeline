using Blazr.App.Core;
using Blazr.App.Data;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(Blazr.App.Controllers.DboWeatherForecastController).Assembly));

Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
builder.Services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(dbOptions);

builder.Services.AddAuthentication("GuidTokenAuthentication").AddScheme<GuidTokenAuthOptions, GuidTokenAuthHandler>("GuidTokenAuthentication", null);

var app = builder.Build();

// Add the test data to the In Memory Db
WeatherAppDataServices.AddTestData(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
