using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;
using Utils;
using AdventOfCode2024.CSharp.Utils;
namespace AdventOfCode2024.CSharp.Day18;

public class Day18
{

  [Theory]
  [InlineData("Day18.Sample", 6, 12, 22)]
  [InlineData("Day18", 70, 1024, 326)]
  public void Part1(string file, long size, int take, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var grid = input.Take(take).ToHashSet();
    Walk(grid, size).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day18.Sample", 6, "6,1")]
  [InlineData("Day18", 70, "18,62")]
  public void Part2(string file, long size, string expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var index = MiscUtils.BinarySearch(input.Count, (take) => {
      var grid = input.Take(take).ToHashSet();
      return Walk(grid, size) == null;
    }) ?? throw new ApplicationException();

    var x = input[index - 1];
    $"{x.X},{x.Y}".Should().Be(expected);
  }

  [Theory]
  [InlineData("Day18.Sample", 6, "6,1")]
  [InlineData("Day18", 70, "18,62")]
  public void Part2_2(string file, int size, string expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));

    var point = MiscUtils.BinarySearch(input.Count, (take) => {
      var walls = input.Take(take).ToHashSet();
      return !CanFindGoalFromStart(walls, size);
    }) ?? throw new ApplicationException();
    
    var wall = input[point-1];
    $"{wall.X},{wall.Y}".Should().Be(expected);
    return;
  }

  [Theory]
  [InlineData("Day18.Sample", 6, "6,1")]
  [InlineData("Day18", 70, "18,62")]
  public void Part2_3(string file, int size, string expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));

    var path = FindAnyPath([], size);

    HashSet<Point> walls = [];
    foreach(var wall in input) {
      walls.Add(wall);
      if (path.Contains(wall)) {
        path = FindAnyPath(walls, size);
        if (path.Count == 0)
        {
          $"{wall.X},{wall.Y}".Should().Be(expected);
          return;
        }
      }
    }
    
    throw new ApplicationException();
  }

  [Theory]
  [InlineData("Day18.Sample", 6, "6,1")]
  [InlineData("Day18", 70, "18,62")]
  public void Part2_by_union(string file, int size, string expected)
  {
    var walls = FormatInput(AoCLoader.LoadLines(file));

    var closed = new Dictionary<Point, DisjointSet>();

    DisjointSet MakeWall(Point first, Vector v, long count) {
      var current = first;
      var result = new DisjointSet();
      if (!closed.TryAdd(current, result)) throw new ApplicationException();
      for(var i = 0; i < count; i++) {
        current += v;
        if (!closed.TryAdd(current, result)) throw new ApplicationException();
      }
      return result;
    }

    var leftWall = MakeWall(new(0, -1), Vector.South, size);
    var rightWall = MakeWall(new(0, size+1), Vector.South, size);
    var topWall = MakeWall(new(-1, 0), Vector.East, size);
    var bottomWall = MakeWall(new(size + 1, 0), Vector.East, size);

    Point needle = Point.Zero;
    foreach(var wall in walls) {
      needle = wall;
      var ds = new DisjointSet();
      if (!closed.TryAdd(wall, ds)) throw new ApplicationException();

      foreach(var next in Vector.CompassRose.Select(v => wall + v).Where(next => closed.ContainsKey(next))) {
        ds.Union(closed[next]);
      }
      
      if (leftWall.SameUnion(topWall)) break;
      if (leftWall.SameUnion(rightWall)) break;
      if (rightWall.SameUnion(bottomWall)) break;
      if (topWall.SameUnion(bottomWall)) break;
    }
    
    $"{needle.X},{needle.Y}".Should().Be(expected);
  }

  public static HashSet<Point> FindAnyPath(HashSet<Point> walls, long size) {
    var goal = new Point(size, size);
    var start = Point.Zero;
    var stack = new Stack<Point>([start]);

    var closed = new Dictionary<Point, Point>
    {
      { start, start}
    };

    while (stack.TryPop(out var current)) {
      foreach(var v in Vector.Cardinals) {
        var next = current + v;
        if (next.X < 0 || next.X > size || next.Y < 0 || next.Y > size) continue;
        if (walls.Contains(next)) continue;
        if (next == goal) {
          HashSet<Point> result = [goal, start, current];
          while (current != start) {
            current = closed[current];
            result.Add(current);
          }
          return result;
        }
        var cl = closed[current];
        if (closed.ContainsKey(next)) continue;
        closed[next] = current;
        stack.Push(next);
      }
    }
    return [];
  }
  
  public static bool CanFindGoalFromStart(HashSet<Point> walls, long size) {
    var start = Point.Zero;
    var goal = new Point(size, size);
    var stack = new Stack<Point>([start]);
    var closed = new HashSet<Point>([..walls, start]);

    while (stack.TryPop(out var current)) {
      foreach(var v in Vector.Cardinals) {
        var next = current + v;
        if (next == goal) return true;
        if (next.X < 0 || next.X > size || next.Y < 0 || next.Y > size) continue;
        if (closed.Contains(next)) continue;
        closed.Add(next);
        stack.Push(next);
      }
    }
    return false;
  }

  public static long? Walk(HashSet<Point> walls, long size) {
    var goal = new Point(size, size);
    var start = Point.Zero;
    var queue = new Queue<Point>([start]);

    var closed = new Dictionary<Point, long>
    {
      { start, 0 }
    };

    while (queue.TryDequeue(out var current)) {
      var cl = closed[current];
      foreach(var v in Vector.Cardinals) {
        var next = current + v;
        if (next.X < 0 || next.X > size || next.Y < 0 || next.Y > size) continue;
        if (walls.Contains(next)) continue;
        if (next == goal) return cl + 1;
        if (closed.TryGetValue(next, out long value2) && value2 <= cl + 1) continue;
        closed[next] = cl + 1;
        queue.Enqueue(next);
      }
    }
    return null;
  }

  private static List<Point> FormatInput(List<string> input)
  {
    return P.Format("{},{}", P.Long, P.Long)
      .Select(it => new Point(it.Second, it.First))
      .ParseMany(input);
  }
}
