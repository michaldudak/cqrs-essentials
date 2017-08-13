using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace CqrsEssentials.Autofac
{
	public class AutofacCommandDispatcher : ICommandDispatcher
	{
		private readonly ILifetimeScope _lifetimeScope;

		public AutofacCommandDispatcher(ILifetimeScope lifetimeScope)
		{
			_lifetimeScope = lifetimeScope;
		}

		protected virtual void HandlerExecuting(ICommand command)
		{
		}

		protected virtual void HandlerExecuted(ICommand command)
		{
		}

		public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand
		{
			using (var scope = _lifetimeScope.BeginLifetimeScope())
			{
				IAsyncCommandHandler<TCommand> asyncHandler;
				ICommandHandler<TCommand> syncHandler;

				var asyncHandlerExists = scope.TryResolve(out asyncHandler);
				var syncHandlerExists = scope.TryResolve(out syncHandler);

				if (asyncHandlerExists && syncHandlerExists)
				{
					throw new MultipleCommandHandlersDefinedException(GetCommandName(command));
				}

				if (!asyncHandlerExists && !syncHandlerExists)
				{
					throw new HandlerNotFoundException(GetCommandName(command));
				}

				HandlerExecuting(command);

				if (asyncHandlerExists)
				{
					await asyncHandler.HandleAsync(command, cancellationToken);
				}
				else
				{
					syncHandler.Handle(command);
				}

				HandlerExecuted(command);
			}
		}

		// TODO: merge common parts with the method above
		public async Task DispatchDynamicallyAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var scope = _lifetimeScope.BeginLifetimeScope())
			{
				var asyncCommandHandlerType = typeof(IAsyncCommandHandler<>).MakeGenericType(command.GetType());
				var syncCommandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

				object asyncHandler;
				object syncHandler;

				var asyncHandlerExists = scope.TryResolve(asyncCommandHandlerType, out asyncHandler);
				var syncHandlerExists = scope.TryResolve(syncCommandHandlerType, out syncHandler);

				if (asyncHandlerExists && syncHandlerExists)
				{
					throw new MultipleCommandHandlersDefinedException(GetCommandName(command));
				}

				if (!asyncHandlerExists && !syncHandlerExists)
				{
					throw new HandlerNotFoundException(GetCommandName(command));
				}

				HandlerExecuting(command);

				if (asyncHandlerExists)
				{
					var result = asyncHandler.GetType().GetRuntimeMethod("HandleAsync", new[] { command.GetType(), typeof(CancellationToken) }).Invoke(asyncHandler, new object[] { command, cancellationToken });
					await (Task)result;
				}
				else
				{
					syncHandler.GetType().GetRuntimeMethod("Handle", new[] { command.GetType() }).Invoke(syncHandler, new[] { command });
				}

				HandlerExecuted(command);
			}
		}

		private static string GetCommandName(object command)
		{
			return command.GetType().Name;
		}
	}
}