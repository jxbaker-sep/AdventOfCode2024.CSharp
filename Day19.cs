using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;
using Utils;
using AdventOfCode2024.CSharp.Utils;

namespace AdventOfCode2024.CSharp.Day19;

public class Day19
{

  [Theory]
  [InlineData("Day19.Sample", 6)]
  [InlineData("Day19", 300)]
  public void Part1(string file, int expected)
  {
    var input = FormatInput(AoCLoader.LoadFile(file));
    Dictionary<string, bool> cache = [];
    input.Patterns.Count(pattern => IsPossible(pattern, input.Towels, cache))
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day19.Sample", 16)]
  [InlineData("Day19", 624802218898092L)]
  public void Part2(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadFile(file));
    Dictionary<string, long> cache = [];
    input.Patterns.Sum(pattern => CountVariations(pattern, input.Towels, cache))
      .Should().Be(expected);
  }

  private static long CountVariations(string pattern, IReadOnlyList<string> towels, Dictionary<string, long> cache)
  {
    if (cache.TryGetValue(pattern, out var cached)) return cached;
    if (pattern.Length == 0) return 1;
    long result = 0;
    foreach (var towel in towels)
    {
      if (pattern.StartsWith(towel))
      {
        result += CountVariations(pattern[towel.Length..], towels, cache);
      }
    }
    cache[pattern] = result;
    return result;
  }

  private static bool IsPossible(string pattern, IReadOnlyList<string> towels, Dictionary<string, bool> cache)
  {
    if (cache.TryGetValue(pattern, out var cached)) return cached;
    if (pattern.Length == 0) return true;
    foreach (var towel in towels)
    {
      if (pattern.StartsWith(towel) && IsPossible(pattern[towel.Length..], towels, cache))
      {
        cache[pattern] = true;
        return true;
      }
    }
    cache[pattern] = false;
    return false;
  }

  public record Onsen(IReadOnlyList<string> Towels, IReadOnlyList<string> Patterns);

  private static Onsen FormatInput(string input)
  {
    var sp = P.Letter.Plus().Join().Trim();
    return P.Format("{} {}", sp.Plus(","), sp.Plus()).Select(it => new Onsen(it.First, it.Second)).Parse(input);
  }
}
