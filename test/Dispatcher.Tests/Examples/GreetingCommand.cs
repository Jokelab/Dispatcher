namespace Dispatcher.Tests.Examples
{
   
    public class GreetingCommand : ICommand<string>
    {
        public string? Name { get; set; }
    }
}
