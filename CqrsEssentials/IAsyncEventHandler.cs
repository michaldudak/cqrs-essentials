using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface IAsyncEventHandler<in TEvent> where TEvent : IEvent
	{
		Task HandleAsync(TEvent anEvent);
	}
}