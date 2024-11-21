namespace Parser;

public interface IParseResult<T> { }

public record ParseSuccess<T>(T Value, int PositionAfter) : IParseResult<T>;

public interface IParseFailure{}

public record ParseFailure<T>() : IParseResult<T>, IParseFailure;

public abstract class Parser<T>
{
  public IParseResult<T> Parse(string input)
  {
    return Parse(input.ToCharArray(), 0);
  }

  abstract public IParseResult<T> Parse(char[] input, int position);
}
