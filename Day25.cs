using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;

namespace AdventOfCode2024.CSharp.Day25;

public class Day25
{
  [Theory]
  [InlineData("Day25.Sample", 3)]
  [InlineData("Day25", 0)]
  public void Part1(string file, long expected)
  {
    var elements = FormatInput(AoCLoader.LoadFile(file));

    long result = 0;
    foreach(var lck in elements.Where(it => it.IsLock)) {
      foreach(var key in elements.Where(it => !it.IsLock)) {
        result += lck.Tumblers.Zip(key.Tumblers).All(it => it.First + it.Second < 6) ? 1 : 0;
      }
    }
    result.Should().Be(expected);
  }

  record Element(bool IsLock, List<int> Tumblers);

  private static List<Element> FormatInput(string input)
  {
    var elements = input.Split("\n\n");
    var result = new List<Element>();
    foreach(var element in elements) {
      var lines = element.Split("\n").ToList();
      var isLock = lines[0] == "#####";
      if (isLock) {
        lines = lines[1..];
      }
      else lines = lines[..^1];
      var tumblers = Enumerable.Range(0, 5).Select(n => lines.Count(line => line[n] == '#')).ToList();
      result.Add(new(isLock, tumblers));
    }
    return result;
  }
}
