using System.Collections.Generic;
using ReflectionMagic;

namespace SeekU.Eventing
{
    /// <summary>
    /// Event bus for marshaling events
    /// </summary>
    public class InProcessEventBus : IEventBus
    {
        private readonly IDependencyResolver _dependencyResolver;

        /// <summary>
        /// Creates an instance of the event bus
        /// </summary>
        /// <param name="dependencyResolver">Object responsible for resolving dependencies</param>
        public InProcessEventBus(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        /// <summary>
        /// Publishes events to all event handlers
        /// </summary>
        /// <param name="domainEvent">Domain event instance</param>
        public void PublishEvent(DomainEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();

            var handlerType = typeof (IHandleDomainEvents<>);

            var handlers = _dependencyResolver.ResolveAll(handlerType.MakeGenericType(domainEventType));

            //TODO Order by some index
            foreach (var handler in handlers)
            {
                handler.AsDynamic().Handle(domainEvent);
            }
        }

        /// <summary>
        /// Publishes a list of events to all event handlers
        /// </summary>
        /// <param name="domainEvents">List of domain events</param>
        public void PublishEvents(IEnumerable<DomainEvent> domainEvents)
        {
            //TODO transaction scope
            foreach (var domainEvent in domainEvents)
            {
                PublishEvent(domainEvent);
            }
        }
    }
}
