namespace Dispatcher;

public interface IDispatcher
{
    public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

    public IEnumerable<Task> Publish(IEvent @event, CancellationToken cancellationToken = default);
}
  