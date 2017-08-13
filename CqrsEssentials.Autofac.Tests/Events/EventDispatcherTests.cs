using System;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;

namespace CqrsEssentials.Autofac.Tests.Events
{
	public class EventDispatcherTests
	{
		[SetUp]
		public void BeforeEach()
		{
			GlobalState.Reset();
		}

		[Test]
		public async Task GivenEventWithRegisteredSyncHandler_WhenDispatchedStatically_ShouldRunTheHandler()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
				builder.RegisterType<TestEventHandler1>().As<IEventHandler<TestEvent>>());

			// when
			var anEvent = new TestEvent(1);
			await eventDispatcher.DispatchAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 1);
		}

		[Test]
		public async Task GivenEventWithRegisteredSyncHandler_WhenDispatchedDynamically_ShouldRunTheHandler()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
				builder.RegisterType<TestEventHandler1>().As<IEventHandler<TestEvent>>());

			// when
			IEvent anEvent = new TestEvent(1);
			await eventDispatcher.DispatchDynamicallyAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 1);
		}

		[Test]
		public async Task GivenEventWithRegisteredAsyncHandler_WhenDispatchedStatically_ShouldRunTheHandler()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
				builder.RegisterType<TestEventHandler1>().As<IAsyncEventHandler<TestEvent>>());

			// when
			var anEvent = new TestEvent(1);
			await eventDispatcher.DispatchAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 1);
		}

		[Test]
		public async Task GivenEventWithRegisteredAsyncHandler_WhenDispatchedDynamically_ShouldRunTheHandler()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
				builder.RegisterType<TestEventHandler1>().As<IAsyncEventHandler<TestEvent>>());

			// when
			IEvent anEvent = new TestEvent(1);
			await eventDispatcher.DispatchDynamicallyAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 1);
		}

		[Test]
		public async Task GivenEventWithTwoRegisteredSyncHandlers_WhenDispatchedStatically_ShouldRunBoth()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
			{
				builder.RegisterType<TestEventHandler1>().As<IEventHandler<TestEvent>>();
				builder.RegisterType<TestEventHandler2>().As<IEventHandler<TestEvent>>();
			});

			// when
			var anEvent = new TestEvent(2);
			await eventDispatcher.DispatchAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 22);
		}

		[Test]
		public async Task GivenEventWithTwoRegisteredSyncHandlers_WhenDispatchedDynamically_ShouldRunBoth()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
			{
				builder.RegisterType<TestEventHandler1>().As<IEventHandler<TestEvent>>();
				builder.RegisterType<TestEventHandler2>().As<IEventHandler<TestEvent>>();
			});

			// when
			IEvent anEvent = new TestEvent(2);
			await eventDispatcher.DispatchDynamicallyAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 22);
		}

		[Test]
		public async Task GivenEventWithTwoRegisteredAsyncHandlers_WhenDispatchedStatically_ShouldRunBoth()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
			{
				builder.RegisterType<TestEventHandler1>().As<IAsyncEventHandler<TestEvent>>();
				builder.RegisterType<TestEventHandler2>().As<IAsyncEventHandler<TestEvent>>();
			});

			// when
			var anEvent = new TestEvent(2);
			await eventDispatcher.DispatchAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 22);
		}

		[Test]
		public async Task GivenEventWithTwoRegisteredAsyncHandlers_WhenDispatchedDynamically_ShouldRunBoth()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
			{
				builder.RegisterType<TestEventHandler1>().As<IAsyncEventHandler<TestEvent>>();
				builder.RegisterType<TestEventHandler2>().As<IAsyncEventHandler<TestEvent>>();
			});

			// when
			IEvent anEvent = new TestEvent(2);
			await eventDispatcher.DispatchDynamicallyAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 22);
		}


		[Test]
		public async Task GivenEventWithTwoRegisteredSyncAndAsyncHandlers_WhenDispatchedStatically_ShouldRunBoth()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
			{
				builder.RegisterType<TestEventHandler1>().As<IEventHandler<TestEvent>>();
				builder.RegisterType<TestEventHandler2>().As<IAsyncEventHandler<TestEvent>>();
			});

			// when
			var anEvent = new TestEvent(2);
			await eventDispatcher.DispatchAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 22);
		}

		[Test]
		public async Task GivenEventWithTwoRegisteredSyncAndAsyncHandlers_WhenDispatchedDynamically_ShouldRunBoth()
		{
			// given
			var eventDispatcher = CreateEventDispatcher(builder =>
			{
				builder.RegisterType<TestEventHandler1>().As<IEventHandler<TestEvent>>();
				builder.RegisterType<TestEventHandler2>().As<IAsyncEventHandler<TestEvent>>();
			});

			// when
			IEvent anEvent = new TestEvent(2);
			await eventDispatcher.DispatchDynamicallyAsync(anEvent);

			// then
			Assert.AreEqual(GlobalState.Current, 22);
		}

		private static IEventDispatcher CreateEventDispatcher(Action<ContainerBuilder> registration)
		{
			var container = TestHelpers.SetupDI(registration);
			return container.Resolve<IEventDispatcher>();
		}
	}
}
