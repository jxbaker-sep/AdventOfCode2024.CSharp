using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;

using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp.Tests;

public class ParserTests
{
    [Fact]
    public void AnyFails()
    {
        P.Any.Invoking(it => it.Parse("")).Should().Throw<ApplicationException>();
    }

    [Fact]
    public void BeforeTest()
    {
        P.Long.Before("+").ParseOrNullStruct("123").Should().BeNull();
        P.Long.Before("+").ParseOrNullStruct("123+").Should().Be(123);
        (P.Long.Before("+") | P.Long).Parse("123").Should().Be(123);
        (P.Letter.Select(_=>0L).Before("1") | P.Long).Parse("1").Should().Be(1);
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
    public void RangeTest()
    {
        P.Digit.Range(1,3).Join().Parse("12345").Should().Be("123");
        P.Digit.Range(1,10).Join().Parse("12345").Should().Be("12345");
        P.Digit.Range(6,10).Invoking(it => it.Parse("12345")).Should().Throw<ApplicationException>();
    }

    [Fact]
    public void IdentifierTest()
    {
        static bool IsFirst(char a) => a == '_' || char.IsLetter(a);
        var first = P.Any.Where(IsFirst);
        var subsequent = P.Any.Where(it => IsFirst(it) || char.IsNumber(it));
        var idParser = P.Sequence(first, subsequent.Star())
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
    public void NumberTest(string input, long value) => P.Long.Parse(input).Should().Be(value);

    [Theory]
    [InlineData("1", 1)]
    [InlineData("1 + 1", 2)]
    [InlineData("(1 + 1)", 2)]
    [InlineData("(1) + (1)", 2)]
    [InlineData("(1 + 2) + 3", 6)]
    [InlineData("1 + 2 + 3", 6)]
    [InlineData("1 * 2 + 3", 5)]
    [InlineData("1 + 2 * 3", 7)]
    [InlineData("1 * 2 + 3 * 4", 14)]
    [InlineData("1 * 2 * 3 * 4", 24)]
    [InlineData("1 * (2 + 3) * 4", 20)]
    [InlineData("1 * 2 * 3 + 4", 10)]
    [InlineData("(1 + 2) * (3 + 4)", 21)]
    public void SimpleCalculatorTest(string input, long expected)
    {
        // 1 + 2 * 3
        // 2 * 3 + 1
        // expr: ( expr ) | Number 
        var expression = P.Defer<long>();
        var simpleValue = expression.Between("(", ")") | P.Long.Trim();
        var operatorExpression = P.Sequence(simpleValue.Before("+"), expression).Select(it => it.First + it.Second)
            | P.Sequence(simpleValue.Before("*"), simpleValue.Before("+"), expression).Select(it => it.First * it.Second + it.Third)
            | P.Sequence(simpleValue.Before("*"), expression).Select(it => it.First * it.Second);
        expression.Actual = operatorExpression | simpleValue;

        expression.Parse(input).Should().Be(expected);
    }


    [Theory]
    [InlineData("abc 1 def", 1)]
    [InlineData(" abc  \t\n 2   def   \n", 2)]
    [InlineData("abc145def", 145)]
    public void BetweenTest(string input, long expected)
    {
        var p = P.Long.Between("abc", "def");
        p.Parse(input).Should().Be(expected);
    }

    [Fact]
    public void EndTest()
    {
        P.String("abc").End().Parse("abc").Should().Be("abc");
        P.String("abc").End().Invoking(it => it.Parse("abcd")).Should().Throw<ApplicationException>();
    }

    [Fact]
    public void OrTest()
    {
        var parser = P.Letter | P.Digit;
        parser.Parse("1").Should().Be('1');
        parser.Parse("a").Should().Be('a');
        parser.Parse("B").Should().Be('B');
        parser.Invoking(it => it.Parse("-")).Should().Throw<ApplicationException>();
    }

    [Fact]
    public void PeekNotTest()
    {
        var parser = P.Long.Trim().PeekNot(P.String("STOP").End());
        parser.Parse("12 1").Should().Be(12);
        parser.Parse("234 STOP no not really").Should().Be(234);
        parser.Invoking(it => it.Parse("234 STOP")).Should().Throw<ApplicationException>();
    }

    [Fact]
    public void PeekTest()
    {
        var parser = P.Long.Trim().Peek(P.String("STOP").End());
        parser.ParseOrNullStruct("12 1").Should().BeNull();
        parser.ParseOrNullStruct("234 STOP no not really").Should().BeNull();
        parser.ParseOrNullStruct("234 STOP").Should().Be(234);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData(@"{
        ""foo"": ""bar"",
        ""nested"": {
            ""a"": 1,
            ""b"": false,
            ""c"": 1.2
        } 
    }")]
    public void JsonParserTest(string jsonData)
    {
        var anyJsonObject = P.Defer<P.Unit>();
        var jsonInt = P.Long.Trim().Discard();
        var jsonFloat = P.Sequence(P.Long.Before("."), P.Long).Trim().Discard();
        var jsonBool = (P.String("true") | P.String("false")).Trim().Discard();
        var stringElement = P.Any.After("\\") | P.Any.Where(it => it != '\"');
        var jsonString = stringElement.Star().Between("\"", "\"").Trim().Discard();
        var jsonKeyValue = P.Sequence(jsonString.Before(":"), anyJsonObject);
        var jsonObject = P.Sequence(jsonKeyValue, jsonKeyValue.After(",").Star()).Optional().Between("{", "}").Discard();
        anyJsonObject.Actual = jsonFloat | jsonInt | jsonBool | jsonString | jsonObject;

        anyJsonObject.Parse(jsonData).Should().Be(new P.Unit());
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("2, 3", 6)]
    [InlineData("2, 3\n4,5,6", 126)]
    public void SumOfProducts(string input, long expected)
    {
        P.Sequence(P.Long, P.Long.After(",").Star()).Before(P.EndOfLine)
            .Select(it => it.First * it.Second.Append(1).Product())
            .Star()
            .End()
            .Select(it => it.Sum())
            .Parse(input)
            .Should().Be(expected);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("1", 1)]
    [InlineData("1,2", 3)]
    [InlineData("1, 2, 3", 6)]
    public void StarWithSeperatorTest(string input, int expected) => P.Long.Star(",").Parse(input).Sum().Should().Be(expected);

    [Theory]
    [InlineData("1", 1)]
    [InlineData("1,2", 3)]
    [InlineData("1, 2, 3", 6)]
    public void PlusWithSeperatorTest(string input, int expected) => P.Long.Plus(",").Parse(input).Sum().Should().Be(expected);

    [Fact]
    public void OperatorPlusTest1()
    {
        (P.Long.Before(",").Star() + P.Long).Parse("1,2,3").Should().BeEquivalentTo(new List<int>{1,2,3});
    }

    [Fact]
    public void OperatorPlusTest3()
    {
        (P.Long.Before(",") + P.Long).Parse("1,2").Should().BeEquivalentTo(new List<int>{1,2});
    }

    [Fact]
    public void OperatorPlusTest4()
    {
        (P.Long.Before(",") + P.Long.Before(",") + P.Long).Parse("1,2,3").Should().BeEquivalentTo(new List<int>{1,2,3});
    }

    [Fact]
    public void Sequence2Tests()
    {
        P.Sequence(P.Long, P.Letter).Parse("1A").Should().Be((1, 'A'));
    }

    [Fact]
    public void Sequence3Tests()
    {
        P.Sequence(P.Long, P.Letter, P.Any).Parse("1A*").Should().Be((1, 'A', '*'));
    }

    [Fact]
    public void Sequence4Tests()
    {
        P.Sequence(P.Long, P.Letter, P.Any, P.String("-2-")).Parse("1A*-2-").Should().Be((1, 'A', '*', "-2-"));
    }
}