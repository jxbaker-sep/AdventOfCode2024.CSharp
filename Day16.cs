using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
namespace AdventOfCode2024.CSharp.Day16;

public class Day16
{
  private const char Wall = '#';
  private const char Start = 'S';
  private const char End = 'E';

  [Theory]
  [InlineData("Day16.Sample", 7036L, 45)]
  [InlineData("Day16.Sample.2", 11048L, 64)]
  [InlineData("Day16.Alternate.1", 21148, 149)]
  [InlineData("Day16.Alternate.2", 5078, 413)]
  [InlineData("Day16.Alternate.3", 41210, 514)]
  [InlineData("Day16", 105508L, 548)]
  public void Part1(string file, long expected, int expected2)
  {
    var world = FormatInput(AoCLoader.LoadLines(file));
    FindShortestPaths(world).Should().Be((expected, expected2));
  }

  public static (long Score, int Count) FindShortestPaths(Dictionary<Point, char> world) {
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
        if (world[next.Point] == Start) continue;
        open.Enqueue((next.Point, next.Vector));
      }
    }
    open.Clear();
    Dictionary<(Point Point, Vector Vector), long> CostToStart = [];
    CostToStart.Add((start, Vector.East), 0);

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
        if (world[next.Point] == End) continue;
        open.Enqueue((next.Point, next.Vector));
      }
    }

    var lowestScore = CostToStart.Where(kv => kv.Key.Point == goal).Select(it => it.Value).Min();

    var n = world.Where(it => it.Value != Wall).Select(it => it.Key).Where(point => Vector.Cardinals.Any(vector => 
      CostToGoal.TryGetValue((point, vector), out var g) && CostToStart.TryGetValue((point, vector), out var s)
          && g + s <= lowestScore)).Count();

    return (lowestScore, n);
  }

  private static Dictionary<Point, char> FormatInput(List<string> input)
  {
    return input.Gridify();
  }
}
