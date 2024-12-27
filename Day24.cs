using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Microsoft.Z3;
using Parser;
using Utils;
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
    ComputeOutput(gates).Should().Be(expected);
  }

  [Theory]
  // [InlineData("Day24.Sample.2", "z00,z01,z02,z05")]
  [InlineData("Day24")]
  public void Part2(string file)
  {
    var gates = FormatInput(AoCLoader.LoadFile(file));

    gates = gates.Where(it => it.Type != GateType.Literal)
      .Select(it => {
        var z = new[]{it.Input1, it.Input2}.Order().ToList();
        return it with {Input1 = z[0], Input2 = z[1]};
      })
      .ToList();

    var za = gates.Count(it => it.Output.StartsWith('z'));

    // z00 = x00 XOR y00
    // z00_carry = X00 AND Y00
    // z01_based = x01 XOR y01
    // z01 = 

    // x01   y01   carry   result   new carry
    //  0     0       0       0        0
    //  1     0       0       1        0       
    //  0     1       0       1        0         
    //  1     1       0       0        1         
    //  0     0       1       1        0         result = (x01 XOR y01) XOR carry
    //  1     0       1       0        1         newcarry = (x01 AND y01) OR (CARRY AND (x01 XOR y01))
    //  0     1       1       0        1
    //  1     1       1       1        1

    // Output of looking for missing XORs
    //  7
    // 23
    // 27
    // 45 // expected: no y45 or x45, last bit

    // gnj AND scw -> z07
    // gnj XOR scw -> shj

    // jwb OR hjp -> z23
      // gvb AND pvr -> hjp
    // ????
    
    // x27 AND y27 -> z27
    // x27 XOR y27 -> vpt
    // vpt XOR hdg -> kcd
    
    // wmr OR btg -> z45

    // y16 AND x16 -> tpk
    // x16 XOR y16 -> wkb

    // gvb XOR pvr -> pfn
    // jwb OR hjp -> z23

    var g1 = gates.Where(g => g.Output == "z07").Single();
    var g2 = gates.Where(g => g.Output == "shj").Single();
    var g3 = gates.Where(g => g.Output == "z27").Single();
    var g4 = gates.Where(g => g.Output == "kcd").Single();
    var g5 = gates.Where(g => g.Output == "tpk").Single();
    var g6 = gates.Where(g => g.Output == "wkb").Single();
    var g7 = gates.Where(g => g.Output == "pfn").Single();
    var g8 = gates.Where(g => g.Output == "z23").Single();
    gates = Swap(gates, g1, g2);
    gates = Swap(gates, g3, g4);
    gates = Swap(gates, g5, g6);
    gates = Swap(gates, g7, g8);

    var s = new[]{g1.Output, g2.Output, g3.Output, g4.Output, g5.Output, g6.Output, g7.Output, g8.Output}
      .Order().ToList().Join(",");

    var carry = gates.Where(it => it.Output == "dhb").Single();
    for (var i = 1 ; i < za-1; i++) {
      var x = $"x{i:00}";
      var y = $"y{i:00}";
      var z = $"z{i:00}";
      // Find the XOR for this z
      var xor = gates.Where(it => it.Output == z && it.Type == GateType.Xor).Single();
      // FIND CARRY FOR THIS Z
      var newcarry1 = gates.Where(it => it.Input1 == x && it.Input2 == y && it.Type == GateType.And).Single();
      // find xor xor carry
      var xor2 = gates.Where(it => it.Input1 == x && it.Input2 == y && it.Type == GateType.Xor).Single();
      (xor.Input1 == carry.Output || xor.Input2 == carry.Output).Should().BeTrue();
      var newCarry2 = gates.Where(it => it.Type == GateType.Or && it.HasInput(newcarry1.Output)).Single();
      carry = newCarry2;
    }

  }

  static List<Gate> Swap(List<Gate> gates, Gate g1, Gate g2)
  {
    return gates
      .Select(g => (g == g1) ? (g1 with {Output = g2.Output}) :
                   (g == g2) ? (g2 with {Output = g1.Output}) :
                   g)
      .ToList();
  }

  public long ComputeOutput(List<Gate> gates)
  {
    using Context ctx = new([]);
    var solver = ctx.MkSimpleSolver();
    var variables = gates.Select(it => it.Output)
      .ToDictionary(it => it, it => ctx.MkBoolConst(it));
    foreach(var gate in gates) {
      if (gate.Type == GateType.Literal) {
        solver.Assert(ctx.MkEq(variables[gate.Output], ctx.MkBool(gate.Literal == 1)));
      }
      else if (gate.Type == GateType.And) {
        solver.Assert(ctx.MkEq(variables[gate.Output], ctx.MkAnd(variables[gate.Input1], variables[gate.Input2])));
      }
      else if (gate.Type == GateType.Or) {
        solver.Assert(ctx.MkEq(variables[gate.Output], ctx.MkOr(variables[gate.Input1], variables[gate.Input2])));
      }
      else if (gate.Type == GateType.Xor) {
        solver.Assert(ctx.MkEq(variables[gate.Output], ctx.MkXor(variables[gate.Input1], variables[gate.Input2])));
      }
    }

    if (solver.Check() != Status.SATISFIABLE) throw new ApplicationException();

    var x= gates
      .Select(g => g.Output)
      .Where(label => label.StartsWith("z"))
      .OrderByDescending(it => it)
      .ToList();

    return gates
      .Select(g => g.Output)
      .Where(label => label.StartsWith("z"))
      .OrderByDescending(it => it)
      .Select(label => ((BoolExpr)solver.Model.ConstInterp(variables[label])).IsTrue ? 1L : 0L)
      .Aggregate((a,b) => a * 2 + b);
  }


  public enum GateType {
    Literal, And, Or, Xor
  };
  public record Gate(string Output, GateType Type, string Input1, string Input2, long Literal)
  {
    public bool HasInput(string i) => i == Input1 || i == Input2;
  }

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
