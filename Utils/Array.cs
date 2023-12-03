﻿namespace Utils;

public static class ArrayExtensions
{
	public static char[,] ToCharArray(this string[] strings)
	{
		var lenght = strings.First().Length;
		var result = new char[strings.Count(), lenght];
		for (int i = 0; i < strings.Count(); i++)
		{
			var s = strings.ElementAt(i);

			for (int j = 0; j < lenght; j++)
			{
				result[i, j] = s[j];
			}
		}

		return result;
	}

	public static R[,] Select<T, R>(this T[,] items, Func<T, R> f)
	{
		int d0 = items.GetLength(0);
		int d1 = items.GetLength(1);
		R[,] result = new R[d0, d1];

		for (int i = 0; i < d0; i++)
		{
			for (int j = 0; j < d1; j++)
			{
				result[i, j] = f(items[i, j]);
			}
		}

		return result;
	}

	public static IEnumerable<T> Select<T>(this T[,] items, IEnumerable<Point> points)
	{
		foreach (var point in points)
		{
			if (point.IsWithin(items))
			{
				yield return items[point.X, point.Y];
			}
		}
	}

	public static IEnumerable<ArrayItem<T>> SelectItems<T>(this T[,] items, IEnumerable<Point> points)
	{
		foreach (var point in points)
		{
			if (point.IsWithin(items))
			{
				yield return new ArrayItem<T>(point, items[point.X, point.Y]);
			}
		}
	}

	public static IEnumerable<T> Flatten<T>(this T[,] items)
	{
		int d0 = items.GetLength(0);
		int d1 = items.GetLength(1);

		for (int i = 0; i < d0; i++)
		{
			for (int j = 0; j < d1; j++)
			{
				yield return items[i, j];
			}
		}
	}

	public static IEnumerable<Point> GetNeighbours<T>(this T[,] array, Point point, bool includeDiagonal)
	{
		return GenerateNeighbours(point, includeDiagonal)
			.Where(p => p.IsWithin(array))
			.ToList();
	}

	public static IEnumerable<T> GetNeighbourValues<T>(this T[,] array, Point point, bool includeDiagonal)
	{
		return array.Select(GenerateNeighbours(point, includeDiagonal));
	}

	public static IEnumerable<ArrayItem<T>> GetNeighbourItems<T>(this T[,] array, Point point, bool includeDiagonal)
	{
		return array.SelectItems(GenerateNeighbours(point, includeDiagonal));
	}

	public static T? GetLeft<T>(Point point, T[,] array)
	{
		if (point.X <= 0)
		{
			return default(T);
		}

		return array[point.X - 1, point.Y];
	}

	public static T? GetRight<T>(Point point, T[,] array)
	{
		if (point.X >= array.GetLength(0) - 1)
		{
			return default(T);
		}

		return array[point.X + 1, point.Y];
	}

	public static T? GetBottom<T>(Point point, T[,] array)
	{
		if (point.Y <= 0)
		{
			return default(T);
		}

		return array[point.X, point.Y - 1];
	}

	public static T? GetTop<T>(Point point, T[,] array)
	{
		if (point.Y >= array.GetLength(1) - 1)
		{
			return default(T);
		}

		return array[point.X, point.Y + 1];
	}

	private static IEnumerable<Point> GenerateNeighbours(Point point, bool includeDiagonal)
	{
		// 0 0 0
		// 0 p 0
		// 0 0 0
		yield return new Point(point.X - 1, point.Y);
		yield return new Point(point.X + 1, point.Y);
		yield return new Point(point.X, point.Y - 1);
		yield return new Point(point.X, point.Y + 1);

		if (includeDiagonal)
		{
			yield return new Point(point.X - 1, point.Y - 1);
			yield return new Point(point.X - 1, point.Y + 1);
			yield return new Point(point.X + 1, point.Y - 1);
			yield return new Point(point.X + 1, point.Y + 1);
		}
	}
}