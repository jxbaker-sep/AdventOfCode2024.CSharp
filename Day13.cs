using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;
using Microsoft.Z3;
namespace AdventOfCode2024.CSharp.Day13;

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
  [InlineData("Day13.Sample", 480)]
  [InlineData("Day13", 35574L)]
  public void Part1_z3(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    input.Select(it => Prize_z3(it)).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day13.Sample", 875318608908L)]
  [InlineData("Day13", 80882098756071)]
  public void Part2(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var scale = 10000000000000;
    input.Select(it => Prize((it.A, it.B, new Point(it.Prize.Y + scale, it.Prize.X + scale)))).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day13.Sample", 875318608908L)]
  [InlineData("Day13", 80882098756071)]
  public void Part2_z3(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var scale = 10000000000000;
    input.Select(it => Prize_z3((it.A, it.B, new Point(it.Prize.Y + scale, it.Prize.X + scale)))).Sum().Should().Be(expected);
  }

  static long Prize_z3(Machine machine)
  {
    using Context ctx = new([]);
    var ax = ctx.MkInt(machine.A.X);
    var ay = ctx.MkInt(machine.A.Y);
    var bx = ctx.MkInt(machine.B.X);
    var by = ctx.MkInt(machine.B.Y);
    var px = ctx.MkInt(machine.Prize.X);
    var py = ctx.MkInt(machine.Prize.Y);

    var apress = ctx.MkIntConst("apress");
    var bpress = ctx.MkIntConst("bpress");

    var solver = ctx.MkSimpleSolver();
    solver.Assert(ctx.MkEq(px, ctx.MkAdd(ctx.MkMul(ax, apress), ctx.MkMul(bx, bpress))));
    solver.Assert(ctx.MkEq(py, ctx.MkAdd(ctx.MkMul(ay, apress), ctx.MkMul(by, bpress))));

    if (solver.Check() != Status.SATISFIABLE) return 0;
    return ((IntNum)solver.Model.ConstInterp(apress)).Int64 * 3 + ((IntNum)solver.Model.ConstInterp(bpress)).Int64;
  }

  static long Prize(Machine machine)
  {
    var ax = machine.A.X;
    var bx = machine.B.X;
    var ay = machine.A.Y;
    var by = machine.B.Y;
    var px = machine.Prize.X;
    var py = machine.Prize.Y;

    var denominator = ax * by - ay * bx;
    var numerator = ax * py - ay * px;

    var bpress = Math.DivRem(numerator, denominator, out var bremainder);

    if (bremainder != 0) return 0;

    var apress = Math.DivRem(px - bx * bpress, ax, out var aremainder);

    if (aremainder != 0) return 0;

    return apress * 3 + bpress;
  }

  public record Machine(Vector A, Vector B, Point Prize);

  private static List<Machine> FormatInput(List<string> input)
  {
    var button = P.Format("Button {}: X+{}, Y+{}", P.Any, P.Long, P.Long)
      .Select(it => new Vector(it.Third, it.Second));
    var prize = P.Format("Prize: X={}, Y={}", P.Long, P.Long)
      .Select(it => new Point(it.Second, it.First));
    var machine = P.Sequence(button, button, prize).Select(it => new Machine(it.First, it.Second, it.Third));
    return machine.Star().End().Parse(input.Join("\n"));
  }
}
