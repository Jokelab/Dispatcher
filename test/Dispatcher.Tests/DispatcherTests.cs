using Dispatcher.Extensions;
using Dispatcher.Tests.Examples;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Tests
{
    public class DispatcherTests
    {
        private readonly IDispatcher _dispatcher;
        public DispatcherTests()
        {
            var services = new ServiceCollection();
           // services.AddDispatcher();

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
    }

}