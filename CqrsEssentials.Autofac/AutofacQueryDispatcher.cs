using System.Reflection;
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
				var asyncGenericType = typeof(IAsyncQueryHandler<,>);
				var closedAsyncGeneric = asyncGenericType.MakeGenericType(query.GetType(), typeof(TResult));

				object asyncHandler;
				if (scope.TryResolve(closedAsyncGeneric, out asyncHandler))
				{
					var result = asyncHandler
						.GetType()
						.GetRuntimeMethod("HandleAsync", new[] { query.GetType(), typeof(CancellationToken) })
						.Invoke(asyncHandler, new object[] { query, cancellationToken });

					return await (Task<TResult>)result;
				}

				var syncGenericType = typeof(IQueryHandler<,>);
				var closedSyncGeneric = syncGenericType.MakeGenericType(query.GetType(), typeof(TResult));

				object syncHandler;
				if (scope.TryResolve(closedSyncGeneric, out syncHandler))
				{
					var result = syncHandler.GetType().GetRuntimeMethod("Handle", new[] { query.GetType() }).Invoke(syncHandler, new[] { query });
					return (TResult)result;
				}

				var typeName = query.GetType().Name;
				throw new HandlerNotFoundException(typeName);
			}
		}
	}
}
