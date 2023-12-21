using Utils;

var map = File.ReadAllLines("input.txt")
	.ToCharArray();

if (map.GetLength(1) != map.GetLength(0)) throw new Exception("Unexpected input");
if (map.GetLength(0) % 2 != 1) throw new Exception("Unexpected input");

var mapSize = map.GetLength(0);
var mapCentre = mapSize / 2;

var startingPoint = map.FindArrayItem('S').Single().Point;
map[startingPoint.X, startingPoint.Y] = '.';

var reachablePlaces1 = ProcessSteps(map, startingPoint, 64);
Console.WriteLine("1: " + reachablePlaces1.Count);

long stepsOnLargeMap = 26501365;
var tilesTraveled = (stepsOnLargeMap - mapCentre) / mapSize;

var stepsOverTile = (stepsOnLargeMap - mapCentre) % mapSize;
if (stepsOverTile != 0) throw new Exception("Assumption that only full tiles are traveled");
if (tilesTraveled % 2 != 0) throw new Exception("Assumption that odd amount of tiles have been traveled");

var mapLarge = map.Repeat(5, 5);
var startingPointLarge = startingPoint.GetSouthOf(mapSize * 2).GetEastOf(mapSize * 2);
var reachablePlacesInTemplate = ProcessSteps(mapLarge, startingPointLarge, mapSize * 2 + mapCentre);
var countInCategories = Categorize(reachablePlacesInTemplate, 5, mapSize);

var evenFullTile = countInCategories[2, 2];
var oddFullTile = countInCategories[2, 1];
var diamondEndTiles = countInCategories[2, 0] + countInCategories[0, 2] + countInCategories[2, 4] + countInCategories[4, 2];
var allDiagonalBigTiles = countInCategories[1, 1] + countInCategories[1, 3] + countInCategories[3, 1] + countInCategories[3, 3];
var allDiagonalSmallTiles = countInCategories[0, 1] + countInCategories[4, 1] + countInCategories[0, 3] + countInCategories[4, 3];
var countOfFullEven = (tilesTraveled - 1) * (tilesTraveled - 1); // diagonal square
var countOfFullOdd = tilesTraveled * tilesTraveled;
var countOfDiagonalBig = tilesTraveled - 1;
var countOfDiagonalSmall = tilesTraveled;
var a = countOfFullEven * evenFullTile;
var b = countOfFullOdd * oddFullTile;
var c = countOfDiagonalBig * allDiagonalBigTiles;
var d = countOfDiagonalSmall * allDiagonalSmallTiles;
var e = diamondEndTiles;
var total = a + b + c + d + e;

Console.WriteLine("2: " + total);


static HashSet<Point> ProcessSteps(char[,] map, Point startingPoint, int count)
{
	var currentPositions = new HashSet<Point> { startingPoint };

	for (int i = 0; i < count; i++)
	{
		currentPositions = GetNextPositions(currentPositions, map);
	}

	return currentPositions;
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

	return nextSteps;
}

static int[,] Categorize(HashSet<Point> points, int bucketsCount, int partSize)
{
	var result = new int[bucketsCount, bucketsCount];

	foreach (var point in points)
	{
		var bucketX = point.X / partSize;
		var bucketY = point.Y / partSize;
		result[bucketX, bucketY]++;
	}

	return result;
}