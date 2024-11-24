using System;

namespace Parser;

public class DeferredParser<T> : IParser<T>
{
    public IParser<T>? Actual {get;set;}
    public ParseResult<T> Parse(char[] input, int position)
    {
        if (Actual == null) throw new ApplicationException();
        return Actual.Parse(input, position);
    }
}