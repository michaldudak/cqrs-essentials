using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface IQueryDispatcher
	{
		/// <summary>
		/// Runs the query handler registered for the given command type.
		/// If there is no handler for a given query type or there is more than one, this method will throw.
		/// </summary>
		/// <typeparam name="TResult">Type of the query</typeparam>
		/// <param name="query">Instance of the query</param>
		/// <param name="cancellationToken">Optional cancellation token</param>
		/// <returns>Task that resolves to a result of the query handler</returns>
		Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken)); 
	}
}