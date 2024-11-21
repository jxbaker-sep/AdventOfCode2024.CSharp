namespace Parser;

public class CharParser : IParser<char>
{
  public IParseResult<char> Parse(char[] input, int position)
  {
    if (position < input.Length)
    {
      return new ParseSuccess<char>(input[position], position + 1);
    }
    return new ParseFailure<char>();
  }
}
