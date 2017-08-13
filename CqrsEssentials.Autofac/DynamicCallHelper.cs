using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CqrsEssentials.Autofac
{
	internal static class DynamicCallHelper
	{
		public static Task CallHandleAsync(object handler, object parameter, CancellationToken cancellationToken)
		{
			return (Task)handler
				.GetType()
				.GetRuntimeMethod("HandleAsync", new[] { parameter.GetType(), typeof(CancellationToken) })
				.Invoke(handler, new object[] { parameter, cancellationToken });
		}

		public static Task CallHandleAsync(object handler, object parameter)
		{
			return (Task)handler
				.GetType()
				.GetRuntimeMethod("HandleAsync", new[] { parameter.GetType() })
				.Invoke(handler, new object[] { parameter });
		}

		public static void CallHandle(object handler, object parameter)
		{
			handler.GetType().GetRuntimeMethod("Handle", new[] { parameter.GetType() }).Invoke(handler, new[] { parameter });
		}
	}
}