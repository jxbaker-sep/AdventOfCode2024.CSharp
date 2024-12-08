
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;
namespace AdventOfCode2024.CSharp.Day08;

public class Day08
{
  [Theory]
  [InlineData("Day08.Sample", 14)]
  [InlineData("Day08", 0)]
  public void Part1(string file, int expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    GetAntinodes1(input).Count.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day08.Sample", 34)]
  [InlineData("Day08.Sample.2", 9)]
  [InlineData("Day08", 1287)]
  public void Part2(string file, int expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    GetAntinodes2(input).Count.Should().Be(expected);
  }

  private static HashSet<Point> GetAntinodes1(Day08Input input)
  {
    HashSet<Point> antinodes = [];

    var byFrequency = input.World.GroupBy(it => it.Frequency, it => it.Point);

    foreach (var frequency in byFrequency)
    {
      foreach (var (a, b) in frequency.ToList().Pairs())
      {
        var v = a.VectorTo(b);
        var a1 = b + v;
        var a2 = a - v;
        if (a1.Y >= 0 && a1.Y < input.Height && a1.X >= 0 && a1.X < input.Width)
        {
          antinodes.Add(a1);
        }
        if (a2.Y >= 0 && a2.Y < input.Height && a2.X >= 0 && a2.X < input.Width)
        {
          antinodes.Add(a2);
        }
      }
    }
    return antinodes;
  }


  private static HashSet<Point> GetAntinodes2(Day08Input input)
  {
    HashSet<Point> antinodes = [];

    var byFrequency = input.World.GroupBy(it => it.Frequency, it => it.Point);

    foreach (var frequency in byFrequency)
    {
      foreach (var (a, b) in frequency.ToList().Pairs())
      {
        var v = a.VectorTo(b);

        var point = a;
        while (OnMap(point, input))
        {
          antinodes.Add(point);
          point += v;
        }

        point = a - v;
        while (OnMap(point, input))
        {
          antinodes.Add(point);
          point -= v;
        }
      }
    }
    return antinodes;
  }

  static bool OnMap(Point point, Day08Input input)
  {
    return point.Y >= 0 && point.Y < input.Height && point.X >= 0 && point.X < input.Width;
  }


  private static Day08Input FormatInput(List<string> input)
  {
    var result = input.SelectMany((line, row) => line.Select((c, col) => (new Point(row, col), c)));
    return new(result.Where(it => it.c != '.').ToHashSet(),
            input.Count, input[0].Length);
  }
}
