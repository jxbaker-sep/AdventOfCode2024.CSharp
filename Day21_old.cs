using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Utils;

namespace AdventOfCode2024.CSharp.Day21_old;

public class Day21_old
{
  [Theory]
  [InlineData("Day21.Sample", 126384)]
  [InlineData("Day21", 157892)]
  public void Part1(string file, long expected)
  {
    var codes = AoCLoader.LoadLines(file);
    codes.Sum(code =>
      {
        var min = NumericKeypadRobot('A', code)
        .Select(r0 =>
        {
          var r1 = DPadRobot('A', r0);
          var min = r1.Select(it => it.Length).Min();
          return r1.First(it => it.Length == min);
        })
        .SelectMany(r1 => DPadRobot('A', r1))
        .Select(it => it.Length)
        .Min();
        var n = Convert.ToInt64(code[..^1]);
        return n * min;
      }
    ).Should().Be(expected);
  }

  [Theory]
  // [InlineData("Day21.Sample", 126384)]
  [InlineData("Day21", 0)]
  public void Part2(string file, long expected)
  {
    var codes = AoCLoader.LoadLines(file);
    codes.Sum(code =>
      {
        var nkr = NumericKeypadRobot('A', code).ToList();
        for (var i = 0; i < 25; i++)
        {
          Console.WriteLine($"{i}, {nkr.First()}, {nkr.First().Length}");
          nkr = nkr.Select(r0 =>
          {
            var r1 = DPadRobot('A', r0);
            var min = r1.Select(it => it.Length).Min();
            return r1.First(it => it.Length == min);
          }).ToList();
        }
        var n = Convert.ToInt64(code[..^1]);
        return n * nkr.Select(it => it.Length).Min();
      }
    ).Should().Be(expected);
}

[Fact]
public void Sanity()
{
  NumericKeypadRoutes('A', '0').Should().BeEquivalentTo(new[] { "<A" });
  NumericKeypadRoutes('9', '1').Should().BeEquivalentTo(new[] { "<<vvA", "<v<vA", "<vv<A", "v<<vA", "v<v<A", "vv<<A" });
  NumericKeypadRoutes('0', '8').Should().BeEquivalentTo(new[] { "^^^A" });
  NumericKeypadRoutes('8', '0').Should().BeEquivalentTo(new[] { "vvvA" });
  NumericKeypadRoutes('5', '1').Should().BeEquivalentTo(new[] { "v<A", "<vA" });
  NumericKeypadRoutes('1', '5').Should().BeEquivalentTo(new[] { "^>A", ">^A" });

  NumericKeypadRobot('A', "029A").Should().Contain(new[] { "<A^A>^^AvvvA", "<A^A^>^AvvvA", "<A^A^^>AvvvA" });

  NumericKeypadRobot('A', "029A").SelectMany(r0 => DPadRobot('A', r0)).Should().Contain("v<<A>>^A<A>AvA<^AA>A<vAAA>^A");

  var temp = NumericKeypadRobot('A', "029A")
    .SelectMany(r0 => DPadRobot('A', r0))
    .GroupBy(it => it.Length)
    .ToDictionary(it => it.Key, it => it.ToList());


  "029A".ToCharArray()
    .Aggregate(('A', 0L), (a, b) => (b, a.Item2 + Count(NumericKeypad, a.Item1, b)))
    .Item2.Should().Be("<A^A>^^AvvvA".Length);
}

public long Count(IReadOnlyDictionary<char, Point> pad, char start, char goal)
{
  return pad[start].ManhattanDistance(pad[goal]) + 1; // +1 to press "A"
}

Dictionary<(char, string), List<string>> NumericKeypadRobotCache = [];
public IEnumerable<string> NumericKeypadRobot(char current, string input)
{
  if (input == "") return [""];
  if (input.Length == 1) return NumericKeypadRoutes(current, input[0]);
  if (NumericKeypadRobotCache.TryGetValue((current, input), out var cached)) return cached;
  List<string> routes = [];
  var paths = NumericKeypadRoutes(current, input[0]);
  foreach (var sub in NumericKeypadRobot(input[0], input[1..]))
  {
    routes.AddRange(paths.Select(p => p + sub));
  }

  routes = routes.Distinct().ToList(); // WHY???
  NumericKeypadRobotCache[(current, input)] = routes;
  return routes;
}

Dictionary<(char, string), List<string>> DPadRobotCache = [];
public IEnumerable<string> DPadRobot(char current, string input)
{
  if (input == "") return [""];
  if (input.Length == 1) return DPadRoutes(current, input[0]);
  if (DPadRobotCache.TryGetValue((current, input), out var cached)) return cached;
  List<string> routes = [];
  for(var i = 1; i < input.Length - 1; i++) {
    var input2 = input[..^i];
    if (DPadRobotCache.TryGetValue((current, input2), out var cached2)) {
      foreach (var sub in DPadRobot(input2[^1], input[input2.Length..]))
      {
        routes.AddRange(cached2.Select(p => p + sub));
      }
      routes = routes.Distinct().ToList(); // WHY????
      DPadRobotCache[(current, input)] = routes;
      return routes;
    }
  }
  var paths = DPadRoutes(current, input[0]);
  foreach (var sub in DPadRobot(input[0], input[1..]))
  {
    routes.AddRange(paths.Select(p => p + sub));
  }
  routes = routes.Distinct().ToList(); // WHY????
  DPadRobotCache[(current, input)] = routes;
  return routes;
}

readonly IReadOnlyDictionary<char, Point> NumericKeypad = new Dictionary<char, Point>{
      { '7', new(0, 0) }, { '8', new(0, 1) }, { '9', new(0, 2) },
      { '4', new(1, 0) }, { '5', new(1, 1) }, { '6', new(1, 2) },
      { '1', new(2, 0) }, { '2', new(2, 1) }, { '3', new(2, 2) },
                          { '0', new(3, 1) }, { 'A', new(3, 2) },
    };

Dictionary<(char, char), List<string>> NumericKeypadRoutesCache = [];
public List<string> NumericKeypadRoutes(char start, char goal)
{
  if (NumericKeypadRoutesCache.TryGetValue((start, goal), out var routes)) return routes;

  List<string> result = Djikstra(start, goal, NumericKeypad).ToList();

  NumericKeypadRoutesCache[(start, goal)] = result;
  return result;
}

readonly IReadOnlyDictionary<char, Point> DPad = new Dictionary<char, Point>{
                          { '^', new(0, 1) }, { 'A', new(0, 2) },
      { '<', new(1, 0) }, { 'v', new(1, 1) }, { '>', new(1, 2) },
    };

Dictionary<(char, char), List<string>> DPadRouteCache = [];
public List<string> DPadRoutes(char start, char goal)
{
  if (DPadRouteCache.TryGetValue((start, goal), out var routes)) return routes;
  List<string> result = Djikstra(start, goal, DPad).ToList();

  DPadRouteCache[(start, goal)] = result;
  return result;
}

private static List<string> Djikstra_old(char start, char goal, IReadOnlyDictionary<char, Point> keypad)
{
  var p1 = keypad[start];
  var p2 = keypad[goal];
  var result = new List<string>();
  var open = new Queue<(Point Point, string Path)>([(p1, "")]);
  while (open.TryDequeue(out var current))
  {
    if (current.Point == p2)
    {
      result.Add(current.Path + "A");
      continue;
    }
    if (current.Point.X < p2.X && keypad.Values.Contains(current.Point + Vector.East)) open.Enqueue((current.Point + Vector.East, current.Path + ">"));
    else if (current.Point.X > p2.X && keypad.Values.Contains(current.Point + Vector.West)) open.Enqueue((current.Point + Vector.West, current.Path + "<"));
    if (current.Point.Y < p2.Y && keypad.Values.Contains(current.Point + Vector.South)) open.Enqueue((current.Point + Vector.South, current.Path + "v"));
    else if (current.Point.Y > p2.Y && keypad.Values.Contains(current.Point + Vector.North)) open.Enqueue((current.Point + Vector.North, current.Path + "^"));
  }

  return result;
}

private static IEnumerable<string> Djikstra(char start, char goal, IReadOnlyDictionary<char, Point> keypad)
{
  if (start == goal) {
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
