# CQRS Essentials

This repository contains source code for two libraries: CQRSEssentials and CQRSEssentials.Autofac.

The documentation is still a work in progress.

## CQRSEssentials
The CQRSEssentials package consists solely of interfaces that can be used by other libraries (such as CQRSEssentials.Autofac) to implement command, query and event dispatching.

## CQRSEssentials.Autofac
This package implements `ICommandDispatcher`, `IQueryDispatcher` and `IEventBus` using Autofac.

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