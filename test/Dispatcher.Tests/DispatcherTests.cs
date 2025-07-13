using Dispatcher.Extensions;
using Dispatcher.OtherAssembly;
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
            services.AddDispatcher(configuration =>
            {
                configuration.AssembliesToScan.Add(typeof(GreetingRequest).Assembly);
            });

            var sp = services.BuildServiceProvider();
            _dispatcher = sp.GetRequiredService<IDispatcher>();
        }

        [Fact]
        public async Task Send_dispatches_basic_command()
        {
            var greeting = await _dispatcher.Send(new GreetingRequest { Name = "World" });
            Assert.Equal("Hello, World!", greeting);
        }

        [Fact]
        public async Task Publish_dispatches_basic_event()
        {
            var userUpdatedEvent = new UserUpdated { UserName = "John Doe" };
            var tasks = _dispatcher.Publish(userUpdatedEvent);
            Assert.NotEmpty(tasks);
            await Task.WhenAll(tasks);
        }

        [Fact]
        public async Task Event_can_be_published_from_requestHandler()
        {
            // arrange
            var services = new ServiceCollection();
            var eventHandlerCalled = false;
            services.AddDispatcher(configuration =>
            {
                configuration.AssembliesToScan.Add(typeof(CreateUserRequest).Assembly);
            });

            services.AddSingleton<IEventHandler<UserCreated>>(new TestUserCreatedHandler(() 
                => eventHandlerCalled = true
                ));

            var sp = services.BuildServiceProvider();
            var dispatcher = sp.GetRequiredService<IDispatcher>();

            var createUserrequest = new CreateUserRequest("Test name");

            // act
            var model = await dispatcher.Send(createUserrequest);

            // Assert
            Assert.NotNull(model);
            Assert.True(eventHandlerCalled, "UserCreated event handler was not called.");
        }

        // Test handler implementation
        private class TestUserCreatedHandler : IEventHandler<UserCreated>
        {
            private readonly Action _onHandle;
            public TestUserCreatedHandler(Action onHandle) => _onHandle = onHandle;
            public Task Handle(UserCreated @event, CancellationToken cancellationToken = default)
            {
                _onHandle();
                return Task.CompletedTask;
            }
        }

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
    }
}