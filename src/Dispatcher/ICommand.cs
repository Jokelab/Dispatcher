namespace Dispatcher;

/// <summary>
/// Marker interface for commands in the dispatcher system.
/// </summary>
public interface ICommand
{
}

public interface ICommand<TResponse> : ICommand
{
}