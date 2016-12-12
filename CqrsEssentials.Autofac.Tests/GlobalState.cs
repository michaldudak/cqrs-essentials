namespace CqrsEssentials.Autofac.Tests
{
	public static class GlobalState
	{
		public static int Current { get; set; }

		public static void Reset() => Current = 0;
	}
}