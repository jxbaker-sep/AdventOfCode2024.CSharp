using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Parser;

public class RangeParser<T>(IParser<T> other, int min = 0, int max = int.MaxValue) : IParser<List<T>>
{
    public IParseResult<List<T>> Parse(char[] input, int position)
    {
        var output = new List<T>();
        var result = other.Parse(input, position);
        while (output.Count < max && result is ParseSuccess<T> r)
        {
            output.Add(r.Value);
            position = r.PositionAfter;
            Console.WriteLine(position);
            result = other.Parse(input, position);
        }
        if (output.Count < min) return new ParseFailure<List<T>>();
        return new ParseSuccess<List<T>>(output, position);
    }
}