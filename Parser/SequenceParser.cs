namespace Parser;

public class SequenceParser<T1, T2>(Parser<T1> first, Parser<T2> second) : Parser<(T1 First, T2 Second)>
{
    public override IParseResult<(T1, T2)> Parse(char[] input, int position)
    {
        var r1 = first.Parse(input, position);
        if (r1 is ParseSuccess<T1> r)
        {
            var e2 = second.Parse(input, r.PositionAfter);
            if (e2 is ParseSuccess<T2> e) return new ParseSuccess<(T1, T2)>((r.Value, e.Value), e.PositionAfter);
        }
        return new ParseFailure<(T1, T2)>();
    }
}

public static class SequenceParser
{
    public static SequenceParser<T1, T2> From<T1, T2>(Parser<T1> first, Parser<T2> second) => new SequenceParser<T1, T2>(first, second);
}