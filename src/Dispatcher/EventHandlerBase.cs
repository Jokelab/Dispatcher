namespace Dispatcher;

internal abstract class EventHandlerBase
{
    public abstract IEnumerable<Task> Handle(object @event, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
