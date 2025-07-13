namespace Dispatcher.Tests.Examples;

public class LoggingBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
{
    private readonly MessageWriter _writer;
    public LoggingBehavior(MessageWriter writer)
    {
        _writer = writer;
    }
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        _writer.WriteMessage($"[Start] Handling {typeof(TRequest).Name}");
        var response = await next();
        _writer.WriteMessage($"[End] Handling {typeof(TRequest).Name}");
        return response;
    }
}
