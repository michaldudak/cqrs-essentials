namespace CqrsEssentials
{
	public interface IEventHandler<TEvent> where TEvent : IEvent
	{
		void Handle(TEvent anEvent);
	}
}