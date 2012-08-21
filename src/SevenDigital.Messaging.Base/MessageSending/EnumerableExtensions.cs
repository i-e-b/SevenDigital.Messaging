using System;
using System.Collections.Generic;

namespace SevenDigital.Messaging.MessageSending
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Call an action for each item in the enumerable.
		/// </summary>
		public static void ForEach<T>(this IEnumerable<T> thing, Action<T> action)
		{
			foreach (var t in thing) action(t);
		}
	}
}
