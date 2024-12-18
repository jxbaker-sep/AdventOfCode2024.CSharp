
using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
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
    var input = FormatInput2(AoCLoader.LoadLines(file));
    var compressed = Compress2(input);
    compressed.Select(it => (it.Index * it.Length + MathUtils.Triangle(it.Length - 1)) * it.Id ).Sum().Should().Be(expected);
  }

  private static long Checksum(List<long> compressed)
  {
    return compressed.Select((it, index) => it == -1 ? 0 : it * index).Sum();
  }

  private static List<long> Compress1(List<long> input)
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

  private static List<Record> Compress2(List<Record> input)
  {
    var files = input.Where(it => it.Id != -1).ToDictionary(it => it.Id, it => it);
    var free = new LinkedList<(int Index, int Length)>(input.Where(it => it.Id == -1).Select(it => (it.Index, it.Length)).OrderBy(it=>it.Index));

    for(var id = input.Select(it => it.Id).Max(); id >= 0; id--)
    {
      var file = files[id];
      var needle = free.Nodes()
        .TakeWhile(node => node.Value.Index < file.Index)
        .SkipWhile(node => node.Value.Length < file.Length)
        .FirstOrDefault();
      if (needle == null) continue;

      files[id] = file with {Index = needle.Value.Index}; 

      if (needle.Value.Length > file.Length)
      {
        free.AddAfter(needle, (needle.Value.Index + file.Length, needle.Value.Length - file.Length));
      }
      free.Remove(needle);
    }

    return [.. files.Values];
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

  public record Record(int Index, int Length, long Id);


  private static List<Record> FormatInput2(List<string> input)
  {
    var line = input.Single().Select(it => Convert.ToInt32($"{it}"));
    var id = 0L;
    var result = new List<Record>();
    var isFile = true;
    var index = 0;
    foreach(var item in line) {
      if (isFile) {
        result.Add(new(index, item, id));
        id += 1;
      } else {
        result.Add(new(index, item, -1L));
      }
      isFile = !isFile;
      index += item;
    }
    return result;
  }
}
