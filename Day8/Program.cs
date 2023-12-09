using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var pattern = input[0];
var map = input.Skip(2).Select(l => ParseMap(l)).ToDictionary(x => x.pos, x => x);



var starting = map.Select(m => m.Key).Where(p => p[2] == 'A');

var lengthFromStart = starting.Select(pos => GetLength(pos, IsEnd2, pattern, map)).ToList();
var lengthOfCycle = lengthFromStart.Select(s => GetLength(s.end, IsEnd2, pattern, map)).ToList();

var differencCycles = lengthFromStart.Zip(lengthOfCycle).Where(x => x.First.length != x.Second.length).ToList();
if (differencCycles.Any()) throw new Exception();

var greatestCommonDenominator = GetGreatestCommonDenominator(lengthOfCycle.Select(l => l.length).ToArray());

var count1 = GetLength("AAA", IsEnd1, pattern, map);
var count2 = lengthOfCycle
	.Select(cycle => cycle.length / greatestCommonDenominator)
	.Aggregate((long)1, (acc, val) => acc * val) * greatestCommonDenominator;

Console.WriteLine("1: " + count1.length);
Console.WriteLine("2: " + count2);

bool IsEnd1(string pos) => pos == "ZZZ";
bool IsEnd2(string pos) => pos[2] == 'Z';

Length GetLength(string start, Func<string, bool> isEnd, string pattern, Dictionary<string, Map> map)
{
	var pos = start;
	var length = 0;

	foreach (var c in IteratePattern(pattern))
	{
		var m = map[pos];
		length++;
		pos = c == 'L' ? m.left : m.right;
		if (isEnd(pos)) break;
	}

	return new Length(start, pos, length);
}

Map ParseMap(string line)
{
	var template = new Regex("(?<pos>-?[A-Z][A-Z][A-Z]) = \\((?<left>-?[A-Z][A-Z][A-Z]), (?<right>-?[A-Z][A-Z][A-Z])\\)");
	var matches = template.Match(line);
	var pos = matches.Groups["pos"].Value;
	var left = matches.Groups["left"].Value;
	var right = matches.Groups["right"].Value;
	return new Map(pos, left, right);
}

IEnumerable<char> IteratePattern(string pattern)
{
	while (true)
	{
		foreach (char c in pattern)
		{
			yield return c;
		}
	}
}

static int GetGreatestCommonDenominator(int[] numbers)
{
	return numbers.Aggregate(GetGCM);
}

static int GetGCM(int a, int b)
{
	return b == 0 ? a : GetGCM(b, a % b);
}

record Map(string pos, string left, string right);
record Length(string start, string end, int length);