
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;
namespace AdventOfCode2024.CSharp.Day07;

public class Day07
{
  [Theory]
  [InlineData("Day07.Sample", 3749)]
  [InlineData("Day07", 267566105056L)]
  public void Part1(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    input.Select(Compute).OfType<long>().Sum()
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day07.Sample", 11387)]
  [InlineData("Day07", 116094961956019L)]
  public void Part2(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    input.Select(Compute2).OfType<long>().Sum()
      .Should().Be(expected);
  }

  private long? Compute((long Test, List<long> Terms) input)
  {
    var permutations = PermuteOperators(input.Terms.Count - 1);
    var x = permutations.Select(perm => Compute1(input.Terms, perm, input.Test)).ToList();
     var y = x.FirstOrDefault(result => result == input.Test);
     return y;
  }

  private long? Compute2((long Test, List<long> Terms) input)
  {
    var permutations = PermuteOperators(input.Terms.Count - 1);
    var x = permutations.Select(perm => Compute1(input.Terms, perm, input.Test) as long?)
      .FirstOrDefault(result => result == input.Test);
    if (x != null) return x;
    permutations = PermuteOperators2(input.Terms.Count - 1);
    return permutations.Select(perm => Compute1(input.Terms, perm, input.Test))
      .FirstOrDefault(result => result == input.Test);
  }

  private static long Compute1(List<long> terms, List<Func<long, long, long>> perm, long test)
  {
    var result = terms[0];
    foreach(var (term, operation) in terms.Skip(1).Zip(perm)) {
      result = operation(result, term);
      if (result > test) return 0;
    }
    return result;
  }

  private static List<List<Func<long, long, long>>> PermuteOperators(int length)
  {
    static long add(long a, long b) => a + b;
    static long mul(long a, long b) => a * b;
    if (length == 0) return [ [] ];
    var next = PermuteOperators(length -1);
    return next.SelectMany(it => new List<List<Func<long, long, long>>>([ [ ..it, add ], [ ..it, mul ] ]))
      .ToList();
  }

  private static List<List<Func<long, long, long>>> PermuteOperators2(int length)
  {
    static long add(long a, long b) => a + b;
    static long mul(long a, long b) => a * b;
    static long concat(long a, long b) => Convert.ToInt64($"{a}{b}");
    if (length == 0) return [ [] ];
    var next = PermuteOperators2(length -1);
    return next.SelectMany(it => new List<List<Func<long, long, long>>>([ [ ..it, add ], 
    [ ..it, mul ], [ ..it, concat ] ]))
      .ToList();
  }

  private static List<(long, List<long>)> FormatInput(List<string> input)
  {
    return input.Select(P.Long.Before(":").Then(P.Long.Trim().Star()).Parse).ToList();
  }
}
