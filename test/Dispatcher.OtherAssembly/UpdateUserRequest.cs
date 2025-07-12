using Dispatcher.Interfaces;

namespace Dispatcher.OtherAssembly
{
    public class UpdateUserRequest : IRequest<bool>
    {
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
