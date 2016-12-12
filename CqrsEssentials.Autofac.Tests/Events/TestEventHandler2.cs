using System.Threading.Tasks;

namespace CqrsEssentials.Autofac.Tests.Events
{
	public class TestEventHandler2 : IEventHandler<TestEvent>, IAsyncEventHandler<TestEvent>
	{
		public void Handle(TestEvent anEvent)
		{
			GlobalState.Current += anEvent.Argument * 10;
		}

		public Task HandleAsync(TestEvent anEvent)
		{
			GlobalState.Current += anEvent.Argument * 10;
			return Task.CompletedTask;
		}
	}
}