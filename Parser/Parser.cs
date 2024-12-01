using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Parser;

public interface IParseResult<T>
{
  public IParseResult<T2> Then<T2>(Func<ParseSuccess<T>, IParseResult<T2>> fct);
  public IParseResult<T> Or(Parser<T> other);
}

public record ParseSuccess<T>(T Value, char[] Data, int Position) : IParseResult<T>
{
  public IParseResult<T2> Then<T2>(Func<ParseSuccess<T>, IParseResult<T2>> fct)
  {
    return fct(this);
  }

  public IParseResult<T> Or(Parser<T> other)
  {
    return this;
  }
}

public record ParseFailure<T>(string Annotation, char[] Data, int Position) : IParseResult<T>
{
  public string Message {
    get {
      return $"{Annotation}: at {Data[Position..].Take(10).Join()}...";
    }
  }

  public IParseResult<T2> Then<T2>(Func<ParseSuccess<T>, IParseResult<T2>> fct)
  {
    return new ParseFailure<T2>(Annotation, Data, Position);
  }

  public IParseResult<T> Or(Parser<T> other)
  {
    return other.Parse(Data, Position);
  }
}

public static class ParseResult
{
  public static ParseSuccess<T> From<T>(T t, char[] data, int i) => new(t, data, i);
}

public abstract class Parser<T>
{
  abstract public IParseResult<T> Parse(char[] input, int position);

  public static Parser<T> operator|(Parser<T> lhs, Parser<T> rhs) {
    return Parser.From((c,i) => {
      var temp = lhs.Parse(c, i);
      var x = temp.Or(rhs);
      return x;
    });
  }

  public static Parser<List<T>> operator+(Parser<List<T>> lhs, Parser<T> rhs) => ParserBuiltins.Sequence(lhs, rhs).Select(it => it.First.Append(it.Second).ToList());
}

public class EZParser<T>(Func<char[], int, IParseResult<T>> Action) : Parser<T>
{
    override public IParseResult<T> Parse(char[] input, int position)
    {
        return Action(input, position);
    }
}

public class Parser
{
  public static Parser<T> From<T>(Func<char[], int, IParseResult<T>> action) => new EZParser<T>(action);
}