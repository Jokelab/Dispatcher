using Dispatcher.Interfaces;

namespace Dispatcher.OtherAssembly
{
    public record CreateUserRequest(string Name) : IRequest<UserModel>
    {
    }
}
