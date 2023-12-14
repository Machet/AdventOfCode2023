using Utils;

var array = File.ReadAllLines("input.txt")
	.ToCharArray();

var directionEnumerator = new List<char> { 'N', 'W', 'S', 'E' }.IterateInCircles().GetEnumerator();

var rocks = array.SelectItems().Where(i => i.Item == 'O').Select(i => i.Point).ToHashSet();
var walls = array.SelectItems().Where(i => i.Item == '#').Select(i => i.Point).ToList();

var xRanges = Enumerable.Range(0, array.GetLength(0))
	.ToDictionary(i => i, i => GenerateAvailableRanges(walls.Where(w => w.X == i).Select(w => w.Y).ToList(), array.GetLength(1) - 1).ToList());
var yRanges = Enumerable.Range(0, array.GetLength(1))
	.ToDictionary(i => i, i => GenerateAvailableRanges(walls.Where(w => w.Y == i).Select(w => w.X).ToList(), array.GetLength(0) - 1).ToList());

var rocksState1 = Tilt(rocks, 'N', xRanges, yRanges).ToHashSet();

var states = new Dictionary<int, List<HashSet<Point>>>();
var statesByLoop = new Dictionary<int, HashSet<Point>>();
var setComparer = HashSet<Point>.CreateSetComparer();

var loopStart = 0;
var loopLength = 0;

for (int i = 1; i <= 1000000000; i++)
{
	directionEnumerator.MoveNext();
	rocks = Tilt(rocks, directionEnumerator.Current, xRanges, yRanges).ToHashSet();

	var stateCode = setComparer.GetHashCode(rocks);

	if (!states.ContainsKey(stateCode))
	{
		states[stateCode] = new List<HashSet<Point>>();
	}

	if (states[stateCode].Any(s => s.SetEquals(rocks)))
	{
		var ai = statesByLoop.Where(s => s.Value.SetEquals(rocks)).FirstOrDefault();
		loopStart = ai.Key;
		loopLength = i - ai.Key;
		break;
	}
	else
	{
		states[stateCode].Add(rocks);
	}

	statesByLoop.Add(i, rocks);
}

var stateNrAfterLoop = (4000000000 - loopStart) % loopLength;
var finalState = statesByLoop[(int)stateNrAfterLoop + loopStart];

var weight1 = CalculateWeight(rocksState1, array.GetLength(0));
var weightFinal = CalculateWeight(finalState, array.GetLength(0));

Console.WriteLine("1: " + weight1);
Console.WriteLine("2: " + weightFinal);
Console.WriteLine();
Print(array, finalState, walls);

// x vertical y horizontal
IEnumerable<Point> Tilt(HashSet<Point> rocks, char dir, Dictionary<int, List<IntRange>> xRanges, Dictionary<int, List<IntRange>> yRanges)
{
	var isVertical = dir == 'N' || dir == 'S';
	var rangesByAxis = isVertical ? yRanges : xRanges;
	var shiftToEnd = dir == 'S' || dir == 'E';
	Func<Point, int> getBucket = isVertical ? (Point p) => p.Y : (Point p) => p.X;
	Func<Point, int> getPosition = isVertical ? (Point p) => p.X : (Point p) => p.Y;

	foreach (var rockRow in rocks.GroupBy(r => getBucket(r)))
	{
		var ranges = rangesByAxis[rockRow.Key];
		foreach (var range in ranges)
		{
			var availablePosition = shiftToEnd ? range.end : range.start;

			foreach (var rock in rockRow.Where(r => range.Contains(getPosition(r))))
			{
				yield return !isVertical ? new Point(rockRow.Key, availablePosition) : new Point(availablePosition, rockRow.Key);
				availablePosition = shiftToEnd ? availablePosition - 1 : availablePosition + 1;
			}
		}
	}
}

int CalculateWeight(IEnumerable<Point> rocks, int length)
{
	return rocks.Select(r => length - r.X).Sum();
}

IEnumerable<IntRange> GenerateAvailableRanges(List<int> walls, int end)
{
	if (walls.Count == 0)
	{
		yield return new IntRange(0, end);
		yield break;
	}

	walls.Sort();
	var start = 0;
	for (int i = 0; i < walls.Count; i++)
	{
		if (walls[i] != start)
		{
			yield return new IntRange(start, walls[i] - 1);
		}

		start = walls[i] + 1;
	}

	if (start <= end)
	{
		yield return new IntRange(start, end);
	}
}

static void Print(char[,] array, HashSet<Point> rocks, List<Point> walls)
{
	array.Select((p, v) =>
	{
		if (rocks.Contains(p)) return 'O';
		if (walls.Contains(p)) return '#';
		return '.';
	}).Print();

	Console.WriteLine();
}