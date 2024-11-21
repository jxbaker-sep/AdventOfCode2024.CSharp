using System;

namespace Parser;

public class WhereParser<T> : Parser<T>
{
  private readonly Parser<T> other;
  private readonly Func<T, bool> selector;

  public WhereParser(Parser<T> other, Func<T, bool> selector)
  {
    this.other = other;
    this.selector = selector;
  }

  override public IParseResult<T> Parse(char[] input, int position)
  {
    var result = other.Parse(input, position);
    if (result is ParseSuccess<T> r && selector(r.Value))
    {
      return result;
    }
    return new ParseFailure<T>();
  }
}
