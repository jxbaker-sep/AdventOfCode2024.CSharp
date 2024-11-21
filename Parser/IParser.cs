namespace Parser;

public interface IParseResult<T> { }

public record ParseSuccess<T>(T Value, int PositionAfter) : IParseResult<T>;

public interface IParseFailure{}

public record ParseFailure<T>() : IParseResult<T>, IParseFailure;

public interface IParser<T>
{
  public IParseResult<T> Parse(char[] input, int position);
}
