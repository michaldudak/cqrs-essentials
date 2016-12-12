using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace CqrsEssentials.Autofac
{
	public class AutofacQueryDispatcher : IQueryDispatcher
	{
		private readonly ILifetimeScope _lifetimeScope;

		public AutofacQueryDispatcher(ILifetimeScope lifetimeScope)
		{
			_lifetimeScope = lifetimeScope;
		}

		public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var scope = _lifetimeScope.BeginLifetimeScope())
			{
				object asyncHandler;
				object syncHandler;

				var asyncHandlerExists = TryGetAsyncHandler(scope, query, out asyncHandler);
				var syncHandlerExists = TryGetSyncHandler(scope, query, out syncHandler);

				if (asyncHandlerExists && syncHandlerExists)
				{
					throw new MultipleQueryHandlersDefinedException(GetCommandName(query));
				}

				if (!asyncHandlerExists && !syncHandlerExists)
				{
					throw new HandlerNotFoundException(GetCommandName(query));
				}

				object result;
				if (asyncHandlerExists)
				{
					result = asyncHandler
						.GetType()
						.GetRuntimeMethod("HandleAsync", new[] { query.GetType(), typeof(CancellationToken) })
						.Invoke(asyncHandler, new object[] { query, cancellationToken });

					return await (Task<TResult>)result;
				}
				
				result = syncHandler.GetType().GetRuntimeMethod("Handle", new[] { query.GetType() }).Invoke(syncHandler, new object[] { query });
				return (TResult)result;
			}
		}

		private static bool TryGetAsyncHandler<TResult>(ILifetimeScope scope, IQuery<TResult> query, out object handler)
		{
			var asyncGenericType = typeof(IAsyncQueryHandler<,>);
			var closedAsyncGeneric = asyncGenericType.MakeGenericType(query.GetType(), typeof(TResult));
			return scope.TryResolve(closedAsyncGeneric, out handler);
		}

		private static bool TryGetSyncHandler<TResult>(ILifetimeScope scope, IQuery<TResult> query, out object handler)
		{
			var asyncGenericType = typeof(IQueryHandler<,>);
			var closedAsyncGeneric = asyncGenericType.MakeGenericType(query.GetType(), typeof(TResult));
			return scope.TryResolve(closedAsyncGeneric, out handler);
		}

		private static string GetCommandName(object command)
		{
			return command.GetType().Name;
		}
	}
}
