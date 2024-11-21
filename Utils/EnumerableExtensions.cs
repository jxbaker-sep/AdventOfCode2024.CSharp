using System.Collections.Generic;

namespace Utils;

public static class EnumerableExtensions
{
    public static string Join<T>(this IEnumerable<T> objects, string joiner = "") => string.Join(joiner, objects);
}