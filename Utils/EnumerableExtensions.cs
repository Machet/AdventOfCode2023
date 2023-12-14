namespace Utils;

public static class EnumerableExtensions
{
	public static IEnumerable<List<T>> Split<T>(this IEnumerable<T> source, int count)
	{
		return source
			.Select((x, i) => new { Index = i, Value = x })
			.GroupBy(x => x.Index / count)
			.Select(x => x.Select(v => v.Value).ToList());
	}

	public static IEnumerable<T> IterateInCircles<T>(this IEnumerable<T> items)
	{
		while (true)
		{
			foreach (var item in items)
			{
				yield return item;
			}
		}
	}
}
