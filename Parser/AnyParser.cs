namespace Parser;

public class AnyParser : IParser<char>
{
    public ParseResult<char> Parse(char[] input, int position)
    {
        if (position < input.Length) return new(input[position], position+1);
        throw new ParseException("Expected character; At end of input", input, position);
    }
}
