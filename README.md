# CQRS Essentials

This repository contains source code for two libraries: CQRS Essentials and CQRS Essentials Autofac integration.

## CQRS Essentials (CqrsEssentials)
The CqrsEssentials package consists solely of interfaces that can be used by other libraries (such as CqrsEssentials.Autofac) to implement command, query and event dispatching. The library is written for netstandard 1.0, so it can be used basically everywhere.

### Usage
As the CqrsEssentials package contains just a few interfaces, it's not very usable by itself. You can use these interfaces to mark your commands, queries, events and their handlers. The `ICommand`, `IQuery<TResult>` and `IEvent` are marker interfaces - you don't have to implement anything to use them:

```C#
class AddNumbersCommand : ICommand
{
    public AddNumbersCommand(int a, int b)
    {
        A = a;
        B = b;
    }

    public int A { get; }
    public int B { get; }
}

class GetArticleByIdQuery : IQuery<Article>
{
    public GetArticleByIdQuery(int articleId)
    {
        ArticleId = articleId;
    }

    public int ArticleId { get; }
}

// we can even have queries without parameters
class GetAllUsersQuery : IQuery<ICollection<User>>
{
}
```

Marking your classes as queries or commands won't automagically do anything. It is important to do so, however, when you want to use a specific integration library, such as CqrsEssentials.Autofac.

#### Handlers

Apart from `ICommand`, `IQuery<TResult>` and `IEvent`, CqrsEssentials contains also several interfaces for handlers:

* `IQueryHandler<TQuery, TResult>`, `IAsyncQueryHandler<TQuery, TResult>` - used to mark query handlers. Those classes must implement the `TResult Handle(TQuery query)` method (or its async equivalent). Here you would place the logic that actually perform the query (such as a database call, fetching from cache, etc.).

* `ICommandHandler<TCommand>` and `IAsyncCommandHandler<TCommand>` - as the name suggests, you'll want your command handler classes to implement one of these interfaces.

* `IEventHandler<TEvent>` and `IAsyncEventHandler<TEvent>` - similarily to the interfaces above, these ones should be used on event handlers.

#### Dispatchers

If you use an integration library such as CqrsEssentials.Autofac, you don't have to care a lot about implementing the dispatcher interfaces (`ICommandDispatcher`, `IQueryDispatcher` and `IEventDispatcher`). If you do plan to implement your own dispatchers, however, these are the interafaces you'll have to implement.

The dispatchers are the entry points. You'll usually want to inject them into your code (such as MVC controller), pass in a command, query or event, and let the dispatcher's magic unicorns to select an appropriate handler for your input:

```C#
public class PeopleController : Controller
{
    private ICommandDispatcher _commandDispatcher;
    private IQueryDispatcher _queryDispatcher;

    public PeopleController(ICommandDispatcher cd, IQueryDispatcher qd)
    {
        _commandDispatcher = cd;
        _queryDispatcher = qd;
    }

    public async Task<ICollection<Person>> GetAllPeople()
    {
        var query = new GetAllPeopleQuery();
        return await _queryDispatcher.DispatchAsync(query);
    }

    public async Task AddPerson(Person personToAdd)
    {
        var command = new AddPersonCommand(personToAdd);
        await _commandDispatcher.DispatchAsync(command);
    }
}
```

### Installation

The package is available on NuGet gallery as CqrsEssentials: https://www.nuget.org/packages/CQRSEssentials


## CQRS Essentials Autofac integration (CqrsEssentials.Autofac)
This package implements `ICommandDispatcher`, `IQueryDispatcher` and `IEventDispatcher` using Autofac.

Similarily to Autofac, it is written for netstandard 1.1.

Usage:

* When setting up your container, register the `CqrsEssentialsAutofacModule` and each query, command and event handlers you have in the application:

  ```C#
  var builder = new ContainerBuilder();
  builder.RegisterModule<CqrsEssentialsAutofacModule>();
  builder.RegisterType<MyCommandHandler>().As<ICommandHandler<MyCommand>>();
  builder.RegisterType<MyQueryHandler>().As<IQueryHandler<MyQuery>>();
  builder.RegisterType<MyEventHandler>().As<IEventHandler<MyEvent>>();
  // your dependencies here ...
  var container = builder.Build();
  ```

* Inject the appropriate dispatcher interface in place you want to use it:

  ```C#
  public class MyController : Controller
  {
      private ICommandDispatcher _commandDispatcher;

      public MyController(ICommandDispatcher commandDispatcher)
      {
          _commandDispatcher = commandDispatcher;
      }
  }
  ```

* Create an instance of a Command, Query od Event and pass it to the dispatcher:

  ```C#
  // MVC action method, for example
  public async Task DoTheThing()
  {
      var myCommand = new MyCommand("hello");
      await _commandDispatcher.DispatchAsync(myCommand);
  }
  ```

* Your handlers may be synchronous or asynchronous but the dispatcher is always asynchronous.

### Installation

The package is available on NuGet gallery as CqrsEssentials.Autofac: https://www.nuget.org/packages/CQRSEssentials.Autofac
