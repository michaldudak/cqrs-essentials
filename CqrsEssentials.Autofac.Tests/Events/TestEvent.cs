namespace CqrsEssentials.Autofac.Tests.Events
{
	public class TestEvent : IEvent
	{
		public int Argument { get; }

		public TestEvent(int argument)
		{
			Argument = argument;
		}
	}
}