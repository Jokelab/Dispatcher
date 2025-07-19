namespace Dispatcher.Benchmark
{
    internal class BenchmarkEvent: 
        MediatR.INotification,
        IEvent
    {
        public string Name { get; set; } = "BenchmarkEvent";
    }
}
