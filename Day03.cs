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

  private long SumMuls(string arg1)
  {
    var c = arg1.ToCharArray();
    var i = 0;
    var mulp = P.Sequence(P.Digit.Range(1,3).Join().After(P.String("mul(")),
      P.Digit.Range(1,3).Join().Between(P.String(","), P.String(")")))
      .Select(it => Convert.ToInt64(it.First) * Convert.ToInt64(it.Second));
    long sum = 0;
    while (i < c.Length) {
      if (mulp.Parse(c, i) is ParseSuccess<long> s) {
        sum += s.Value;
        i = s.Position;
      } else { i += 1; }
    }
    return sum;
  }

  private long SumMuls2(string arg1)
  {
    var enabled = true;
    var c = arg1.ToCharArray();
    var i = 0;
    var mulp = P.Sequence(P.Digit.Range(1,3).Join().After(P.String("mul(")),
      P.Digit.Range(1,3).Join().Between(P.String(","), P.String(")")))
      .Select(it => Convert.ToInt64(it.First) * Convert.ToInt64(it.Second));
    var dop = P.String("do()");
    var dontp = P.String("don't()");
    long sum = 0;
    while (i < c.Length) {
      if (dop.Parse(c, i) is ParseSuccess<string> s) {
        enabled = true;
        i = s.Position;
      }
      else if (dontp.Parse(c, i) is ParseSuccess<string> s2) {
        enabled = false;
        i = s2.Position;
      }
      else if (mulp.Parse(c, i) is ParseSuccess<long> s3) {
        sum += enabled ? s3.Value : 0;
        i = s3.Position;
      } else { i += 1; }
    }
    return sum;
  }
}
