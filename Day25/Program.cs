using System.Collections.Immutable;

var connections = File.ReadAllLines("input.txt")
	.SelectMany(line => ParseConnection(line).ToList())
	.GroupBy(c => c.from)
	.ToDictionary(x => x.Key, x => x.Select(xx => xx.to).ToList());

var vertices = connections.Keys;
var start = vertices.First();
var firstGroupCount = 1;
var secondGroupCount = 0;

foreach (var other in vertices.Skip(1))
{
	var count = GetConnectionCount(start, other, connections);
	firstGroupCount += count > 3 ? 1 : 0;
	secondGroupCount += count > 3 ? 0 : 1;
}

Console.WriteLine("1: " + (firstGroupCount * secondGroupCount));

static int GetConnectionCount(string from, string to, Dictionary<string, List<string>> connections)
{
	var currentConnections = connections.ToDictionary(x => x.Key, x => x.Value.ToList());
	var count = 0;
	var path = GetShortestPath(from, to, currentConnections);

	while (path.Any())
	{
		count++;
		foreach (var connection in path)
		{
			currentConnections[connection.from].Remove(connection.to);
			currentConnections[connection.to].Remove(connection.from);
		}

		path = GetShortestPath(from, to, currentConnections);
	}

	return count;
}

static List<Connection> GetShortestPath(string from, string to, Dictionary<string, List<string>> connections)
{
	var queue = new Queue<State>();
	var visited = new HashSet<string>();

	queue.Enqueue(new State(from, to, ImmutableHashSet<Connection>.Empty));
	visited.Add(from);

	while (queue.Count != 0)
	{
		var state = queue.Dequeue();
		if (state.from == to)
		{
			return state.connections.ToList();
		}

		foreach (var connection in connections[state.from])
		{
			if (visited.Add(connection))
			{
				queue.Enqueue(new State(connection, to, state.connections.Add(new Connection(state.from, connection))));
			}
		}
	}

	return new List<Connection>();
}

IEnumerable<Connection> ParseConnection(string line)
{
	var parts = line.Split(":");
	foreach (var c in parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries))
	{
		yield return new Connection(parts[0], c);
		yield return new Connection(c, parts[0]);
	}
}

record Connection(string from, string to);
record State(string from, string to, ImmutableHashSet<Connection> connections);