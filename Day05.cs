
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

    data.Pages.Where(p => IsCorrectlyOrdered(data.Ordering, p)).ToList()
      .Select(it => it[it.Count / 2])
      .Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05.Sample", 123)]
  [InlineData("Day05", 6004)]
  public void Part2_1(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    data.Pages.Where(p => !IsCorrectlyOrdered(data.Ordering, p))
      .Select(p => ElfOrder(data.Ordering, p))
      .Select(it => it[it.Count / 2])
      .Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05.Sample", 123)]
  [InlineData("Day05", 6004)]
  public void Part2_2(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    data.Pages.Where(p => !IsCorrectlyOrdered(data.Ordering, p))
      .Select(p => ElfOrder2(data.Ordering, p))
      .Select(it => it[it.Count / 2])
      .Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05.Sample", 123)]
  [InlineData("Day05", 6004)]
  public void Part2_3(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    data.Pages.Where(p => !IsCorrectlyOrdered(data.Ordering, p))
      .Select(p => ElfOrder3(data.Ordering, p))
      .Select(it => it[it.Count / 2])
      .Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05.Sample", 123)]
  [InlineData("Day05", 6004)]
  public void Part2_4(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    data.Pages.Where(p => !IsCorrectlyOrdered(data.Ordering, p))
      .Select(p => ElfOrder4(data.Ordering, p))
      .Select(it => it[it.Count / 2])
      .Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day05.Sample", 123)]
  [InlineData("Day05", 6004)]
  public void Part2_5(string file, long expected)
  {
    var data = Convert(AoCLoader.LoadLines(file));

    data.Pages.Where(p => !IsCorrectlyOrdered(data.Ordering, p))
      .Select(p => ElfOrder5(data.Ordering, p))
      .Select(it => it[it.Count / 2])
      .Sum().Should().Be(expected);
  }

  private static List<long> ElfOrder(Dictionary<long, List<long>> ordering, List<long> pages)
  {
    var open = new Queue<long>(pages);
    var closed = new List<long>();
    var reversed = pages.ToDictionary(page => page, 
      page => ordering.Keys.Where(key => ordering[key].Contains(page) && pages.Contains(key)).ToHashSet());
    
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

  private static List<long> ElfOrder2(Dictionary<long, List<long>> ordering, List<long> pages)
  {
    var open = new Queue<long>(pages);
    var result = new List<long>();
    var reversed = pages.ToDictionary(page => page, 
      page => ordering.Where(kv => kv.Value.Contains(page) && pages.Contains(kv.Key)).Select(it => it.Key).ToHashSet());

    while (result.Count != pages.Count) {
      var first = reversed.Single(kv => kv.Value.Count == 0).Key;
      result.Add(first);
      reversed.Remove(first);
      foreach(var kv in reversed) kv.Value.Remove(first);
    }

    return result;
  }

  private static List<long> ElfOrder3(Dictionary<long, List<long>> ordering, List<long> pages)
  {
    Cache.Clear();
    var filteredOrdering = ordering.Where(kv => pages.Contains(kv.Key)).ToDictionary(kv => kv.Key, kv => kv.Value.Intersect(pages).ToList());
    var absolute = pages.ToDictionary(page => page, page => CreateAbsoluteOrdering(page, filteredOrdering));

    pages.Sort((a,b) => absolute[a].Contains(b) ? -1 : ((a == b) ? 0 : 1));
    return pages;
  }

  private static List<long> ElfOrder4(Dictionary<long, List<long>> ordering, List<long> pages)
  {
    var open = new Stack<long>(pages);
    var closed = new List<long>();
    var reversed = pages.ToDictionary(page => page, 
      page => ordering.Keys.Where(key => ordering[key].Contains(page) && pages.Contains(key)).ToHashSet());

    while (open.TryPeek(out var current)) {
      if (closed.Contains(current)) {
        open.Pop();
        continue;
      }
      if (reversed[current].All(it => closed.Contains(it)))
      {
        open.Pop();
        closed.Add(current);
      } else {
        foreach(var prereq in reversed[current]) open.Push(prereq);
      }
    }

    return closed;
  }

  private static List<long> ElfOrder5(Dictionary<long, List<long>> ordering, List<long> pages)
  {
    var rules = new List<(long First,long Second)>(ordering.SelectMany(kv => kv.Value.Select(v => (kv.Key, v))));
    // open is now the original list of X|Y rule pairs again.
    // Filter the list to contain only rules that reference pages
    rules = rules.Where(it => pages.Contains(it.First) && pages.Contains(it.Second)).ToList();
    List<long> result = [];

    while (rules.Count != 0) {
      var firsts = rules.Select(rule => rule.First).ToHashSet();
      var seconds = rules.Select(rule => rule.Second).ToHashSet();
      // find the single page with no prerequisites
      var page = firsts.Except(seconds).Single();
      result.Add(page);
      rules = rules.Where(it => it.First != page).ToList();
    }

    return result;
  }

  private static readonly Dictionary<long, List<long>> Cache = [];

  private static List<long> CreateAbsoluteOrdering(long page, Dictionary<long, List<long>> filteredOrdering)
  {
    if (Cache.GetValueOrDefault(page) is {} cached) return cached;
    var rule = filteredOrdering.GetValueOrDefault(page) ?? [];
    List<long> result = [];
    foreach(var item in rule) {
      if (result.Contains(item)) continue;
      result = [ .. result, item, .. CreateAbsoluteOrdering(item, filteredOrdering)];
    }
    Cache[page] = result;
    return result;
  }

  private static bool IsCorrectlyOrdered(Dictionary<long, List<long>> ordering, List<long> pages)
  {
    var closed = new HashSet<long>();
    foreach(var page in pages)
    {
      var rules = ordering.GetValueOrDefault(page) ?? [];
      if (rules.Any(closed.Contains)) return false;
      closed.Add(page);
    }
    return true;
  }

  public record Day05Input(Dictionary<long, List<long>> Ordering, List<List<long>> Pages);


  private static Day05Input Convert(List<string> input)
  {
    var p1 = input.TakeWhile(it => it.Length > 0).Select(P.Format("{}|{}", P.Long, P.Long).Parse);
    var p2 = input.SkipWhile(it => it.Length > 0).Skip(1).ToList();
    var map = p1.GroupBy(it => it.First, it => it.Second).ToDictionary(it => it.Key, it => it.ToList());
    return new(
      map,
      p2.Select(P.Long.Star(",").Parse).ToList()
    );
  }
}
