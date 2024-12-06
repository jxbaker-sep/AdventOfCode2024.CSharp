using FluentAssertions;
using Utils;

using Point = AdventOfCode2024.CSharp.Utils.Point;

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
        count += CanRead("XMAS", y, x, data, Point.East) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Point.SouthEast) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Point.South) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Point.SouthWest) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Point.West) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Point.NorthWest) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Point.North) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Point.NorthEast) ? 1 : 0;
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
        if ((CanRead("MAS", y, x, data, Point.SouthEast) ||
             CanRead("SAM", y, x, data, Point.SouthEast)) &&
            (CanRead("MAS", y, x + 2, data, Point.SouthWest) ||
             CanRead("SAM", y, x + 2, data, Point.SouthWest)))
          count += 1;
      }
    }

    count.Should().Be(expected);
  }

  private bool CanRead(string s, int y, int x, List<List<char>> data, Point v)
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