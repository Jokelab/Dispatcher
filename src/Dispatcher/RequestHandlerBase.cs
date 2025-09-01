namespace Dispatcher;
internal abstract class RequestHandlerBase<TResponse>
{
    public abstract Task<TResponse> Handle(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
