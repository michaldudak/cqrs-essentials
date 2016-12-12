using System.Threading.Tasks;

namespace CqrsEssentials.Autofac.Tests.Events
{
	public class TestEventHandler1 : IEventHandler<TestEvent>, IAsyncEventHandler<TestEvent>
	{
		public void Handle(TestEvent anEvent)
		{
			GlobalState.Current += anEvent.Argument;
		}

		public Task HandleAsync(TestEvent anEvent)
		{
			GlobalState.Current += anEvent.Argument;
			return Task.CompletedTask;
		}
	}
}