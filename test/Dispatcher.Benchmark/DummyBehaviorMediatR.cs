namespace Dispatcher.Benchmark;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class DummyBehaviorMediatR<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Start] Handling {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"[End] Handling {typeof(TRequest).Name}");
        return response;
    }
}