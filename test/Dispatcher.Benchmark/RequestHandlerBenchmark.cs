using BenchmarkDotNet.Attributes;
using Dispatcher.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Benchmark
{
    public class RequestHandlerBenchmark
    {
        private IMediator? _mediator;
        private IDispatcher? _dispatcher;


        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddDispatcher();
            services.AddLogging();
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(typeof(BenchmarkRequest).Assembly));

            var sp = services.BuildServiceProvider();
            _mediator = sp.GetRequiredService<IMediator>();
            _dispatcher = sp.GetRequiredService<IDispatcher>();
        }

        [Benchmark]
        public async Task SendRequestWithMediatR()
        {
            await _mediator!.Send(new BenchmarkRequest());
        }

        [Benchmark]
        public async Task SendRequestWithDispatcher()
        {
            await _dispatcher!.Send(new BenchmarkRequest());
        }
    }
}

