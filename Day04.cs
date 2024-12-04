using FluentAssertions;
using Utils;

using Vector = AdventOfCode2024.CSharp.Utils.Vector;

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
        count += CanRead("XMAS", y, x, data, Vector.East) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Vector.SouthEast) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Vector.South) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Vector.SouthWest) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Vector.West) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Vector.NorthWest) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Vector.North) ? 1 : 0;
        count += CanRead("XMAS", y, x, data, Vector.NorthEast) ? 1 : 0;
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

  private bool CanRead(string s, int y, int x, List<List<char>> data, Vector v)
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

  private List<List<char>> Convert(string[] strings) => strings.Select(it => it.ToCharArray().ToList()).ToList();
}