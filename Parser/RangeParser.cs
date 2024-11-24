using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Parser;

public class RangeParser<T>(IParser<T> other, int min = 0, int max = int.MaxValue) : IParser<List<T>>
{
    public ParseResult<List<T>> Parse(char[] input, int position)
    {
        var output = new List<T>();
        while (output.Count < max)
        {
            try {
                var result = other.Parse(input, position);
                position = result.PositionAfter;
                output.Add(result.Value);
            }
            catch(ParseException) {
                break;
            }
        }
        if (output.Count < min) throw new ParseException("error in range parser", input, position);
        return new(output, position);
    }
}