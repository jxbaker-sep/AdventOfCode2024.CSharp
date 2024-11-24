using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

using P = Parser.ParserBuiltins;

namespace Parser;

public static class ParserExtensions
{
  public static IParser<TOut> Select<TIn, TOut>(this IParser<TIn> parser, Func<TIn, TOut> fct)
  {
    return Parser.From((c, i) => {
      var r = parser.Parse(c, i);
      return new ParseResult<TOut>(fct(r.Value), r.PositionAfter);
    });
  }
  public static IParser<T> Where<T>(this IParser<T> parser, Func<T, bool> fct) {
    return Parser.From((c, i) => {
      var result = parser.Parse(c, i);
      if (result is {} r && fct(r.Value)) return r;
      throw new ParseException("error in where parser", c, i);
    });
  }
  public static RangeParser<T> Optional<T>(this IParser<T> parser) => new(parser, 0, 1);
  public static RangeParser<T> Range<T>(this IParser<T> parser, int min, int max) => new(parser, min, max);
  public static RangeParser<T> Star<T>(this IParser<T> parser) => new(parser);
  public static RangeParser<T> Plus<T>(this IParser<T> parser) => new(parser, 1);
  public static T Parse<T>(this IParser<T> parser, string x) => parser.Parse(x.ToCharArray(), 0).Value;
  public static IParser<string> Join<T>(this IParser<List<T>> parser) => parser.Select(it => it.Join());

  public static IParser<T> End<T>(this IParser<T> parser) => Parser.From((c, i) => {
    var r = parser.Parse(c, i);
    if (r.PositionAfter == c.Length) return r;
    throw new ParseException("Expected end-of-input", c, r.PositionAfter);
  });

  public static IParser<T> Before<T, T2>(this IParser<T> parser, IParser<T2> other)
  {
    return P.Sequence(parser, other).Select(it => it.First);
  }

  public static IParser<T> After<T, T2>(this IParser<T> parser, IParser<T2> other)
  {
    return P.Sequence(other, parser).Select(it => it.Second);
  }

  public static IParser<T> Between<T, T2, T3>(this IParser<T> parser, IParser<T2> other1, IParser<T3> other2)
  {
    return P.Sequence(other1, parser, other2).Select(it => it.Second);
  }

  public static IParser<T> Between<T>(this IParser<T> parser, string other1, string other2)
  {
    return parser.Between(P.String(other1).Trim(), P.String(other2).Trim());
  }

  public static IParser<T> Trim<T>(this IParser<T> parser)
  {
    return parser.Between(P.Whitespace.Star(), P.Whitespace.Star());
  }
}
