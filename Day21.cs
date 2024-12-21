using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;

namespace AdventOfCode2024.CSharp.Day21;

public class Day21
{
  const char Left = '<';
  const char Right = '>';
  const char Up = '^';
  const char Down = 'v';
  const char Activate = 'A';

  [Theory]
  [InlineData("Day21.Sample", 126384)]
  // [InlineData("Day21", 157892)]
  public void Part1(string file, long expected)
  {
    var codes = AoCLoader.LoadLines(file);
    codes.Select(code => {
      var dpad = CommandRobots(code, 2);
      var scale = dpad.Scale;
      var n = Convert.ToInt64(code[..^1]);
      Console.WriteLine($"{dpad},{scale},{n}");
      return scale * n;
    }).Sum().Should().Be(expected);
  }

  public static DPadInput CommandRobots(string code, int numberOfDpadRobots)
  {
    if (numberOfDpadRobots == 0) {
      return CommandNumericKeypadRobot(code);
    }
    return CommandDPadRobot(CommandRobots(code, numberOfDpadRobots - 1));
  }

  [Fact]
  public void Sanity()
  {
    CommandNumericKeypadRobot("0").Should().Be(DPadInput.MakeLeft(1) + DPadInput.MakeActivate(1));
    CommandNumericKeypadRobot("02").Should().Be(DPadInput.MakeLeft(1) + DPadInput.MakeUp(1) + DPadInput.MakeActivate(1) * 2);
    CommandNumericKeypadRobot("029").Should().Be(DPadInput.MakeLeft(1) + DPadInput.MakeRight(1) + DPadInput.MakeUp(3) + DPadInput.MakeActivate(1) * 3);
    CommandNumericKeypadRobot("029A").Should().Be(DPadInput.MakeLeft(1) + DPadInput.MakeRight(1) + DPadInput.MakeUp(3) + DPadInput.MakeDown(3) + DPadInput.MakeActivate(1) * 4);
  
    CommandRobots("029A", 0).Should().Be(new DPadInput(1, 1, 3, 3, 4));

    CommandRobots("029A", 1).Should().Be(new DPadInput(5, 5, 3, 3, 12));

  }

  public record DPadInput(long Left, long Right, long Up, long Down, long Activate) {

    public static DPadInput MakeLeft(long l) => new(l, 0, 0, 0, 0);
    public static DPadInput MakeRight(long l) => new(0, l, 0, 0, 0);
    public static DPadInput MakeUp(long l) => new(0, 0, l, 0, 0);
    public static DPadInput MakeDown(long l) => new(0, 0, 0, l, 0);
    public static DPadInput MakeActivate(long l) => new(0, 0, 0, 0, l);

    public static DPadInput operator+(DPadInput lhs, DPadInput rhs) => new(lhs.Left + rhs.Left, lhs.Right + rhs.Right, lhs.Up + rhs.Up, lhs.Down + rhs.Down, lhs.Activate + rhs.Activate);
  
    public static DPadInput operator*(DPadInput lhs, long rhs) => new(lhs.Left * rhs, lhs.Right * rhs, lhs.Up * rhs, lhs.Down * rhs, lhs.Activate * rhs);

    public long Scale => Left + Right + Up + Down + Activate;
  }

  private static DPadInput CommandDPadRobot(DPadInput dpad) {
    return CommandDPadRobot(Left, dpad.Left) + 
           CommandDPadRobot(Right, dpad.Right) + 
           CommandDPadRobot(Up, dpad.Up) + 
           CommandDPadRobot(Down, dpad.Down) +
           CommandDPadRobot(Activate, dpad.Activate);
  } 

  private static DPadInput CommandDPadRobot(char button, long scale) 
  {
    // Inputs: 1 1 3 3 4
    // v<<A >>^A <A >A vA <^A A >A <vA A A >^A
    if (scale == 0) return new DPadInput(0,0,0,0,0);
    return button switch{
      Activate => DPadInput.MakeActivate(scale),
      Right => DPadInput.MakeDown(1) + DPadInput.MakeActivate(scale),
      Up => DPadInput.MakeLeft(1) + DPadInput.MakeActivate(scale),
      Down => DPadInput.MakeLeft(1) + DPadInput.MakeDown(1) + DPadInput.MakeActivate(scale),
      Left => DPadInput.MakeLeft(2) + DPadInput.MakeDown(1) + DPadInput.MakeActivate(scale),
      _ => throw new ApplicationException()
    };
  }

  private static DPadInput CommandNumericKeypadRobot(string code) {
    return code.Aggregate((new DPadInput(0,0,0,0,0), Activate), (a, b) => (a.Item1 + CommandNumericKeypadRobot(a.Item2, b), b)).Item1;
  }

  private static DPadInput CommandNumericKeypadRobot(char start, char goal)
  {
    if (start == goal)
    {
      return DPadInput.MakeActivate(1);
    }
    var keypad = NumericKeypad;
    var p1 = keypad[start];
    var p2 = keypad[goal];

    var horizontal = p1.X < p2.X
      ? DPadInput.MakeRight(p2.X - p1.X)
      : DPadInput.MakeLeft(p1.X - p2.X);

    var vertical = p1.Y < p2.Y
      ? DPadInput.MakeDown(p2.Y - p1.Y)
      : DPadInput.MakeUp(p1.Y - p2.Y);

    return horizontal + vertical + DPadInput.MakeActivate(1);
  }

  readonly static IReadOnlyDictionary<char, Point> NumericKeypad = new Dictionary<char, Point>{
      { '7', new(0, 0) }, { '8', new(0, 1) }, { '9', new(0, 2) },
      { '4', new(1, 0) }, { '5', new(1, 1) }, { '6', new(1, 2) },
      { '1', new(2, 0) }, { '2', new(2, 1) }, { '3', new(2, 2) },
                          { '0', new(3, 1) }, { 'A', new(3, 2) },
    };
}
