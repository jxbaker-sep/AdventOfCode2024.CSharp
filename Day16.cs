using System.ComponentModel;
using System.Net.Sockets;
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
namespace AdventOfCode2024.CSharp.Day16;

public class Day16
{
  private const char Wall = '#';
  private const char Start = 'S';
  private const char End = 'E';

  [Theory]
  [InlineData("Day16.Sample", 7036)]
  [InlineData("Day16.Sample.2", 11048)]
  [InlineData("Day16", 105508)]
  public void Part1(string file, long expected)
  {
    var world = FormatInput(AoCLoader.LoadLines(file));
    BestPaths(world).First().Score.Should().Be(expected);
  }

  [Theory]
  [InlineData("Day16.Sample", 7036, 45)]
  [InlineData("Day16.Sample.2", 11048, 64)]
  [InlineData("Day16", 105508, 548)]
  public void Part2(string file, long lowestScore, int expected)
  {
    var world = FormatInput(AoCLoader.LoadLines(file));
    PointsOnScoringPaths(world, lowestScore).ToHashSet().Count.Should().Be(expected);
  }

  public static IEnumerable<(List<Point> Path, long Score)> BestPaths(Dictionary<Point, char> world) {
    var start = world.Where(kv => kv.Value == Start).Single().Key;
    var goal = world.Where(kv => kv.Value == End).Single().Key;

    var open = new PriorityQueue<(Point Point, Vector Vector, long Score, List<Point> Path)>(it => it.Score);
    open.Enqueue((start, Vector.East, 0, [start]));
    Dictionary<(Point, Vector), long> closed = [];
    closed[(start, Vector.East)] = 0;

    long? lowestScore = null;

    while (open.TryDequeue(out var current)) {
      if (lowestScore != null && current.Score > lowestScore) yield break;
      if (world[current.Point] == End) {
        lowestScore = current.Score;
        yield return (current.Path, current.Score);
      }
      if (closed[(current.Point, current.Vector)] < current.Score) continue;
      foreach(var next in new[]{
        (current.Point + current.Vector, current.Vector, current.Score + 1),
        (current.Point + current.Vector.RotateLeft(), current.Vector.RotateLeft(), current.Score + 1001),
        (current.Point + current.Vector.RotateRight(), current.Vector.RotateRight(), current.Score + 1001),
      }) {
        if (world[next.Item1] == Wall) continue;
        if (closed.TryGetValue((next.Item1, next.Item2), out var existing) && existing <= next.Item3) continue;
        closed[(next.Item1, next.Item2)] = next.Item3;
        var nextPath = current.Path.ToList();
        nextPath.Add(next.Item1);
        open.Enqueue((next.Item1, next.Item2, next.Item3, nextPath));
      }
    }
  }

  public static IEnumerable<Point> PointsOnScoringPaths(Dictionary<Point, char> world, long lowestScore) {
    var start = world.Where(kv => kv.Value == Start).Single().Key;
    var goal = world.Where(kv => kv.Value == End).Single().Key;
    HashSet<Point> result = [start, goal];

    Dictionary<(Point Point, Vector Vector), long> CostToGoal = [];
    CostToGoal.Add((goal, Vector.East), 0);
    CostToGoal.Add((goal, Vector.West), 0);
    CostToGoal.Add((goal, Vector.South), 0);
    CostToGoal.Add((goal, Vector.North), 0);

    var open = new Queue<(Point Point, Vector Vector)>();
    open.Enqueue((goal, Vector.East));
    open.Enqueue((goal, Vector.North));

    while (open.TryDequeue(out var current)) {
      var currentScore = CostToGoal[current];
      foreach(var next in new[]{
        (Point: current.Point - current.Vector, Vector: current.Vector,               Score: currentScore + 1),
        (Point: current.Point - current.Vector, Vector: current.Vector.RotateLeft(),  Score: currentScore + 1001),
        (Point: current.Point - current.Vector, Vector: current.Vector.RotateRight(), Score: currentScore + 1001)
      }) {
        if (world[next.Point] == Wall) continue;
        if (CostToGoal.TryGetValue((next.Point, next.Vector), out var existing) && existing <= next.Score) continue;
        CostToGoal[(next.Point, next.Vector)] = next.Score;
        if (lowestScore < next.Score) continue;
        if (world[next.Point] == Start) continue;
        open.Enqueue((next.Point, next.Vector));
      }
    }
    open.Clear();
    Dictionary<(Point Point, Vector Vector), long> CostToStart = [];
    CostToStart.Add((start, Vector.East), 0);
    CostToStart.Add((start, Vector.North), 0);
    CostToStart.Add((start, Vector.South), 0);
    CostToStart.Add((start, Vector.West), 0);

    open = new Queue<(Point Point, Vector Vector)>();
    open.Enqueue((start, Vector.East));

    while (open.TryDequeue(out var current)) {
      var currentScore = CostToStart[current];
      foreach(var next in new[]{
        (Point: current.Point + current.Vector,              Vector: current.Vector,               Score: currentScore + 1),
        (Point: current.Point + current.Vector.RotateLeft(), Vector: current.Vector.RotateLeft(),  Score: currentScore + 1001),
        (Point: current.Point + current.Vector.RotateRight(), Vector: current.Vector.RotateRight(), Score: currentScore + 1001)
      }) {
        if (world[next.Point] == Wall) continue;
        if (CostToStart.TryGetValue((next.Point, next.Vector), out var existing) && existing <= next.Score) continue;
        CostToStart[(next.Point, next.Vector)] = next.Score;
        if (lowestScore < next.Score) continue;
        if (world[next.Point] == Start) continue;
        open.Enqueue((next.Point, next.Vector));
      }
    }

    foreach(var point in world.Where(it => it.Value != Wall).Select(it => it.Key)) {
      foreach(var vector in Vector.Cardinals) {
        if (CostToGoal.TryGetValue((point, vector), out var g) && CostToStart.TryGetValue((point, vector), out var s)
          && g + s <= lowestScore) yield return point;
      }
    }
  }

  private static Dictionary<Point, char> FormatInput(List<string> input)
  {
    return input.Gridify();
  }
}
