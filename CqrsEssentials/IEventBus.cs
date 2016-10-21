using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface IEventBus
	{
		Task SendAsync<TEvent>(TEvent @event) where TEvent : IEvent;
	}
}