namespace Dispatcher.Tests.Examples
{
    public class GreetingHandler : ICommandHandler<GreetingCommand, string>
    {
        public Task<string> Handle(GreetingCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult($"Hello, {command.Name}!");
        }
    }
}
