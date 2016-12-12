using System;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;

namespace CqrsEssentials.Autofac.Tests.Queries
{
	public class QueryDispatcherTests
	{
		[Test]
		public async Task GivenQueryWithRegisteredSyncHandler_ShouldRunTheHandler()
		{
			// given
			var queryDispatcher = CreateQueryDispatcher(builder => builder.RegisterType<TestQueryHandler>().As<IQueryHandler<TestQuery, string>>());

			// when
			var query = new TestQuery { QueryParameter = 42 };
			var result = await queryDispatcher.DispatchAsync(query);

			Assert.AreEqual("42", result);
		}

		[Test]
		public async Task GivenQueryWithRegisteredAsyncHandler_ShouldRunTheHandler()
		{
			// given
			var queryDispatcher = CreateQueryDispatcher(builder => builder.RegisterType<TestQueryHandler>().As<IAsyncQueryHandler<TestQuery, string>>());

			// when
			var query = new TestQuery { QueryParameter = 42 };
			var result = await queryDispatcher.DispatchAsync(query);

			Assert.AreEqual("42", result);
		}

		[Test]
		public void GivenQueryWithRegisteredBothAsyncAndSyncHandlers_ShouldThrowMultipleQueryHandlersDefinedException()
		{
			// given
			var queryDispatcher = CreateQueryDispatcher(builder =>
			{
				builder.RegisterType<TestQueryHandler>().As<IQueryHandler<TestQuery, string>>();
				builder.RegisterType<TestQueryHandler>().As<IAsyncQueryHandler<TestQuery, string>>();
			});

			// when
			var query = new TestQuery { QueryParameter = 42 };
			Assert.ThrowsAsync<MultipleQueryHandlersDefinedException>(async () => await queryDispatcher.DispatchAsync(query));
		}

		private IQueryDispatcher CreateQueryDispatcher(Action<ContainerBuilder> registration)
		{
			var container = TestHelpers.SetupDI(registration);
			return container.Resolve<IQueryDispatcher>();
		} 
	}
}