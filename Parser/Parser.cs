using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Parser;

public record ParseResult<T>(T Value, int PositionAfter);

public static class ParseResult
{
  public static ParseResult<T> From<T>(T t, int i) => new(t, i);
}

public abstract class Parser<T>
{
  abstract public ParseResult<T> Parse(char[] input, int position);

  public static Parser<T> operator|(Parser<T> lhs, Parser<T> rhs) {
    return Parser.From((c,i) => {
      try {
        return lhs.Parse(c, i);
      }
      catch {
        return rhs.Parse(c, i);
      }
    });
  }

  public static Parser<List<T>> operator+(Parser<List<T>> lhs, Parser<T> rhs) => ParserBuiltins.Sequence(lhs, rhs).Select(it => it.First.Append(it.Second).ToList());
}

public class ParseException(string message, char[] input, int position) : ApplicationException() {
    public override string Message {
      get {
        return $"{message}: at {input[position..].Take(10).Join()}...";
      }
    }
}

public class EZParser<T>(Func<char[], int, ParseResult<T>> Action) : Parser<T>
{
    override public ParseResult<T> Parse(char[] input, int position)
    {
        return Action(input, position);
    }
}

public class Parser
{
  public static Parser<T> From<T>(Func<char[], int, ParseResult<T>> action) => new EZParser<T>(action);
}