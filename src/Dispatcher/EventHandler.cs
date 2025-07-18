using Microsoft.Extensions.DependencyInjection;

namespace Dispatcher
{
    internal class EventHandler<TEvent> : EventHandlerBase
        where TEvent : IEvent
    {
        public IEnumerable<Task> Handle(IEvent @event, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            var eventHandlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
            foreach (var handler in eventHandlers)
            {
                tasks.Add(handler.Handle((TEvent)@event, cancellationToken));
            }
            return tasks;
        }

        public override IEnumerable<Task> Handle(object @event, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            return Handle((IEvent)@event, serviceProvider, cancellationToken);
        }
    }
}
