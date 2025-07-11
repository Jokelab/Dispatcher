namespace Dispatcher.OtherAssembly
{
    public record CreateUserCommand(string Name) : ICommand<UserModel>
    {
    }
}
