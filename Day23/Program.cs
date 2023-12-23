using Utils;

var map = File.ReadAllLines("input.txt")
	.ToCharArray();

var starting = map.FindArrayItemInRow('.', 0).Single().Point;
var end = map.FindArrayItemInRow('.', map.GetLength(0) - 1).Single().Point;

var result = ProcessPaths(starting, end, map).ToList();
Console.WriteLine("1: " + result.MaxBy(s => s.length)!.length);

static IEnumerable<State> ProcessPaths(Point starting, Point end, char[,] map)
{
	var toProcess = new Queue<State>();
	toProcess.Enqueue(new State(starting, starting, 0));
	while (toProcess.Count > 0)
	{
		var state = toProcess.Dequeue();
		if (state.position == end)
		{
			yield return state; continue;
		}

		var nextStates = state.GetNextStates(map).ToList();
		toProcess.EnqueueRange(nextStates);
	}
}

record State(Point position, Point previousPosition, int length)
{
	public IEnumerable<State> GetNextStates(char[,] map)
	{
		foreach (var pos in GenerateNextPositions(map).Where(p => p != null))
		{
			yield return new State(pos!.Point, position, length + 1);
		}
	}

	public IEnumerable<ArrayItem<char>?> GenerateNextPositions(char[,] map)
	{
		yield return GetPosition(map.GetNorthOf(position), '.');
		yield return GetPosition(map.GetSouthOf(position), 'v');
		yield return GetPosition(map.GetWestOf(position), '<');
		yield return GetPosition(map.GetEastOf(position), '>');
	}

	public ArrayItem<char>? GetPosition(ArrayItem<char>? position, char allowedValue)
	{
		if (position == null) return null;
		if (position.Point == previousPosition) return null;
		return (position.Item == '.' || position.Item == allowedValue) ? position : null;
	}
}
