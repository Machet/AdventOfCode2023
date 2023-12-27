
using Utils;

var vectors = File.ReadAllLines("input.txt")
	.Select(line => ParseVector(line))
	.ToList();

var area = new Area(200000000000000, 200000000000000, 400000000000000, 400000000000000);

// part 1
var acc = 0;
foreach (var pair in vectors.GenerateAllCombinations())
{
	if (pair.first.IsParallelXY(pair.second)) continue;
	var collision = pair.first.GetCollisionPointXY(pair.second);
	if (area.Contains(collision) && !pair.first.IsInThePast(collision) && !pair.second.IsInThePast(collision))
	{
		acc++;
	}
}

Console.WriteLine("1: " + acc);

// part 2
var dx = GetPossibleVelocity(vectors, v => v.dx, v => v.x);
var dy = GetPossibleVelocity(vectors, v => v.dy, v => v.y);
var dz = GetPossibleVelocity(vectors, v => v.dz, v => v.z);

if (dx.Count != 1 || dy.Count != 1 || dz.Count != 1)
{
	throw new Exception("Input is not refined");
}

var vectorsInRockCord = vectors
	.Select(v => new Vector(v.x, v.y, v.z, v.dx - dx.First(), v.dy - dy.First(), v.dz - dz.First()))
	.Take(2)
	.ToList();

var (x, y) = vectorsInRockCord[0].GetCollisionPointXY(vectorsInRockCord[1]);
var timeToPoint1 = (x - vectorsInRockCord[0].x) / (vectorsInRockCord[0].dx);
var z = vectorsInRockCord[0].dz * timeToPoint1 + vectorsInRockCord[0].z;

Console.WriteLine("2: " + (x + y + z));

static List<long> GetPossibleVelocity(List<Vector> vectors, Func<Vector, long> getVelocity, Func<Vector, long> getPosition)
{
	var commonVelocity = vectors.GroupBy(getVelocity).Where(g => g.Count() > 1).ToList();
	List<long>? possibleValues = null;

	foreach (var pointsWithSameVelocity in commonVelocity)
	{
		foreach (var pair in pointsWithSameVelocity.GenerateAllCombinations())
		{
			if (possibleValues == null)
			{
				possibleValues = GetPossibleVelocityValues(pointsWithSameVelocity.Key, pair.first, pair.second, getPosition).ToList();
			}
			else
			{
				possibleValues = NarrowPossibleVelocityValues(pointsWithSameVelocity.Key, pair.first, pair.second, getPosition, possibleValues).ToList();
			}
		}
	}

	return possibleValues ?? throw new Exception();
}

static IEnumerable<long> GetPossibleVelocityValues(long hailVelocity, Vector hailStone1, Vector hailStone2, Func<Vector, long> getPos)
{
	var diff = getPos(hailStone1) - getPos(hailStone2);
	var divisors = MathHelpers.GetDivisorsOf(diff).ToList();
	return divisors.Select(d => hailVelocity - d);
}

static IEnumerable<long> NarrowPossibleVelocityValues(long hailVelocity, Vector hailStone1, Vector hailStone2, Func<Vector, long> getPos, List<long> possibleValues)
{
	var diff = getPos(hailStone1) - getPos(hailStone2);
	foreach (var possibleValue in possibleValues)
	{
		var possibleDivisor = hailVelocity - possibleValue;
		if (diff % possibleDivisor == 0)
		{
			yield return possibleValue;
		}
	}
}



static CollisionPoint? GetCommonIntersectionPoint(List<Vector> examplePoints)
{
	CollisionPoint? collisionPoint = null;
	var first = examplePoints.First();
	foreach (var second in examplePoints.Skip(1))
	{
		if (first.IsParallelXY(second)) return null;
		var collision = first.GetCollisionPointXY(second);
		if (collisionPoint == null)
		{
			collisionPoint = collision;
		}
		else
		{
			if (collision != collisionPoint)
			{
				return null;
			}
		}
	}

	return collisionPoint;
}

Vector ParseVector(string line)
{
	var parts = line.Split("@");
	var partsA = parts[0].Split(",");
	var partsB = parts[1].Split(",");
	return new Vector(long.Parse(partsA[0]), long.Parse(partsA[1]), long.Parse(partsA[2]),
		long.Parse(partsB[0]), long.Parse(partsB[1]), long.Parse(partsB[2]));
}

record Vector(long x, long y, long z, long dx, long dy, long dz)
{
	internal CollisionPoint GetCollisionPointXY(Vector second)
	{
		var a1 = dy != 0 ? dy / (decimal)dx : 0;
		var b1 = y - a1 * x;
		var a2 = second.dy != 0 ? second.dy / (decimal)second.dx : 0;
		var b2 = second.y - a2 * second.x;
		var pointX = (b2 - b1) / (a1 - a2);
		var pointY = (a2 * b1 - a1 * b2) / (a2 - a1);

		return new CollisionPoint(pointX, pointY);
	}

	internal bool IsInThePast(CollisionPoint collision)
	{
		var xChange = collision.x - x;
		var yChange = collision.y - y;
		var isValidX = dx >= 0 ? xChange >= 0 : xChange < 0;
		var isValidY = dy >= 0 ? yChange >= 0 : yChange < 0;
		return !isValidX || !isValidY;
	}

	internal bool IsParallelXY(Vector second)
	{
		return dy / (decimal)dx == second.dy / (decimal)second.dx;
	}
}

record CollisionPoint(decimal x, decimal y);

record Area(long startX, long startY, long endX, long endY)
{
	public bool Contains(CollisionPoint point) => point.x >= startX && point.x <= endX && point.y >= startY && point.y <= endY;
}