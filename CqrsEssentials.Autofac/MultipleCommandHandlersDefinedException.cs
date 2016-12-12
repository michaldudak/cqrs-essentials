using System;

namespace CqrsEssentials.Autofac
{
	public class MultipleCommandHandlersDefinedException : Exception
	{
		internal MultipleCommandHandlersDefinedException(string typeName)
			: base($"Both sync and async handlers for type {typeName} were found. Only one handler can exist for a given Command.")
		{
		}
	}
}