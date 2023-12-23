using System.Collections.Immutable;
using System.Data;
using Utils;

var map = File.ReadAllLines("input.txt")
	.ToCharArray();

var starting = map.FindArrayItemInRow('.', 0).Single().Point;
var end = map.FindArrayItemInRow('.', map.GetLength(0) - 1).Single().Point;

var result1 = ProcessPaths(starting, end, map).ToList();
var connections = GenerateConnections(starting, end, map);
var pathLenghts = GetPathLengths(starting, end, connections).ToList();

Console.WriteLine("1: " + result1.MaxBy(s => s.length)!.length);
Console.WriteLine("2: " + pathLenghts.Max());

static IEnumerable<State> ProcessPaths(Point starting, Point end, char[,] map)
{
	var toProcess = new Queue<State>();
	toProcess.Enqueue(new State(starting, starting, starting, 0));
	while (toProcess.Count > 0)
	{
		var state = toProcess.Dequeue();
		if (state.position == end)
		{
			yield return state; continue;
		}

		var nextStates = state.GetNextStates(map, true).ToList();
		toProcess.EnqueueRange(nextStates);
	}
}

static IEnumerable<long> GetPathLengths(Point starting, Point end, List<Connection> connections)
{
	var connectionMap = connections.GroupBy(c => c.from)
		.ToDictionary(g => g.Key, g => g);

	var toProcess = new Queue<PathState>();
	toProcess.Enqueue(new PathState(starting, 0, ImmutableHashSet<Point>.Empty));

	while (toProcess.Count > 0)
	{
		var state = toProcess.Dequeue();
		var connected = connectionMap[state.current];

		if (state.current == end)
		{
			yield return state.length;
			continue;
		}

		foreach (var connection in connected)
		{
			if (state.visited.Contains(connection.to)) continue;
			toProcess.Enqueue(new PathState(connection.to, state.length + connection.length, state.visited.Add(state.current)));
		}
	}
}

static List<Connection> GenerateConnections(Point starting, Point end, char[,] map)
{
	var toProcess = new Queue<State>();
	var visited = new HashSet<Point>();
	var connections = new HashSet<Connection>();

	toProcess.Enqueue(new State(starting, starting, starting, 0));
	while (toProcess.Count > 0)
	{
		var state = toProcess.Dequeue();

		if (state.position == end)
		{
			connections.Add(new Connection(state.startingPositoin, end, state.length));
			continue;
		}

		var nextStates = state.GetNextStates(map, false).ToList();
		if (nextStates.Count > 1)
		{
			foreach (var nextState in nextStates)
			{
				connections.Add(new Connection(state.startingPositoin, state.position, state.length));
				if (visited.Add(nextState.position))
				{
					toProcess.Enqueue(new State(state.position, nextState.position, nextState.previousPosition, 1));
				}
			}
		}
		else
		{
			visited.AddRange(nextStates.Select(s => s.position));
			toProcess.EnqueueRange(nextStates);
		}
	}

	return connections.Union(connections.Select(c => new Connection(c.to, c.from, c.length))).ToList();
}

record State(Point startingPositoin, Point position, Point previousPosition, int length)
{
	public IEnumerable<State> GetNextStates(char[,] map, bool withSlopes)
	{
		var next = withSlopes ? GenerateNextPositionsWithSlopes(map) : GenerateNextPositions(map);
		foreach (var pos in next.Where(p => p != null))
		{
			yield return new State(startingPositoin, pos!.Point, position, length + 1);
		}
	}

	public IEnumerable<ArrayItem<char>?> GenerateNextPositions(char[,] map)
	{
		yield return GetPosition(map.GetNorthOf(position));
		yield return GetPosition(map.GetSouthOf(position));
		yield return GetPosition(map.GetWestOf(position));
		yield return GetPosition(map.GetEastOf(position));
	}

	public IEnumerable<ArrayItem<char>?> GenerateNextPositionsWithSlopes(char[,] map)
	{
		yield return GetPosition(map.GetNorthOf(position), '.');
		yield return GetPosition(map.GetSouthOf(position), 'v');
		yield return GetPosition(map.GetWestOf(position), '<');
		yield return GetPosition(map.GetEastOf(position), '>');
	}

	public ArrayItem<char>? GetPosition(ArrayItem<char>? position, char? allowedValue = null)
	{
		if (position == null) return null;
		if (position.Point == previousPosition) return null;
		if (position.Item == '#') return null;
		if (position.Item == '.') return position;
		return (allowedValue == null || position.Item == allowedValue) ? position : null;
	}
}

record Connection(Point from, Point to, int length);
record PathState(Point current, int length, ImmutableHashSet<Point> visited);