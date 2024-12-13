
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
    input.Select(MinTokensToWin).Sum().Should().Be(expected);
  }

  // [Theory]
  // [InlineData("Day13.Sample", 0)]
  // // [InlineData("Day13", 35574L)]
  // public void Part2(string file, long expected)
  // {
  //   var input = FormatInput(AoCLoader.LoadLines(file));
  //   input.Select(MinTokensToWin2).Sum().Should().Be(expected);
  // }

  private long MinTokensToWin(Machine machine)
  {
    var x= WinningTokensAlongAxis(machine.A.X, machine.B.X, machine.Prize.X)
      .Intersect(WinningTokensAlongAxis(machine.A.Y, machine.B.Y, machine.Prize.Y))
      .Order()
      .Take(1)
      .ToList();
    if (x.Count == 1) return x[0];
    return 0;
  }

  private IEnumerable<long> WinningTokensAlongAxis(long x1, long x2, long prize, bool max100 = true)
  {
    // var max1 = prize / x1;
    // if (max100) max1 = new[]{max1, 100}.Min();
    // for(var press1 = 0; press1 <= max1; press1++) {
    //   var even = (prize - x1 * press1) % x2 == 0;
    //   if (!even) continue;
    //   var press2 = (prize - press1 * x1) / x2;
    //   if (max100 && press2 > 100) continue;
    //   yield return press1 * 3 + press2;
    // }
    var x11 = x1;
    var x22 = x2;
    var factorsa = MathUtils.Factorize(x1);
    var factorsb = MathUtils.Factorize(x2);
    var tempa = factorsa;
    var tempb = factorsb;
    factorsa = factorsa.RemoveCommon(factorsb);
    factorsb = tempb.RemoveCommon(tempa);
    x1 = factorsa.Product();
    x2 = factorsb.Product();

    var maxa = prize / x1;
    if (max100 && maxa > 100) maxa = 100;

    for (var a = 0L; a <= maxa ; a++) {
      var subprize = prize - a * x11;
      if (subprize % x22 == 0)
      {
        var b = subprize / x22;
        if (max100 && b > 100) continue;
        for (; a <= maxa && b >= 0; a += x2, b -= x1) {
          yield return a * 3 + b;
        }
        yield break;
      }
    }
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
