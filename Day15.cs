using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
namespace AdventOfCode2024.CSharp.Day15;

public class Day15
{
  private const char Robot = '@';
  private const char Empty = '.';
  private const char SmallBox = 'O';
  private const char BigBoxLeft = '[';
  private const char Wall = '#';
  private const char BigBoxRight = ']';

  [Theory]
  [InlineData("Day15.Sample", 2028)]
  [InlineData("Day15.Sample.2", 10092)]
  [InlineData("Day15", 1430536)]
  public void Part1(string file, long expected)
  {
    var world = FormatInput(AoCLoader.LoadFile(file));

    var current = world.Grid.Where(kv => kv.Value == Robot).Single().Key;
    world.Grid[current] = Empty;

    foreach(var v in world.Instructions) {
      var next = current + v;
      if (TryPush(world.Grid, next, v)) {
        current = next;
      }
    }

    world.Grid.Where(kv => kv.Value == SmallBox)
      .Select(it => it.Key)
      .Select(it => it.Y * 100 + it.X)
      .Sum()
      .Should().Be(expected);
  }

  [Theory]
  [InlineData("Day15.Sample.2", 9021)]
  [InlineData("Day15", 1452348)]
  public void Part2(string file, long expected)
  {
    var world = FormatInput(AoCLoader.LoadFile(file));
    world = world with {Grid = Expand(world.Grid)};
    var current = world.Grid.Where(kv => kv.Value == Robot).Single().Key;
    world.Grid[current] = Empty;

    foreach(var v in world.Instructions) {
      var next = current + v;
      if (TryPushBig(world.Grid, next, v, out var temp)) {
        world = world with {Grid = temp};
        current = next;
      }
      // Print(world.Grid, current);
    }
    // Print(world.Grid, current);
    world.Grid.Where(kv => kv.Value == BigBoxLeft)
      .Select(it => it.Key)
      .Select(it => it.Y * 100 + it.X)
      .Sum()
      .Should().Be(expected);
  }

  private Dictionary<Point, char> Expand(Dictionary<Point, char> grid)
  {
    var result = new Dictionary<Point, char>();
    foreach(var (key, value) in grid)
    {
      if (value == Wall) {
        result[new(key.Y, key.X * 2)] = Wall;
        result[new(key.Y, key.X * 2 + 1)] = Wall;
      }
      else if (value == SmallBox) {
        result[new(key.Y, key.X * 2)] = BigBoxLeft;
        result[new(key.Y, key.X * 2 + 1)] = BigBoxRight;
      }
      else if (value == Empty) {
        result[new(key.Y, key.X * 2)] = Empty;
        result[new(key.Y, key.X * 2 + 1)] = Empty;
      }
      else if (value == Robot) {
        result[new(key.Y, key.X * 2)] = Robot;
        result[new(key.Y, key.X * 2 + 1)] = Empty;
      }
      else throw new ApplicationException();
    }
    return result;
  }

  public bool TryPush(Dictionary<Point, char> grid, Point p, Vector v) {
    var c = grid[p];
    if (c == Wall) return false;
    if (c == Empty) return true;
    if (c == SmallBox) {
      if (TryPush(grid, p + v, v)) {
        grid[p+v] = c;
        grid[p] = Empty;
        return true;
      }
      return false;
    }
    throw new ApplicationException();
  }

  public bool TryPushBig(Dictionary<Point, char> grid, Point p, Vector v, out Dictionary<Point, char> result) {
    var c = grid[p];
    result = grid;
    if (c == Wall) return false;
    if (c == Empty) return true;
    if (c == BigBoxLeft || c == BigBoxRight) {
      if (v == Vector.East || v == Vector.West) {
        // same as before
        if (TryPushBig(grid, p + v, v, out var temp)) {
          result = temp;
          result[p+v] = c;
          result[p] = Empty;
          return true;
        }
        return false;
      }
      else {
        // push both ends of the block
        var p2 = c == BigBoxLeft ? (p + Vector.East) : (p + Vector.West);
        var clone = grid.Clone();
        if (TryPushBig(clone, p + v, v, out var t2) && TryPushBig(t2, p2 + v, v, out var t3)) {
          result = t3;
          t3[p + v] = c;
          t3[p2 + v] = c == BigBoxLeft ? BigBoxRight : BigBoxLeft;
          t3[p] = Empty;
          t3[p2] = Empty;
          return true;
        }
        return false;
      }
    }
    throw new ApplicationException();
  }

  public record World(Dictionary<Point, char> Grid, List<Vector> Instructions);

  private void Print(Dictionary<Point, char> grid, Point current)
  {
    var points = grid.Keys.ToHashSet();
    long minx = points.Select(it => it.X).Min();
    long maxx = points.Select(it => it.X).Max();
    long miny = points.Select(it => it.Y).Min();
    long maxy = points.Select(it => it.Y).Max();

    for (var y = miny; y <= maxy; y++)
    {
      for (var x = minx; x <= maxx; x++)
      {
        if (new Point(y,x) == current) Console.Write(Robot);
        else Console.Write(grid[new(y,x)]);
      }
      Console.WriteLine();
    }
  }

  private static World FormatInput(string input)
  {
    var paragraphs = input.Split("\n\n");
    paragraphs.Should().HaveCount(2);

    var grid = paragraphs[0].Split("\n").ToList().Gridify();
    var instructions = paragraphs[1].Split("\n").Join().Select(it => it switch {
      '^' => Vector.North,
      '>' => Vector.East,
      'v' => Vector.South,
      '<' => Vector.West,
      _ => throw new ApplicationException()
    })
    .ToList();
    return new(grid, instructions);
  }
}
