
using AdventOfCode2024.CSharp.Utils;
using AdventOfCode2024.CSharp.Utils.TestInputs;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp.Day06;

record Foo(int i);

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
    HashSet<Point> tried = [data.Start, ..data.World];
    foreach(var (point, vector) in originalPath)
    {
      var next = point + vector;
      if (next.Y < 0 || next.Y >= data.Height || next.X < 0 || next.X >= data.Width) continue;
      if (tried.Contains(next)) continue;
      tried.Add(next);
      if (Walk(data with {World = [.. data.World, next], Start = point, StartingVector = vector}).Item2) count += 1;
    }

    count.Should().Be(expected);
  }

  private static (HashSet<(Point, Vector)>, bool) Walk(Day06Input data)
  {
    var v = data.StartingVector;
    var current = data.Start;

    HashSet<(Point, Vector)> visited = [(data.Start, v)];
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

  private static Day06Input FormatInput(List<string> input)
  {
    var result = input.SelectMany((line, row) => line.Select((c, col) => (new Point(row, col), c)));
    return new(result.Where(it => it.c == '#').Select(it => it.Item1).ToHashSet(),
            result.Single(it => it.c == '^').Item1,
            input.Count, input[0].Length, Vector.North);
  }
}
