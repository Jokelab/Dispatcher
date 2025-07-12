using Dispatcher.Interfaces;

namespace Dispatcher.OtherAssembly
{
    internal class UserCreatedHandler : IEventHandler<UserCreated>
    {
        public Task Handle(UserCreated @event, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Created user ID: {@event.User.Id}, Name: {@event.User.Name}");
            return Task.CompletedTask;
        }
    }
}
