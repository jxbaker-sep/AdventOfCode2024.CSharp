
using System.Net.Security;
using System.Runtime.CompilerServices;
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;
namespace AdventOfCode2024.CSharp.Day12;

public class Day12
{
  [Theory]
  [InlineData("Day12.Sample", 1930)]
  [InlineData("Day12", 1549354L)]
  public void Part1(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    long sum = 0;
    while (input.Any())
    {
      var section = Section(input);
      sum += section.area * section.perimeter;
    }
    sum.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day12.Sample", 1206)]
  [InlineData("Day12.Sample.2", 368)]
  [InlineData("Day12.Sample.3", 236)]
  [InlineData("Day12", 937032L)] 
  public void Part2(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    long sum = 0;
    while (input.Count != 0)
    {
      var section = Section(input);
      sum += section.area * section.perimeter2;
    }
    sum.Should().Be(expected);
  }

  public (long area, long perimeter, long perimeter2, char plant) Section(Dictionary<Point, char> garden)
  {
    Queue<Point> open = new([garden.First().Key]);
    long perimeter = 0;
    char plant = garden.First().Value;
    HashSet<Point> closed = [];
    while (open.TryDequeue(out var current))
    {
      if (closed.Contains(current)) continue;
      if (!garden.TryGetValue(current, out char value) || value != plant)
      {
        perimeter += 1;
        continue;
      }
      if (garden[current] != plant) { continue; }
      garden.Remove(current);
      closed.Add(current);
      foreach (var vector in Vector.Cardinals)
      {
        open.Enqueue(current + vector);
      }
    }

    var perimeter2 = FindPerimeter3(closed);

    return (closed.Count, perimeter, perimeter2, plant);
  }

  [Fact]
  public void Sample()
  {
    FindPerimeter3([new(0,0)]).Should().Be(4);
    FindPerimeter3([new(0,0), new(0,1)]).Should().Be(4);
    FindPerimeter3([new(0,0), new(0,1), new(1,0), new(1,1)]).Should().Be(4);
    FindPerimeter3([new(0,1), new(1,0), new(1,1), new(1,2), new(2,1)]).Should().Be(12);
    FindPerimeter3([new(0,0), new(0,1), new(1,0), new(0,2), new(1,1),
      new(0,3), new(1,2), new(1,3), new(2,2), new(2,3), new(3,2), new(2,4)]).Should().Be(10);
  }

  private static long FindPerimeter3(HashSet<Point> closed)
  {
    long perimeter = 0;
    var minx = closed.Select(it => it.X).Min();
    var miny = closed.Select(it => it.Y).Min();
    var maxx = closed.Select(it => it.X).Max();
    var maxy = closed.Select(it => it.Y).Max();
    for (var y = miny; y <= maxy; y++)
    {
      var wallOnLeft = false;
      var wallOnRight = false;
      var current = new Point(y, minx);
      var vector = Vector.East;
      var left = vector.RotateLeft();
      var right = vector.RotateRight();
      for (; current.X <= maxx; current += vector)
      {
        if (!closed.Contains(current)) {
          wallOnLeft = false;
          wallOnRight = false;
          continue;
        }
        if (!closed.Contains(current + left))
        {
          if (!wallOnLeft) perimeter += 1;
          wallOnLeft = true;
        }
        else wallOnLeft = false;
        if (!closed.Contains(current + right))
        {
          if (!wallOnRight) perimeter += 1;
          wallOnRight = true;
        }
        else wallOnRight = false;
      }
    }

    for (var x = minx; x <= maxx; x++)
    {
      var wallOnLeft = false;
      var wallOnRight = false;
      var current = new Point(miny, x);
      var vector = Vector.South;
      var left = vector.RotateLeft();
      var right = vector.RotateRight();
      for (; current.Y <= maxy; current += vector)
      {
        if (!closed.Contains(current)) {
          wallOnLeft = false;
          wallOnRight = false;
          continue;
        }
        if (!closed.Contains(current + left))
        {
          if (!wallOnLeft) perimeter += 1;
          wallOnLeft = true;
        }
        else wallOnLeft = false;
        if (!closed.Contains(current + right))
        {
          if (!wallOnRight) perimeter += 1;
          wallOnRight = true;
        }
        else wallOnRight = false;
      }
    }
    return perimeter;
  }

  private static Dictionary<Point, char> FormatInput(List<string> input)
  {
    return input.Gridify();
  }
}