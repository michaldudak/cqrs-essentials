using System;

namespace CqrsEssentials.Autofac
{
	public class MultipleQueryHandlersDefinedException : Exception
	{
		internal MultipleQueryHandlersDefinedException(string typeName)
			: base($"Both sync and async handlers for type {typeName} were found. Only one handler can exist for a given Query.")
		{
		}
	}
}