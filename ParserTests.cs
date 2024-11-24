using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Parser;
using Utils;

using P = Parser.ParserBuiltins;

public class ParserTests
{
    [Fact]
    public void AnyFails()
    {
        P.Any.Invoking(it => it.Parse("")).Should().Throw<ParseException>();
    }


    [Theory]
    [InlineData("a", 'a')]
    [InlineData("bde", 'b')]
    public void Char(string input, char expected)
    {
        P.Any.Parse(input).Should().Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("ab", "a")]
    public void Optional(string input, string expected)
    {
        P.Any.Optional().Join().Parse(input).Should().Be(expected);
    }

    [Fact]
    void RangeTest()
    {
        P.Digit.Range(1,3).Join().Parse("12345").Should().Be("123");
        P.Digit.Range(1,10).Join().Parse("12345").Should().Be("12345");
        P.Digit.Range(6,10).Invoking(it => it.Parse("12345")).Should().Throw<ApplicationException>();
    }

    [Fact]
    public void IdentifierTest()
    {
        bool InRange(char a, char b, char c) => b <= a && a <= c;
        bool IsFirst(char a) => a == '_' || InRange(a, 'a', 'z') || InRange(a, 'A', 'Z');
        bool IsDigit(char a) => InRange(a, '0', '9');
        var first = P.Any.Where(it => IsFirst(it));
        var alnum = P.Any.Where(it => IsFirst(it) || IsDigit(it));
        var idParser = P.Sequence(first, alnum.Star())
            .Select(it => $"{it.First}{it.Second.Join()}");
        idParser.Parse("_").Should().Be("_");
        idParser.Invoking(it => it.Parse("")).Should().Throw<ApplicationException>();
        idParser.Invoking(it => it.Parse("123asdasd_")).Should().Throw<ApplicationException>();

    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("12", 12)]
    [InlineData("123", 123)]
    [InlineData("12345", 12345)]
    void NumberTest(string input, long value) => P.Number.Parse(input).Should().Be(value);

    // [Fact]
    // void SimpleCalculatorTest()
    // {
    //     var expression = new DeferredParser<long>();
    //     var paranthesizedExpression = 
    //     expression.Actual = P.Choice(paranthesizedExpression, simpleExpression)
    // }


    [Theory]
    [InlineData("abc 1 def", 1)]
    [InlineData(" abc  \t\n 2   def   \n", 2)]
    [InlineData("abc145def", 145)]
    void BetweenTest(string input, long expected)
    {
        var p = P.Number.Between("abc", "def");
        p.Parse(input).Should().Be(expected);
    }

    [Fact]
    void EndTest()
    {
        P.String("abc").End().Parse("abc").Should().Be("abc");
        P.String("abc").End().Invoking(it => it.Parse("abcd")).Should().Throw<ParseException>();
    }
}