namespace Dispatcher.Interfaces;
public interface IEventHandler<TEvent> where TEvent : IEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken);
}
