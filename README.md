# Dispatcher
Decouple commands, events and their handlers via a dispatcher object.

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
