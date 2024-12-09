using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Utils;

namespace AdventOfCode2024.CSharp.Tests
{
  public class UtilsTests
  {
    [Fact]
    public void WindowTests()
    {
      List<int> input = [1, 2, 3];
      input.Windows(0).Should().BeEquivalentTo(new List<List<int>>{
        new() {}, new() {}, new() {}
      });

      input.Windows(1).Should().BeEquivalentTo(new List<List<int>>{
        new() {1}, new() {2}, new() {3}
      });

      input.Windows(2).Should().BeEquivalentTo(new List<List<int>>{
        new() {1,2}, new() {2,3}
      });

      input.Windows(3).Should().BeEquivalentTo(new List<List<int>>{
        new() {1,2,3}
      });

      input = [1, 2, 3, 4, 5];
      input.Windows(3).Should().BeEquivalentTo(new List<List<int>>{
        new() {1,2,3},
        new() {2,3,4},
        new() {3,4,5},
      });
    }

    [Fact]
    public void PairTest()
    {
      int[] v = [1, 2, 3, 4, 5];
      v.ToList().Pairs().Should().BeEquivalentTo(new (int, int)[] {
        (1,2), (1,3), (1,4), (1,5),
        (2,3), (2,4), (2,5),
        (3,4), (3,5),
        (4,5)
      });
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 3)]
    [InlineData(3, 6)]
    [InlineData(4, 10)]
    [InlineData(5, 15)]
    [InlineData(6, 21)]
    public void TriangleTest(long x, long expected)
    {
      MathUtils.Triangle(x).Should().Be(expected);
    }
  }
}