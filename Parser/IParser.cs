using System;
using System.Linq;
using Utils;

namespace Parser;

public record ParseResult<T>(T Value, int PositionAfter);

public interface IParser<T>
{
  public ParseResult<T> Parse(char[] input, int position);
}

public class ParseException(string message, char[] input, int position) : ApplicationException() {
    public override string Message {
      get {
        return $"{message}: at {input[position..(position+10)].Join()}...";
      }
    }
}

public class EZParser<T>(Func<char[], int, ParseResult<T>> Action) : IParser<T>
{
    public ParseResult<T> Parse(char[] input, int position)
    {
        return Action(input, position);
    }
}

public class Parser
{
  public static IParser<T> From<T>(Func<char[], int, ParseResult<T>> action) => new EZParser<T>(action);
}