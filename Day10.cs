
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;
namespace AdventOfCode2024.CSharp.Day10;

public class Day10
{
  [Theory]
  [InlineData("Day10.Sample", 36)]
  [InlineData("Day10", 512)]
  public void Part1(string file, int expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    ScoreTrailheads(input).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day10.Sample", 81)]
  [InlineData("Day10", 1045)]
  public void Part2(string file, int expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    ScoreTrailheads2(input).Should().Be(expected);
  }

  private static int ScoreTrailheads(Dictionary<Point, int> input)
  {
    Dictionary<Point, HashSet<Point>> cache = [];

    return input.Where(kv => kv.Value == 0)
      .Select(it => ReachablePeaks(input, it.Key, cache).Count)
      .Sum();
  }

  private static int ScoreTrailheads2(Dictionary<Point, int> input)
  {
    Dictionary<Point, int> cache = [];

    return input.Where(kv => kv.Value == 0)
      .Select(it => CountPaths(input, it.Key, cache))
      .Sum();
  }

  private static HashSet<Point> ReachablePeaks(Dictionary<Point, int> input, Point point, Dictionary<Point, HashSet<Point>> cache)
  {
    if (cache.TryGetValue(point, out var v)) return v;
    if (input[point] == 9)
    {
      cache[point] = [point];
      return [point];
    }
    HashSet<Point> result = [];
    foreach(var vector in Vector.Cardinals) {
      var point2 = point + vector;
      if (input.TryGetValue(point2, out var p2i) && p2i == input[point] + 1)
      {
        result.UnionWith(ReachablePeaks(input, point2, cache));
      }
    }
    cache[point] = result;
    return result;
  }

  private static int CountPaths(Dictionary<Point, int> input, Point point, Dictionary<Point, int> cache)
  {
    if (cache.TryGetValue(point, out var v)) return v;
    if (input[point] == 9)
    {
      cache[point] = 1;
      return 1;
    }
    int result = 0;
    foreach(var vector in Vector.Cardinals) {
      var point2 = point + vector;
      if (input.TryGetValue(point2, out var p2i) && p2i == input[point] + 1)
      {
        result += CountPaths(input, point2, cache);
      }
    }
    cache[point] = result;
    return result;
  }

  private static Dictionary<Point, int> FormatInput(List<string> input)
  {
    return input.Gridify().ToDictionary(kv => kv.Key, kv => kv.Value - '0');
  }
}
