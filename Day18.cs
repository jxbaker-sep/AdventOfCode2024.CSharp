using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;
using Utils;
using AdventOfCode2024.CSharp.Utils;
namespace AdventOfCode2024.CSharp.Day18;

public class Day18
{

  [Theory]
  [InlineData("Day18.Sample", 6, 12, 22)]
  [InlineData("Day18", 70, 1024, 326)]
  public void Part1(string file, long size, int take, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var grid = input.Take(take).ToHashSet();
    Walk(grid, size)!.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day18.Sample", 6, "6,1")]
  [InlineData("Day18", 70, "18,62")]
  public void Part2(string file, long size, string expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var index = MiscUtils.BinarySearch(input.Count, (take) => {
      var grid = input.Take((int)take).ToHashSet();
      return Walk(grid, size) == null;
    })!;

    var x = input[(int)index - 1];
    $"{x.X},{x.Y}".Should().Be(expected);
  }

  public static long? Walk(HashSet<Point> grid, long size) {
    var goal = new Point(size, size);
    var queue = new PriorityQueue<(Point Point, long Steps)>(it => it.Steps + it.Point.ManhattanDistance(goal));
    queue.Enqueue((new(0,0), 0));

    var closed = new Dictionary<Point, long>
    {
      { new(0, 0), 0 }
    };

    while (queue.TryDequeue(out var current)) {
      if (closed.TryGetValue(current.Point, out long value) && value < current.Steps) continue;
      foreach(var v in Vector.Cardinals) {
        var next = current.Point + v;
        if (next.X < 0 || next.X > size || next.Y < 0 || next.Y > size) continue;
        if (grid.Contains(next)) continue;
        if (next == goal) return current.Steps + 1;
        if (closed.TryGetValue(next, out long value2) && value2 <= current.Steps + 1) continue;
        closed[next] = current.Steps + 1;
        queue.Enqueue((next, current.Steps + 1));
      }
    }
    return null;
  }

  private static List<Point> FormatInput(List<string> input)
  {
    return P.Format("{},{}", P.Long, P.Long)
      .Select(it => new Point(it.Second, it.First))
      .ParseMany(input);
  }
}
