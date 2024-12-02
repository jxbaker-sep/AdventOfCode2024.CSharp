using System.Security.Cryptography;
using FluentAssertions;
using Parser;
using Utils;
using Xunit.Sdk;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp;

public class Day02
{
  [Theory]
  [InlineData("Day02.Sample", 2)]
  [InlineData("Day02", 598)]
  public void Part1(string path, int expected)
  {
    var data = Convert(AoCLoader.LoadLines(path));
    data.Count(IsSafe1).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day02.Sample", 4)]
  [InlineData("Day02", 634)]
  public void Part2(string path, int expected)
  {
    var data = Convert(AoCLoader.LoadLines(path));
    data.Count(IsSafe2).Should().Be(expected);
  }

  private static bool IsSafe1(List<long> it)
  {
    var sign = Math.Sign(it[0] - it[1]);
    return it.Windows(2).All(w => IsSafe(w, sign));
  }

  private static bool IsSafe(List<long> w, int sign)
  {
    return Math.Sign(w[0] - w[1]) == sign
        && Math.Abs(w[0] - w[1]) <= 3
        && w[0] != w[1];
  }

  private static bool IsSafe2(List<long> items)
  {
    if (IsSafe1(items)) return true;
    for (var i = 0; i < items.Count; i++)
    {
      var temp = items.ToList();
      temp.RemoveAt(i);
      if (IsSafe1(temp)) return true;
    }
    return false;
  }

  private static List<List<long>> Convert(string[] data)
  {
    return data.Select(it => P.Number.Trim().Plus().Parse(it)).ToList();
  }
}
