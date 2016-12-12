using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials.Autofac.Tests.Queries
{
	public class TestQueryHandler : IQueryHandler<TestQuery, string>, IAsyncQueryHandler<TestQuery, string>
	{
		public string Handle(TestQuery query)
		{
			return query.QueryParameter.ToString();
		}

		public Task<string> HandleAsync(TestQuery query, CancellationToken cancellationToken = new CancellationToken())
		{
			return Task.FromResult(query.QueryParameter.ToString());
		}
	}
}