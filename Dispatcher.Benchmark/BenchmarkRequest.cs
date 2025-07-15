namespace Dispatcher.Benchmark;

public class BenchmarkRequest : 
    MediatR.IRequest<string>, 
    IRequest<string>
{
    public string? Name { get; set; }
}
