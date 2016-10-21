using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;

namespace CqrsEssentials.Autofac.Tests
{
	public class QueryDispatcherTests
	{
		IContainer _container;
		IQueryDispatcher _queryDispatcher;

		[SetUp]
		public void RegisterDependencies()
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule<CqrsEssentialsAutofacModule>();
			builder.RegisterType<TestQueryHandler>().As<IQueryHandler<TestQuery, string>>();
			//builder.RegisterType<TestQueryHandler>().As<IAsyncQueryHandler<TestQuery, string>>();

			_container = builder.Build();
			_queryDispatcher = _container.Resolve<IQueryDispatcher>();
		}

		[Test]
		public async Task GivenQueryWithRegisteredHandler_ShouldRunTheHandler()
		{
			var query = new TestQuery { QueryParameter = 42 };
			var result = await _queryDispatcher.DispatchAsync(query);

			Assert.AreEqual("Query handler ran with parameter = 42", result);
		}
	}
}