using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface IQueryDispatcher
	{
		Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken)); 
	}
}