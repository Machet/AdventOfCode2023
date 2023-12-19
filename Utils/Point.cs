namespace Utils;

public record Point(int X, int Y)
{
	public bool IsWithin<T>(T[,] array)
	{
		return X >= 0 && X < array.GetLength(0) && Y >= 0 && Y < array.GetLength(1);
	}

	public Point GetNorthOf(int dist = 1)
	{
		return new Point(X - dist, Y);
	}

	public Point GetSouthOf(int dist = 1)
	{
		return new Point(X + dist, Y);
	}

	public Point GetWestOf(int dist = 1)
	{
		return new Point(X, Y - dist);
	}

	public Point GetEastOf(int dist = 1)
	{
		return new Point(X, Y + dist);
	}

	public IEnumerable<Point> GetNeighbours()
	{
		yield return GetNorthOf();
		yield return GetEastOf();
		yield return GetSouthOf();
		yield return GetWestOf();
	}
}