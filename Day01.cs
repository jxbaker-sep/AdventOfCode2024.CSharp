using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Parser;
using Utils;

using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp;

public class Day01
{
    [Theory]
    [InlineData("Day01.Sample", 11)]
    [InlineData("Day01", 2904518L)]
    public void Part1(string path, long expected)
    {
        var data = Convert(AoCLoader.LoadLines(path));
        data.Item1.Zip(data.Item2).Select(it => Math.Abs(it.First - it.Second))
            .Sum()
            .Should().Be(expected);
    }


    [Theory]
    [InlineData("Day01.Sample", 31)]
    [InlineData("Day01", 18650129)]
    public void Part2(string path, long expected)
    {
        var data = Convert(AoCLoader.LoadLines(path));
        data.Item1.Select(it => data.Item2.Count(z => z == it) * it)
            .Sum()
            .Should().Be(expected);
    }

    private static (List<long>, List<long>) Convert(string[] data)
    {
        var temp = data.Select(it => (P.Number.Trim() + P.Number).Parse(it)).ToList();
        return (
            temp.Select(it => it[0]).Order().ToList(),
            temp.Select(it => it[1]).Order().ToList()
        );
    }
}
