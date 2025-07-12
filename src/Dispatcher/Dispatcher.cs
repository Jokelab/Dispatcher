
using Dispatcher.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher;
internal class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private const string HandleMethodName = "Handle";
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        // Dynamically construct the handler type interface so it can be resolved from the service provider
        var handlerTypeInterface = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerTypeInterface);
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for request type {request.GetType().FullName}.");
        }

        var behaviors = _serviceProvider.GetServices(typeof(IBehavior<,>).MakeGenericType(request.GetType(), typeof(TResponse))).Reverse();
        var behaviorTypeInterface = typeof(IBehavior<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var behaviorHandleMethod = behaviorTypeInterface.GetMethod(HandleMethodName);

        // Use reflection to invoke the Handle method
        var handleMethod = handlerTypeInterface.GetMethod(HandleMethodName);
        if (handleMethod == null)
            throw new InvalidOperationException($"{handlerTypeInterface.GetType().FullName} does not implement Handle method.");

        Func<Task<TResponse>> handlerDelegate = () => (Task<TResponse>?)handleMethod.Invoke(handler, [request, cancellationToken])!;
        foreach (var behavior in behaviors)
        {
            if (behaviorHandleMethod == null)
                throw new InvalidOperationException($"{behaviorTypeInterface.GetType().FullName} does not implement Handle method.");
            // Wrap the handler delegate with the behavior
            var previousHandler = handlerDelegate;
            handlerDelegate = () => (Task<TResponse>?)behaviorHandleMethod.Invoke(behavior, [request, previousHandler, cancellationToken])!;
        }

        return await handlerDelegate().ConfigureAwait(false);
    }

    public IEnumerable<Task> Publish(IEvent @event, CancellationToken cancellationToken = default)
    {
        // Dynamically construct the handler type interface so it can be resolved from the service provider
        var handlerTypeInterface = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        var handlers = _serviceProvider.GetServices(handlerTypeInterface);
        var tasks = new List<Task>();
        if (!handlers.Any())
            return tasks; // No handlers registered for this event, so return an empty list

        // Use reflection to invoke the Handle method on evert event handler
        var handleMethod = handlerTypeInterface.GetMethod(HandleMethodName);
        if (handleMethod == null)
            throw new InvalidOperationException($"{handlerTypeInterface.GetType().FullName} does not implement Handle method.");

        foreach (var handler in handlers)
        {
            var task = (Task)handleMethod.Invoke(handler, [@event, cancellationToken])!;
            if (task != null)
                tasks.Add(task);
        }
        return tasks;
    }
}