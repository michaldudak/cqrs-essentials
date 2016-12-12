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

		private static string GetCommandName(object command)
		{
			return command.GetType().Name;
		}
	}
}