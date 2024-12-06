

namespace AdventOfCode2024.CSharp.Utils;


public record Point(long Y, long X) {

  public static Point operator+(Point v1, Vector v2) => new(v1.Y + v2.Y, v1.X + v2.X);
}