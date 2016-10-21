using System;

namespace CqrsEssentials.Autofac
{
	public class HandlerNotFoundException : Exception
	{
		internal HandlerNotFoundException(string typeName)
			: base($"No concrete handlers for type {typeName} were found.")
		{
		}
	}
}