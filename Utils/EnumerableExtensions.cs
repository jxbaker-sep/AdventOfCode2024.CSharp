using System.Collections.Generic;

namespace Utils;

public static class EnumerableExtensions
{
    public static string Join<T>(this IEnumerable<T> objects, string joiner = "") => string.Join(joiner, objects);

    public static IEnumerable<List<T>> Windows<T>(this IEnumerable<T> self, int size)
    {
        var q = new Queue<T>();
        foreach(var item in self)
        {
            q.Enqueue(item);
            if (q.Count > size) q.Dequeue();
            if (q.Count == size) yield return q.ToList();
        }
    }

    public static long Product(this IEnumerable<long> self) => self.Aggregate((a,b) => a * b);
}