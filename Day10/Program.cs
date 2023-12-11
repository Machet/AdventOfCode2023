using Utils;

var input = File.ReadAllLines("input.txt")
	.ToCharArray();

var start = input.FindArrayItem('S').Single();
input[start.Point.X, start.Point.Y] = '└';
var toProcess = Enumerable.Repeat(new ArrayItem<char>(start.Point, '└'), 1);
var pipeLength = 0;
var isInPipe = new HashSet<Point>();

var test = new char[input.GetLength(0), input.GetLength(1)].FillWith(' ');
test[start.Point.X, start.Point.Y] = 'S';

while (toProcess.Any())
{
	var next = GetNextPipe(toProcess, isInPipe).ToList();
	isInPipe.AddRange(toProcess.Select(x => x.Point));
	toProcess = input.SelectItems(next);
	pipeLength++;
}

var isOutside = new HashSet<Point>() { new Point(0, 0) };
var outsidesToCheck = new Queue<Point>(isOutside);

while (outsidesToCheck.Any())
{
	var item = outsidesToCheck.Dequeue();
	var newOutsides = input.GetNeighbours(item)
		.Where(i => !isOutside.Contains(i) && !isInPipe.Contains(i))
		.ToList();

	foreach (var o in newOutsides)
	{
		outsidesToCheck.Enqueue(o);
		isOutside.Add(o);
	}
}

var traverseStart = isInPipe.OrderBy(p => p.X).ThenBy(p => p.Y).First();

var current = traverseStart;
var dir = 's';
do
{
	isOutside.AddRange(GetLeftOutside(input.GetItem(current), dir));
	(current, dir) = GetNextDirectinal(input.GetItem(current), dir);

} while (current != traverseStart);

input.Select((point, item) => isInPipe.Contains(point) ? item : isOutside.Contains(point) ? 'o' : ' ').Print();
var result2 = input.Select((point, item) => point).Flatten().Where(p => !isInPipe.Contains(p) && !isOutside.Contains(p)).Count();
Console.WriteLine("1: " + --pipeLength);
Console.WriteLine("2: " + result2);

IEnumerable<Point> GetNextPipe(IEnumerable<ArrayItem<char>> pos, HashSet<Point> visited)
{
	return pos.SelectMany(p => GetConnected(p)).Where(p => !visited.Contains(p));
}

IEnumerable<Point> GetConnected(ArrayItem<char> pos)
{
	yield return pos.Item switch
	{
		'┘' => new Point(pos.Point.X - 1, pos.Point.Y),
		'┌' => new Point(pos.Point.X, pos.Point.Y + 1),
		'└' => new Point(pos.Point.X - 1, pos.Point.Y),
		'┐' => new Point(pos.Point.X, pos.Point.Y - 1),
		'-' => new Point(pos.Point.X, pos.Point.Y - 1),
		'|' => new Point(pos.Point.X - 1, pos.Point.Y),
		_ => throw new Exception()
	};

	yield return pos.Item switch
	{
		'┘' => new Point(pos.Point.X, pos.Point.Y - 1),
		'┌' => new Point(pos.Point.X + 1, pos.Point.Y),
		'└' => new Point(pos.Point.X, pos.Point.Y + 1),
		'┐' => new Point(pos.Point.X + 1, pos.Point.Y),
		'-' => new Point(pos.Point.X, pos.Point.Y + 1),
		'|' => new Point(pos.Point.X + 1, pos.Point.Y),
		_ => throw new Exception()
	};
}

// ┌---┐
// |   |
// └---┘
(Point, char) GetNextDirectinal(ArrayItem<char> pos, char cameFrom)
{
	return (pos.Item, cameFrom) switch
	{
		('┘', 'w') => (new Point(pos.Point.X - 1, pos.Point.Y), 's'),
		('┌', 's') => (new Point(pos.Point.X, pos.Point.Y + 1), 'w'),
		('└', 'e') => (new Point(pos.Point.X - 1, pos.Point.Y), 's'),
		('┐', 's') => (new Point(pos.Point.X, pos.Point.Y - 1), 'e'),
		('-', 'e') => (new Point(pos.Point.X, pos.Point.Y - 1), 'e'),
		('|', 's') => (new Point(pos.Point.X - 1, pos.Point.Y), 's'),
		('┘', 'n') => (new Point(pos.Point.X, pos.Point.Y - 1), 'e'),
		('┌', 'e') => (new Point(pos.Point.X + 1, pos.Point.Y), 'n'),
		('└', 'n') => (new Point(pos.Point.X, pos.Point.Y + 1), 'w'),
		('┐', 'w') => (new Point(pos.Point.X + 1, pos.Point.Y), 'n'),
		('-', 'w') => (new Point(pos.Point.X, pos.Point.Y + 1), 'w'),
		('|', 'n') => (new Point(pos.Point.X + 1, pos.Point.Y), 'n'),
		_ => throw new Exception()
	};
}

IEnumerable<Point> GetLeftOutside(ArrayItem<char> pos, char cameFrom)
{
	return (pos.Item, cameFrom) switch
	{
		('┘', 'n') => new List<Point> { new Point(pos.Point.X, pos.Point.Y + 1), new Point(pos.Point.X + 1, pos.Point.Y) },
		('┘', 'w') => new List<Point> { },
		('┐', 'w') => new List<Point> { new Point(pos.Point.X - 1, pos.Point.Y), new Point(pos.Point.X, pos.Point.Y + 1) },
		('┐', 's') => new List<Point> { },
		('┌', 's') => new List<Point> { new Point(pos.Point.X - 1, pos.Point.Y), new Point(pos.Point.X, pos.Point.Y - 1) },
		('┌', 'e') => new List<Point> { },
		('└', 'e') => new List<Point> { new Point(pos.Point.X + 1, pos.Point.Y), new Point(pos.Point.X, pos.Point.Y - 1) },
		('└', 'n') => new List<Point> { },
		('-', 'w') => new List<Point> { new Point(pos.Point.X - 1, pos.Point.Y), },
		('-', 'e') => new List<Point> { new Point(pos.Point.X + 1, pos.Point.Y) },
		('|', 'n') => new List<Point> { new Point(pos.Point.X, pos.Point.Y + 1) },
		('|', 's') => new List<Point> { new Point(pos.Point.X, pos.Point.Y - 1) },
		_ => throw new Exception()
	};
}