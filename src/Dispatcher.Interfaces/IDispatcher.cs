namespace Dispatcher;
public interface IDispatcher
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    public IEnumerable<Task> Publish(IEvent @event, CancellationToken cancellationToken = default);
}
