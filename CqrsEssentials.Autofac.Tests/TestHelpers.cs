using Autofac;
using System;

namespace CqrsEssentials.Autofac.Tests
{
	public static class TestHelpers
	{
		public static IContainer SetupDI(Action<ContainerBuilder> additionalRegistration)
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule<CqrsEssentialsAutofacModule>();
			additionalRegistration?.Invoke(builder);
			return builder.Build();
		}
	}
}
