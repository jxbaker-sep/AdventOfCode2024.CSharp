using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;
namespace AdventOfCode2024.CSharp.Day11;

public class Day11
{
  [Theory]
  [InlineData("Day11.Sample", 25, 55_312)]
  [InlineData("Day11.Sample", 75, 65_601_038_650_482L)]
  [InlineData("Day11", 25, 202_019L)]
  [InlineData("Day11", 75, 239_321_955_280_205L)]
  public void Part1(string file, int blinks, long expected)
  {
    var input = FormatInput(AoCLoader.LoadFile(file));
    input.Select(it => NumberOfStonesAfterBlinking(it, blinks)).Sum().Should().Be(expected);
  }

  readonly Dictionary<(long, int), long> Cache = [];
  private long NumberOfStonesAfterBlinking(long value, int blinksRemaining)
  {
    if (blinksRemaining == 0) return 1;
    if (Cache.TryGetValue((value, blinksRemaining), out var cached)) return cached;
    long result;
    if (value == 0) result = NumberOfStonesAfterBlinking(1, blinksRemaining - 1);
    else
    {
      var digits = $"{value}";
      if (digits.Length % 2 == 0)
      {
        var lhs = Convert.ToInt64(digits[..(digits.Length / 2)]);
        var rhs = Convert.ToInt64(digits[(digits.Length / 2)..]);
        result = NumberOfStonesAfterBlinking(lhs, blinksRemaining - 1) +
          NumberOfStonesAfterBlinking(rhs, blinksRemaining - 1);
      }
      else
      {
        result = NumberOfStonesAfterBlinking(value * 2024, blinksRemaining - 1);
      }
    }
    Cache[(value, blinksRemaining)] = result;
    return result;
  }

  private static List<long> FormatInput(string input)
  {
    return P.Long.Trim().Star().Parse(input);
  }
}
