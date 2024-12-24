using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Microsoft.Z3;
using Parser;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp.Day24;

public class Day24
{
  [Theory]
  [InlineData("Day24.Sample", 2024)]
  [InlineData("Day24", 57588078076750)]
  public void Part1(string file, long expected)
  {
    var gates = FormatInput(AoCLoader.LoadFile(file));

    using Context ctx = new([]);
    var solver = ctx.MkSimpleSolver();
    var variables = gates.Select(it => it.Label)
      .ToDictionary(it => it, it => ctx.MkBoolConst(it));
    foreach(var gate in gates) {
      if (gate.Type == GateType.Literal) {
        solver.Assert(ctx.MkEq(variables[gate.Label], ctx.MkBool(gate.Literal == 1)));
      }
      else if (gate.Type == GateType.And) {
        solver.Assert(ctx.MkEq(variables[gate.Label], ctx.MkAnd(variables[gate.Input1], variables[gate.Input2])));
      }
      else if (gate.Type == GateType.Or) {
        solver.Assert(ctx.MkEq(variables[gate.Label], ctx.MkOr(variables[gate.Input1], variables[gate.Input2])));
      }
      else if (gate.Type == GateType.Xor) {
        solver.Assert(ctx.MkEq(variables[gate.Label], ctx.MkXor(variables[gate.Input1], variables[gate.Input2])));
      }
    }

    if (solver.Check() != Status.SATISFIABLE) throw new ApplicationException();

    var x= gates
      .Select(g => g.Label)
      .Where(label => label.StartsWith("z"))
      .OrderByDescending(it => it)
      .ToList();

    gates
      .Select(g => g.Label)
      .Where(label => label.StartsWith("z"))
      .OrderByDescending(it => it)
      .Select(label => ((BoolExpr)solver.Model.ConstInterp(variables[label])).IsTrue ? 1L : 0L)
      .Aggregate((a,b) => a * 2 + b)
      .Should().Be(expected);
  }


  public enum GateType {
    Literal, And, Or, Xor
  };
  public record Gate(string Label, GateType Type, string Input1, string Input2, long Literal);

  private static List<Gate> FormatInput(string input)
  {
    var label = P.Word + (P.Letter | P.Digit).Star().Join();
    var result = P.Sequence(P.Format("{}: {}", label, P.Long)
      .Select(it => new Gate(it.First, GateType.Literal, "", "", it.Second))
      .Star().Trim()
      ,
      P.Format("{} {} {} -> {}", label, P.Word, label, label)
        .Select(it => new Gate(it.Fourth, it.Second switch {
          "AND" => GateType.And,
          "XOR" => GateType.Xor,
          "OR" => GateType.Or,
          _ => throw new ApplicationException()
        }, it.First, it.Third, 0))
        .Star()
    ).Parse(input);
    return [..result.First, ..result.Second];
  }
}
