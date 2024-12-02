using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
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

  public record Unit { }

  public static readonly Parser<Unit> EndOfInput = Parser.From<Unit>((c, i) =>
  {
    if (i == c.Length) return ParseResult.From(new Unit(), c, i);
    return new ParseFailure<Unit>("Expected end-of-input", c, i);
  });

  public static readonly Parser<Unit> EndOfLine = EndOfInput | String("\n").Discard();

  public static Parser<string> String(string s)
  {
    return Parser.From((c, i) =>
    {
      if (c.Length >= i + s.Length && c[i..(i + s.Length)].Join() == s) return ParseResult.From(s, c, i + s.Length);
      return new ParseFailure<string>($"expected string {s}", c, i);
    });
  }

  public static Parser<(T1 First, T2 Second)> Sequence<T1, T2>(Parser<T1> p1, Parser<T2> p2)
  {
    return p1.Then(p2);
  }

  public static Parser<(T1 First, T2 Second, T3 Third)> Sequence<T1, T2, T3>(Parser<T1> p1, Parser<T2> p2, Parser<T3> p3)
  {
    return Sequence(p1, p2).Then(p3).Select(it => (it.First.First, it.First.Second, it.Second));
  }

  public static Parser<(T1 First, T2 Second, T3 Third, T4 Fourth)> Sequence<T1, T2, T3, T4>(Parser<T1> p1, Parser<T2> p2, Parser<T3> p3, Parser<T4> p4)
  {
    return Sequence(p1, p2, p3).Then(p4)
      .Select(it => (it.First.First, it.First.Second, it.First.Third, it.Second));
  }
}
