namespace Dispatcher.Tests.Examples
{
    public class LoggingBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[Start] Handling {typeof(TRequest).Name}");
            var response = await next();
            Console.WriteLine($"[End] Handling {typeof(TRequest).Name}");
            return response;
        }
    }
}
