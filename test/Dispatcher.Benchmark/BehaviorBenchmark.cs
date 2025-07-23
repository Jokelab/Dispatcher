using BenchmarkDotNet.Attributes;
using Dispatcher.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher.Benchmark
{
    public class BehaviorBenchmark
    {
        private IMediator? _mediator;
        private IDispatcher? _dispatcher;


        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddDispatcher(config =>
                {
                    config.AssembliesToScan.Add(typeof(BenchmarkRequest).Assembly);
                    config.Behaviors.Add(typeof(DummyBehaviorDispatcher<,>));
                }
            );
            //services.AddLogging();
            //services.AddMediatR(config =>
            //{
            //    config.RegisterServicesFromAssemblies(typeof(BenchmarkRequest).Assembly);
            //    config.AddBehavior(typeof(DummyBehaviorMediatR<,>));


            //});

            var sp = services.BuildServiceProvider();
           // _mediator = sp.GetRequiredService<IMediator>();
            _dispatcher = sp.GetRequiredService<IDispatcher>();
        }

        //[Benchmark]
        //public async Task SendRequestWithBehaviorsWithMediatR()
        //{
        //    await _mediator!.Send(new BenchmarkRequest());
        //}

        [Benchmark]
        public async Task SendRequestWithBehaviorsDispatcher()
        {
            await _dispatcher!.Send(new BenchmarkRequest());
        }
    }
}

