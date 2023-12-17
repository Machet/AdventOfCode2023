using System.Collections.Immutable;
using Utils;

var input = File.ReadAllLines("input.txt")
	.ToCharArray();

var inputMap = input.Select(x => int.Parse(x.ToString()));
var end = new Point(input.GetLength(0) - 1, input.GetLength(1) - 1);

var initialState = new State(new Point(0, 0), 0, 'E', 0, 3, new Rules(inputMap, 0, 3), ImmutableList<Point>.Empty);
var result1 = Process(initialState, end);

var initialState2 = new State(new Point(0, 0), 0, 'U', 0, 10, new Rules(inputMap, 4, 10), ImmutableList<Point>.Empty);
var result2 = Process(initialState2, end);

Console.WriteLine("1: " + result1);
Console.WriteLine("2: " + result2);

static int Process(State initialState, Point end)
{
	var toProcess = new PriorityQueue<State, int>();
	toProcess.Enqueue(initialState, 0);
	var visited = new Dictionary<(Point, char, int, int), int>();

	while (toProcess.Count > 0)
	{
		var state = toProcess.Dequeue();
		var newStates = state.GetNextStates().ToList();

		foreach (var newState in newStates)
		{
			var stateShort = (newState.point, newState.dir, newState.turnAllowedIn, newState.turnRequiredIn);
			if (visited.ContainsKey(stateShort) && visited[stateShort] <= newState.totalEnergy)
			{
				continue;
			}

			if (newState.point == end && newState.turnAllowedIn <= 0)
			{
				return newState.totalEnergy;
			}

			visited[stateShort] = newState.totalEnergy;
			toProcess.Enqueue(newState, newState.Score);
		}
	}

	return 0;
}

record Rules(int[,] map, int minPathBeforeTurn, int maxPathBeforeTurn);

record State(Point point, int totalEnergy, char dir, int turnAllowedIn, int turnRequiredIn, Rules rules, ImmutableList<Point> trail)
{
	public static Dictionary<char, HashSet<char>> PossibleDirs = new()
	{
		{ 'U', new () { 'S', 'E' } },
		{ 'N', new () { 'E', 'W', 'N'} },
		{ 'S', new () { 'E', 'W', 'S'} },
		{ 'W', new () { 'N', 'S', 'W'} },
		{ 'E', new () { 'N', 'S', 'E'} },
	};

	public int Score = totalEnergy + rules.map.GetLength(0) - point.X + rules.map.GetLength(1) - point.Y;

	public IEnumerable<State> GetNextStates()
	{
		var possibleDirs = PossibleDirs[dir];
		var map = rules.map;

		foreach (var nextDir in possibleDirs)
		{
			var isCurrent = nextDir == dir;
			if (isCurrent && turnRequiredIn == 0)
			{
				continue;
			}

			if (!isCurrent && turnAllowedIn > 0)
			{
				continue;
			}

			var newPos = nextDir switch
			{
				'N' => map.GetNorthOf(point),
				'S' => map.GetSouthOf(point),
				'W' => map.GetWestOf(point),
				'E' => map.GetEastOf(point),
				_ => throw new ArgumentException()
			};

			if (newPos != null)
			{
				var newTurnRequiredIn = isCurrent ? turnRequiredIn - 1 : rules.maxPathBeforeTurn - 1;
				var newTurnAllowedIn = isCurrent ? turnAllowedIn - 1 : rules.minPathBeforeTurn - 1;
				yield return new State(newPos.Point, totalEnergy + newPos.Item, nextDir, newTurnAllowedIn, newTurnRequiredIn, rules, trail.Add(point));
			}
		}
	}
}

