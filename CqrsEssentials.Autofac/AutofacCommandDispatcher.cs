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
				if (scope.TryResolve(out asyncHandler))
				{
					HandlerExecuting(command);
					await asyncHandler.HandleAsync(command, cancellationToken);
					HandlerExecuted(command);
					return;
				}

				ICommandHandler<TCommand> syncHandler;
				if (scope.TryResolve(out syncHandler))
				{
					HandlerExecuting(command);
					syncHandler.Handle(command);
					HandlerExecuted(command);
					return;
				}

				var typeName = command.GetType().Name;
				throw new HandlerNotFoundException(typeName);
			}
		}
	}
}