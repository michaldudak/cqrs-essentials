using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface IEventDispatcher
	{
		Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IEvent;

		Task DispatchDynamicallyAsync(IEvent @event);
	}
}