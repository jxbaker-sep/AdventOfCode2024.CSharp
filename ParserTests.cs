using System;
using System.Collections.Generic;
using FluentAssertions;
using Parser;
using Utils;

using P = Parser.ParserUtils;

public class ParserTests
{
    [Fact]
    public void Char()
    {
        var cp = new CharParser();
        cp.Parse("").Should().BeAssignableTo<IParseFailure>();
        cp.Parse("a").Should().BeOfType<ParseSuccess<char>>()
            .Which.Value.Should().Be('a');
        cp.Parse("bde").Should().BeOfType<ParseSuccess<char>>()
            .Which.Value.Should().Be('b');
        cp.Parse("bde").Should().BeOfType<ParseSuccess<char>>()
            .Which.PositionAfter.Should().Be(1);
        
    }

    [Fact]
    public void Optional()
    {
        var cp = new CharParser().Optional();
        cp.Parse("").Should().BeOfType<ParseSuccess<List<char>>>()
            .Which.Value.Should().BeEmpty();
        cp.Parse("a").Should().BeOfType<ParseSuccess<List<char>>>()
            .Which.Value.Should().BeEquivalentTo(['a']);
        cp.Parse("bde").Should().BeOfType<ParseSuccess<List<char>>>()
            .Which.Value.Should().BeEquivalentTo(['b']);
        cp.Parse("bde").Should().BeOfType<ParseSuccess<List<char>>>()
            .Which.PositionAfter.Should().Be(1);
    }

    [Fact] 
    void RangeTest()
    {
        P.Digit.Range(1,3).Join().ParseOrThrow("12345").Should().Be("123");
        P.Digit.Range(1,10).Join().ParseOrThrow("12345").Should().Be("12345");
        P.Digit.Range(6,10).Invoking(it => it.ParseOrThrow("12345")).Should().Throw<ApplicationException>();
    }

    [Fact]
    public void IdentifierTest()
    {
        bool InRange(char a, char b, char c) => b <= a && a <= c;
        bool IsFirst(char a) => a == '_' || InRange(a, 'a', 'z') || InRange(a, 'A', 'Z');
        bool IsDigit(char a) => InRange(a, '0', '9');
        var first = new CharParser().Where(it => IsFirst(it));
        var alnum = new CharParser().Where(it => IsFirst(it) || IsDigit(it));
        var idParser = P.Sequence(first, alnum.Star())
            .Select(it => $"{it.First}{it.Second.Join()}");
        idParser.ParseOrThrow("_").Should().Be("_");
        idParser.Invoking(it => it.ParseOrThrow("")).Should().Throw<ApplicationException>();
        idParser.Invoking(it => it.ParseOrThrow("123asdasd_")).Should().Throw<ApplicationException>();

    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("12", 12)]
    [InlineData("123", 123)]
    [InlineData("12345", 12345)]
    void NumberTest(string input, long value) => P.Number.ParseOrThrow(input).Should().Be(value);

}