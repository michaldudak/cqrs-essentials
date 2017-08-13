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
				var asyncHandlerExists = scope.TryResolve(out IAsyncCommandHandler<TCommand> asyncHandler);
				var syncHandlerExists = scope.TryResolve(out ICommandHandler<TCommand> syncHandler);

				EnsureSingleHandlerExists(command, asyncHandlerExists, syncHandlerExists);

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

		public async Task DispatchDynamicallyAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var scope = _lifetimeScope.BeginLifetimeScope())
			{
				var asyncCommandHandlerType = typeof(IAsyncCommandHandler<>).MakeGenericType(command.GetType());
				var syncCommandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

				var asyncHandlerExists = scope.TryResolve(asyncCommandHandlerType, out object asyncHandler);
				var syncHandlerExists = scope.TryResolve(syncCommandHandlerType, out object syncHandler);

				EnsureSingleHandlerExists(command, asyncHandlerExists, syncHandlerExists);

				HandlerExecuting(command);

				if (asyncHandlerExists)
				{
					await DynamicCallHelper.CallHandleAsync(asyncHandler, command, cancellationToken);
				}
				else
				{
					DynamicCallHelper.CallHandle(syncHandler, command);
				}

				HandlerExecuted(command);
			}
		}

		private void EnsureSingleHandlerExists(ICommand command, bool asyncHandlerExists, bool syncHandlerExists)
		{
			if (asyncHandlerExists && syncHandlerExists)
			{
				throw new MultipleCommandHandlersDefinedException(GetCommandName(command));
			}

			if (!asyncHandlerExists && !syncHandlerExists)
			{
				throw new HandlerNotFoundException(GetCommandName(command));
			}
		}

		private static string GetCommandName(object command)
		{
			return command.GetType().Name;
		}
	}
}