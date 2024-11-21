using System;

namespace Parser;

public class WhereParser<T> : IParser<T>
{
  private readonly IParser<T> other;
  private readonly Func<T, bool> selector;

  public WhereParser(IParser<T> other, Func<T, bool> selector)
  {
    this.other = other;
    this.selector = selector;
  }

  public IParseResult<T> Parse(char[] input, int position)
  {
    var result = other.Parse(input, position);
    if (result is ParseSuccess<T> r && selector(r.Value))
    {
      return result;
    }
    return new ParseFailure<T>();
  }
}
