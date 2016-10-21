using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials.Autofac.Tests
{
	public class TestQueryHandler : IQueryHandler<TestQuery, string>, IAsyncQueryHandler<TestQuery, string>
	{
		public string Handle(TestQuery query)
		{
			return $"Query handler ran with parameter = {query.QueryParameter}";
		}

		public async Task<string> HandleAsync(TestQuery query, CancellationToken cancellationToken = new CancellationToken())
		{
			return $"Query handler ranAsync with parameter = {query.QueryParameter}";
		}
	}
}