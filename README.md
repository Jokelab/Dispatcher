# Dispatcher
Decouple commands, events and their handlers via a dispatcher object.

## Setup
Add the NuGet package to your project and register the assemblies that contain your commands, events and their handlers.
```C#
services.AddDispatcher(config=>
{
    config.AssembliesToScan.Add(typeof(GreetingCommand).Assembly);
});
```

## Commands
Commands can be defined by inheriting the `ICommand<T>` interface, where the T parameter is the response type of the command. A command object should hold the parameters of a request.
```C#
public class GreetingCommand : ICommand<string>
{
    public string? Name { get; set; }
}

```
Define a handler that processes the command and returns a result. There must be exactly one handler for a command.
```C#
public class GreetingHandler : ICommandHandler<GreetingCommand, string>
{
   public Task<string> Handle(GreetingCommand command, CancellationToken cancellationToken)
   {
       return Task.FromResult($"Hello, {command.Name}!");
   }
}
```

To use the command, inject the dispatcher via DI and call the `Send` method with the commmand as a parameter. Notice that the HelloWorldGreeting class is decoupled from the GreetingHandler class.
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
       var command = new GreetingCommand { Name = "World" };
       // Use the dispatcher to send the command and get the response
       var response = await _dispatcher.Send(command);

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
The `Publish` command returns an `IEnumerable<Task>` with a task for every executing handler. It is up to the publisher to either ignore the tasks, or wait for any or all tasks to complete.
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
