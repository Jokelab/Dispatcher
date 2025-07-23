# Dispatcher
Decouple requests, events and their handlers via a dispatcher object.

## Setup
Add the NuGet package to your project and register the assemblies that contain your Requests, events and their handlers.
```C#
services.AddDispatcher(config=>
{
    config.AssembliesToScan.Add(typeof(GreetingRequest).Assembly);
});
```

## Requests
Requests can be defined by inheriting the `IRequest<TResponse>` interface, where the TResponse parameter is the response type of the request. A request object holds the parameters of a request.
```C#
public class GreetingRequest : IRequest<string>
{
    public string? Name { get; set; }
}

```
Define a handler that processes the request and returns a result. There must be exactly one handler for a request.
```C#
public class GreetingHandler : IRequestHandler<GreetingRequest, string>
{
   public Task<string> Handle(GreetingRequest request, CancellationToken cancellationToken)
   {
       return Task.FromResult($"Hello, {request.Name}!");
   }
}
```

To use a request, inject the dispatcher via DI and call the `Send` method with the request object. Notice that the HelloWorldGreeting class is decoupled from the GreetingHandler class.
```C#
public class HelloWorldGreeting
{
   private readonly IDispatcher _dispatcher;
   public HelloWorldGreeting(IDispatcher dispatcher)
   {
       _dispatcher = dispatcher;
   }

   public async Task<string> GreetAsync()
   {
       var request = new GreetingRequest { Name = "World" };
       var response = await _dispatcher.Send(request);

       // Returns "Hello, World!"
       return response;
   }
}
```
## Events
Events represent something that has happened. The publisher of an event should not be concerned if there are zero, one or more handlers of the event.
An event can be defined by inheriting the `IEvent` interface. 
```C#
public class UserUpdated : IEvent
{
    public string? UserName { get; set; }
}
```

Define zero or more handlers that can do something with the event.
```C#
public class UserUpdatedHandler : IEventHandler<UserUpdated>
{
    public Task Handle(UserUpdated @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"User updated: {@event.UserName}");
        return Task.CompletedTask;
    }
}
```
Here is an example class that publishes the `UserUpdated` event.
The `Publish` Request returns an `IEnumerable<Task>` with a task for every executing handler. It is up to the publisher to either ignore the tasks, or wait for any or all tasks to complete.
```C#
 public class User
 {
     private readonly IDispatcher _dispatcher;
     private string _name = "John Doe";
     public User(IDispatcher dispatcher)
     {
         _dispatcher = dispatcher;
     }

     public async Task SaveUser()
     {
         var userUpdatedEvent = new UserUpdated { UserName = _name };
         var tasks = _dispatcher.Publish(userUpdatedEvent);
         await Task.WhenAll(tasks);
     }
 }
 ```

## Behaviors
Behaviors allow to add instructions before and after a requesthandler runs. They will not be invoked for eventhandlers.
The following example logs the start and end of every request.

```C#
public class LoggingBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
{
    private readonly ILogger _logger;
    public LoggingBehavior(ILogger logger)
    {
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[Start] Handling {typeof(TRequest).Name}");

        var response = await next();

        _logger.LogInformation($"[End] Handling {typeof(TRequest).Name}");
        return response;
    }
}
```

Behaviors must be explicitly registered. Their order is also important since the calls will be wrapped.
Consider the following setup:
```C#
services.AddDispatcher(configuration =>
{
   configuration.AssembliesToScan.Add(typeof(GreetingRequest).Assembly);
   configuration.Behaviors.Add(typeof(LoggingBehavior<,>));
   configuration.Behaviors.Add(typeof(ValidationBehavior<,>));
});
```

When a greeting request is dispatched the logging behavior runs first, then the validation behavior. Every behavior can break the chain when it doesn't call the `next()` behavior.

