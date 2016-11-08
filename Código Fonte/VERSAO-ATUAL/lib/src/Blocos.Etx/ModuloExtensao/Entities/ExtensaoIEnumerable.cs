using System;
using System.Collections;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloExtensao.Entities
{
	public static class ExtensaoIEnumerable
	{
		// Foreach com índice
		public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
		{
			var i = 0;
			foreach (var e in ie) action(e, i++);
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (seenKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}
	}
}
