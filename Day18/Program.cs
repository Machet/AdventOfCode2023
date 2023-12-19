using Utils;

var input = File.ReadAllLines("input.txt")
	.Select(line => ParseDirection(line))
	.ToList();

var input2 = ParseMisspelledDirections(input);

var points = GenerateTerrain(input).ToList();
var points2 = GenerateTerrain(input2).ToList();

Console.WriteLine("1: " + CalculateArea(points));
Console.WriteLine("2: " + CalculateArea(points2));

long CalculateArea(List<Point> points)
{
	var rows = points.GroupBy(p => p.X).OrderBy(p => p.Key).ToList();
	var lines = GetLines(points);
	var verticalLines = lines.Where(l => l.IsVertical).ToList();
	var horizontalLines = lines.Where(l => l.IsHorizontal).ToList();
	var rectangles = new List<Rectangle>();

	for (int i = 0; i < rows.Count - 1; i++)
	{
		int rowPos = rows[i].Key;
		int nextRowPos = rows[i + 1].Key;
		var intersectedLines = verticalLines.Where(l => l.IntersectVertical(rowPos)).OrderBy(l => l.start.Y).ToList();
		var distances = GetHorizontalDistance(intersectedLines).ToList();

		foreach (var dist in distances)
		{
			rectangles.Add(new Rectangle(new Point(rowPos, dist.start), new Point(nextRowPos, dist.end)));
		}
	}

	var overlaps = rectangles.SelectMany(r => rectangles.Where(rr => rr != r).Select(rr => rr.GetOvelappingArea(r)))
		.Where(r => r != null)
		.Distinct()
		.ToList();

	return rectangles.Select(r => r.Size).Sum() - overlaps.Select(r => r.Size).Sum();
}

IEnumerable<Distance> GetHorizontalDistance(List<Line> verticalLines)
{
	for (int j = 0; j < verticalLines.Count; j = j + 2)
	{
		yield return new Distance(verticalLines[j].start.Y, verticalLines[j + 1].start.Y);
	}
}

IEnumerable<Line> GetLines(List<Point> points)
{
	var current = points[0];
	for (var i = 1; i < points.Count; i++)
	{
		yield return new Line(current, points[i]);
		current = points[i];
	}
}

IEnumerable<Point> GenerateTerrain(List<Direction> directions)
{
	var current = new Point(0, 0);

	foreach (var dir in directions)
	{
		current = dir.dir switch
		{
			'L' => current.GetWestOf(dir.count),
			'R' => current.GetEastOf(dir.count),
			'U' => current.GetNorthOf(dir.count),
			'D' => current.GetSouthOf(dir.count),
			_ => throw new NotImplementedException()
		};

		yield return current;
	}
}

Direction ParseDirection(string line)
{
	var x = line.Split(' ');
	return new Direction(x[0][0], int.Parse(x[1]), x[2]);
}

Direction ParseDirectionFromColor(string line)
{
	var dir = line[5] switch
	{
		'0' => 'R',
		'1' => 'D',
		'2' => 'L',
		'3' => 'U',
		_ => throw new Exception()
	};

	var hex = Convert.ToInt32(line.Substring(0, 5), 16);
	return new Direction(dir, hex, line);
}

List<Direction> ParseMisspelledDirections(List<Direction> points)
{
	return points.Select(p => p.color.Replace("(#", string.Empty).Replace(")", string.Empty))
		.Select(c => ParseDirectionFromColor(c))
		.ToList();
}

record Direction(char dir, int count, string color);

record Distance(int start, int end)
{
	public int Length => end - start + 1;

	internal Distance GetOverlappingLength(Distance another)
	{
		var containsStart = start <= another.start && another.start <= end;
		var containsEnd = start <= another.end && another.end <= end;
		return containsStart
			? new Distance(another.start, Math.Min(end, another.end))
			: containsEnd
				? new Distance(Math.Max(start, another.start), another.end)
				: new Distance(0, 0);
	}
}

record Line(Point start, Point end)
{
	public bool IsHorizontal => start.X == end.X;
	public bool IsVertical => start.Y == end.Y;

	public bool IntersectVertical(int horizontalLine) => Math.Min(start.X, end.X) <= horizontalLine && Math.Max(start.X, end.X) > horizontalLine;
}

record Rectangle(Point start, Point end)
{
	public long Size = (long)(end.X - start.X + 1) * (end.Y - start.Y + 1);

	public Rectangle? GetOvelappingArea(Rectangle another)
	{
		var intStartX = Math.Max(start.X, another.start.X);
		var intEndX = Math.Min(end.X, another.end.X);
		var intStartY = Math.Max(start.Y, another.start.Y);
		var intEndY = Math.Min(end.Y, another.end.Y);

		return (intStartX <= intEndX && intStartY <= intEndY)
			? new Rectangle(new Point(intStartX, intStartY), new Point(intEndX, intEndY))
			: null;
	}
}
