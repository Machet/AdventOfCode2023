using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Utils;

var input = File.ReadAllLines("input.txt").ToList();
var workflowsEnd = input.IndexOf("");

var workflows = input.GetRange(0, workflowsEnd)
	.Select(line => ParseWorkflow(line))
	.ToList();

var parts = input.GetRange(workflowsEnd + 1, input.Count - workflowsEnd - 1)
	.Select(line => ParsePart(line))
	.ToList();

var result = ProcessParts(parts, workflows);
var result2 = ProcessWorkflows(workflows);

Console.WriteLine("1: " + result);
Console.WriteLine("2: " + result2);

static int ProcessParts(List<Part> parts, List<Workflow> workflows)
{
	var map = workflows.ToDictionary(w => w.name, w => w);
	var acc = 0;

	foreach (Part part in parts)
	{
		var workflow = map["in"];
		while (true)
		{
			var destination = workflow.Process(part);

			if (destination == "A")
			{
				acc += part.GetScore();
				break;
			}

			if (destination == "R")
			{
				break;
			}

			workflow = map[destination];
		}
	}

	return acc;
}

static long ProcessWorkflows(List<Workflow> workflows)
{
	var map = workflows.ToDictionary(w => w.name, w => w);
	long acc = 0;
	var toProcess = new Queue<RangeSelectionState>();
	var availableRange = new AttributeRange(1, 4000);
	var attributes = ImmutableDictionary<string, AttributeRange?>.Empty.Add("x", availableRange).Add("m", availableRange).Add("a", availableRange).Add("s", availableRange);
	toProcess.Enqueue(new RangeSelectionState(attributes, "in"));

	while (toProcess.Count > 0)
	{
		var state = toProcess.Dequeue();
		if (state.IsFinished)
		{
			acc += state.GetScore();
			continue;
		}

		var workflow = map[state.destination];
		var rangesThatApply = state.ranges;
		foreach (var rule in workflow.rules)
		{
			var applyWhen = rule.GetRangesWhenApply(rangesThatApply);
			toProcess.Enqueue(new RangeSelectionState(applyWhen, rule.destination));
			rangesThatApply = rule.GetRangesWhenNotApply(rangesThatApply);
		}

		toProcess.Enqueue(new RangeSelectionState(rangesThatApply, workflow.defaultAction));
	}

	return acc;
}

static Part ParsePart(string line)
{
	var template = new Regex("{x=(?<x>-?[0-9]*),m=(?<m>-?[0-9]*),a=(?<a>-?[0-9]*),s=(?<s>-?[0-9]*)");
	var matches = template.Match(line);
	var x = int.Parse(matches.Groups["x"].Value);
	var m = int.Parse(matches.Groups["m"].Value);
	var a = int.Parse(matches.Groups["a"].Value);
	var s = int.Parse(matches.Groups["s"].Value);
	return new Part(x, m, a, s);
}

static Workflow ParseWorkflow(string line)
{
	var workflowParts = line.Replace("}", "").Split("{");
	var ruleParts = workflowParts[1].Split(",");
	var rules = ruleParts.SkipLast(1).Select(r => ParseRule(r)).ToList();
	return new Workflow(workflowParts[0], rules, ruleParts[^1]);
}

static Rule ParseRule(string line)
{
	var template = new Regex("(?<attribute>-?[a-zA-Z]+)(?<operator>-?[><])(?<value>-?[0-9]*):(?<destination>-?[a-zA-Z]+)");
	var matches = template.Match(line);
	var attribute = matches.Groups["attribute"].Value;
	var operatorValue = matches.Groups["operator"].Value;
	var value = int.Parse(matches.Groups["value"].Value);
	var destination = matches.Groups["destination"].Value;
	return new Rule(attribute, operatorValue[0], value, destination);
}

record Part(int x, int m, int a, int s)
{
	public int GetScore() => x + m + a + s;
}

record Rule(string atributeToCheck, char operation, int value, string destination)
{
	public bool AppliesTo(Part part)
	{
		var attributeValue = atributeToCheck switch
		{
			"x" => part.x,
			"m" => part.m,
			"a" => part.a,
			"s" => part.s,
			_ => throw new Exception()
		};

		return operation switch
		{
			'<' => attributeValue < value,
			'>' => attributeValue > value,
			_ => throw new Exception()
		};
	}

	public ImmutableDictionary<string, AttributeRange?> GetRangesWhenApply(ImmutableDictionary<string, AttributeRange?> ranges)
	{
		var range = ranges[atributeToCheck];
		var result = operation switch
		{
			'<' => range?.SmallerThan(value),
			'>' => range?.BiggerThan(value),
			_ => throw new Exception()
		};

		return ranges.SetItem(atributeToCheck, result);
	}

	public ImmutableDictionary<string, AttributeRange?> GetRangesWhenNotApply(ImmutableDictionary<string, AttributeRange?> ranges)
	{
		var range = ranges[atributeToCheck];
		var result = operation switch
		{
			'<' => range?.BiggerThan(value - 1),
			'>' => range?.SmallerThan(value + 1),
			_ => throw new Exception()
		};

		return ranges.SetItem(atributeToCheck, result);
	}
}

record Workflow(string name, List<Rule> rules, string defaultAction)
{
	public string Process(Part part)
	{
		foreach (var rule in rules)
		{
			if (rule.AppliesTo(part))
			{
				return rule.destination;
			}
		}

		return defaultAction;
	}
}

record RangeSelectionState(ImmutableDictionary<string, AttributeRange?> ranges, string destination)
{
	public bool IsFinished => destination == "R" || destination == "A";

	public long GetScore()
	{
		return destination == "R"
			? 0
			: ranges.Values.Select(range => range?.GetLength() ?? 0).Aggregate(1L, (acc, v) => acc * v);
	}
}

record AttributeRange(int start, int end)
{
	public int GetLength() => end - start + 1;

	public AttributeRange? BiggerThan(int value)
	{
		if (value < start || value > end) return null;
		return new AttributeRange(value + 1, end);
	}

	public AttributeRange? SmallerThan(int value)
	{
		if (value < start || value > end) return null;
		return new AttributeRange(start, value - 1);
	}
}