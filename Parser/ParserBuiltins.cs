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

  public static readonly Parser<Unit> EndOfInput = Parser.From<Unit>((c, i) => {
    if (i == c.Length) return ParseResult.From(new Unit(), c, i);
    return new ParseFailure<Unit>("Expected end-of-input", c, i);
  });

  public static readonly Parser<Unit> EndOfLine = EndOfInput | String("\n").Discard();

  public static Parser<string> String(string s)
  {
    return Parser.From<string>((c, i) => {
      if (c.Length >= i + s.Length && c[i..(i+s.Length)].Join() == s) return ParseResult.From(s, c, i + s.Length);
      return new ParseFailure<string>($"expected string {s}", c, i);
    });
  }

  public static Parser<(T1 First, T2 Second)> Sequence<T1, T2>(Parser<T1> p1, Parser<T2> p2)
  {
    return Parser.From((c, i) => p1.Parse(c,i)
      .Then(v => 
        p2.Parse(v.Data, v.Position).Then(v2 => ParseResult.From((v.Value, v2.Value), v2.Data, v2.Position))
      ));
  }

  public static Parser<(T1 First, T2 Second, T3 Third)> Sequence<T1, T2, T3>(Parser<T1> p1, Parser<T2> p2, Parser<T3> p3)
  {
    return Parser.From((c, i) => p1.Parse(c,i)
      .Then(v => 
        p2.Parse(v.Data, v.Position).Then(v2 => ParseResult.From((v.Value, v2.Value), v2.Data, v2.Position))
      )
      .Then(v =>
        p3.Parse(v.Data, v.Position).Then(v2 => ParseResult.From((v.Value.Item1, v.Value.Item2, v2.Value), v2.Data, v2.Position))
      ));
  }

}
