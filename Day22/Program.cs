using Utils;
// 431
var cubes = File.ReadAllLines("input.txt")
	.Select(line => ReadCubes(line))
	.ToList();

var cubesOnGround = cubes.Where(c => c.start.z == 1 || c.end.z == 1).ToHashSet();
var fallingCubes = cubes.Except(cubesOnGround).ToList();
var collisions = new List<Collision>();

foreach (var fallingCube in fallingCubes.OrderBy(c => c.LowestLevel))
{
	var collides = false;
	var cubesByLevel = cubesOnGround
		.GroupBy(c => c.HighestLevel)
		.Select(c => new { level = c.Key, blocks = c.ToList() })
		.OrderByDescending(x => x.level);

	foreach (var level in cubesByLevel)
	{
		foreach (var standingCube in level.blocks)
		{
			if (fallingCube.CollidesXY(standingCube))
			{
				var afterFallCube = fallingCube.TranslateZAt(standingCube.HighestLevel + 1);
				cubesOnGround.Add(afterFallCube);
				collisions.Add(new Collision(standingCube, afterFallCube));
				collides = true;
			}
		}

		if (collides) break;
	}

	if (!collides)
	{
		var afterFallCube = fallingCube.TranslateZAt(1);
		cubesOnGround.Add(afterFallCube);
	}
}

var blocksCarriedByMultiple = collisions.GroupBy(x => x.top)
	.Where(x => x.Count() > 1)
	.Select(x => x.Key)
	.ToList();

var blocksThatCarySafeBlocks = collisions.GroupBy(x => x.bottom)
	.Where(x => x.Select(xx => xx.top).All(xx => blocksCarriedByMultiple.Contains(xx)))
	.ToList();

var blocksOnTop = cubesOnGround.Where(c => !collisions.Any(col => col.bottom == c));
var toDesintegrate = blocksThatCarySafeBlocks.Count() + blocksOnTop.Count();

var blocksThatCauseFall = collisions.GroupBy(x => x.bottom)
	.Where(x => x.Select(xx => xx.top).All(xx => blocksCarriedByMultiple.Contains(xx)))
	.ToList();

var totalFailingCount = 0;
foreach (var block in collisions.Select(c => c.bottom).Distinct())
{
	totalFailingCount += GetFailingCount(collisions, block);
}

Console.WriteLine("1: " + toDesintegrate);
Console.WriteLine("2: " + totalFailingCount);

Cube ReadCubes(string line)
{
	var parts = line.Split("~");
	var startParts = parts[0].Split(",").Select(i => int.Parse(i)).ToList();
	var endParts = parts[1].Split(",").Select(i => int.Parse(i)).ToList();
	return new Cube(new Point3D(startParts[0], startParts[1], startParts[2]), new Point3D(endParts[0], endParts[1], endParts[2]));
}

int GetFailingCount(List<Collision> collisions, Cube cubeToRemove)
{
	var existingCollisions = collisions.ToList();
	var cubesToRemove = new Queue<Cube>();
	cubesToRemove.Enqueue(cubeToRemove);
	int count = 0;
	while (cubesToRemove.Count != 0)
	{
		var toRemove = cubesToRemove.Dequeue();
		var collisionsWithToRemove = existingCollisions.Where(c => c.bottom == toRemove).ToList();
		var cubesThatCanFall = collisionsWithToRemove.Select(c => c.top).ToList();
		existingCollisions = existingCollisions.Except(collisionsWithToRemove).ToList();

		var cubesThatWillFall = cubesThatCanFall.Where(cube => !existingCollisions.Any(col => col.top == cube)).ToList();
		count += cubesThatWillFall.Count;
		cubesToRemove.EnqueueRange(cubesThatWillFall);
	}

	return count;
}

record Point3D(int x, int y, int z);
record Collision(Cube bottom, Cube top);
record Cube(Point3D start, Point3D end)
{
	public int LowestLevel = Math.Min(start.z, end.z);
	public int HighestLevel = Math.Max(start.z, end.z);

	internal bool CollidesXY(Cube another)
	{
		var intStartX = Math.Max(start.x, another.start.x);
		var intEndX = Math.Min(end.x, another.end.x);
		var intStartY = Math.Max(start.y, another.start.y);
		var intEndY = Math.Min(end.y, another.end.y);

		return (intStartX <= intEndX && intStartY <= intEndY);
	}

	internal Cube TranslateZAt(int newZ)
	{
		var height = HighestLevel - LowestLevel;
		return new Cube(new Point3D(start.x, start.y, newZ), new Point3D(end.x, end.y, newZ + height));
	}

	public override string ToString()
	{
		return $"({start.x}, {start.y}, {start.z})~({end.x}, {end.y}, {end.z})";
	}
}

