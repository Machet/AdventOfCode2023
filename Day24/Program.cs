
using Utils;

var vectors = File.ReadAllLines("input.txt")
	.Select(line => ParseVector(line))
	.ToList();

var area = new Area(200000000000000, 200000000000000, 400000000000000, 400000000000000);

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