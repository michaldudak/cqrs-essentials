namespace CqrsEssentials.Autofac.Tests.Queries
{
	public class TestQuery : IQuery<string>
	{
		public int QueryParameter { get; set; }
	}
}
