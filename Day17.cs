using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;
using Utils;
using AdventOfCode2024.CSharp.Utils;
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


  [Fact]
  public void Part2_2()
  {
    var program = FormatInput(AoCLoader.LoadFile("Day17"));
    List<long> x = program.Codes.ToList();
    var result = DetermineFinal(x, LongPow2((x.Count - 1) * 3)).ToList();

    program = program with { A = result[0]};
    Run(program).ToList().Should().BeEquivalentTo(program.Codes);
    result[0].Should().Be(258394985014171);
  }

  IEnumerable<long> DetermineFinal(List<long> items, long baseValue) {
    if (items.Count == 0) {
      yield return baseValue;
      yield break;
    }
    for (var i = 0; i < 8; i ++) {
      var a0 = baseValue + (i * LongPow2((items.Count - 1) * 3));
      var a = a0 / LongPow2 ((items.Count - 1) * 3); 
      var b = (a % 8) ^ 7 ; 
      var c = a / LongPow2(b); 
      var output = (b ^ c ^ 7) % 8;
      if (output == items[^1]) {
        foreach(var sub in DetermineFinal(items[..^1], a0)) yield return sub;
      }
    }
  }

  [Theory]
  [InlineData("Day17.Sample.2", 117440)]
  [InlineData("Day17", 258394985014171)]
  public void Part2_3(string file, long expected)
  {
    var program = FormatInput(AoCLoader.LoadFile(file));
    List<long> x = program.Codes.ToList();
    var result = Determine(program, x, 1, 0) ?? throw new ApplicationException();
    result.Should().Be(expected);
  }

  long? Determine(Program program, List<long> items, int n, long baseValue) {
    for (var i = 0; i < 8; i ++) {
      var attempt = baseValue + i;
      if (Enumerable.SequenceEqual(Run(program with {A = attempt}).ToList(), items[(items.Count -n)..])) {
        if (n == items.Count) return attempt;
        if (Determine(program, items, n + 1, attempt << 3) is {} x) return x;
      }
    }
    return null;
  }

  public IEnumerable<long> Run(Program program)
  {
    while (program.IP < program.Codes.Count)
    {
      var (np, output) = Next(program);
      if (output is long i) yield return i;
      program = np;
    }
  }

  public (Program, long? output) Next(Program program) {
    long? output = null;
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
          program = program with { A = program.A / LongPow2(combo()) };
          break;
        case bxl:
          program = program with { B = program.B ^ operand };
          break;
        case bst:
          program = program with { B = combo() % 8 };
          break;
        case jnz:
          if(program.A != 0) next = (int)operand;
          break;
        case bxc:
          program = program with { B = program.B ^ program.C };
          break;
        case outputInstruction:
          output = combo() % 8;
          break;
        case bdv:
          program = program with { B = program.A / LongPow2(combo()) };
          break;
        case cdv:
          program = program with { C = program.A / LongPow2(combo()) };
          break;
      }
      program = program with { IP = next };
      return (program, output);
  }

  private long LongPow2(long y) {
    var result = 1L;
    for(long x = 0; x < y; x++) result *= 2;
    return result;
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

  public record Program(long A, long B, long C, int IP, IReadOnlyList<long> Codes);

  private static Program FormatInput(string input)
  {
    return P.Format("Register A: {} Register B: {} Register C: {} Program: {}", P.Long, P.Long, P.Long, P.Long.Star(","))
      .Select(it => new Program(it.First, it.Second, it.Third, 0, it.Fourth))
      .Parse(input);
  }
}
