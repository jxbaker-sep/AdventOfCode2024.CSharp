using FluentAssertions;
using Parser;
using Utils;
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
        data.Count(it => IsSafe1(it)).Should().Be(expected);
    }

    [Theory]
    [InlineData("Day02.Sample", 4)]
    [InlineData("Day02", 634)] // 619 too low
    public void Part2(string path, int expected)
    {
        var data = Convert(AoCLoader.LoadLines(path));
        data.Count(it => IsSafe2(it)).Should().Be(expected);
    }

  private bool IsSafe1(List<long> it)
  {
    var isDecreasing = Math.Sign(it[0] - it[1]);
    return it.Windows(2).All(w => IsSafe(w, isDecreasing));
  }

  private static bool IsSafe(List<long> w, int isDecreasing)
  {
    return Math.Sign(w[0] - w[1]) == isDecreasing 
        && Math.Abs(w[0] - w[1]) <= 3
        && w[0] != w[1];
  }

  private bool IsSafe2(List<long> items)
  {
    if (IsSafe1(items)) return true;
    for(var i = 0; i < items.Count ;i++)
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
