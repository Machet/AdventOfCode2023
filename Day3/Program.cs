using Utils;

char[,] map = File.ReadAllLines("input.txt")
	.ToCharArray();

var total = 0;
var potentialGears = new Dictionary<Point, List<int>>();

for (int x = 0; x < map.GetLength(0); x++)
{
	var number = 0;
	var isPart = false;
	var maybeGears = new HashSet<Point>();

	for (int y = 0; y < map.GetLength(1); y++)
	{
		if (!char.IsDigit(map[x, y]))
		{
			if (isPart)
			{
				total += number;
				AddPotentialGears(maybeGears, number);
			}

			number = 0;
			isPart = false;
			maybeGears = new HashSet<Point>();
		}
		else
		{
			number = number * 10 + int.Parse(map[x, y].ToString());
			isPart = isPart || map.GetNeighbourValues(new Point(x, y), includeDiagonal: true).Any(v => IsSymbol(v));
			maybeGears.AddRange(map.GetNeighbourItems(new Point(x, y), includeDiagonal: true).Where(i => MayBeGear(i.Item)).Select(i => i.Point));
		}
	}

	if (isPart)
	{
		total += number;
		AddPotentialGears(maybeGears, number);
	}
}

var gears = potentialGears.Where(g => g.Value.Count == 2);
Console.WriteLine("1: " + total);
Console.WriteLine("2: " + gears.Select(g => g.Value[0] * g.Value[1]).Sum());

bool IsSymbol(char c) => !char.IsDigit(c) && c != '.';
bool MayBeGear(char c) => c == '*';

void AddPotentialGears(IEnumerable<Point> maybeGears, int number)
{
	foreach (var point in maybeGears)
	{
		if (!potentialGears.ContainsKey(point))
		{
			potentialGears[point] = new List<int>();
		}

		potentialGears[point].Add(number);
	}
}