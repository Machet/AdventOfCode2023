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

var blocksCarryingMultiple = collisions.GroupBy(x => x.bottom)
	.Where(x => x.Select(xx => xx.top).All(xx => blocksCarriedByMultiple.Contains(xx)))
	.ToList();

var blocksOnTop = cubesOnGround.Where(c => !collisions.Any(col => col.bottom == c));
var toDesintegrate = blocksCarryingMultiple.Count() + blocksOnTop.Count();

Console.WriteLine("1: " + toDesintegrate);

Cube ReadCubes(string line)
{
	var parts = line.Split("~");
	var startParts = parts[0].Split(",").Select(i => int.Parse(i)).ToList();
	var endParts = parts[1].Split(",").Select(i => int.Parse(i)).ToList();
	return new Cube(new Point3D(startParts[0], startParts[1], startParts[2]), new Point3D(endParts[0], endParts[1], endParts[2]));
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

