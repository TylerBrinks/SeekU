namespace SeekU.Eventing
{
    /// <summary>
    /// Represents an event handler for type T
    /// </summary>
    /// <typeparam name="T">Type of event to handle</typeparam>
    public interface IHandleDomainEvents<in T> where T : DomainEvent
    {
        void Handle(T domainEvent);
    }
}