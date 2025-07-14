namespace Dispatcher.Tests.Examples;

public class ValidationBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
{
    private readonly MessageWriter _writer;
    public ValidationBehavior(MessageWriter writer)
    {
        _writer = writer;
    }
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
    {
        _writer.WriteMessage($"[Start] Validation {typeof(TRequest).Name}");
        var response = await next();
        _writer.WriteMessage($"[End] Validation {typeof(TRequest).Name}");
        return response;
    }
}
