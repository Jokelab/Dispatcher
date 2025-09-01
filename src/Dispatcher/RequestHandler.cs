using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher;
internal class RequestHandler<TRequest, TResponse> : RequestHandlerBase<TResponse>
    where TRequest : IRequest<TResponse>
{

    public async Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider,
 CancellationToken cancellationToken)
    {
        Task<TResponse> handler(CancellationToken t = default) => serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
            .Handle((TRequest)request, cancellationToken);

        Func<Task<TResponse>> handlerDelegate = async () => await handler(cancellationToken);

        // Build the handler delegate chain

        var behaviors = serviceProvider.GetServices<IBehavior<TRequest, TResponse>>().ToArray();
        // Avoid LINQ Reverse for performance, use for loop instead
        for (int i = behaviors.Length - 1; i >= 0; i--)
        {
            var previousHandler = handlerDelegate;
            var behavior = behaviors[i];
            handlerDelegate = async () => await behavior.Handle((TRequest)request, previousHandler, cancellationToken);
        }

        return await handlerDelegate();
    }

    public override Task<TResponse> Handle(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return Handle((IRequest<TResponse>)request, serviceProvider, cancellationToken);
    }
}
