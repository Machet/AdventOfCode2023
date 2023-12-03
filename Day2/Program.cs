var games = File.ReadAllLines("input.txt")
	.Select(x => ParseGame(x))
	.ToList();

Console.WriteLine("1: " + games.Where(g => g.Draws.All(d => IsPossible(d))).Select(g => g.Id).Sum());
Console.WriteLine("2: " + games.Select(g => g.GetPower()).Sum());

bool IsPossible(Draw d)
{
	return d.Reds <= 12 && d.Greens <= 13 && d.Blues <= 14;
}

Game ParseGame(string description)
{
	var parts = description.Split(":");
	return new Game
	{
		Id = int.Parse(parts[0].Replace("Game ", string.Empty)),
		Draws = parts[1].Split(";").Select(s => ParseDraw(s)).ToList()
	};
}

Draw ParseDraw(string drawText)
{
	var drawParts = drawText.Trim().Split(",");
	var draw = new Draw(0, 0, 0);

	foreach (var part in drawParts)
	{
		var countParts = part.Trim().Split(" ");
		var count = int.Parse(countParts[0]);
		var type = countParts[1];
		draw = type switch
		{
			"blue" => draw with { Blues = count },
			"red" => draw with { Reds = count },
			"green" => draw with { Greens = count },
			_ => throw new Exception()
		};
	}

	return draw;
}

class Game
{
	public int Id { get; set; }
	public List<Draw> Draws { get; set; } = new List<Draw>();

	public int GetPower() => Draws.Select(d => d.Reds).Max() * Draws.Select(d => d.Blues).Max() * Draws.Select(d => d.Greens).Max();
}

record Draw(int Reds, int Greens, int Blues);
