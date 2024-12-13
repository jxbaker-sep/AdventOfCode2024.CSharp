
using System.Net.Security;
using System.Runtime.CompilerServices;
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;
namespace AdventOfCode2024.CSharp.Day13;

using Machine = (Vector A, Vector B, Point Prize);

public class Day13
{
  [Theory]
  [InlineData("Day13.Sample", 480)]
  [InlineData("Day13", 35574L)]
  public void Part1(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    input.Select(it => Prize(it)).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day13.Sample", 875318608908L)]
  [InlineData("Day13", 0L)] // 474697749331L too low, 81356796472639 too high
  public void Part2(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var scale = 10000000000000;
    input.Select(it => Prize((it.A, it.B, new Point(it.Prize.Y + scale, it.Prize.X + scale)))).Sum().Should().Be(expected);
  }


  static long Prize(Machine machine)
  {
    var ax = machine.A.X;
    var ay = machine.A.Y;
    var bx = machine.B.X;
    var by = machine.B.Y;
    var px = machine.Prize.X;
    var py = machine.Prize.Y;

    // a = (5400*22)-(67*8400) / ((22*34) + (-94*67))
    var d1 = py * bx - by * px;
    var d2 = bx * ay - ax * by;
    var a = Math.DivRem(d1, d2, out var remainder);

    if (remainder != 0)
    {
      return 0;
    }

    var b = (px - ax * a) / bx;

    return 3 * a + 1 * b;
  }

  private static List<Machine> FormatInput(List<string> input)
  {
    var button = P.Sequence(P.Long.After(P.Any.Between("Button ", ": X+")), P.Long.After(", Y+")).Trim()
      .Select(it => new Vector(it.Second, it.First));
    var prize = P.Sequence(P.Long.After("Prize: X="), P.Long.After(", Y=")).Trim()
      .Select(it => new Point(it.Second, it.First));
    var machine = P.Sequence(button, button, prize);
    return machine.Star().End().Parse(input.Join("\n"));
  }
}
