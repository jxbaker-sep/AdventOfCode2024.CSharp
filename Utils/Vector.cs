

namespace AdventOfCode2024.CSharp.Utils;


public record Vector(long Y, long X) {
  public static Vector North {get;} = new(-1, 0);
  public static Vector East {get;} = new(0, 1);
  public static Vector South = new(1, 0);
  public static Vector West = new(0, -1);

  public static Vector NorthEast = North + East;
  public static Vector SouthEast = South + East;
  public static Vector NorthWest = North + West;
  public static Vector SouthWest = South + West;

  public static Vector operator+(Vector v1, Vector v2) => new(v1.Y + v2.Y, v1.X + v2.X);

  internal Vector RotateRight() => new(X, -Y);
}