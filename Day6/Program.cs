using Utils;

var input = File.ReadAllLines("input.txt");
var times = input[0].Split(":")[1].ToLongList();
var distances = input[1].Split(":")[1].ToLongList();

long result1 = 1;
for (int i = 0; i < times.Count; i++)
{
	result1 *= GetWinningCount(times[i], distances[i]);
}

var raceTime = long.Parse(input[0].Split(":")[1].Replace(" ", string.Empty));
var winningDistance = long.Parse(input[1].Split(":")[1].Replace(" ", string.Empty));
var result2 = GetWinningCount(raceTime, winningDistance);

Console.WriteLine("1: " + result1);
Console.WriteLine("2: " + result2);

static long GetWinningCount(long totalTime, long bestDistance)
{
	long winningCount = 0;

	for (int chargingTime = 1; chargingTime < totalTime; chargingTime++)
	{
		if (chargingTime * (totalTime - chargingTime) > bestDistance)
		{
			winningCount++;
		}
	}

	return winningCount;
}