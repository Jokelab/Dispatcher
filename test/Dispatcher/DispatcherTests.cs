using Dispatcher.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Tests
{
    public class DispatcherTests
    {
        private readonly IDispatcher _dispatcher;
        public DispatcherTests()
        {
            var services = new ServiceCollection();
            services.AddDispatcher();

            services.AddDispatcher(configuration =>
            {
                configuration.AssembliesToScan.Add(typeof(GreetingCommand).Assembly);
            });

            var sp = services.BuildServiceProvider();
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

        public class UserUpdatedHandler : IEventHandler<UserUpdated>
        {
            public Task Handle(UserUpdated @event, CancellationToken cancellationToken)
            {
                Console.WriteLine($"User updated: {@event.UserName}");
                return Task.CompletedTask;
            }
        }
     
        public class UserUpdated : IEvent
        {
            public string? UserName { get; set; }
        }

    }

}