using FluentAssertions;
using Utils;

namespace AdventOfCode2024.CSharp.Tests;

public class UtilsTests
{
  [Fact]
  public void WindowTests()
  {
    var input = new List<int>{1,2,3};
    input.Windows(0).Should().BeEquivalentTo(new List<List<int>>{
      new List<int>{}, new List<int>{}, new List<int>{}
    });

    input.Windows(1).Should().BeEquivalentTo(new List<List<int>>{
      new List<int>{1}, new List<int>{2}, new List<int>{3}
    });

    input.Windows(2).Should().BeEquivalentTo(new List<List<int>>{
      new List<int>{1,2}, new List<int>{2,3}
    });

    input.Windows(3).Should().BeEquivalentTo(new List<List<int>>{
      new List<int>{1,2,3}
    });

    input = new List<int>{1,2,3,4,5};
    input.Windows(3).Should().BeEquivalentTo(new List<List<int>>{
      new List<int>{1,2,3},
      new List<int>{2,3,4},
      new List<int>{3,4,5},
    });
  }
}