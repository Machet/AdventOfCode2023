using Day12;
using Day12.Automata;
using Utils;

var input = File.ReadAllLines("input.txt")
	.Select(line => Parse(line))
	.ToList();

var unfoldedInput = input.Select(i => new DataRecord(
		string.Join("?", Enumerable.Repeat(i.template, 5)),
		Enumerable.Repeat(i.groups, 5).SelectMany(x => x).ToList()))
	.ToList();

var result1 = input.Select(i => GetPossibleArrangements(i)).Sum();
var result2 = unfoldedInput.Select(i => GetPossibleArrangements(i)).Sum();

Console.WriteLine("1: " + result1);
Console.WriteLine("2: " + result2);

static long GetPossibleArrangements(DataRecord record)
{
	var processingState = new ProcessingState();
	processingState.Add(BuildParser(record.groups));
	var template = record.template;

	for (int i = 0; i < template.Length; i++)
	{
		processingState = processingState.Process(template[i], template.Length - i);
	}

	return processingState.GetValidStatesCount();
}

static ParsingState BuildParser(List<int> groups)
{
	var waitTillEnd = new WaitForEnd("end");

	ParsingState current = waitTillEnd;
	var expectedLength = 0;

	for (int i = groups.Count - 1; i >= 0; i--)
	{
		var group = groups[i];
		expectedLength = expectedLength > 0 ? expectedLength + 1 : expectedLength;
		current = new ExpectEmpty($"expect space after g{i}", current, expectedLength, expectedLength == 0);

		for (int j = group; j > 1; j--)
		{
			expectedLength++;
			current = new ExpectGroup($"expect g{i}[{group}] - item({j})", current, expectedLength);
		}

		expectedLength++;
		current = new WaitForGroup($"wait for g{i}[{group}] - item(1)", current, expectedLength);
	}

	return current;
}

DataRecord Parse(string line)
{
	var x = line.Split(" ");
	return new DataRecord(x[0], x[1].ToIntList(','));
}

record DataRecord(string template, List<int> groups)
{
	public override string ToString()
	{
		return $"{template} {string.Join(",", groups)}";
	}
}
