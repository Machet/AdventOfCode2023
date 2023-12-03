namespace Utils;

public record Point(int X, int Y)
{
	public bool IsWithin<T>(T[,] array)
	{
		return X >= 0 && X < array.GetLength(0) && Y >= 0 && Y < array.GetLength(1);
	}
}
