using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Utils;

namespace AdventOfCode2024.CSharp.Day04;

public class Day04
{
  [Theory]
  [InlineData("Day04.Sample", 18)]
  [InlineData("Day04", 2633)]
  public void Part1(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    var count = 0;
    for (var y = 0; y < data.Count; y++)
    {
      for (var x = 0; x < data[0].Count; x++)
      {
        count += Vector.CompassRose.Count(v => CanRead("XMAS", y, x, data, v));
      }
    }

    count.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day04.Sample.2", 9)]
  [InlineData("Day04", 1936)]
  public void Part2(string file, int expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    var count = 0;
    for (var y = 0; y < data.Count; y++)
    {
      for (var x = 0; x < data[0].Count; x++)
      {
        if ((CanRead("MAS", y, x, data, Vector.SouthEast) ||
             CanRead("SAM", y, x, data, Vector.SouthEast)) &&
            (CanRead("MAS", y, x + 2, data, Vector.SouthWest) ||
             CanRead("SAM", y, x + 2, data, Vector.SouthWest)))
          count += 1;
      }
    }

    count.Should().Be(expected);
  }

  private static bool CanRead(string s, int y, int x, List<List<char>> data, Vector v)
  {
    for (int i = 0; i < s.Length; i++)
    {
      if (y < 0 || y >= data.Count) return false;
      if (x < 0 || x >= data[0].Count) return false;
      if (data[y][x] != s[i]) return false;
      y += (int)v.Y;
      x += (int)v.X;
    }
    return true;
  }

  private static List<List<char>> Convert(List<string> strings) => strings.Select(it => it.ToCharArray().ToList()).ToList();
}