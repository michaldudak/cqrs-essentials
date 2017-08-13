using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface IEventDispatcher
	{
		/// <summary>
		/// Runs all the registered event handlers for the given event type.
		/// Note that the type of the event must be known at compile time. If it's not, call DispatchDynamicallyAsync() instead.
		/// </summary>
		/// <typeparam name="TEvent">Type of the event</typeparam>
		/// <param name="event">Instance of the event</param>
		/// <returns>Task that completes when all handlers finish processing</returns>
		Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IEvent;

		/// <summary>
		/// Runs all the registered event handlers for the given event type.
		/// Use this method if you don't know the exact event type at compile time (e.g. it's just an IEvent).
		/// If you do know it, call DispatchAsync() instead, it'll be faster.
		/// </summary>
		/// <param name="event">Instance of the event</param>
		/// <returns>Task that completes when all handlers finish processing</returns>
		Task DispatchDynamicallyAsync(IEvent @event);
	}
}