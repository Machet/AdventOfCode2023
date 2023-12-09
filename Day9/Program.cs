using Utils;

var inputLists = File.ReadAllLines("input.txt")
	.Select(l => l.ToIntList())
	.ToList();

var result1 = inputLists
	.Select(l => GetHistoryPrediction(l))
	.Sum();

var result2 = inputLists
	.Select(l => { l.Reverse(); return l; })
	.Select(l => GetHistoryPrediction(l))
	.Sum();

Console.WriteLine("1: " + result1);
Console.WriteLine("2: " + result2);

int GetHistoryPrediction(List<int> numbers)
{
	var diffs = GetDifferences(numbers).ToList();
	var diffPrediction = diffs.All(d => d == 0) ? 0 : GetHistoryPrediction(diffs);
	return diffPrediction + numbers.Last();
}

IEnumerable<int> GetDifferences(List<int> numbers)
{
	for (int i = 1; i < numbers.Count; i++)
	{
		yield return numbers[i] - numbers[i - 1];
	}
}
