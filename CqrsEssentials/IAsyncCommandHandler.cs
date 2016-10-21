using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface IAsyncCommandHandler<in TCommand> where TCommand : ICommand
	{
		Task HandleAsync(TCommand command, CancellationToken cancellationToken = default(CancellationToken));
	}
}