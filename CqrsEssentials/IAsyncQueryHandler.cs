using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials
{
	public interface IAsyncQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
	{
		Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default(CancellationToken));
	}
}