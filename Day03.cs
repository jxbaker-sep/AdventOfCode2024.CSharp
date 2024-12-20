using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp;

public class Day03
{
  [Theory]
  [InlineData("Day03.Sample", 161)]
  [InlineData("Day03", 165225049L)]
  public void Part1(string path, int expected)
  {
    var data = AoCLoader.LoadLines(path);
    SumMuls(data.Join(" ")).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day03.Sample.2", 48)]
  [InlineData("Day03", 108830766L)]
  public void Part2(string path, int expected)
  {
    var data = AoCLoader.LoadLines(path);
    SumMuls2(data.Join(" ")).Should().Be(expected);
  }

  private static long SumMuls(string arg1)
  {
    return (
      P.Format("mul({},{})", P.Digit.Range(1, 3).Join(), P.Digit.Range(1, 3).Join())
      .Select(it => Convert.ToInt64(it.First) * Convert.ToInt64(it.Second))
      | P.Any.Select(_ => 0L)
    ).Star().Select(it => it.Sum())
    .Parse(arg1);
  }

  private static long SumMuls2(string input)
  {
    return (
      P.Sequence(
        P.String("don't()"),
        P.Any.Until(P.String("do()"))
      ).Select(_ => 0L)
      | P.Format("mul({},{})", P.Digit.Range(1, 3).Join(), P.Digit.Range(1, 3).Join())
         .Select(it => Convert.ToInt64(it.First) * Convert.ToInt64(it.Second))
      | P.Any.Select(_ => 0L)
    ).Star().Select(it => it.Sum())
    .Parse(input);
  }
}
