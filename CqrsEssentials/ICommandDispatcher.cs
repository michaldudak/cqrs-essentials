using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface ICommandDispatcher
	{
		/// <summary>
		/// Runs the command handler registered for the given command type.
		/// If there is no handler for a given command type or there is more than one, this method will throw.
		/// Note that the type of the command must be known at compile time. If it's not, call DispatchDynamicallyAsync() instead.
		/// </summary>
		/// <typeparam name="TCommand">Type of the command</typeparam>
		/// <param name="command">Instance of the command</param>
		/// <param name="cancellationToken">Optional task cancellation token</param>
		/// <returns>Task that completes when the handler finished processing</returns>
		Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken)) where TCommand : ICommand;

		/// <summary>
		/// Runs the command handler registered for the given command type.
		/// If there is no handler for a given command type or there is more than one, this method will throw.
		/// Use this method if you don't know the exact command type at compile time (e.g. it's just an ICommand).
		/// If you do know it, call DispatchAsync() instead, it'll be faster.
		/// </summary>
		/// <param name="command">Instance of the command</param>
		/// <param name="cancellationToken">Optional task cancellation token</param>
		/// <returns>Task that completes when the handler finished processing</returns>
		Task DispatchDynamicallyAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken));
	}
}