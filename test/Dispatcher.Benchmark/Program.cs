using BenchmarkDotNet.Running;
using Dispatcher.Benchmark;

BenchmarkRunner.Run<RequestHandlerBenchmark>();
//BenchmarkRunner.Run<EventHandlerBenchmark>();