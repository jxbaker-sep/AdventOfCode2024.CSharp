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

    var ts = connections.Select(it => it.First).Where(it => it[0] == 't').Distinct();

    HashSet<(string, string, string)> master = [];
    foreach(var t in ts)
    {
      var twoWay = connections.Where(it => it.First == t)
        .SelectMany(c1 => connections.Where(c2 => c2.First == c1.Second && c2.Second != t && connections.Contains(new(c2.Second, t)))
          .Select(c2 => {
            List<string> l = [c1.First, c1.Second, c2.Second];
            l.Sort();
            return (l[0], l[1], l[2]);
          }))
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
    foreach(var (first, seconds) in d) {
      HashSet<(string, string, string)> temp = [];
      foreach(var second in seconds) {
        foreach(var third in d[second]) {
          if (d[third].Contains(first)) {
            List<string> l = [first, second, third];
            l.Sort();
            temp.Add((l[0], l[1], l[2]));
          }
        }
      }
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
