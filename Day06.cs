
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp.Day06;

public class Day06
{
  [Theory]
  [InlineData("Day06.Sample", 41)]
  [InlineData("Day06", 5534)]
  public void Part1(string file, int expected)
  {
    var data = FormatInput(AoCLoader.LoadLines(file));
    var visited = Walk(data).Item1;

    visited.Select(it => it.Item1).ToHashSet().Count.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day06.Sample", 6)]
  [InlineData("Day06", 2262)]
  public void Part2(string file, int expected)
  {
    var data = FormatInput(AoCLoader.LoadLines(file));
    var originalPath = Walk(data).Item1;

    var count = 0;
    HashSet<Point> tried = [data.Start];
    foreach(var (point, vector) in originalPath)
    {
      var next = point + vector;
      if (next.Y < 0 || next.Y >= data.Height || next.X < 0 || next.X >= data.Width) continue;
      if (tried.Contains(next)) continue;
      tried.Add(next);
      var original = data.World.ToHashSet();
      data.World.Add(next);

      if (Walk(data).Item2) count += 1;

      data.World = original;
    }

    count.Should().Be(expected);
  }

  private static (HashSet<(Point, Point)>, bool) Walk((HashSet<Point> World, Point Start, int Height, int Width) data)
  {
    var v = Point.North;
    var current = data.Start;

    HashSet<(Point, Point)> visited = [(data.Start, v)];
    while (true)
    {
      var next = current + v;
      if (data.World.Contains(next))
      {
        v = v.RotateRight();
        visited.Add((current, v));
        continue;
      }
      current = next;
      if (current.Y < 0 || current.Y >= data.Height || current.X < 0 || current.X >= data.Width) break;
      if (!visited.Add((current, v))) return ([], true);
    }

    return (visited, false);
  }

  private static (HashSet<Point> World, Point Start, int Height, int Width) FormatInput(List<string> input)
  {
    var result = input.SelectMany((line, row) => line.Select((c, col) => (new Point(row, col), c)));
    return (result.Where(it => it.c == '#').Select(it => it.Item1).ToHashSet(),
            result.Single(it => it.c == '^').Item1,
            input.Count, input[0].Length);
  }
}
