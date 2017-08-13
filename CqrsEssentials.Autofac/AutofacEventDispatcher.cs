using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using System.Collections.Generic;
using System.Collections;

namespace CqrsEssentials.Autofac
{
	public class AutofacEventDispatcher : IEventDispatcher
	{
		private const string HandleMethodName = "Handle";
		private const string HandleAsyncMethodName = "HandleAsync";

		private readonly ILifetimeScope _lifetimeScope;

		public AutofacEventDispatcher(ILifetimeScope lifetimeScope)
		{
			_lifetimeScope = lifetimeScope;
		}

		protected virtual void HandlersExecuting(IEvent @event)
		{
		}

		protected virtual void HandlersExecuted(IEvent @event)
		{
		}

		public async Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IEvent
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

		public async Task DispatchDynamicallyAsync(IEvent @event)
		{
			using (var scope = _lifetimeScope.BeginLifetimeScope())
			{
				var asyncEventHandlerType = typeof(IAsyncEventHandler<>).MakeGenericType(@event.GetType());
				var syncEventHandlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());

				var asyncEventHandlerCollectionType = typeof(ICollection<>).MakeGenericType(asyncEventHandlerType);
				var syncEventHandlerCollectionType = typeof(ICollection<>).MakeGenericType(syncEventHandlerType);

				var asyncHandlers = (ICollection)scope.Resolve(asyncEventHandlerCollectionType);
				var syncHandlers = (ICollection)scope.Resolve(syncEventHandlerCollectionType);
				
				HandlersExecuting(@event);

				var asyncTasks = new List<Task>(asyncHandlers.Count);

				foreach (var asyncHandler in asyncHandlers)
				{
					var result = (Task)asyncHandler.GetType().GetRuntimeMethod(HandleAsyncMethodName, new[] { @event.GetType() }).Invoke(asyncHandler, new object[] { @event });
					asyncTasks.Add(result);
				}

				foreach (var syncHandler in syncHandlers)
				{
					syncHandler.GetType().GetRuntimeMethod(HandleMethodName, new[] { @event.GetType() }).Invoke(syncHandler, new object[] { @event });
				}

				await Task.WhenAll(asyncTasks);

				HandlersExecuted(@event);
			}
		}
	}
}