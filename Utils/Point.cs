

namespace AdventOfCode2024.CSharp.Utils;


public record Point(long Y, long X) {
  public static Point North {get;} = new(-1, 0);
  public static Point East {get;} = new(0, 1);
  public static Point South = new(1, 0);
  public static Point West = new(0, -1);

  public static Point NorthEast = North + East;
  public static Point SouthEast = South + East;
  public static Point NorthWest = North + West;
  public static Point SouthWest = South + West;

  public static Point operator+(Point v1, Point v2) => new(v1.Y + v2.Y, v1.X + v2.X);

  internal Point RotateRight() => new(X, -Y);
}