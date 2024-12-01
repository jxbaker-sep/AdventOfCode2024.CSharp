using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

using P = Parser.ParserBuiltins;

namespace Parser;

public static class ParserExtensions
{
  public static Parser<TOut> Select<TIn, TOut>(this Parser<TIn> parser, Func<TIn, TOut> fct)
  {
    return Parser.From((c, i) => {
      var r = parser.Parse(c, i);
      return new ParseResult<TOut>(fct(r.Value), r.PositionAfter);
    });
  }
  public static Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> fct, string annotation = "") {
    return Parser.From((c, i) => {
      var result = parser.Parse(c, i);
      if (result is {} r && fct(r.Value)) return r;
      throw new ParseException($"error in where parser: {annotation}", c, i);
    });
  }
  public static RangeParser<T> Optional<T>(this Parser<T> parser) => new(parser, 0, 1);
  public static RangeParser<T> Range<T>(this Parser<T> parser, int min, int max) => new(parser, min, max);
  public static RangeParser<T> Star<T>(this Parser<T> parser) => new(parser);

  public static Parser<List<T>> Star<T>(this Parser<T> parser, string seperator) => parser.Before(seperator).Star() + parser;

  public static RangeParser<T> Plus<T>(this Parser<T> parser) => new(parser, 1);
  public static T Parse<T>(this Parser<T> parser, string x) => parser.Parse(x.ToCharArray(), 0).Value;
  public static T? ParseOrNull<T>(this Parser<T> parser, string x) where T: class {
    try {
      return parser.Parse(x.ToCharArray(), 0).Value;
    }
     catch(ParseException) {
      return null;
    }
  }
  public static Parser<string> Join<T>(this Parser<List<T>> parser) => parser.Select(it => it.Join());

  public static Parser<T> End<T>(this Parser<T> parser) => parser.Before(P.EndOfInput);

  public static Parser<T> Before<T, T2>(this Parser<T> parser, Parser<T2> other)
  {
    return P.Sequence(parser, other).Select(it => it.First);
  }

  public static Parser<T> Before<T>(this Parser<T> parser, string other) => parser.Before(P.String(other).Trim());
  public static Parser<T> After<T>(this Parser<T> parser, string other) => parser.After(P.String(other).Trim());

  public static Parser<T> After<T, T2>(this Parser<T> parser, Parser<T2> other)
  {
    return P.Sequence(other, parser).Select(it => it.Second);
  }

  public static Parser<T> Between<T, T2, T3>(this Parser<T> parser, Parser<T2> other1, Parser<T3> other2)
  {
    return P.Sequence(other1, parser, other2).Select(it => it.Second);
  }

  public static Parser<T> Between<T>(this Parser<T> parser, string other1, string other2)
  {
    return parser.Between(P.String(other1).Trim(), P.String(other2).Trim());
  }

  public static Parser<T> Trim<T>(this Parser<T> parser)
  {
    return parser.Between(P.Whitespace.Star(), P.Whitespace.Star());
  }

  public static Parser<T> Peek<T>(this Parser<T> p1, string p2) => p1.Peek(P.String(p2).Trim());


  public static Parser<T> Peek<T, T2>(this Parser<T> p1, Parser<T2> p2)
  {
    return Parser.From((c,i) => {
      var r1 = p1.Parse(c,i);
      p2.Parse(c, r1.PositionAfter);
      return r1;
    });
  }

  public static Parser<T> PeekNot<T>(this Parser<T> p1, string p2) => p1.PeekNot(P.String(p2));


  public static Parser<T> PeekNot<T, T2>(this Parser<T> p1, Parser<T2> p2)
  {
    return Parser.From((c,i) => {
      var r1 = p1.Parse(c,i);
      try {
        p2.Parse(c, r1.PositionAfter);
      } catch (ParseException) {
        return r1;
      }
      throw new ParseException("Failed PeekNot assertion", c, r1.PositionAfter);
    });
  }

  public static Parser<P.Unit> Discard<T>(this Parser<T> p) => p.Select(_ => new P.Unit());
}
