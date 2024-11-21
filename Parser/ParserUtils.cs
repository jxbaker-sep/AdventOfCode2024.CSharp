using System.Linq;

namespace Parser;


public static class ParserUtils
{
  public static readonly IParser<int> Digit = new CharParser().Where(it => '0' <= it && it <= '9').Select(it => it - '0');
  public static readonly IParser<long> Number = Digit.Plus().Select(it => it.Select(it => (long)it).Aggregate((a,b) => a * 10 +  b));

  public static IParser<(T1 First, T2 Second)> Sequence<T1, T2>(IParser<T1> p1, IParser<T2> p2)
  {
    return SequenceParser.From(p1, p2);
  }

  public static IParser<(T1 First, T2 Second, T2 Third)> Sequence<T1, T2, T3>(IParser<T1> p1, IParser<T2> p2, IParser<T3> p3)
  {
    return SequenceParser.From(p1, SequenceParser.From(p2, p3))
      .Select(it => (it.First, it.Second.First, it.Second.First));
  }
}