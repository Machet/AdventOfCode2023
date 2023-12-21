using Utils;

var map = File.ReadAllLines("input.txt")
	.ToCharArray();

var startingPoint = map.FindArrayItem('S').Single();
map[startingPoint.Point.X, startingPoint.Point.Y] = '.';

var possiblePlaces = ProcessSteps(map, startingPoint, 64);

Console.WriteLine("1: " + possiblePlaces);

static int ProcessSteps(char[,] map, ArrayItem<char> startingPoint, int count)
{
	var currentPositions = new HashSet<Point> { startingPoint.Point };

	for (int i = 0; i < count; i++)
	{
		currentPositions = GetNextPositions(currentPositions, map);
	}

	return currentPositions.Count;
}

static HashSet<Point> GetNextPositions(HashSet<Point> currentPositions, char[,] map)
{
	var nextSteps = new HashSet<Point>();

	foreach (var currentPosition in currentPositions)
	{
		var possibleMoves = map.GetNeighbourItems(currentPosition)
			.Where(i => i.Item != '#');

		foreach (var move in possibleMoves)
		{
			nextSteps.Add(move.Point);
		}
	}

	//map.Select((p, i) => nextSteps.Contains(p) ? '0' : i).Print();

	return nextSteps;
}