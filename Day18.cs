using FluentAssertions;
using Parser;
using P = Parser.ParserBuiltins;
using Utils;
using AdventOfCode2024.CSharp.Utils;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
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

    var leftWall = Enumerable.Range(0, size+1).Select(y => new Point(y, -1)).ToList();
    var rightWall = Enumerable.Range(0, size+1).Select(y => new Point(y, size + 1)).ToList();
    var topWall = Enumerable.Range(0, size+1).Select(x => new Point(-1, x)).ToList();
    var bottomWall = Enumerable.Range(0, size+1).Select(x => new Point(size + 1, x)).ToList();
    DisjointSet<Point> ds = new();
    foreach(var set in new[]{leftWall, rightWall, topWall, bottomWall}) {
      foreach(var item in set) ds.Union(set[0], item);
    }

    Point needle = Point.Zero;
    foreach(var wall in walls) {
      needle = wall;
      ds.MakeSet(wall);
      foreach(var next in Vector.CompassRose.Select(v => wall + v).Where(next => ds.Contains(next))) {
        ds.Union(wall, next);
      }
      
      if (ds.SameUnion(leftWall[0], topWall[0])) break;
      if (ds.SameUnion(leftWall[0], rightWall[0])) break;
      if (ds.SameUnion(rightWall[0], bottomWall[0])) break;
      if (ds.SameUnion(topWall[0], bottomWall[0])) break;
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

class DisjointSet<T> where T: notnull {

  private class Node(T Value, Node? Parent)
  {
    public readonly T Value = Value;
    public int Rank {get;set;} = 0;
    public Node? Parent = Parent;
  }

  readonly Dictionary<T, Node> Forest = [];

  public void MakeSet(T x) {
    if (Forest.ContainsKey(x)) return;
    Forest[x] = new(x, null);
  }

  public bool SameUnion(T t1, T t2) {
    return FindNode(Forest[t1]) == FindNode(Forest[t2]);
  }

  public bool Contains(T x) {
    return Forest.ContainsKey(x);
  }

  public T Find(T x) {
    return FindNode(Forest[x]).Value;
  }

  private static Node FindNode(Node x) {
    if (x.Parent == null) return x;
    x.Parent = FindNode(x.Parent);
    return x.Parent;
  }

  public void Union(T x, T y) {
    MakeSet(x);
    MakeSet(y);
    var nodeX = FindNode(Forest[x]);
    var nodeY = FindNode(Forest[y]);

    if (nodeX == nodeY) return;

    if (nodeX.Rank < nodeY.Rank) {
      (nodeY, nodeX) = (nodeX, nodeY);
    }

    nodeY.Parent = nodeX;
    if (nodeX.Rank == nodeY.Rank) {
      nodeX.Rank += 1;
    }
  }
}