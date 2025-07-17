using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace Dispatcher;
internal class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private const string HandleMethodName = "Handle";
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    private static readonly ConcurrentDictionary<Type, object> _handlerTypeCache = new();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handler = (RequestHandlerBase<TResponse>)_handlerTypeCache.GetOrAdd(request.GetType(), static requestType =>
        {
            var wrapperType = typeof(RequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (RequestHandlerBase<TResponse>)wrapper;
        });

        return handler.Handle(request, _serviceProvider, cancellationToken);
    }

    public IEnumerable<Task> Publish(IEvent @event, CancellationToken cancellationToken = default)
    {
        var eventType = @event.GetType();
        var handlerTypeInterface = typeof(IEventHandler<>).MakeGenericType(eventType);
        var handlers = _serviceProvider.GetServices(handlerTypeInterface);
        var tasks = new List<Task>();
        if (!handlers.Any())
            return tasks;

        var handleMethod = handlerTypeInterface.GetMethod(HandleMethodName);
        if (handleMethod == null)
            throw new InvalidOperationException($"{handlerTypeInterface.FullName} does not implement Handle method.");

        foreach (var handler in handlers)
        {
            var task = (Task)handleMethod.Invoke(handler, [@event, cancellationToken])!;
            if (task != null)
                tasks.Add(task);
        }
        return tasks;
    }
}