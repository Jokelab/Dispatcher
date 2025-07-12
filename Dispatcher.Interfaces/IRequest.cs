namespace Dispatcher.Interfaces;
/// <summary>
/// Marker interface for requests in the dispatcher system.
/// </summary>
public interface IRequest
{
}

public interface IRequest<TResponse> : IRequest
{
}