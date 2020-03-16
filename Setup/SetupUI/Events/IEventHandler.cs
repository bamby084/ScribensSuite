
namespace SetupUI.Events
{
    public interface IEventHandler<TEvent>
        where TEvent: EventBase
    {
        void HandleEvent(TEvent @event);
    }
}
