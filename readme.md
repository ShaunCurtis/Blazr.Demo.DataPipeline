# Building A Succinct DoteNetCore CQS Data Pipeline

The CQS implementations I've seen have always looked incredibly verbose: the number of classes scared me.

I recently had cause to revisit CQS on an application re-write and decided to work on creating a more succinct implementation.  This article describes what I've achieved.

## Test Data

The Appendix provides a summary of the data classes and test data provider.  There's a full description in the documents section of the repository.

## Repository

The data repository associated with this article is here: [Blazr.Demo.DataPipeline](https://github.com/ShaunCurtis/Blazr.Demo.DataPipeline).  There's some extra documents in the repository on design and detail on the data classes, test data provider and database context. 

## Introduction

CQS - not to be confused with CQRS - is fundimentally a programming style.  Every action is either:

1. A *Command* - a request to make a data change.
2. A *Query* - a request to get some data.

A *Command* returns either status information or nothing.  Commands **NEVER** returns data.

A *Query* returns a data set.  Queries **NEVER** makes changes to the state of the data.  There are **NO SIDE EFFECTS** to making a query.

It's a excellent pattern to apply universally across your code.

Each action has a *Command/Query* class that defines the action and a *Handler* class to execute the defined action.  Normally a one-to-one relationship: a unique handler for every request.

In essence:

- A *Request* object defines the information a *Handler* needs to execute the request and what it expects in return - the *Result* .

- A *Handler* object executes the necessary code and returns the defined *Result* using data provided by the *Request*.  

Conceptually it's very simple, and relatively easy to implement.  The problem is each database action requires a request and a handler object. Lots of classes defining and repeating the same old code.

## Solution Layout and Design

The solution consists of a set of libraries organised on Clean Design principles.  It's designed to work in any DotNetCore environment.  `Blazr.Core` and `Blazr.Data` are the two base libraries that can be used for any implementation.  `Blazr.Demo.Core` and `Blazr.Demo.Data` are the two application specific libraries.

The front end application is an XUnit test project to both demonstrate and test the code.

I use it in Blazor projects. 

## Interfaces and Base Classes

The basic methodology can be defined by two generic interfaces.

`ICQSRequest` defines any request:

1. It says the request produces an output defined as `TResult`.
2. It has a unique `TransactionId` to track the transaction (if required and implemented).

```csharp
public interface ICQSRequest<out TResult>
{
    public Guid TransactionId { get;}
}
```

`ICQSHandler` defines any handler that executes an `ICQSRequest` instance:

1. The handler gets a `TRequest` which implements the `ICQSRequest` interface.
2. The handler outputs a `TResult` as defined in the `ICQSRequest` interface.
3. It has a single `ExecuteAsync` method that takes a `TRequest` and returns `TResult`.

```csharp
public interface ICQSHandler<in TRequest, out TResult>
    where TRequest : ICQSRequest<TResult>
{
    TResult ExecuteAsync(TRequest request);
}
```

To build a more succinct implementation:

 - We must Accept the 80/20 rule.  Not every request can be fulfilled with our our standard implementation, but 80% is a lot of effort and classes to save on.
 - We need a methodology for the 20%.
 - We need a "compliant" generics based ORM to interface with our data store.  This implementation uses *Entity Framework* which provides that. 
 - Code some quite complicated generics in the base classes to abstract functionality into boilerplate code.

## Results

The solution defines a set of standard results to return: `TResult` of the request.  They are defined as `record` with static constructors and contains status information and, if a query, data.  They must be serializable to use in APIs.  Each is shown below:

```csharp
public record ListProviderResult<TRecord>
{
    public IEnumerable<TRecord> Items { get; init; }
    public int TotalItemCount { get; init; }
    public bool Success { get; init; }
    public string? Message { get; init; }
    //....Constructors
}
```
```csaharp
public record RecordProviderResult<TRecord>
{
    public TRecord? Record { get; init; }
    public bool Success { get; init; }
    public string? Message { get; init; }
    //....Constructors
}
```
```csaharp
public record CommandResult
{
    public Guid NewId { get; init; }
    public bool Success { get; init; }
    public string Message { get; init; }
    //....Constructors
}
```
```csaharp
public record FKListProviderResult
{
    public IEnumerable<IFkListItem> Items { get; init; }
    public bool Success { get; init; }
    public string? Message { get; init; }
    //....Constructors
}
```

All implement static constructors to tightly control the content.

## Base Classes

`TRecord` represents the data classes retrieved from the data store using the ORM.  It's qualified as a `class` that implements an empty constructor `new()`.

The Request interface/class structure looks like this:

![CQS Request Classes](./documents/images/cqs-request-classes.png)

And the Handler interface/class structure looks like this:

![CQS Handler Classes](./documents/images/cqs-handler-classes.png)


### Commands

All commands:

1. Take a record which we define as `TRecord`.
2. Fix `TResult` as an async `ValueTask<CommandResult>`.

An interface that implements `ICQSRequest` and this functionality.

```csharp
public interface IRecordCommand<TRecord> 
    : ICQSRequest<ValueTask<CommandResult>>
{
    public TRecord Record { get;}
}
```

And an abstract implementation.

```csharp
public abstract class RecordCommandBase<TRecord>
     : IRecordCommand<TRecord>
{
    public Guid TransactionId { get; } = Guid.NewGuid(); 
    public TRecord Record { get; protected set; } = default!;

    protected RecordCommandBase() { }
}
```

We can now define the Add/Delete/Update specific commands.  All use static constructors to control and validate content.  There needs to be a one-to-one relationship (requests -> handlers) so we define a handler for each type of command.


```csharp
public class AddRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
{
    private AddRecordCommand() { }

    public static AddRecordCommand<TRecord> GetCommand(TRecord record)
        => new AddRecordCommand<TRecord> { Record = record };
}
```
```csharp
public class DeleteRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
{
    private DeleteRecordCommand() { }

    public static DeleteRecordCommand<TRecord> GetCommand(TRecord record)
        => new DeleteRecordCommand<TRecord> { Record = record };
}
```
```csharp
public class UpdateRecordCommand<TRecord>
     : RecordCommandBase<TRecord>
{
    private UpdateRecordCommand() { }

    public static UpdateRecordCommand<TRecord> GetCommand(TRecord record)
        => new UpdateRecordCommand<TRecord> { Record = record };
}
```

#### Command Handlers

There's no benefit in creating interfaces or base classes for handlers so we implement Create/Update/Delete commands as three separate classes.  `TRecord` defines the record class and `TDbContext` the `DbContext` used in the DI `DbContextFactory`.

We use the built in generic methods in `DbContext`, so don't need the specific `DbContext`.   `Set<TRecord>` method finds the `DbSet` instances of `TRecord` and `Update<TRecord>`, `Add<TRecord>` and `Delete<TRecord>` methods with `SaveChangesAsync` implement the commands. 

All the handlers follow the same pattern.

1. The constructor passes in the DbContext factory and the command request to execute.
2. `Execute`:
   1. Gets a DbContext.
   2. Calls the generic `Add/Update/Delete` on the context passing in the record.  Internally EF finds the recordset and the specific record and replaces it with the one supplied.
   3. Calls `SaveChanges` on the DbContext that commits the changes to the data store.
   4. Checks we have one change and returns a `CommandResult`.

This is the Add Handler:

```csharp
public class AddRecordCommandHandler<TRecord, TDbContext>
    : ICQSHandler<AddRecordCommand<TRecord>, ValueTask<CommandResult>>
    where TDbContext : DbContext
    where TRecord : class, new()
{
    protected IDbContextFactory<TDbContext> factory;

    public AddRecordCommandHandler(IDbContextFactory<TDbContext> factory)
        =>  this.factory = factory;

    public async ValueTask<CommandResult> ExecuteAsync(AddRecordCommand<TRecord> command)
    {
        using var dbContext = factory.CreateDbContext();
        dbContext.Add<TRecord>(command.Record);
        return await dbContext.SaveChangesAsync() == 1
            ? CommandResult.Successful("Record Saved")
            : CommandResult.Failure("Error saving Record");
    }
}
``` 

### Queries

Queries aren't quite so uniform.

1. There are various types of `TResult`.
2. They have specific operations such as *Where* and *OrderBy*.

To handle these requirements we define three Query requests:

#### RecordQuery

This returns a `RecordProviderResult` containing a single record based on a provided Uid.

```csharp
public record RecordQuery<TRecord>
    : ICQSRequest<ValueTask<RecordProviderResult<TRecord>>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
    public Guid GuidId { get; init; }

    protected RecordQuery() { }

    public static RecordQuery<TRecord> GetQuery(Guid recordId)
        => new RecordQuery<TRecord> { GuidId = recordId };
}
```

#### ListQuery

This returns a `ListProviderResult` containing a *paged* `IEnumerable` of `TRecord`.

We define an interface:

```
public interface IListQuery<TRecord>
    : ICQSRequest<ValueTask<ListProviderResult<TRecord>>>
    where TRecord : class, new()
{
    public int StartIndex { get; }
    public int PageSize { get; }
    public string? SortExpressionString { get; }
    public string? FilterExpressionString { get; }
}
```

A base abstract implementation:

```csharp
public abstract record ListQueryBase<TRecord>
    :IListQuery<TRecord>
    where TRecord : class, new()
{
    public int StartIndex { get; init; }
    public int PageSize { get; init; }
    public string? SortExpressionString { get; init; }
    public string? FilterExpressionString { get; init; }
    public Guid TransactionId { get; init; } = Guid.NewGuid();

    protected ListQueryBase() { }
}
```

And finally a generic query:

```csharp
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
```

We separate the code into the interface/abstract base class pattern so can implement custom List queries.  If these inherit from `ListQuery`, we run into issues with factories and pattern matching methods.  Using a base class to implement the boilerplate code solves this problem.

#### FKListQuery

This returns a `FkListProviderResult` containing an `IEnumerable` of `IFkListItem`.  `FkListItem` is a simple object containing a *Guid/Name* pair.  It's principle use is in foreign key *Select* controls in the UI.

```csharp
public record FKListQuery<TRecord>
    : ICQSRequest<ValueTask<FKListProviderResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
}
```

### Query Handlers

The corresponding query handlers are:

#### RecordQueryHandler

Creating a "generic" version can be challenging depending on the ORM.

The key concepts to note are:

1. The use of *unit of work* `DbContexts` from the `IDbContextFactory`.
2. `_dbContext.Set<TRecord>()` gets the `DbSet` for `TRecord`.
3. The use of two methodologies to apply the query.  

```csharp
public class RecordQueryHandler<TRecord, TDbContext>
    : ICQSHandler<RecordQuery<TRecord>, ValueTask<RecordProviderResult<TRecord>>>
        where TRecord : class, new()
        where TDbContext : DbContext
{
    private IDbContextFactory<TDbContext> _factory;

    public RecordQueryHandler(IDbContextFactory<TDbContext> factory)
        =>  _factory = factory;

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync(RecordQuery<TRecord> query)
    {
        var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        TRecord? record = null;

        // first check if the record implements IRecord.  If so we can do a cast and then do the query via the Uid property directly 
        if ((new TRecord()) is IRecord)
            record = await dbContext.Set<TRecord>().SingleOrDefaultAsync(item => ((IRecord)item).Uid == query.GuidId);

        // Try and use the EF FindAsync implementation
        if (record is null)
                record = await dbContext.FindAsync<TRecord>(query.GuidId);

        if (record is null)
            return RecordProviderResult<TRecord>.Failure("No record retrieved");

        return RecordProviderResult<TRecord>.Successful(record);
    }
}
```

#### ListQueryHandler

The key concepts to note here are:

1. The use of *unit of work* `DbContexts` from the `IDbContextFactory`.
2. `_dbContext.Set<TRecord>()` to get the `DbSet` for `TRecord`.
3. The use of `IQueryable` to build queries.
4. The need for two queries.  One to get the "paged" recordset and one to get the total record count.
5. The use of `System.Linq.Dynamic` to do the sorting and filtering.  This will be discussed later.

```csharp
public class ListQueryHandler<TRecord, TDbContext>
    : IListQueryHandler<TRecord>
        where TDbContext : DbContext
        where TRecord : class, new()
{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected int count = 0;

    protected IDbContextFactory<TDbContext> factory;
    protected IListQuery<TRecord> listQuery = default!;

    public ListQueryHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync(IListQuery<TRecord> query)
    {
        if (query is null)
            return ListProviderResult<TRecord>.Failure("No Query Defined");

        listQuery = query;

        if (await this.GetItemsAsync())
            await this.GetCountAsync();

        return ListProviderResult<TRecord>.Successful(this.items, this.count);
    }

    protected virtual async ValueTask<bool> GetItemsAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        if (listQuery.FilterExpressionString is not null)
            query = query
                .Where(listQuery.FilterExpressionString)
                .AsQueryable();

        if (listQuery.SortExpressionString is not null)
            query = query.OrderBy(listQuery.SortExpressionString);

        if (listQuery.PageSize > 0)
            query = query
                .Skip(listQuery.StartIndex)
                .Take(listQuery.PageSize);

        if (query is IAsyncEnumerable<TRecord>)
            this.items = await query.ToListAsync();
        else
            this.items = query.ToList();

        return true;
    }

    protected virtual async ValueTask<bool> GetCountAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        if (listQuery.FilterExpressionString is not null)
            query = query
                .Where(listQuery.FilterExpressionString)
                .AsQueryable();

        if (query is IAsyncEnumerable<TRecord>)
            count = await query.CountAsync();
        else
            count = query.Count();

        return true;
    }
}
```

#### FKListQueryHandler

```csharp
public class FKListQueryHandler<TRecord, TDbContext>
    : ICQSHandler<FKListQuery<TRecord>, ValueTask<FKListProviderResult>>
        where TDbContext : DbContext
        where TRecord : class, IFkListItem, new()
{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected IDbContextFactory<TDbContext> factory;

    public FKListQueryHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<FKListProviderResult> ExecuteAsync(FKListQuery<TRecord> listQuery)
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        if (listQuery is null)
            return FKListProviderResult.Failure("No Query defined");

        IEnumerable<TRecord> dbSet = await dbContext.Set<TRecord>().ToListAsync();

        return FKListProviderResult.Successful(dbSet);
    }
}
```

## Implementing the Handlers

The handlers are designed to be use in two ways:

1. Individually as dependancy injected services.
2. Though a dependancy injected factory.

we'll see both used in testing.

### The Generic Factory Broker

The broker uses a single method `ExecuteAsync(Request)`, with implementations for each request that maps the correct handler, executes the request and provides the expected result.

```csharp
var TResult = await DataBrokerInstance.ExecuteAsync<TRecord>(TRequest);
```

The interface used to define the service in DI:

```csharp
public interface ICQSDataBroker
{    
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new();
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(UpdateRecordCommand<TRecord> command) where TRecord : class, new();
    public ValueTask<CommandResult> ExecuteAsync<TRecord>(DeleteRecordCommand<TRecord> command) where TRecord : class, new();
    public ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(ListQuery<TRecord> query) where TRecord : class, new();
    public ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordQuery<TRecord> query) where TRecord : class, new();
    public ValueTask<FKListProviderResult> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new();
}
```

And a *Server* Broker implementation:

```csharp
public class CQSDataBroker<TDbContext>
    :ICQSDataBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;
    private readonly IServiceProvider _serviceProvider;

    public CQSDataBroker(IDbContextFactory<TDbContext> factory, IServiceProvider serviceProvider)
    { 
        _factory = factory;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(AddRecordCommand<TRecord> command) where TRecord : class, new()
    {
        var handler = new AddRecordCommandHandler<TRecord, TDbContext>(_factory, command);
        return await handler.ExecuteAsync();
    }

    //.... Update and Delete ExecuteAsyncs

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(ListQuery<TRecord> query) where TRecord : class, new()
    {
        var handler = new ListQueryHandler<TRecord, TDbContext>(_factory, query);
        return await handler.ExecuteAsync();
    }

    public async ValueTask<RecordProviderResult<TRecord>> ExecuteAsync<TRecord>(RecordQuery<TRecord> query) where TRecord : class, new()
    {
        var handler = new RecordQueryHandler<TRecord, TDbContext>(_factory, query);
        return await handler.ExecuteAsync();
    }

    public async ValueTask<FKListProviderResult> ExecuteAsync<TRecord>(FKListQuery<TRecord> query) where TRecord : class, IFkListItem, new()
    {
        var handler = new FKListQueryHandler<TRecord, TDbContext>(_factory, query);
        return await handler.ExecuteAsync();
    }

    public ValueTask<object> ExecuteAsync<TRecord>(object query)
        => throw new NotImplementedException();
}
```

Note the catch all method that throws an expection. 

### Testing the Broker

#### SetUp

Here's the setup for the Broker demo tests.  It sets up a DI services container and passes the instance to the test.


```csharp
public CQSBrokerTests()
    // Creates an instance of the Test Data provider
    => _weatherTestDataProvider = WeatherTestDataProvider.Instance();

private ServiceProvider GetServiceProvider()
{
    // Creates a Service Collection
    var services = new ServiceCollection();

    // Adds the application services to the collection
    Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
    services.AddDbContextFactory<TDbContext>(options);
    services.AddSingleton<ICQSDataBroker, CQSDataBroker<InMemoryWeatherDbContext>>();

    // Creates a Service Provider from the Services collection
    // This is our DI container
    var provider = services.BuildServiceProvider();

    // Adds the test data to the in memory database
    var factory = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>();
    WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherDbContext>(factory);

    return provider!;
}
```

#### Tests

A typical test to get a list of Weather Locations:

```csharp
[Fact]
public async void TestWeatherLocationListCQSDataBroker()
{
    // Build our DI container
    var provider = GetServiceProvider();
    //Get the Data Broker
    var broker = provider.GetService<ICQSDataBroker>()!;

    // Get the control record count from the Test Data Provider
    var testRecordCount = _weatherTestDataProvider.WeatherLocations.Count();
    int pageSize = 10;
    // Get the expected recordset count.
    // It should be either the page size or the total record count if that's smaller
    var testCount = testRecordCount > pageSize ? pageSize : testRecordCount ;

    // Create a list request
    var listRequest = new ListProviderRequest<DboWeatherLocation>(0, pageSize);

    // Create a ListQuery and execute the query on the Data Broker against the DboWeatherLocation recordset
    var query = new ListQuery<DboWeatherLocation>(listRequest);
    var result = await broker.ExecuteAsync<DboWeatherLocation>(query);

    // Check we have success
    Assert.True(result.Success);
    // Check the recordset count
    Assert.Equal(testCount, result.Items.Count());
    // Check the total count os correct against the test provider
    Assert.True(result.TotalItemCount == testRecordCount);
}
```

And a Add command Test:

```csharp
[Fact]
public async void TestAddCQSDataBroker()
{
    var provider = GetServiceProvider();
    var broker = provider.GetService<ICQSDataBroker>()!;
    var newRecord = _weatherTestDataProvider.GetForecast();
    var id = newRecord!.Uid;

    var command = new AddRecordCommand<DboWeatherForecast>(newRecord);
    var result = await broker.ExecuteAsync(command);

    var query = new RecordQuery<DboWeatherForecast>(id);
    var checkResult = await broker.ExecuteAsync(query);

    Assert.True(result.Success);
    Assert.Equal(newRecord, checkResult.Record);
}
```
## Custom Requests

### Filtered Lists

This is probably the most common custom request.  The standard `ListQuery` uses Dynamic Linq, so you can build the query as a string to pass in the query.  However, Dynamic Linq is not efficient, so I prefer to define custom queries wherever I use them a lot.

All such queries can use a customized `BaseListQuery`.

Our example custom query filters the WeatherForecast based on the Location.

#### Query

1. Inherits from `ListQueryBase` fixing `TRecord` as `DvoWeatherForecast`.
2. Defines a `WeatherLocationId` property.
3. Defines a static creator method.

```csharp
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
```

#### Handler

This is built on the same pattern as the generic handler.

```csharp
public class WeatherForecastListQueryHandler<TDbContext>
    : IListQueryHandler<DvoWeatherForecast>
        where TDbContext : DbContext
{
    protected IEnumerable<DvoWeatherForecast> items = Enumerable.Empty<DvoWeatherForecast>();
    protected int count = 0;

    protected IDbContextFactory<TDbContext> factory;
    protected WeatherForecastListQuery listQuery = default!;

    public WeatherForecastListQueryHandler(IDbContextFactory<TDbContext> factory)
    {
        this.factory = factory;
    }

    public async ValueTask<ListProviderResult<DvoWeatherForecast>> ExecuteAsync(IListQuery<DvoWeatherForecast> query)
    {
        if (query is null || query is not WeatherForecastListQuery)
            return new ListProviderResult<DvoWeatherForecast>(new List<DvoWeatherForecast>(), 0, false, "No Query Defined");

        listQuery = (WeatherForecastListQuery)query;

        if (await this.GetItemsAsync())
            await this.GetCountAsync();

        return new ListProviderResult<DvoWeatherForecast>(this.items, this.count);
    }

    protected virtual async ValueTask<bool> GetItemsAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<DvoWeatherForecast> query = dbContext.Set<DvoWeatherForecast>();

        if (listQuery.WeatherLocationId != Guid.Empty)
            query = query
                .Where(item => item.WeatherLocationId == listQuery.WeatherLocationId)
                .AsQueryable();

        if (listQuery.SortExpressionString is not null)
            query = query.OrderBy(listQuery.SortExpressionString);

        if (listQuery.PageSize > 0)
            query = query
                .Skip(listQuery.StartIndex)
                .Take(listQuery.PageSize);

        if (query is IAsyncEnumerable<DvoWeatherForecast>)
            this.items = await query.ToListAsync();
        else
            this.items = query.ToList();

        return true;
    }

    protected virtual async ValueTask<bool> GetCountAsync()
    {
        var dbContext = this.factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        IQueryable<DvoWeatherForecast> query = dbContext.Set<DvoWeatherForecast>();

        if (listQuery.WeatherLocationId != Guid.Empty)
            query = query
                .Where(item => item.WeatherLocationId == listQuery.WeatherLocationId)
                .AsQueryable();

        if (query is IAsyncEnumerable<DvoWeatherForecast>)
            count = await query.CountAsync();
        else
            count = query.Count();

        return true;
    }
}
```

The handler can be defined in DI:

```csharp
services.AddScoped<IListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();
```

#### Broker

We can add a method into the standard broker to handle `IListQueryHandler<TRecord>`.  Note we can only define one `IListQueryHandler` per data class using this methodology.

The `ICQSDataBroker` definition: 

```csharp
public interface ICQSDataBroker
{
    public ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(IListQuery<TRecord> query) where TRecord : class, new();
}
```

And the implementation in `CQSDataBroker`:

```csharp
public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync<TRecord>(IListQuery<TRecord> query) where TRecord : class, new()
{
    var queryType = query.GetType();
    var handler = _serviceProvider.GetService<IListQueryHandler<TRecord>>();
    if (handler == null)
        throw new NullReferenceException("No Handler service registed for the List Query");

    return await handler.ExecuteAsync(query);
}
```

#### Testing

Update `CQSBrokerTests` adding the custom Handler:

```csharp
    private ServiceProvider GetServiceProvider()
    {
        // Creates a Service Collection
        var services = new ServiceCollection();
        // Adds the application services to the collection
        Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
        services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(dbOptions);
        services.AddSingleton<ICQSDataBroker, CQSDataBroker<InMemoryWeatherDbContext>>();
        services.AddScoped<IListQueryHandler<DvoWeatherForecast>, WeatherForecastListQueryHandler<InMemoryWeatherDbContext>>();
        // Creates a Service Provider from the Services collection
        // This is our DI container
        var provider = services.BuildServiceProvider();

        // Adds the test data to the in memory database
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>();
        WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherDbContext>(factory);

        return provider!;
    }
```

And add a test:

```csharp
    [Fact]
    public async void TestCustomDvoWeatherForecastListCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<ICQSDataBroker>()!;
        var locationId = _weatherTestDataProvider.WeatherLocations.First().Uid;

        var testRecordCount = _weatherTestDataProvider.WeatherForecasts.Where(item => item.WeatherLocationId == locationId).Count();
        int pageSize = 10;
        var testCount = testRecordCount > pageSize ? pageSize : testRecordCount;

        var listRequest = new ListProviderRequest<DvoWeatherForecast>(0, pageSize);

        var query = WeatherForecastListQuery.GetQuery(locationId, listRequest);
        var result = await broker.ExecuteAsync<DvoWeatherForecast>(query);

        Assert.True(result.Success);
        Assert.Equal(testCount, result.Items.Count());
        Assert.True(result.TotalItemCount == testRecordCount);
    }
```

### Identity Provider

This demostrates a full custom implementation.  It gets a result that contains a `ClaimsIdentity` (part of the Authentication system) from a database identity table.

For reference the database record is:

```csharp
public record DboIdentity
{
    [Key] public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; init; } = String.Empty;
    public string Role { get; init; } = String.Empty;
}
```

The result:

```csharp
public class IdentityRequestResult
{
    public ClaimsIdentity? Identity { get; init; } = null;
    public bool Success { get; init; } = false;
    public string Message { get; init; } = string.Empty;

    public static IdentityRequestResult Failure(string message)
        => new IdentityRequestResult {Message = message };

    public static IdentityRequestResult Successful(ClaimsIdentity identity, string? message = null)
        => new IdentityRequestResult {Identity = identity, Success=true, Message = message ?? string.Empty };
}
```

The query request:

```csharp
public record IdentityQuery
    : ICQSRequest<ValueTask<IdentityRequestResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();
    public Guid IdentityId { get; init; } = Guid.Empty;

    public static IdentityQuery Query(Guid Uid)
        => new IdentityQuery { IdentityId = Uid };
}
```

A handler interface: we may need Server and API versions.

```csharp
public interface IIdentityQueryHandler
    : ICQSHandler<IdentityQuery, ValueTask<IdentityRequestResult>>
{}
```

And the handler:

```csharp
public class IdentityQueryHandler<TDbContext>
    : ICQSHandler<IdentityQuery, ValueTask<IdentityRequestResult>>
        where TDbContext : DbContext
{
    protected IDbContextFactory<TDbContext> factory;

    public IdentityQueryHandler(IDbContextFactory<TDbContext> factory)
        => this.factory = factory;

    public async ValueTask<IdentityRequestResult> ExecuteAsync(IdentityQuery query)
    {
        var dbContext = this.factory.CreateDbContext();
        IQueryable<DboIdentity> queryable = dbContext.Set<DboIdentity>();
        if (queryable is not null)
        {
            var record = await queryable.SingleOrDefaultAsync(item => item.Id == query.IdentityId);
            if (record is not null)
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, record.Id.ToString()),
                    new Claim(ClaimTypes.Name, record.Name),
                    new Claim(ClaimTypes.Role, record.Role)
                });
                return IdentityRequestResult.Successful(identity);
            }
            return IdentityRequestResult.Failure("No Identity exists.");
        }
        return IdentityRequestResult.Failure("No Identity Records Found.");
    }
}
```

And the demo test:

```csharp
public class CQSCustomTests
{
    private WeatherTestDataProvider _weatherTestDataProvider;

    public CQSCustomTests()
        // Creates an instance of the Test Data provider
        => _weatherTestDataProvider = WeatherTestDataProvider.Instance();

    private ServiceProvider GetServiceProvider()
    {
        // Creates a Service Collection
        var services = new ServiceCollection();
        // Adds the application services to the collection
        Action<DbContextOptionsBuilder> dbOptions = options => options.UseInMemoryDatabase($"WeatherDatabase-{Guid.NewGuid().ToString()}");
        services.AddWeatherAppServerDataServices<InMemoryWeatherDbContext>(dbOptions);
        services.AddScoped<IIdentityQueryHandler, IdentityQueryHandler<InMemoryWeatherDbContext>>();
        // Creates a Service Provider from the Services collection
        // This is our DI container
        var provider = services.BuildServiceProvider();

        // Adds the test data to the in memory database
        var factory = provider.GetService<IDbContextFactory<InMemoryWeatherDbContext>>();
        WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherDbContext>(factory);

        return provider!;
    }

    [Fact]
    public async void TestIdentityCQSDataBroker()
    {
        var provider = GetServiceProvider();
        var broker = provider.GetService<IIdentityQueryHandler>()!;

        var testRecord = _weatherTestDataProvider.Identities.Skip(1).First();

        var query = IdentityQuery.GetQuery(testRecord.Id);
        var result = await broker.ExecuteAsync(query);

        Assert.True(result.Success);
        Assert.NotNull(result.Identity);
        Assert.Equal(testRecord.Name, result.Identity.Name);
    }
}
```

## Summary

Hopefully I demonstrated a different, more succinct approach to implementing the CQS pattern.  I'm now a convert.

I've intentionally not implemented transaction logging or centralised exception handling.

## Appendix

### The Data Store

The backend database for this article and repository is an In-Memory Entity Framework database.  It's main advantage over other methods of mocking a data store is it works with the DbContext factory and supports multiple contexts.  I use In-Memory queries to emulate views.

The TestDataProvider has a method that populates it's data into a DbContext.

The full DbContext looks like this:

```csharp
public class InMemoryWeatherDbContext
    : DbContext
{
    public DbSet<DboWeatherForecast> DboWeatherForecast { get; set; } = default!;
    public DbSet<DvoWeatherForecast> DvoWeatherForecast { get; set; } = default!;
    public DbSet<DboWeatherSummary> DboWeatherSummary { get; set; } = default!;
    public DbSet<DboWeatherLocation> DboWeatherLocation { get; set; } = default!;
    public DbSet<FkWeatherSummary> FkWeatherSummary { get; set; } = default!;
    public DbSet<FkWeatherLocation> FkWeatherLocation { get; set; } = default!;
    public DbSet<DboIdentity> DboIdentity { get; set; } = default!;

    public InMemoryWeatherDbContext(DbContextOptions<InMemoryWeatherDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboWeatherForecast>().ToTable("WeatherForecast");
        modelBuilder.Entity<DboWeatherSummary>().ToTable("WeatherSummary");
        modelBuilder.Entity<DboWeatherLocation>().ToTable("WeatherLocation");
        modelBuilder.Entity<DboIdentity>().ToTable("Identity");

        modelBuilder.Entity<DvoWeatherForecast>()
            .ToInMemoryQuery(()
            => from f in this.DboWeatherForecast
               join s in this.DboWeatherSummary! on f.WeatherSummaryId equals s.Uid into fs
               from fsjoin in fs
               join l in this.DboWeatherLocation! on f.WeatherLocationId equals l.Uid into fl
               from fljoin in fl
               select new DvoWeatherForecast
               {
                   Uid = f.Uid,
                   WeatherSummaryId = f.WeatherSummaryId,
                   WeatherLocationId = f.WeatherLocationId,
                   Date = f.Date,
                   Summary = fsjoin.Summary,
                   Location = fljoin.Location,
                   TemperatureC = f.TemperatureC,
               })
            .HasKey(x => x.Uid);

        modelBuilder.Entity<FkWeatherSummary>()
            .ToInMemoryQuery(()
            => from s in this.DboWeatherSummary!
               select new FkWeatherSummary
               {
                   Id =s.Uid,
                   Name = s.Summary
               })
            .HasKey(x => x.Id);

        modelBuilder.Entity<FkWeatherLocation>()
            .ToInMemoryQuery(()
            => from l in this.DboWeatherLocation!
               select new FkWeatherLocation
               {
                   Id = l.Uid,
                   Name = l.Location
               })
            .HasKey(x => x.Id);
    }
}
```

There's a readme in the repository that provides a full description of the test data setup.

