

namespace AdventOfCode2024.CSharp.Utils;


public record Point(long Y, long X) {

  public static Point operator+(Point point, Vector vector) => new(point.Y + vector.Y, point.X + vector.X);
  public static Point operator-(Point point, Vector vector) => new(point.Y - vector.Y, point.X - vector.X);
  public Vector VectorTo(Point point2) => new(point2.Y - Y, point2.X - X);

  public long ManhattanDistance(Point other) => Math.Abs(X-other.X) + Math.Abs(Y - other.Y);
}