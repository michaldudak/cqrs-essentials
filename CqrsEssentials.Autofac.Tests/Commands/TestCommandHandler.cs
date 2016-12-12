using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials.Autofac.Tests.Commands
{
	public class TestCommandHandler : ICommandHandler<TestCommand>, IAsyncCommandHandler<TestCommand>
	{
		public void Handle(TestCommand command)
		{
			GlobalState.Current = command.CommandParmeter;
		}

		public Task HandleAsync(TestCommand command, CancellationToken cancellationToken = default(CancellationToken))
		{
			GlobalState.Current = command.CommandParmeter;
			return Task.CompletedTask;
		}
	}
}