using System.Collections.Generic;
using System.Linq;
using Utils;

namespace AdventOfCode2024.CSharp;

public class Day01
{
    [Fact]
    public void Part1()
    {
        Assert.True(true);
    }

    [Theory]
    [InlineData("Day01.Sample", 12)]
    public void Part2(string path, long expected) 
    {
        var data = Convert(AoCLoader.LoadLines(path));
        Assert.Equal(data.Single(), expected);
    }

    private IReadOnlyList<long> Convert(string[] data) => data.Select(it => System.Convert.ToInt64(it)).ToList();
}
