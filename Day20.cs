using System.Runtime.Serialization;
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using FluentAssertions.Numeric;

namespace AdventOfCode2024.CSharp.Day20;

public class Day20
{

  private const char Start = 'S';
  private const char End = 'E';
  private const char Wall = '#';
  private const char Open = '.';

  [Theory]
  [InlineData("Day20", 1445)]
  public void Part1(string file, int expected)
  {
    var grid = FormatInput(AoCLoader.LoadLines(file));
    var distances = GetDistancesToGoal(grid);
    var savings = ComputeSavings(grid, distances);
    savings.Count(it => it >= 100).Should().Be(expected);
  }

  [Fact]
  public void Part1SanityCheck() {
    var grid = FormatInput(AoCLoader.LoadLines("Day20.Sample"));
    var start = grid.Single(kv => kv.Value == Start).Key;

    var distances = GetDistancesToGoal(grid);
    distances[start].Should().Be(84);

    var savings = ComputeSavings(grid, distances);
    savings.Count(it => it == 2).Should().Be(14);
    savings.Count(it => it == 4).Should().Be(14);
    savings.Count(it => it == 6).Should().Be(2);
    savings.Count(it => it == 8).Should().Be(4);
    savings.Count(it => it == 10).Should().Be(2);
    savings.Count(it => it == 12).Should().Be(3);
    savings.Count(it => it == 20).Should().Be(1);
    savings.Count(it => it == 36).Should().Be(1);
    savings.Count(it => it == 38).Should().Be(1);
    savings.Count(it => it == 40).Should().Be(1);
    savings.Count(it => it == 64).Should().Be(1);
  }

  private List<long> ComputeSavings(Dictionary<Point, char> grid, Dictionary<Point, long> distances)
  {
    List<long> result = [];

    foreach(var wall in grid.Where(kv => kv.Value == '#').Select(it => it.Key)) {
      foreach (var v in Vector.Cardinals) {
        var next = wall + v;
        var previous = wall - v;
        if (distances.TryGetValue(next, out var n) && distances.TryGetValue(previous, out var p)) {
          if (p > n) result.Add(p - n - 2);
        }
      }
    }
    return result;
  }

  Dictionary<Point, long> GetDistancesToGoal(Dictionary<Point, char> grid) {
    var end = grid.Single(kv => kv.Value == End).Key;
    Dictionary<Point, long> closed = [];
    closed[end] = 0;
    Queue<Point> open = [];
    open.Enqueue(end);

    while (open.TryDequeue(out var current)) {
      var cd = closed[current];
      foreach(var v in Vector.Cardinals) {
        var next = current + v;
        if (closed.ContainsKey(next)) continue;
        if (grid[next] == '#') continue;
        closed[next] = cd + 1;
        open.Enqueue(next);
      }
    }

    return closed;
  }

  private static Dictionary<Point, char> FormatInput(List<string> input) => input.Gridify();
}
