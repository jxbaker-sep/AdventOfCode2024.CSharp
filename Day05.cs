
using AdventOfCode2024.CSharp.TestInputs;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp.Day05;

public class Day05
{
  [Theory]
  [InlineData("Day05.Sample", 143)]
  [InlineData("Day05", 4774L)]
  public void Part1(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    var x= data.Pages.Where(p => IsCorrectlyOrdered(data.Ordering, p)).ToList();
    x.Select(it => it[it.Count / 2])
      .Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05.Sample", 123)]
  [InlineData("Day05", 6004)]
  public void Part2(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    var x= data.Pages.Where(p => !IsCorrectlyOrdered(data.Ordering, p))
      .Select(p => ElfOrder(data.Ordering, p))
      .ToList();
    x.Select(it => it[it.Count / 2])
      .Sum().Should().Be(expected);
  }

  private static List<long> ElfOrder(Dictionary<long, List<long>> ordering, List<long> pages)
  {
    var open = new Queue<long>(pages);
    var closed = new List<long>();
    var reversed = pages.ToDictionary(page => page, 
      page => ordering.Keys.Where(key => ordering[key].Contains(page) && pages.Contains(key)).ToList());

    while (open.TryDequeue(out var current)) {
      if (reversed[current].All(it => closed.Contains(it)))
      {
        closed.Add(current);
      } else {
        open.Enqueue(current);
      }
    }

    return closed;
  }

  private static bool IsCorrectlyOrdered(Dictionary<long, List<long>> ordering, List<long> pages)
  {
    var closed = new HashSet<long>();
    foreach(var page in pages)
    {
      var rules = ordering.GetValueOrDefault(page);
      if (rules is not {})
      {
        closed.Add(page);
        continue;
      }
      if (rules.Any(closed.Contains)) return false;
      closed.Add(page);
    }
    return true;
  }

  private static Day05Input Convert(List<string> input)
  {
    var p1 = input.TakeWhile(it => it.Length > 0).Select(P.Long.Before("|").Then(P.Long).Parse);
    var p2 = input.SkipWhile(it => it.Length > 0).Skip(1).ToList();
    var map = p1.GroupBy(it => it.First).ToDictionary(it => it.Key, it => it.Select(z=>z.Second).ToList());
    return new(
      map,
      p2.Select(it => P.Long.Star(",").Parse(it)).ToList()
    );
  }
}
