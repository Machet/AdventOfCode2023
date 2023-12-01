var result1 = File.ReadAllLines("input1.txt")
	.Select(l => l.Where(c => char.IsDigit(c)))
	.Select(l => int.Parse($"{l.First()}{l.Last()}"))
	.Sum();

Console.WriteLine(result1);

var digits = new Dictionary<string, int>()
{
	{"one", 1 },
	{"two", 2 },
	{"three", 3 },
	{"four", 4 },
	{"five", 5 },
	{"six", 6 },
	{"seven", 7 },
	{"eight", 8 },
	{"nine", 9 },
	{"1", 1 },
	{"2", 2 },
	{"3", 3 },
	{"4", 4 },
	{"5", 5 },
	{"6", 6 },
	{"7", 7 },
	{"8", 8 },
	{"9", 9 },
};

var result2 = File.ReadAllLines("input1.txt")
	.Select(l =>  FindFirst(l, digits) * 10 + FindLast(l, digits));
	

Console.WriteLine(result2.Sum());

int FindFirst(string l, Dictionary<string, int> digits)
{
	return digits.Select(d => new { Index = l.IndexOf(d.Key), Value = d.Value })
		.Where(f => f.Index != -1)
		.OrderBy(f => f.Index)
		.First().Value;
}

int FindLast(string l, Dictionary<string, int> digits)
{
	return digits.Select(d => new { Index = l.LastIndexOf(d.Key), Value = d.Value })
		.Where(f => f.Index != -1)
		.OrderByDescending(f => f.Index)
		.First().Value;
}