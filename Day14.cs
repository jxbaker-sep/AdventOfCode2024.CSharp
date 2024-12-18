using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;
namespace AdventOfCode2024.CSharp.Day14;

public class Day14
{
  [Theory]
  [InlineData("Day14.Sample", 11, 7, 12)]
  [InlineData("Day14", 101, 103, 230900224)]
  public void Part1(string file, int width, int height, long expected)
  {
    var robots = FormatInput(AoCLoader.LoadLines(file));

    foreach (var second in Enumerable.Range(1, 100))
    {
      robots = robots.Select(robot => Move(robot, width, height)).ToList();
    }

    robots.Where(r => r.Point.X != width / 2 && r.Point.Y != height / 2)
      .Select(r => (r.Point.X <= width / 2, r.Point.Y <= height / 2))
      .GroupBy(it => it)
      .Select(it => it.ToList())
      .Select(it => it.LongCount())
      .Product()
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day14", 101, 103, 6532L)]
  public void Part2(string file, int width, int height, long expected)
  {
    var robots = FormatInput(AoCLoader.LoadLines(file));

    foreach (var second in Enumerable.Range(1, 10000))
    {
      robots = robots.Select(robot => Move(robot, width, height)).ToList();
      var h = robots.Select(it => it.Point).ToHashSet();

      if (h.Any(r1 => h.Contains(new(r1.Y, r1.X + 1)) &&
                      h.Contains(new(r1.Y, r1.X + 2)) &&
                      h.Contains(new(r1.Y, r1.X + 3)) &&
                      h.Contains(new(r1.Y, r1.X + 4)) &&
                      h.Contains(new(r1.Y, r1.X + 5)) &&
                      h.Contains(new(r1.Y, r1.X + 6)) &&
                      h.Contains(new(r1.Y, r1.X + 7)) && 
                      h.Contains(new(r1.Y, r1.X + 8)) &&
                      h.Contains(new(r1.Y, r1.X + 9))))
      {
        ((long)second).Should().Be(expected);
        return;
      }
    }
    throw new ApplicationException();
  }

  private void Print(List<Robot> robots)
  {
    var points = robots.Select(r => r.Point).ToHashSet();
    long minx = points.Select(it => it.X).Min();
    long maxx = points.Select(it => it.X).Max();
    long miny = points.Select(it => it.Y).Min();
    long maxy = points.Select(it => it.Y).Max();

    for (var y = miny; y <= maxy; y++)
    {
      for (var x = minx; x <= maxx; x++)
      {
        Console.Write(points.Contains(new(y, x)) ? '#' : ' ');
      }
      Console.WriteLine();
    }
  }

  private Robot Move(Robot robot, int width, int height)
  {
    var p2 = robot.Point + robot.Vector;
    if (p2.X < 0) p2 = p2 with { X = width + p2.X };
    if (p2.X >= width) p2 = p2 with { X = p2.X - width };
    if (p2.Y < 0) p2 = p2 with { Y = height + p2.Y };
    if (p2.Y >= height) p2 = p2 with { Y = p2.Y - height };
    return robot with { Point = p2 };
  }

  private record Robot(Point Point, Vector Vector);

  private static List<Robot> FormatInput(List<string> input)
  {
    return P.Format("p={},{} v={},{}", P.Long, P.Long, P.Long, P.Long)
      .Select(it => new Robot(new(it.Second, it.First), new(it.Fourth, it.Third)))
      .ParseMany(input);
  }
}
