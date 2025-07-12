using Dispatcher.Interfaces;
using Dispatcher.OtherAssembly;

public record UserCreated(UserModel User) : IEvent;