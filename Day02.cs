using AdventOfCode2024.CSharp.Utils;
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

  [Theory]
  [InlineData("Day02.Sample", 4)]
  [InlineData("Day02", 634)]
  public void Part3(string path, int expected)
  {
    var data = Convert(AoCLoader.LoadLines(path));
    data.Count(IsSafe3).Should().Be(expected);
  }

  private static bool IsSafe1(List<long> items)
  {
    var sign = Math.Sign(items[0] - items[1]);
    return items.Windows(2).All(w => IsSafe(w[0], w[1], sign));
  }

  private static bool IsSafe(long a, long b, int sign)
  {
    var d = a - b;
    return Math.Sign(d) == sign
        && Math.Abs(d) <= 3
        && a != b;
  }

  private static bool IsSafe2(List<long> items)
  {
    if (IsSafe1(items)) return true;
    for (var i = 0; i < items.Count; i++)
    {
      List<long> temp = [ .. items[0..i], .. items[(i+1)..] ];
      if (IsSafe1(temp)) return true;
    }
    return false;
  }

  private static bool IsSafe3(List<long> items)
  {
    var temp = new int[]{0, 0, 0};
    foreach (var w in items.Windows(2)) temp[Math.Sign(w[0] - w[1]) + 1]++;
    var sign = temp[0] > temp[2] ? -1 : 1;

    var ws = items.Windows(2).Select(w => IsSafe(w[0], w[1], sign)).ToList();
    var fwsi = ws.Select((it,index)=>(it,index)).Where(it => !it.it).Select(it => it.index).ToList();
    if (fwsi.Count == 0) return true;
    if (fwsi.Count == 1) 
    {
      var index = fwsi[0];
      if (index == 0) return true;
      if (index == ws.Count - 1) return true;
      return IsSafe(items[index-1], items[index+1], sign)
        || IsSafe(items[index], items[index + 2], sign);
    }
    if (fwsi.Count > 2) return false;
    if (fwsi[0] + 1 != fwsi[1]) return false;
    return IsSafe(items[fwsi[0]], items[fwsi[0] + 2], sign);
  }

  private static List<List<long>> Convert(List<string> data)
  {
    return P.Long.Trim().Plus().ParseMany(data);
  }
}
