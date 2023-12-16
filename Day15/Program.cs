var sequence = File.ReadLines("input.txt").First();

var hashes = sequence.Split(',')
	.Select(x => CalculateHash(x))
	.ToList();

var boxes = Enumerable.Range(0, 256).ToDictionary(i => i, i => new List<Lens>());
var steps = sequence.Split(',').Select(x => Parse(x)).ToList();

foreach (var step in steps)
{
	var boxNumber = CalculateHash(step.lens.label);
	var box = boxes[boxNumber];
	if (step.shouldAdd)
	{
		var existingIndex = box.FindIndex(l => l.label == step.lens.label);
		if (existingIndex >= 0)
		{
			box.RemoveAt(existingIndex);
			box.Insert(existingIndex, step.lens);
		}
		else
		{
			box.Add(step.lens);
		}
	}
	else
	{
		box.RemoveAll(l => l.label == step.lens.label);
	}
}

Console.WriteLine("1: " + hashes.Sum());
Console.WriteLine("2: " + CalculateFocusingPower(boxes));

int CalculateHash(string value)
{
	var acc = 0;
	foreach (var c in value)
	{
		acc += c;
		acc *= 17;
		acc = acc % 256;
	}

	return acc;
}

int CalculateFocusingPower(Dictionary<int, List<Lens>> boxes)
{
	int acc = 0;
	foreach (var box in boxes)
	{
		var boxNr = box.Key + 1;
		for (int i = 0; i < box.Value.Count; i++)
		{
			var lensSlot = i + 1;
			var lens = box.Value[i];
			acc += boxNr * lensSlot * lens.length;
		}
	}

	return acc;
}

Step Parse(string input)
{
	if (input.Contains("="))
	{
		var value = input.Split("=");
		return new Step(new Lens(value[0], int.Parse(value[1])), true);
	}
	else
	{
		return new Step(new Lens(input.Replace("-", string.Empty), 0), false);
	}
}

record Step(Lens lens, bool shouldAdd);

record Lens(string label, int length);
