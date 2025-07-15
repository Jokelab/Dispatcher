namespace Dispatcher.Benchmark.Examples;

public class BenchmarkHandler :
    MediatR.IRequestHandler<BenchmarkRequest, string>,
    IRequestHandler<BenchmarkRequest, string>
{
    public Task<string> Handle(BenchmarkRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Hello, {request.Name}!");
    }
}
