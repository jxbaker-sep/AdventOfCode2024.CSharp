using System.Collections.Generic;

namespace Parser;

public class RangeParser<T>(Parser<T> other, int min = 0, int max = int.MaxValue) : Parser<List<T>>
{
    override public ParseResult<List<T>> Parse(char[] input, int position)
    {
        var output = new List<T>();
        ParseException? e = null;
        while (output.Count < max)
        {
            try {
                var result = other.Parse(input, position);
                position = result.PositionAfter;
                output.Add(result.Value);
            }
            catch(ParseException e2) {
                e = e2;
                break;
            }
        }
        if (output.Count < min) throw new ParseException($"error in range parser: {e?.Message}", input, position);
        return new(output, position);
    }
}