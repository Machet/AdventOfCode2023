using Utils;

var input = File.ReadAllLines("input.txt")
	.ToCharArray();

var emptyColumns = Enumerable.Range(0, input.GetLength(0))
	.Where(i => input.GetColumn(i).All(c => c == '.'))
	.ToList();

var emptyRows = Enumerable.Range(0, input.GetLength(1))
	.Where(i => input.GetRow(i).All(c => c == '.'))
	.ToList();

var planets = input.SelectItems()
	.Where(i => i.Item == '#')
	.Select(i => i.Point)
	.ToList();

var distances1 = planets
	.SelectMany(p1 => planets.Select(p2 => CalculateDistance(p1, p2, emptyColumns, emptyRows, 1)))
	.ToList();

var distances2 = planets
	.SelectMany(p1 => planets.Select(p2 => CalculateDistance(p1, p2, emptyColumns, emptyRows, 1000000)))
	.ToList();

Console.WriteLine("1: " + distances1.Sum() / 2);
Console.WriteLine("2: " + distances2.Sum() / 2);


long CalculateDistance(Point p1, Point p2, List<int> emptyColumns, List<int> emptyRows, int space)
{
	var minX = Math.Min(p1.X, p2.X);
	var maxX = Math.Max(p1.X, p2.X);
	var minY = Math.Min(p1.Y, p2.Y);
	var maxY = Math.Max(p1.Y, p2.Y);

	return maxX - minX + maxY - minY + emptyColumns.Where(c => c > minY && c < maxY).Count() * (space - 1) + emptyRows.Where(r => r > minX && r < maxX).Count() * (space - 1);
}