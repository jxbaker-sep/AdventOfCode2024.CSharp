using System.Runtime.Serialization;
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Microsoft.VisualBasic;

namespace AdventOfCode2024.CSharp.Day21;

public class Day21
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
        .SelectMany(r0 => DPadRobot('A', r0))
        .Select(r1 => r1.Aggregate(('A', 0L), (a, b) => (b, a.Item2 + Count(DPad, a.Item1, b))).Item2)
        // .SelectMany(r1 => DPadRobot('A', r1))
        // .Select(it => it.Length)
        .Min(); 
        var n = Convert.ToInt64(code[..^1]);
        Console.WriteLine($"{code}: {min} {n}");
        return n * min;
      }
    ).Should().Be(expected);
  }

  // [Fact]
  // public void Part1SanityCheck()
  // {
  //   NumericKeypadRoutes('A', '0').Should().BeEquivalentTo(new[]{"<A"});
  //   NumericKeypadRoutes('9', '1').Should().BeEquivalentTo(new[]{"<<vvA", "<v<vA", "<vv<A", "v<<vA", "v<v<A", "vv<<A"});
  //   NumericKeypadRoutes('0', '8').Should().BeEquivalentTo(new[]{"^^^A"});
  //   NumericKeypadRoutes('8', '0').Should().BeEquivalentTo(new[]{"vvvA"});
  //   NumericKeypadRoutes('5', '1').Should().BeEquivalentTo(new[]{"v<A", "<vA"});
  //   NumericKeypadRoutes('1', '5').Should().BeEquivalentTo(new[]{"^>A", ">^A"});

  //   NumericKeypadRobot('A', "029A").Should().Contain(new[]{"<A^A>^^AvvvA", "<A^A^>^AvvvA", "<A^A^^>AvvvA"});

  //   NumericKeypadRobot('A', "029A").SelectMany(r0 => DPadRobot('A', r0)).Should().Contain("v<<A>>^A<A>AvA<^AA>A<vAAA>^A");

  //   // Robot0('A', "029A")
  //   //   .SelectMany(r0 => DirectionKeypadRobot('A', r0))
  //   //   .SelectMany(r1 => DirectionKeypadRobot('A', r1))
  //   //   .Count().Should().Be(0);
  //     // .Should().Contain("v<<A>>^A<A>AvA<^AA>A<vAAA>^A");


  //   "029A".ToCharArray()
  //     .Aggregate(('A', 0L), (a, b) => (b, a.Item2 + Count(NumericKeypad, a.Item1, b)))
  //     .Item2.Should().Be("<A^A>^^AvvvA".Length);
  // }

  public long Count(IReadOnlyDictionary<char, Point> pad, char start, char goal) {
    return pad[start].ManhattanDistance(pad[goal]) + 1; // +1 to press "A"
  }

  Dictionary<(char, string), List<string>> NumericKeypadRobotCache = [];
  public IEnumerable<string> NumericKeypadRobot(char current, string input) {
    if (input == "") return [""];
    if (input.Length == 1) return NumericKeypadRoutes(current, input[0]);
    if (NumericKeypadRobotCache.TryGetValue((current, input), out var cached)) return cached;
    List<string> routes = [];
    var paths = NumericKeypadRoutes(current, input[0]);
    foreach(var sub in NumericKeypadRobot(input[0], input[1..])) {
      routes.AddRange(paths.Select(p => p + sub));
    }

    NumericKeypadRobotCache[(current, input)] = routes;
    return routes;
  }

  Dictionary<(char, string), List<string>> DPadRobotCache = [];
  public IEnumerable<string> DPadRobot(char current, string input) {
    if (input == "") return [""];
    if (input.Length == 1) return DPadRoutes(current, input[0]);
    if (DPadRobotCache.TryGetValue((current, input), out var cached)) return cached;
    List<string> routes = [];
    var paths = DPadRoutes(current, input[0]);
    foreach(var sub in DPadRobot(input[0], input[1..])) {
      routes.AddRange(paths.Select(p => p + sub));
    }
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
    
    List<string> result = Djikstra(start, goal, NumericKeypad);

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
    List<string> result = Djikstra(start, goal, DPad);

    DPadRouteCache[(start, goal)] = result;
    return result;
  }

  private static List<string> Djikstra(char start, char goal, IReadOnlyDictionary<char, Point> keypad)
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
      if (current.Point.X > p2.X && keypad.Values.Contains(current.Point + Vector.West)) open.Enqueue((current.Point + Vector.West, current.Path + "<"));
      if (current.Point.Y < p2.Y && keypad.Values.Contains(current.Point + Vector.South)) open.Enqueue((current.Point + Vector.South, current.Path + "v"));
      if (current.Point.Y > p2.Y && keypad.Values.Contains(current.Point + Vector.North)) open.Enqueue((current.Point + Vector.North, current.Path + "^"));
    }

    return result;
  }
}