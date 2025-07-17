using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher
{
    internal class RequestHandler<TRequest, TResponse> : RequestHandlerBase<TResponse>
        where TRequest : IRequest<TResponse>
    {

        public Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider,
     CancellationToken cancellationToken)
        {
            Task<TResponse> handler(CancellationToken t = default) => serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
                .Handle((TRequest)request, cancellationToken);

            Func<Task<TResponse>> handlerDelegate = () => handler(cancellationToken);

            // Build the handler delegate chain

            var behaviors = serviceProvider.GetServices<IBehavior<TRequest, TResponse>>();
            var behaviorArray = behaviors.ToArray();
            // Avoid LINQ Reverse for performance, use for loop instead
            for (int i = behaviorArray.Length - 1; i >= 0; i--)
            {
                var previousHandler = handlerDelegate;
                var behavior = behaviorArray[i];
                handlerDelegate = () => behavior.Handle((TRequest)request, previousHandler, cancellationToken);
            }

            return handlerDelegate();
        }

        public override Task<TResponse> Handle(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            return Handle((IRequest<TResponse>)request, serviceProvider, cancellationToken);
        }
    }
}
