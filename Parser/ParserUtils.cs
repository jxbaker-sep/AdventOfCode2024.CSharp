using System.Linq;

namespace Parser;


public static class ParserUtils
{
  public static readonly Parser<int> Digit = new CharParser().Where(it => '0' <= it && it <= '9').Select(it => it - '0');
  public static readonly Parser<long> Number = Digit.Plus().Select(it => it.Select(it => (long)it).Aggregate((a,b) => a * 10 +  b));

  public static Parser<(T1 First, T2 Second)> Sequence<T1, T2>(Parser<T1> p1, Parser<T2> p2)
  {
    return SequenceParser.From(p1, p2);
  }

  public static Parser<(T1 First, T2 Second, T2 Third)> Sequence<T1, T2, T3>(Parser<T1> p1, Parser<T2> p2, Parser<T3> p3)
  {
    return SequenceParser.From(p1, SequenceParser.From(p2, p3))
      .Select(it => (it.First, it.Second.First, it.Second.First));
  }
}