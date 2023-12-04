var lines = File.ReadAllLines("input.txt");

var result = lines
	.Select(l =>
	{
		var cards = l.Split(":");
		var number = int.Parse(cards[0].Replace("Card ", string.Empty).Trim());
		var parts = cards[1].Split("|");
		var winning = parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n.Trim()));
		var have = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n.Trim()));
		var match = have.Intersect(winning).ToList();
		return new Lotery(number, match.Count);
	}).ToList();

long winningCount = 0;

var toProcess = new Queue<int>(result.Select(r => r.number));
var winCounts = result.ToDictionary(x => x.number, x => x.wins);

while (toProcess.Count > 0)
{
	winningCount++;
	var item = toProcess.Dequeue();
	var wins = winCounts[item];
	for (int i = 0; i < wins; i++)
	{
		toProcess.Enqueue(item + 1 + i);
	}
}

Console.WriteLine("1: " + result.Select(r => r.wins > 0 ? Math.Pow(2, r.wins - 1) : 0).Sum());
Console.WriteLine("2: " + winningCount);

record Lotery(int number, int wins);