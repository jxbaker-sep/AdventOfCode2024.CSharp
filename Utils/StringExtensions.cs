using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2024.CSharp.Utils;

public static class StringExtensions
{
    public static List<string> Lines(this string s) => s.Split('\n').Select(it => it.Trim()).ToList();
}

public static class IEnumerableExtensions
{
    public static long Product(this IEnumerable<long> self) => self.Aggregate((a,b) => a * b);
}