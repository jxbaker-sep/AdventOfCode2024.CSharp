using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Utils;

using P = Parser.ParserBuiltins;

namespace Parser;

public static class ParserExtensions
{
  public static Parser<TOut> Select<TIn, TOut>(this Parser<TIn> parser, Func<TIn, TOut> fct)
  {
    return Parser.From<TOut>((c, i) => {
      var m = parser.Parse(c, i);
      if (m is ParseSuccess<TIn> s) return ParseResult.From(fct(s.Value), s.Data, s.Position);
      if (m is ParseFailure<TIn> f) return new ParseFailure<TOut>(f.Message, f.Data, i);
      throw new ApplicationException("This shouldnt happen");
    });
  }

  public static Parser<TOut> Then<TIn, TOut>(this Parser<TIn> parser, Func<ParseSuccess<TIn>, IParseResult<TOut>> action)
  {
    return Parser.From((c, i) => {
      var m = parser.Parse(c, i);
      if (m is ParseSuccess<TIn> v) return action(v);
      return new ParseFailure<TOut>((m as ParseFailure<TIn>)!.Message, c, i);
    });
  }

  public static Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> fct, string annotation = "") {
    return Parser.From<T>((c, i) => {
      var result = parser.Parse(c, i);
      if (result is ParseSuccess<T> r && fct(r.Value)) return r;
      if (result is ParseFailure<T> f) return f;
      return new ParseFailure<T>(annotation, c, i);
    });
  }
  public static RangeParser<T> Optional<T>(this Parser<T> parser) => new(parser, 0, 1);
  public static RangeParser<T> Range<T>(this Parser<T> parser, int min, int max) => new(parser, min, max);
  public static RangeParser<T> Star<T>(this Parser<T> parser) => new(parser);

  public static Parser<List<T>> Plus<T>(this Parser<T> parser, string seperator) => parser + parser.After(seperator).Star();
  

  public static Parser<List<T>> Star<T>(this Parser<T> parser, string seperator) => parser.Plus(seperator).Optional()
    .Select(v => v.Count == 0 ? [] : v[0]);

  public static RangeParser<T> Plus<T>(this Parser<T> parser) => new(parser, 1);
  public static T Parse<T>(this Parser<T> parser, string x) {
    var m = parser.Parse(x.ToCharArray(), 0);
    if (m is ParseSuccess<T> r) return r.Value;
    throw new ApplicationException((m as ParseFailure<T>)!.Message);
  }
    
  public static T? ParseOrNull<T>(this Parser<T> parser, string x) where T: class {
    var m = parser.Parse(x.ToCharArray(), 0);
    if (m is ParseSuccess<T> r) return r.Value;
    return null;
  }

  public static T? ParseOrNullStruct<T>(this Parser<T> parser, string x) where T: struct {
    var m = parser.Parse(x.ToCharArray(), 0);
    if (m is ParseSuccess<T> r) return r.Value;
    return null;
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
    return p1.Then<T,T>(v => {
        var peeked = p2.Parse(v.Data, v.Position);
        if (peeked is ParseSuccess<T2>) return v;
        return new ParseFailure<T>((peeked as ParseFailure<T2>)!.Annotation, v.Data, v.Position);
      });
  }

  public static Parser<T> PeekNot<T>(this Parser<T> p1, string p2) => p1.PeekNot(P.String(p2));


  public static Parser<T> PeekNot<T, T2>(this Parser<T> p1, Parser<T2> p2)
  {
    return p1.Then<T, T>(v => {
        var peeked = p2.Parse(v.Data, v.Position);
        if (peeked is ParseFailure<T2>) return v;
        return new ParseFailure<T>("Negative peek failed", v.Data, v.Position);
      });
  }

  public static Parser<P.Unit> Discard<T>(this Parser<T> p) => p.Select(_ => new P.Unit());
}
