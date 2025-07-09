namespace Dispatcher.OtherAssembly
{
    public class UpdateUserHandler: ICommandHandler<UpdateUserCommand, bool>
    {
        public Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            // Simulate updating a user in a database or other storage
            Console.WriteLine($"Updating user {command.UserId} with Name: {command.UserName}, Email: {command.Email}");
            return Task.FromResult(true); // Simulate success
        }
    }

}
