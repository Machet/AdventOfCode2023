using Utils;

var input = File.ReadAllLines("input.txt")
	.ToCharArray();

var start = new Beam(new Point(0, -1), 'E');
var energized1 = GetEnergizedCount(start, input);

var starting = GenerateStartingBeams(input).ToList();
var energized = starting.Select(x => GetEnergizedCount(x, input)).ToList();

Console.WriteLine("1: " + energized1);
Console.WriteLine("2: " + energized.Max());

static int GetEnergizedCount(Beam startingPosition, char[,] input)
{
	var toProcess = new Queue<Beam>();
	var energized = new HashSet<Point>();
	var visited = new HashSet<Beam>();
	toProcess.Enqueue(startingPosition);

	while (toProcess.Count > 0)
	{
		var newPositions = Fly(toProcess.Dequeue(), input);
		foreach (var position in newPositions)
		{
			if (visited.Add(position))
			{
				toProcess.Enqueue(position);
				energized.Add(position.point);
			}
		}
	}

	return energized.Count;
}

IEnumerable<Beam> GenerateStartingBeams(char[,] input)
{
	var maxX = input.GetLength(0);
	var maxY = input.GetLength(1);

	for (int i = 0; i < maxX; i++)
	{
		yield return new Beam(new Point(i, -1), 'E');
		yield return new Beam(new Point(i, maxY), 'W');
	}

	for (int i = 0; i < maxY; i++)
	{
		yield return new Beam(new Point(-1, i), 'S');
		yield return new Beam(new Point(maxX, i), 'N');
	}
}

static List<Beam> Fly(Beam beam, char[,] input)
{
	var nextPos = input.GetInDirection(beam.point, beam.dir);
	if (nextPos == null)
	{
		return new List<Beam>();
	}

	var nextDirs = (beam.dir, nextPos.Item) switch
	{
		(_, '.') => new[] { beam.dir },
		('N', '-') => new[] { 'W', 'E' },
		('S', '-') => new[] { 'W', 'E' },
		('W', '-') => new[] { 'W' },
		('E', '-') => new[] { 'E' },
		('E', '|') => new[] { 'N', 'S' },
		('W', '|') => new[] { 'N', 'S' },
		('N', '|') => new[] { 'N' },
		('S', '|') => new[] { 'S' },
		('E', '/') => new[] { 'N' },
		('S', '/') => new[] { 'W' },
		('W', '/') => new[] { 'S' },
		('N', '/') => new[] { 'E' },
		('E', '\\') => new[] { 'S' },
		('S', '\\') => new[] { 'E' },
		('W', '\\') => new[] { 'N' },
		('N', '\\') => new[] { 'W' },
		_ => throw new ArgumentException()
	};

	return nextDirs.Select(d => new Beam(nextPos.Point, d)).ToList();
}

static void Print(char[,] input, IEnumerable<Beam> toPrint)
{
	input.Select((point, value) =>
	{
		var x = toPrint.FirstOrDefault(vi => vi.point == point);
		return x != null ? x.dir : value == '.' ? ' ' : value;
	}).Print();
}

record Beam(Point point, char dir);