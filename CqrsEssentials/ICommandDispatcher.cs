using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface ICommandDispatcher
	{
		Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand;

		Task DispatchDynamicallyAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken));
	}
}