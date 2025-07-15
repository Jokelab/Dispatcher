// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Dispatcher;
using Dispatcher.Benchmark;
using Dispatcher.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;




BenchmarkRunner.Run<RequestHandlerBenchmark>();