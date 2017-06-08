using Autofac;

namespace CqrsEssentials.Autofac
{
	public class CqrsEssentialsAutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<AutofacCommandDispatcher>().As<ICommandDispatcher>().InstancePerLifetimeScope();
			builder.RegisterType<AutofacEventDispatcher>().As<IEventDispatcher>().InstancePerLifetimeScope();
			builder.RegisterType<AutofacQueryDispatcher>().As<IQueryDispatcher>().InstancePerLifetimeScope();
		}
	}
}
