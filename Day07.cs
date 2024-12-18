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
    input.Where(it => TestReverse(it.Test, it.Terms.ToArray(), false)).Select(it => it.Test).Sum()
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day07.Sample", 11387)]
  [InlineData("Day07", 116094961956019L)]
  public void Part2(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    input.Where(it => TestReverse(it.Test, it.Terms.ToArray(), true)).Select(it => it.Test).Sum()
      .Should().Be(expected);
  }

  private static bool TestReverse(long test, ReadOnlySpan<long> terms, bool includeConcat)
  {
    if (terms.Length == 0) return test == 0;
    if (terms.Length == 1) return test == terms[0];
    if (test <= 0) return false;
    var term = terms[^1];
    if (includeConcat) {
      var ts = $"{test}";
      var tt = $"{term}";
      if (ts.EndsWith(tt)) 
        if (TestReverse(ts.Length == tt.Length ? 0 : Convert.ToInt64(ts[..^tt.Length]), terms[..^1], includeConcat))
          return true;
    }
    if (test % term == 0)
      if (TestReverse(test / term, terms[..^1], includeConcat))
        return true;
    return TestReverse(test - term, terms[..^1], includeConcat);
  }

  private static List<(long Test, List<long> Terms)> FormatInput(List<string> input)
  {
    return input.Select(P.Format("{}: {}", P.Long, P.Long.Trim().Star()).Parse).ToList();
  }
}
