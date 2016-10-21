namespace CqrsEssentials.Autofac.Tests
{
	public class TestQuery : IQuery<string>
	{
		public int QueryParameter { get; set; }
	}
}
