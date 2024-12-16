using System.ComponentModel;
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
namespace AdventOfCode2024.CSharp.Day16;

public class Day16
{
  private const char Wall = '#';
  private const char Start = 'S';
  private const char End = 'E';

  [Theory]
  [InlineData("Day16.Sample", 7036)]
  [InlineData("Day16.Sample.2", 11048)]
  [InlineData("Day16", 105508)]
  public void Part1(string file, long expected)
  {
    var world = FormatInput(AoCLoader.LoadLines(file));
    BestPaths(world).First().Score.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day16.Sample", 45)]
  [InlineData("Day16.Sample.2", 64)]
  [InlineData("Day16", 0)]
  public void Part2(string file, int expected)
  {
    var world = FormatInput(AoCLoader.LoadLines(file));
    BestPaths(world).SelectMany(it => it.Path).ToHashSet().Count.Should().Be(expected);
  }

  public static IEnumerable<(List<Point> Path, long Score)> BestPaths(Dictionary<Point, char> world) {
    var start = world.Where(kv => kv.Value == Start).Single().Key;
    var goal = world.Where(kv => kv.Value == End).Single().Key;

    var open = new PriorityQueue<(Point Point, Vector Vector, long Score, List<Point> Path)>(it => it.Score);
    open.Enqueue((start, Vector.East, 0, [start]));
    Dictionary<(Point, Vector), long> closed = [];
    closed[(start, Vector.East)] = 0;

    long? lowestScore = null;

    while (open.TryDequeue(out var current)) {
      if (lowestScore != null && current.Score > lowestScore) yield break;
      if (world[current.Point] == End) {
        lowestScore = current.Score;
        yield return (current.Path, current.Score);
      }
      if (closed[(current.Point, current.Vector)] < current.Score) continue;
      foreach(var next in new[]{
        (current.Point + current.Vector, current.Vector, current.Score + 1),
        (current.Point + current.Vector.RotateLeft(), current.Vector.RotateLeft(), current.Score + 1001),
        (current.Point + current.Vector.RotateRight(), current.Vector.RotateRight(), current.Score + 1001),
      }) {
        if (world[next.Item1] == Wall) continue;
        if (closed.TryGetValue((next.Item1, next.Item2), out var existing) && existing <= next.Item3) continue;
        closed[(next.Item1, next.Item2)] = next.Item3;
        var nextPath = current.Path.ToList();
        nextPath.Add(next.Item1);
        open.Enqueue((next.Item1, next.Item2, next.Item3, nextPath));
      }
    }
  }

  private static Dictionary<Point, char> FormatInput(List<string> input)
  {
    return input.Gridify();
  }
}
