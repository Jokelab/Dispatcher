using System.Collections.Concurrent;

namespace Dispatcher;
internal class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private static readonly ConcurrentDictionary<Type, object> _requestHandlerTypeCache = new();
    private static readonly ConcurrentDictionary<Type, object> _eventHandlerTypeCache = new();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handler = (RequestHandlerBase<TResponse>)_requestHandlerTypeCache.GetOrAdd(request.GetType(), static requestType =>
        {
            var wrapperType = typeof(RequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for request {requestType}");
            return (RequestHandlerBase<TResponse>)wrapper;
        });

        return handler.Handle(request, _serviceProvider, cancellationToken);
    }

    public IEnumerable<Task> Publish(IEvent @event, CancellationToken cancellationToken = default)
    {
        var handler = (EventHandlerBase)_eventHandlerTypeCache.GetOrAdd(@event.GetType(), static eventType =>
        {
            var wrapperType = typeof(EventHandler<>).MakeGenericType(eventType);
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for event {eventType}");
            return (EventHandlerBase)wrapper;
        });
        return handler.Handle(@event, _serviceProvider, cancellationToken);
    }
}