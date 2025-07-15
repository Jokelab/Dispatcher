using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace Dispatcher;
internal class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private const string HandleMethodName = "Handle";
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    // Caches to avoid repeated reflection and type construction
    private static readonly ConcurrentDictionary<(Type requestType, Type responseType), Type> _handlerTypeCache = new();
    private static readonly ConcurrentDictionary<(Type requestType, Type responseType), MethodInfo> _handlerMethodCache = new();
    private static readonly ConcurrentDictionary<(Type requestType, Type responseType), Type> _behaviorTypeCache = new();
    private static readonly ConcurrentDictionary<(Type requestType, Type responseType), MethodInfo> _behaviorMethodCache = new();

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        // Cache handler type and method
        var handlerTypeInterface = _handlerTypeCache.GetOrAdd((requestType, responseType), static key =>
            typeof(IRequestHandler<,>).MakeGenericType(key.requestType, key.responseType));
        var handleMethod = _handlerMethodCache.GetOrAdd((requestType, responseType), static key =>
            typeof(IRequestHandler<,>).MakeGenericType(key.requestType, key.responseType).GetMethod(HandleMethodName)!);

        var handler = _serviceProvider.GetService(handlerTypeInterface);
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for request type {requestType.FullName}.");
        }

        Func<Task<TResponse>> handlerDelegate = () => (Task<TResponse>)handleMethod.Invoke(handler, [request, cancellationToken])!;

        // Cache behavior type and method
        var behaviorTypeInterface = _behaviorTypeCache.GetOrAdd((requestType, responseType), static key =>
            typeof(IBehavior<,>).MakeGenericType(key.requestType, key.responseType));

        // Get behaviors in reverse order
        var behaviors = _serviceProvider.GetServices(behaviorTypeInterface).Reverse();
        var behaviorHandleMethod = _behaviorMethodCache.GetOrAdd((requestType, responseType), static key =>
            typeof(IBehavior<,>).MakeGenericType(key.requestType, key.responseType).GetMethod(HandleMethodName)!);
        
        // Build the handler delegate chain
        foreach (var behavior in behaviors)
        {
            var previousHandler = handlerDelegate;
            handlerDelegate = () => (Task<TResponse>)behaviorHandleMethod.Invoke(behavior, [request, previousHandler, cancellationToken])!;
        }

        return await handlerDelegate().ConfigureAwait(false);
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