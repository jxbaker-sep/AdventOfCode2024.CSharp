using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Utils;

namespace AdventOfCode2024.CSharp.Day21;

public class Day21
{
  [Theory]
  [InlineData("Day21.Sample", 2, 126384)]
  [InlineData("Day21", 2, 157892)]
  [InlineData("Day21", 25, 197015606336332L)]
  public void Part1(string file, int numberOfDpadRobots, long expected)
  {
    var codes = AoCLoader.LoadLines(file);
    codes.Sum(code =>
      {
        var min = CommandRobots(code, numberOfDpadRobots);
        var n = Convert.ToInt64(code[..^1]);
        return n * min;
      }
    ).Should().Be(expected);
  }

  public long CommandRobots(string code, int numberOfDpadRobots) {
    code = "A" + code;
    long total = 0;
    for(var i = 0; i < code.Length - 1; i++) {
      total += NumericKeypadRoutes(code[i], code[i+1]).Select(route => CommandDpadRobots(route, numberOfDpadRobots)).Min();
    }
    return total;
  }

  private readonly Dictionary<(string, int), long> CodeCache = [];

  public long CommandDpadRobots(string code, int numberOfDpadRobots)
  {
    if (numberOfDpadRobots == 0) return code.Length;
    code = $"A{code}";
    if (CodeCache.TryGetValue((code, numberOfDpadRobots), out var cached)) {return cached;}
    long total = 0;
    for(var i = 0; i < code.Length - 1; i++) {
      total += DPadRoutes(code[i], code[i+1]).Select(route => CommandDpadRobots(route, numberOfDpadRobots - 1)).Min();
    }

    CodeCache[(code, numberOfDpadRobots)] = total;
    return total;
  }

  readonly IReadOnlyDictionary<char, Point> NumericKeypad = new Dictionary<char, Point>{
      { '7', new(0, 0) }, { '8', new(0, 1) }, { '9', new(0, 2) },
      { '4', new(1, 0) }, { '5', new(1, 1) }, { '6', new(1, 2) },
      { '1', new(2, 0) }, { '2', new(2, 1) }, { '3', new(2, 2) },
                          { '0', new(3, 1) }, { 'A', new(3, 2) },
    };

  readonly Dictionary<(char, char), List<string>> NumericKeypadRoutesCache = [];
  public List<string> NumericKeypadRoutes(char start, char goal)
  {
    if (NumericKeypadRoutesCache.TryGetValue((start, goal), out var routes)) return routes;

    var result = FindRoute(start, goal, NumericKeypad).ToList();

    NumericKeypadRoutesCache[(start, goal)] = result;
    return result;
  }

  readonly IReadOnlyDictionary<char, Point> DPad = new Dictionary<char, Point>{
                          { '^', new(0, 1) }, { 'A', new(0, 2) },
      { '<', new(1, 0) }, { 'v', new(1, 1) }, { '>', new(1, 2) },
    };

  readonly Dictionary<(char, char), List<string>> DPadRouteCache = [];
  public List<string> DPadRoutes(char start, char goal)
  {
    if (DPadRouteCache.TryGetValue((start, goal), out var routes)) return routes;
    var result = FindRoute(start, goal, DPad).ToList();

    DPadRouteCache[(start, goal)] = result;
    return result;
  }

  private static IEnumerable<string> FindRoute(char start, char goal, IReadOnlyDictionary<char, Point> keypad)
  {
    if (start == goal)
    {
      yield return "A";
      yield break;
    }
    var p1 = keypad[start];
    var p2 = keypad[goal];

    var lefts = p1.X < p2.X
      ? (Vector.East, (int)(p2.X - p1.X), ">")
      : (Vector.West, (int)(p1.X - p2.X), "<");

    var downs = p1.Y < p2.Y
      ? (Vector.South, (int)(p2.Y - p1.Y), "v")
      : (Vector.North, (int)(p1.Y - p2.Y), "^");

    if (lefts.Item2 > 0 && keypad.Values.Contains(p1 + lefts.Item1 * lefts.Item2))
    {
      yield return Enumerable.Repeat(lefts.Item3, lefts.Item2).Join() +
                   Enumerable.Repeat(downs.Item3, downs.Item2).Join() + "A";
    }
    if (downs.Item2 > 0 && keypad.Values.Contains(p1 + downs.Item1 * downs.Item2))
    {
      yield return Enumerable.Repeat(downs.Item3, downs.Item2).Join() +
                   Enumerable.Repeat(lefts.Item3, lefts.Item2).Join() +
                   "A";
    }
  }
}
