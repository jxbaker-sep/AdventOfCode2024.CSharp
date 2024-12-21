using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;

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
    var savings = ComputeSavings(distances, 2);
    savings.Count(it => it >= 100).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day20", 1008040)]
  public void Part2(string file, long expected) // 983111 too low
  {
    var grid = FormatInput(AoCLoader.LoadLines(file));
    var distances = GetDistancesToGoal(grid);
    var savings = ComputeSavings(distances, 20);
    savings.LongCount(it => it >= 100).Should().Be(expected);
  }

  [Fact]
  public void Part1SanityCheck() {
    var grid = FormatInput(AoCLoader.LoadLines("Day20.Sample"));
    var start = grid.Single(kv => kv.Value == Start).Key;

    var distances = GetDistancesToGoal(grid);
    distances[start].Should().Be(84);

    var savings = ComputeSavings(distances, 2);
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

  [Fact]
  public void Part2SanityCheck() {
    var grid = FormatInput(AoCLoader.LoadLines("Day20.Sample"));
    var start = grid.Single(kv => kv.Value == Start).Key;

    var distances = GetDistancesToGoal(grid);
    distances[start].Should().Be(84);

    var savings = ComputeSavings(distances, 20);
    savings.Count(it => it == 50).Should().Be(32);
    savings.Count(it => it == 52).Should().Be(31);
    savings.Count(it => it == 54).Should().Be(29);
    savings.Count(it => it == 56).Should().Be(39);
    savings.Count(it => it == 58).Should().Be(25);
    savings.Count(it => it == 60).Should().Be(23);
    savings.Count(it => it == 62).Should().Be(20);
    savings.Count(it => it == 64).Should().Be(19);
    savings.Count(it => it == 66).Should().Be(12);
    savings.Count(it => it == 68).Should().Be(14);
    savings.Count(it => it == 70).Should().Be(12);
    savings.Count(it => it == 72).Should().Be(22);
    savings.Count(it => it == 74).Should().Be(4);
    savings.Count(it => it == 76).Should().Be(3);
  }

  private List<long> ComputeSavings(Dictionary<Point, long> distances, long cheatDistance)
  {
    Dictionary<(Point, Point), long> result = [];

    var maxx = distances.Keys.Select(it => it.X).Max();
    var maxy = distances.Keys.Select(it => it.Y).Max();

    foreach(var (first, k_first) in distances) {
      foreach(var y in MiscUtils.InclusiveRange(Math.Max(first.Y - cheatDistance, 0), Math.Min(first.Y + cheatDistance, maxy))) {
        foreach(var x in MiscUtils.InclusiveRange(Math.Max(first.X - cheatDistance, 0), Math.Min(first.X + cheatDistance, maxx))) {
          var next = new Point(y, x);
          if (next.ManhattanDistance(first) > cheatDistance) continue;
          if (distances.TryGetValue(next, out var k_next) && k_next < k_first - next.ManhattanDistance(first)) {
            result[(first, next)] = k_first - k_next - next.ManhattanDistance(first);
          }
        }
      }
    }
    return result.Values.ToList();
  }

  Dictionary<Point, long> GetDistancesToGoal(Dictionary<Point, char> grid) {
    var end = grid.Single(kv => kv.Value == End).Key;
    var closed = new Dictionary<Point, long> {
      { end, 0 }
    };
    var open = new Queue<Point>([end]);

    while (open.TryDequeue(out var current)) {
      var cd = closed[current];
      foreach(var v in Vector.Cardinals) {
        var next = current + v;
        if (closed.ContainsKey(next)) continue;
        if (grid[next] == Wall) continue;
        closed[next] = cd + 1;
        open.Enqueue(next);
      }
    }

    return closed;
  }

  private static Dictionary<Point, char> FormatInput(List<string> input) => input.Gridify();
}
