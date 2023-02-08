using System.Linq;

namespace System.Collections.Generic;

public static class Extensions
{
	public static IList<T> Replace<T>(this IList<T> source, T oldValue, T newValue)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = source.IndexOf(oldValue);
		if (num != -1)
		{
			source[num] = newValue;
		}
		return source;
	}

	public static IList<T> ReplaceAll<T>(this IList<T> source, T oldValue, T newValue)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		int num = -1;
		do
		{
			num = source.IndexOf(oldValue);
			if (num != -1)
			{
				source[num] = newValue;
			}
		}
		while (num != -1);
		return source;
	}

	public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T oldValue, T newValue)
	{
		T oldValue2 = oldValue;
		T newValue2 = newValue;
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return source.Select((T x) => (!EqualityComparer<T>.Default.Equals(x, oldValue2)) ? x : newValue2);
	}

	public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
	{
		return listToClone.Select((T item) => (T)item.Clone()).ToList();
	}
}
