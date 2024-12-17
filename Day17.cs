using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;
using Utils;
namespace AdventOfCode2024.CSharp.Day17;

public class Day17
{
  private const int adv = 0;
  private const int bxl = 1;
  private const int bst = 2;
  private const int jnz = 3;
  private const int bxc = 4;
  private const int outputInstruction = 5;
  private const int bdv = 6;
  private const int cdv = 7;


  [Theory]
  [InlineData("Day17.Sample", "4,6,3,5,6,3,5,2,1,0")]
  [InlineData("Day17", "2,1,4,0,7,4,0,2,3")]
  public void Part1(string file, string expected)
  {
    var input = FormatInput(AoCLoader.LoadFile(file));
    Run(input).Join(",").Should().Be(expected);
  }

  public IEnumerable<long> Run(Program program)
  {
    while (program.IP < program.Codes.Count)
    {
      var (np, output) = Next(program);
      if (output is int i) yield return i;
      program = np;
    }
  }

  public (Program, int? output) Next(Program program) {
    int? output = null;
    var opcode = program.Codes[program.IP];
      var operand = program.Codes[program.IP + 1];
      var next = program.IP + 2;
      var combo = () => operand switch
      {
        <= 3 => operand,
        4 => program.A,
        5 => program.B,
        6 => program.C,
        _ => throw new ApplicationException()
      };
      switch (opcode)
      {
        case adv:
          program = program with { A = program.A >> combo() };
          break;
        case bxl:
          program = program with { B = program.B ^ operand };
          break;
        case bst:
          program = program with { B = combo() % 8 };
          break;
        case jnz:
          if(program.A != 0) next = operand;
          break;
        case bxc:
          program = program with { B = program.B ^ program.C };
          break;
        case outputInstruction:
          output = combo() % 8;
          break;
        case bdv:
          program = program with { B = program.A >> combo() };
          break;
        case cdv:
          program = program with { C = program.A >> combo() };
          break;
      }
      program = program with { IP = next };
      return (program, output);
  }

  [Fact]
  public void OperandTests() {
    Next(new Program(6, 0, 1, 0, [adv, 0])).Item1.A.Should().Be(6);
    Next(new Program(6, 0, 1, 0, [adv, 1])).Item1.A.Should().Be(3);
    Next(new Program(6, 0, 1, 0, [adv, 2])).Item1.A.Should().Be(1);
    Next(new Program(5, 1, 1, 0, [adv, 3])).Item1.A.Should().Be(0);

    Next(new Program(0, 0, 9, 0, [2, 6])).Item1.B.Should().Be(1);
    
    Run(new Program(10, 0, 0, 0, [5,0,5,1,5,4])).Join(",").Should().Be("0,1,2");
    Run(new Program(2024, 0, 0, 0, [0,1,5,4,3,0])).Join(",").Should().Be("4,2,5,6,7,7,7,7,3,1,0");

    Next(new Program(0, 29, 0, 0, [1, 7])).Item1.B.Should().Be(26);
    Next(new Program(0, 2024, 43690, 0, [4, 0])).Item1.B.Should().Be(44354);

  }

  public record Program(int A, int B, int C, int IP, IReadOnlyList<int> Codes);

  private static Program FormatInput(string input)
  {
    return P.Format("Register A: {} Register B: {} Register C: {} Program: {}", P.Long, P.Long, P.Long, P.Long.Star(","))
      .Select(it => new Program((int)it.First, (int)it.Second, (int)it.Third, 0, it.Fourth.Select(it => (int)it).ToList()))
      .Parse(input);
  }
}
