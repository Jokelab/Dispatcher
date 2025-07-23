using MediatR;

namespace Dispatcher.Benchmark;

public class DummyBehaviorDispatcher<TRequest, TResponse> :
    IBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        var response = await next();
        return response;
    }
}
