using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Parser;


public static class ParserBuiltins
{
  public static readonly IParser<char> Any = new AnyParser();
  public static readonly IParser<int> Digit = Any.Where(it => '0' <= it && it <= '9').Select(it => it - '0');
  public static readonly IParser<long> Number = Digit.Plus().Select(it => it.Select(it => (long)it).Aggregate((a, b) => a * 10 + b));

  public static readonly IParser<char> Whitespace = Any.Where(it => new[]{' ','\t','\n'}.Contains(it)); 

  public static IParser<string> String(string s)
  {
    return Parser.From((c, i) => {
      if (c.Length >= i + s.Length && c[i..(i+s.Length)].Join() == s) return new ParseResult<string>(s, i + s.Length);
      throw new ParseException($"expected string {s}", c, i);
    });
  }

  public static IParser<(T1 First, T2 Second)> Sequence<T1, T2>(IParser<T1> p1, IParser<T2> p2)
  {
    return Parser.From((c, i) =>
    {
      var r1 = p1.Parse(c, i);
      var r2 = p2.Parse(c, r1.PositionAfter);
      return new ParseResult<(T1 First, T2 Second)>((r1.Value, r2.Value), r2.PositionAfter);
    });
  }

  public static IParser<(T1 First, T2 Second, T2 Third)> Sequence<T1, T2, T3>(IParser<T1> p1, IParser<T2> p2, IParser<T3> p3)
  {
    return Sequence(p1, Sequence(p2, p3))
      .Select(it => (it.First, it.Second.First, it.Second.First));
  }

  // public static IParser<(List<T1>, List<T2>)> Choice<T1, T2>(IParser<T1> p1, IParser<T2> p2)
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
