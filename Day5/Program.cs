using Utils;

var input = File.ReadAllLines("input.txt")
	.Where(i => !string.IsNullOrWhiteSpace(i))
	.ToList();

var seeds = input[0].Split(':')[1].ToLongList();
var seedRanges = seeds.Split(2).Select(g => new Range(g.First(), g.First() + g.Last())).ToList();

var seedToSoil = new List<MappingRange>();
var soilToFertilizer = new List<MappingRange>();
var fertilizerToWater = new List<MappingRange>();
var waterToLight = new List<MappingRange>();
var lightToTemp = new List<MappingRange>();
var tempToHum = new List<MappingRange>();
var humToLocation = new List<MappingRange>();

var current = seedToSoil;
for (int i = 1; i < input.Count; i++)
{
	if (char.IsDigit(input[i][0]))
	{
		var parts = input[i].ToLongList();
		current.Add(new MappingRange(parts[1], parts[0], parts[2]));
	}
	else
	{
		current = input[i] switch
		{
			"seed-to-soil map:" => seedToSoil,
			"soil-to-fertilizer map:" => soilToFertilizer,
			"fertilizer-to-water map:" => fertilizerToWater,
			"water-to-light map:" => waterToLight,
			"light-to-temperature map:" => lightToTemp,
			"temperature-to-humidity map:" => tempToHum,
			"humidity-to-location map:" => humToLocation,
			_ => throw new ArgumentException()
		};
	}
}

var soilNumbers = MapNumbers(seeds, seedToSoil).ToList();
var fertilizerNumbers = MapNumbers(soilNumbers, soilToFertilizer).ToList();
var waterNumbers = MapNumbers(fertilizerNumbers, fertilizerToWater).ToList();
var lightNumbers = MapNumbers(waterNumbers, waterToLight).ToList();
var tempNumbers = MapNumbers(lightNumbers, lightToTemp).ToList();
var humNumbers = MapNumbers(tempNumbers, tempToHum).ToList();
var locNumbers = MapNumbers(humNumbers, humToLocation).ToList();

var soilRanges = MapRanges(seedRanges, seedToSoil).ToList();
var fertilizerRanges = MapRanges(soilRanges, soilToFertilizer).ToList();
var waterRanges = MapRanges(fertilizerRanges, fertilizerToWater).ToList();
var lightRanges = MapRanges(waterRanges, waterToLight).ToList();
var tempRanges = MapRanges(lightRanges, lightToTemp).ToList();
var humRanges = MapRanges(tempRanges, tempToHum).ToList();
var locRanges = MapRanges(humRanges, humToLocation).ToList();

Console.WriteLine("1: " + locNumbers.Min());
Console.WriteLine("2: " + locRanges.Select(r => r.start).Min());

IEnumerable<long> MapNumbers(IEnumerable<long> numbers, List<MappingRange> toRanges)
{
	foreach (var number in numbers)
	{
		var range = toRanges.FirstOrDefault(r => r.Contains(number));
		yield return range != null ? range.map(number) : number;
	}
}

IEnumerable<Range> MapRanges(List<Range> fromRanges, List<MappingRange> toRanges)
{
	var toProcess = new Queue<Range>(fromRanges);
	while (toProcess.Count > 0)
	{
		var currentFrom = toProcess.Dequeue();
		var match = toRanges.FirstOrDefault(r => r.IsOverlapping(currentFrom));
		if (match == null)
		{
			yield return currentFrom;
		}
		else
		{
			var (mapped, rest) = match.Map(currentFrom);
			yield return mapped;
			toProcess.EnqueueRange(rest);
		}
	}
}

record Range(long start, long end)
{
	public bool Contains(long value) => start <= value && end > value;
	public bool IsOverlapping(Range range) => Contains(range.start) || Contains(range.end - 1);

	public (Range common, List<Range> rest) GetCommon(Range range)
	{
		var matchStart = Contains(range.start) ? range.start : start;
		var matchEnd = Contains(range.end - 1) ? range.end : end;
		var rest1 = !Contains(range.start) ? new Range(range.start, matchStart) : null;
		var rest2 = !Contains(range.end - 1) ? new Range(matchEnd, range.end) : null;

		return (new Range(matchStart, matchEnd), new[] { rest1, rest2 }.Where(r => r != null).Select(r => r!).ToList());
	}
}

record MappingRange(long start, long end, Func<long, long> map) : Range(start, end)
{
	public MappingRange(long start, long mapStart, long count)
		: this(start, start + count, (input) => input + mapStart - start)
	{
	}

	public (Range range, List<Range> rest) Map(Range range)
	{
		var (common, rest) = GetCommon(range);
		if (map(common.start) >= map(common.end))
		{
			throw new Exception();
		}

		return (new Range(map(common.start), map(common.end)), rest);
	}
}