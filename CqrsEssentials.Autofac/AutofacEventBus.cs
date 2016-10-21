using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;

namespace CqrsEssentials.Autofac
{
	public class AutofacEventBus : IEventBus
	{
		private readonly ILifetimeScope _lifetimeScope;

		public AutofacEventBus(ILifetimeScope lifetimeScope)
		{
			_lifetimeScope = lifetimeScope;
		}

		protected virtual void HandlersExecuting(IEvent @event)
		{
		}

		protected virtual void HandlersExecuted(IEvent @event)
		{
		}

		public async Task SendAsync<TEvent>(TEvent @event) where TEvent : IEvent
		{
			using (var scope = _lifetimeScope.BeginLifetimeScope())
			{
				var asyncHandlers = scope.Resolve<ICollection<IAsyncEventHandler<TEvent>>>();
				var syncHandlers = scope.Resolve<ICollection<IEventHandler<TEvent>>>();

				HandlersExecuting(@event);

				var asyncTasks = new List<Task>(asyncHandlers.Count);

				foreach (var asyncHandler in asyncHandlers)
				{
					asyncTasks.Add(asyncHandler.HandleAsync(@event));
				}

				foreach (var syncHandler in syncHandlers)
				{
					syncHandler.Handle(@event);
				}

				await Task.WhenAll(asyncTasks);

				HandlersExecuted(@event);
			}
		}
	}
}