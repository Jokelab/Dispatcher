using Dispatcher.Extensions;
using Dispatcher.Tests.Examples;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Tests
{
    public class BehaviorTests
    {
        private const string StartGreetingRequestMessage = "[Start] Handling GreetingRequest";
        private const string EndGreetingRequestMessage = "[End] Handling GreetingRequest";
        private const string StartValidationMessage = "[Start] Validation GreetingRequest";
        private const string EndValidationMessage = "[End] Validation GreetingRequest";
        [Fact]
        public async Task LoggingBehavior_should_be_invoked_when_command_sent()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddDispatcher(configuration =>
            {
                configuration.AssembliesToScan.Add(typeof(GreetingRequest).Assembly);
                configuration.Behaviors.Add(typeof(LoggingBehavior<,>));
            });
            services.AddSingleton<MessageWriter>();

            var sp = services.BuildServiceProvider();
            var dispatcher = sp.GetRequiredService<IDispatcher>();

            // Act
            await dispatcher.Send(new GreetingRequest { Name = "Test" });

            // Assert
            var writer = sp.GetRequiredService<MessageWriter>();
            var messages = writer.GetMessages();
            Assert.Contains("[Start] Handling GreetingRequest", messages);
            Assert.Contains("[End] Handling GreetingRequest", messages);
        }

        [Fact]
        public async Task Behaviors_should_be_wrapped()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddDispatcher(configuration =>
            {
                configuration.AssembliesToScan.Add(typeof(GreetingRequest).Assembly);
                configuration.Behaviors.Add(typeof(LoggingBehavior<,>));
                configuration.Behaviors.Add(typeof(ValidationBehavior<,>));
            });
            services.AddSingleton<MessageWriter>();

            var sp = services.BuildServiceProvider();
            var dispatcher = sp.GetRequiredService<IDispatcher>();

            // Act
            await dispatcher.Send(new GreetingRequest { Name = "Test" });

            // Assert
            var writer = sp.GetRequiredService<MessageWriter>();
            var messages = writer.GetMessages();
            Assert.Contains(StartGreetingRequestMessage, messages);
            Assert.Contains(EndGreetingRequestMessage, messages);
            Assert.Contains(StartValidationMessage, messages);
            Assert.Contains(EndValidationMessage, messages);
            Assert.True(messages.IndexOf(StartGreetingRequestMessage) < messages.IndexOf(StartValidationMessage));
            Assert.True(messages.IndexOf(EndValidationMessage) < messages.IndexOf(EndGreetingRequestMessage));
        }
    }
}
