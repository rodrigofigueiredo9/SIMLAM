#pragma warning disable
using System;

namespace Yogesh.Extensions
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class ExtensionAttribute : Attribute
	{
	}
}
#pragma warning enable
