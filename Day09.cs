
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;
namespace AdventOfCode2024.CSharp.Day09;

public class Day09
{
  [Theory]
  [InlineData("Day09.Sample", 1928)]
  [InlineData("Day09", 6200294120911L)]
  public void Part1(string file, int expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var compressed = Compress1(input);
    Checksum(compressed).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day09.Sample", 2858)]
  [InlineData("Day09", 6227018762750L)]
  public void Part2(string file, int expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var compressed = Compress2(input, input.Max());
    Checksum(compressed).Should().Be(expected);
  }

  private long Checksum(List<long> compressed)
  {
    return compressed.Select((it, index) => it == -1 ? 0 : it * index).Sum();
  }

  private List<long> Compress1(List<long> input)
  {
    var head = 0;
    var tail = input.Count - 1;
    while (head < tail) {
      if (input[head] != -1){
        head++;
        continue;
      }
      if (input[tail] == -1) {
        tail--;
        continue;
      }
      input[head++] = input[tail];
      input[tail--] = -1;
    }
    return input;
  }

  private List<long> Compress2(List<long> input, long id)
  {
    var result = input.ToList();

    var start = input.IndexOf(id);
    var end = input.LastIndexOf(id);
    var length = end - start + 1;

    var head = 0;
    var freespan = 0;
    var freespanstart = -1;
    while (freespan < length && head < start)
    {
      if (input[head] == -1)
      {
        freespan += 1;
        if (freespanstart == -1) freespanstart = head;
      }
      else 
      {
        freespan = 0;
        freespanstart = -1;
      }
      head++;
    }

    if (freespan == length) {
      for(var i = 0 ; i < length; i++) {
        input[start + i] = -1;
        input[freespanstart + i] = id;
      }
    }

    if (id == 0) return input;
    return Compress2(input, id - 1);
  }

  private static List<long> FormatInput(List<string> input)
  {
    var line = input.Single().Select(it => Convert.ToInt32($"{it}"));
    var id = 0L;
    var result = new List<long>();
    var isFile = true;
    foreach(var item in line) {
      if (isFile) {
        result.AddRange(Enumerable.Repeat(id, item));
        id += 1;
      } else {
        result.AddRange(Enumerable.Repeat(-1L, item));
      }
      isFile = !isFile;
    }
    return result;
  }
}
