using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Parser;


public static class ParserBuiltins
{
  public static readonly Parser<char> Any = new AnyParser();
  public static readonly Parser<char> Letter = Any.Where(char.IsLetter, "IsLetter");
  public static readonly Parser<char> Numeral = Any.Where(char.IsNumber, "IsNumber");
  public static readonly Parser<int> Digit = Numeral.Select(it => it - '0');
  public static readonly Parser<long> Number = Numeral.Plus().Join().Select(Convert.ToInt64);

  public static readonly Parser<char> Whitespace = Any.Where(it => char.IsWhiteSpace(it), "IsWhiteSpace"); 

  public static DeferredParser<T> Defer<T>() => new();

  public record Unit {}

  public static readonly Parser<Unit> EndOfInput = Parser.From((c, i) => {
    if (i == c.Length) return ParseResult.From(new Unit(), i);
    throw new ParseException("Expected end-of-input", c, i);
  });

  public static readonly Parser<Unit> EndOfLine = EndOfInput | String("\n").Discard();

  public static Parser<string> String(string s)
  {
    return Parser.From((c, i) => {
      if (c.Length >= i + s.Length && c[i..(i+s.Length)].Join() == s) return new ParseResult<string>(s, i + s.Length);
      throw new ParseException($"expected string {s}", c, i);
    });
  }

  public static Parser<(T1 First, T2 Second)> Sequence<T1, T2>(Parser<T1> p1, Parser<T2> p2)
  {
    return Parser.From((c, i) =>
    {
      var r1 = p1.Parse(c, i);
      var r2 = p2.Parse(c, r1.PositionAfter);
      return new ParseResult<(T1 First, T2 Second)>((r1.Value, r2.Value), r2.PositionAfter);
    });
  }

  public static Parser<(T1 First, T2 Second, T3 Third)> Sequence<T1, T2, T3>(Parser<T1> p1, Parser<T2> p2, Parser<T3> p3)
  {
    return Parser.From((c, i) =>
    {
      var r1 = p1.Parse(c, i);
      var r2 = p2.Parse(c, r1.PositionAfter);
      var r3 = p3.Parse(c, r2.PositionAfter);
      return ParseResult.From((r1.Value, r2.Value, r3.Value), r3.PositionAfter);
    });
  }

  // public static Parser<(List<T1>, List<T2>)> Choice<T1, T2>(Parser<T1> p1, Parser<T2> p2)
  // {
  //   return Parser.From((c,i) => {
  //     try {
  //       var r1 = p1.Parse(c, i);
  //       return new ParseResult<(List<T1>, List<T2>)>(([r1.Value], []), r1.PositionAfter);
  //     }
  //     catch {
  //       var r2 = p2.Parse(c, i);
  //       return new ParseResult<(List<T1>, List<T2>)>(([], [r2.Value]), r2.PositionAfter);
  //     }
  //     throw new ParseException("Choice parser failed", c, i);
  //   });
  // }
}
