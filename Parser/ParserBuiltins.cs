using Utils;

namespace Parser;


public static class ParserBuiltins
{
  public static readonly Parser<char> Any = new AnyParser();
  public static readonly Parser<char> Letter = Any.Where(char.IsLetter, "IsLetter");
  public static readonly Parser<char> Digit = Any.Where(char.IsNumber, "IsNumber");
  public static readonly Parser<long> Long = Digit.Plus().Join().Select(Convert.ToInt64);

  public static readonly Parser<char> Whitespace = Any.Where(char.IsWhiteSpace, "IsWhiteSpace");

  public static DeferredParser<T> Defer<T>() => new();

  public record Void { }

  public static readonly Parser<Void> EndOfInput = Parser.From((c, i) =>
  {
    if (i == c.Length) return ParseResult.From(new Void(), c, i);
    return new ParseFailure<Void>("Expected end-of-input", c, i);
  });

  public static readonly Parser<Void> EndOfLine = EndOfInput | String("\n").Void();

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

  public static Parser<(T1 First, T2 Second, T3 Third, T4 Fourth, T5 Fifth)> Sequence<T1, T2, T3, T4, T5>(Parser<T1> p1, Parser<T2> p2, Parser<T3> p3, Parser<T4> p4, Parser<T5> p5)
  {
    return Sequence(p1, p2, p3, p4).Then(p5)
      .Select(it => (it.First.First, it.First.Second, it.First.Third, it.First.Fourth, it.Second));
  }

  public static Parser<string> Choice(params string[] choices)
  {
    return choices.Select(it => String(it)).Aggregate((a, b) => a | b);
  }

  public static Parser<T> Format<T>(string format, Parser<T> p1) {
    var parts = format.Split("{}");
    if (parts.Length != 2) throw new ApplicationException("Format error");
    return p1.Between(String(parts[0]), String(parts[1]));
  }
}
