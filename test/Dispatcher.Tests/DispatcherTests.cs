using Dispatcher.Extensions;
using Dispatcher.OtherAssembly;
using Dispatcher.Tests.Examples;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Dispatcher.Tests
{
    public class DispatcherTests
    {
        private readonly IDispatcher _dispatcher;
        public DispatcherTests()
        {
            var services = new ServiceCollection();
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

        [Fact]
        public async Task Basic_publish_event_example()
        {
            var userUpdatedEvent = new UserUpdated { UserName = "John Doe" };
            var tasks = _dispatcher.Publish(userUpdatedEvent);
            await Task.WhenAll(tasks);
        }

        [Fact]
        public async Task Event_can_be_published_from_commandHandler()
        {
            var services = new ServiceCollection();
            services.AddDispatcher(configuration =>
            {
                configuration.AssembliesToScan.Add(typeof(CreateUserCommand).Assembly);
            });

            var sp = services.BuildServiceProvider();
            var dispatcher = sp.GetRequiredService<IDispatcher>();

            var createUserCommand = new CreateUserCommand("Test name");
            var model = await dispatcher.Send(createUserCommand);

        }

    }
}