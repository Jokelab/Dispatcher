using Dispatcher.Extensions;
using Dispatcher.OtherAssembly;
using Dispatcher.Tests.Examples;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Tests.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddDispatcher_ShouldAddDispatcherServices()
        {
            //arrange
            var services = new ServiceCollection();

            //act
            services.AddDispatcher();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetService<IDispatcher>();
            Assert.NotNull(dispatcher);
        }

        [Fact]
        public void AddDispatcher_ShouldAddDispatcherServices_only_once()
        {
            //arrange
            var services = new ServiceCollection();

            //act
            services.AddDispatcher();
            var firstCount = services.Count;
            services.AddDispatcher();
            var secondCount = services.Count;

            // Assert
            Assert.Equal(firstCount, secondCount);
        }

        [Fact]
        public void AddDispatcher_ShouldThrowException_When_CommandHandlerNotFound()
        {
            //arrange
            var services = new ServiceCollection();

            //act and assert
            Assert.Throws<InvalidOperationException>(() => services.AddDispatcher(config => config.Types.Add(typeof(GreetingCommand))));
        }

        [Fact]
        public void AddDispatcher_ShouldNotThrowException_When_EventHandlerNotFound()
        {
            //arrange
            var services = new ServiceCollection();

            //act and assert
            services.AddDispatcher(config => config.Types.Add(typeof(UserUpdated)));
        }

        [Fact]
        public void AddDispatcher_ThrowException_When_MultipleCommandHandlersFound()
        {
            //arrange
            var services = new ServiceCollection();

            //act and assert
            Assert.Throws<InvalidOperationException>(() => services.AddDispatcher(config => config.Types.AddRange([
                typeof(UpdateUserCommand),
                typeof(UpdateUserHandler),
                typeof(SecondUpdateUserHandler),
                ]))
            );
        }

        private class SecondUpdateUserHandler : ICommandHandler<UpdateUserCommand, bool>
        {
            public Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
            {
                return Task.FromResult(true);
            }
        }

        [Fact]
        public void AddDispatcher_Should_load_types_from_other_assembly()
        {
            //arrange
            var services = new ServiceCollection();
            //act
            services.AddDispatcher(config => config.AssembliesToScan.Add(typeof(UpdateUserCommand).Assembly));
            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var dispatcher = serviceProvider.GetService<IDispatcher>();
            Assert.NotNull(dispatcher);
        }



    }
}
