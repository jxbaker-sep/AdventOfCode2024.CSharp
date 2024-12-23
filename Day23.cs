using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp.Day23;

public class Day23
{
  [Theory]
  [InlineData("Day23.Sample", 7)]
  [InlineData("Day23", 1302)]
  public void Part1(string file, int expected)
  {
    var connections = FormatInput(AoCLoader.LoadLines(file)).ToHashSet();
    connections = [..connections, ..connections.Select(it => new Connection(it.Second, it.First))];
    var d = connections.GroupBy(it => it.First, it => it.Second).ToDictionary(it => it.Key, it => it.ToHashSet());
    
    var ts = d.Keys.Where(it => it[0] == 't').Distinct();

    HashSet<(string, string, string)> master = [];
    foreach(var t in ts)
    {
      var twoWay = d[t]
        .SelectMany(c1 => d[c1].Where(c2 => d[c2].Contains(t)).Select(c2 => (c1, c2)))
          .Select(it => {
            List<string> l = [t, it.c1, it.c2];
            l.Sort();
            return (l[0], l[1], l[2]);
          })
        .ToHashSet();
      master.UnionWith(twoWay);
    }

    master.Count.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day23.Sample", "co,de,ka,ta")]
  [InlineData("Day23", "cb,df,fo,ho,kk,nw,ox,pq,rt,sf,tq,wi,xz")]
  public void Part2(string file, string expected)
  {
    var connections = FormatInput(AoCLoader.LoadLines(file)).ToHashSet();
    connections = [..connections, ..connections.Select(it => new Connection(it.Second, it.First))];

    var d = connections.GroupBy(it => it.First, it => it.Second).ToDictionary(it => it.Key, it => it.ToHashSet());

    HashSet<(string, string, string)> largest = [];
    foreach(var first in d.Keys) {
      HashSet<(string, string, string)> temp = d[first]
        .SelectMany(c1 => d[c1].Where(c2 => d[c2].Contains(first)).Select(c2 => (c1, c2)))
          .Select(it => {
            List<string> l = [first, it.c1, it.c2];
            l.Sort();
            return (l[0], l[1], l[2]);
          })
          .ToHashSet();
      if (temp.Count > largest.Count) largest = temp;
    }

    var items = largest.SelectMany(it => new List<string>{it.Item1, it.Item2, it.Item3}).Distinct().Order().ToList();
    items.Join(",").Should().Be(expected);
  }

  public record Connection(string First, string Second);

  private static List<Connection> FormatInput(List<string> input)
  {
    return P.Format("{}-{}", P.Word, P.Word)
      .Select(it => new Connection(it.First, it.Second))
      .ParseMany(input);
  }
}
