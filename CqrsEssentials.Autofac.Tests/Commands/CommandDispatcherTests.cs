using System;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;

namespace CqrsEssentials.Autofac.Tests.Commands
{
	public class CommandDispatcherTests
	{
		[SetUp]
		public void BeforeEach()
		{
			GlobalState.Reset();
		}

		[Test]
		public async Task GivenCommandWithRegisteredSyncHandler_WhenDispatchedStatically_ShouldRunTheHandler()
		{
			// given
			var commandDispatcher = CreateCommandDispatcher(builder =>
				builder.RegisterType<TestCommandHandler>().As<ICommandHandler<TestCommand>>());

			// when
			var command = new TestCommand { CommandParmeter = 42 };
			await commandDispatcher.DispatchAsync(command);

			// then
			Assert.AreEqual(42, GlobalState.Current);
		}

		[Test]
		public async Task GivenCommandWithRegisteredSyncHandler_WhenDispatchedDynamically_ShouldRunTheHandler()
		{
			// given
			var commandDispatcher = CreateCommandDispatcher(builder =>
				builder.RegisterType<TestCommandHandler>().As<ICommandHandler<TestCommand>>());

			// when
			ICommand command = new TestCommand { CommandParmeter = 42 };
			await commandDispatcher.DispatchDynamicallyAsync(command);

			// then
			Assert.AreEqual(42, GlobalState.Current);
		}

		[Test]
		public async Task GivenCommandWithRegisteredAsyncHandler_WhenDispatchedStatically_ShouldRunTheHandler()
		{
			// given
			var commandDispatcher = CreateCommandDispatcher(builder =>
				builder.RegisterType<TestCommandHandler>().As<IAsyncCommandHandler<TestCommand>>());

			// when
			var command = new TestCommand { CommandParmeter = 42 };
			await commandDispatcher.DispatchAsync(command);

			// then
			Assert.AreEqual(42, GlobalState.Current);
		}

		[Test]
		public async Task GivenCommandWithRegisteredAsyncHandler_WhenDispatchedDynamically_ShouldRunTheHandler()
		{
			// given
			var commandDispatcher = CreateCommandDispatcher(builder =>
				builder.RegisterType<TestCommandHandler>().As<IAsyncCommandHandler<TestCommand>>());

			// when
			ICommand command = new TestCommand { CommandParmeter = 42 };
			await commandDispatcher.DispatchDynamicallyAsync(command);

			// then
			Assert.AreEqual(42, GlobalState.Current);
		}

		[Test]
		public void GivenCommandWithNoRegisteredHandlers_WhenDispatchedStatically_ShouldThrowHandlerNotFoundException()
		{
			// given
			var commandDispatcher = CreateCommandDispatcher();

			// when, then
			var command = new TestCommand { CommandParmeter = 42 };
			Assert.ThrowsAsync<HandlerNotFoundException>(async () => await commandDispatcher.DispatchAsync(command));
		}

		[Test]
		public void GivenCommandWithNoRegisteredHandlers_WhenDispatchedDynamically_ShouldThrowHandlerNotFoundException()
		{
			// given
			var commandDispatcher = CreateCommandDispatcher();

			// when, then
			ICommand command = new TestCommand { CommandParmeter = 42 };
			Assert.ThrowsAsync<HandlerNotFoundException>(async () => await commandDispatcher.DispatchDynamicallyAsync(command));
		}

		[Test]
		public void GivenCommandWithRegisteredBothAsyncAndSyncHandlers_WhenDispatchedStatically_ShouldThrowMultipleCommandHandlersDefinedException()
		{
			// given
			var commandDispatcher = CreateCommandDispatcher(builder =>
			{
				builder.RegisterType<TestCommandHandler>().As<ICommandHandler<TestCommand>>();
				builder.RegisterType<TestCommandHandler>().As<IAsyncCommandHandler<TestCommand>>();
			});

			// when, then
			var command = new TestCommand { CommandParmeter = 42 };
			Assert.ThrowsAsync<MultipleCommandHandlersDefinedException>(async () => await commandDispatcher.DispatchAsync(command));
		}

		[Test]
		public void GivenCommandWithRegisteredBothAsyncAndSyncHandlers_WhenDispatchedDynamically_ShouldThrowMultipleCommandHandlersDefinedException()
		{
			// given
			var commandDispatcher = CreateCommandDispatcher(builder =>
			{
				builder.RegisterType<TestCommandHandler>().As<ICommandHandler<TestCommand>>();
				builder.RegisterType<TestCommandHandler>().As<IAsyncCommandHandler<TestCommand>>();
			});

			// when, then
			ICommand command = new TestCommand { CommandParmeter = 42 };
			Assert.ThrowsAsync<MultipleCommandHandlersDefinedException>(async () => await commandDispatcher.DispatchDynamicallyAsync(command));
		}

		private static ICommandDispatcher CreateCommandDispatcher(Action<ContainerBuilder> registration = null)
		{
			var container = TestHelpers.SetupDI(registration);
			return container.Resolve<ICommandDispatcher>();
		}
	}
}
