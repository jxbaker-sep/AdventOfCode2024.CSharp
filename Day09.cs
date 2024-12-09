
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
  public void Part1(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var compressed = Compress1(input);
    Checksum(compressed).Should().Be(expected);
  }

  [Theory]
  [InlineData("Day09.Sample", 2858)]
  [InlineData("Day09", 6227018762750L)]
  public void Part2(string file, long expected)
  {
    var input = FormatInput(AoCLoader.LoadLines(file));
    var compressed = Compress2(input);
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

  private List<long> Compress2(List<long> input)
  {
    Dictionary<long, (int index, int length)> files = [];
    HashSet<(int index, int length)> free = [];

    var i = 1;
    var current = (index: 0, length: 1, id: input[0]);
    while (i < input.Count)
    {
      if (input[i] == current.id) {
        current = (current.index, length: current.length + 1, current.id);
        i ++;
        continue;
      }
      if (current.id == -1) free.Add((current.index, current.length));
      else files[current.id] = (current.index, current.length);
      current = (index: i, length: 1, id: input[i]);
      i++;
    }
    // make sure to add to the last one
    if (current.id == -1) free.Add((current.index, current.length));
    else files[current.id] = (current.index, current.length);

    for(var id = input.Max(); id >= 0; id--)
    {
      var file = files[id];
      free.RemoveWhere(it => it.index >= file.index);
      var freeSpans = free.Where(it => it.length >= file.length).OrderBy(it => it.index).Take(1).ToList();
      if (freeSpans.Count == 0) continue;
      var freeSpan = freeSpans[0];
      for(var x = 0; x < file.length; x++)
      {
        input[freeSpan.index + x] = id;
        input[file.index + x] = -1;
      }
      free.Remove(freeSpan);
      if (freeSpan.length > file.length)
      {
        free.Add((freeSpan.index + file.length, freeSpan.length - file.length));
      }
    }

    return input;
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
