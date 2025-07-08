using Dispatcher.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Tests
{
    public class DispatcherTests
    {
        private readonly IDispatcher _dispatcher;
        public DispatcherTests()
        {
            var sc = new ServiceCollection();
            sc.AddDispatcher();
            var sp = sc.BuildServiceProvider();
            _dispatcher = sp.GetRequiredService<IDispatcher>();
        }

        [Fact]
        public async Task Basic_dispatch_command_example()
        {
            var greeting = await _dispatcher.Send(new GreetingCommand { Name = "World" });
            Assert.Equal("Hello, World!", greeting);
        }

        private class GreetingCommand : ICommand<string>
        {
            public string? Name { get; set; }
        }

        private class GreetingHandler : ICommandHandler<GreetingCommand, string>
        {
            public Task<string> Handle(GreetingCommand command, CancellationToken cancellationToken)
            {
                return Task.FromResult($"Hello, {command.Name}!");
            }
        }

        [Fact]
        public async Task Basic_publish_event_example()
        {
            var userUpdatedEvent = new UserUpdated { UserName = "John Doe" };
            var tasks = _dispatcher.Publish(userUpdatedEvent);
            await Task.WhenAll(tasks);
        }

        public class UserUpdated : IEvent
        {
            public string? UserName { get; set; }
        }



    }

}