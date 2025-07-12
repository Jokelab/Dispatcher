namespace Dispatcher.Tests.Examples;


public class GreetingRequest : IRequest<string>
{
    public string? Name { get; set; }
}
