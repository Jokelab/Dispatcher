namespace Dispatcher.Tests.Examples
{
    public class UserUpdatedHandler : IEventHandler<UserUpdated>
    {
        public Task Handle(UserUpdated @event, CancellationToken cancellationToken)
        {
            Console.WriteLine($"User updated: {@event.UserName}");
            return Task.CompletedTask;
        }
    }
}
