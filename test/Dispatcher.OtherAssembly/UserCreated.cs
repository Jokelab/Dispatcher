using Dispatcher;
using Dispatcher.OtherAssembly;

public record UserCreated(UserModel User): IEvent;