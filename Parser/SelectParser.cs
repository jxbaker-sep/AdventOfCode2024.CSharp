using System;

namespace Parser;

public class SelectParser<TIn, TOut> : IParser<TOut>
{
  private readonly IParser<TIn> other;
  private readonly Func<TIn, TOut> selector;

  public SelectParser(IParser<TIn> other, Func<TIn, TOut> selector)
  {
    this.other = other;
    this.selector = selector;
  }

  public IParseResult<TOut> Parse(char[] input, int position)
  {
    var result = other.Parse(input, position);
    if (result is ParseSuccess<TIn> r)
    {
      return new ParseSuccess<TOut>(selector(r.Value), r.PositionAfter);
    }
    return new ParseFailure<TOut>();
  }
}
