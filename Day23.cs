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
    foreach(var first in ts)
    {
      var twoWay = d[first]
        .SelectMany(second => d[second].Where(third => d[third].Contains(first)).Select(third => (second, third)))
          .Select(it => {
            List<string> l = [first, it.second, it.third];
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
      var temp = d[first]
        .SelectMany(second => d[second].Where(third => d[third].Contains(first)).Select(third => (second, third)))
          .Select(it => {
            List<string> l = [first, it.second, it.third];
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