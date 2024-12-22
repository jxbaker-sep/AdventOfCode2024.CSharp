using AdventOfCode2024.CSharp.Utils;
using FluentAssertions;
using Parser;
using Utils;
using P = Parser.ParserBuiltins;

namespace AdventOfCode2024.CSharp.Day22;

public class Day22
{
  [Theory]
  [InlineData("Day22.Sample", 37327623)]
  [InlineData("Day22", 14726157693)]
  public void Part1(string file, long expected)
  {
    var codes = FormatInput(AoCLoader.LoadLines(file));
    codes.Select(it => GetSecrets(it, 2_000).Last()).Sum().Should().Be(expected);
  }

  [Theory]
  [InlineData("Day22.Sample.2", 23)]
  // [InlineData("Day22", 14726157693)]
  public void Part2(string file, long expected)
  {
    var codes = FormatInput(AoCLoader.LoadLines(file));
    // codes.Select(it => GetSecret(it, 2_000)).Sum().Should().Be(expected);
  }

  [Fact]
  public void Sanity()
  {
    Mix(42, 15).Should().Be(37);
    Prune(100000000).Should().Be(16113920);
    GetSecrets(15887950, 1).Last().Should().Be(16495136);
    GetSecrets(15887950, 2).Last().Should().Be(527345);
    GetSecrets(15887950, 3).Last().Should().Be(704524);
    GetSecrets(15887950, 4).Last().Should().Be(1553684);
    GetSecrets(15887950, 5).Last().Should().Be(12683156);
    GetSecrets(15887950, 6).Last().Should().Be(11100544);
    GetSecrets(15887950, 7).Last().Should().Be(12249484);
    GetSecrets(15887950, 8).Last().Should().Be(7753432);
    GetSecrets(15887950, 9).Last().Should().Be(5908254);
  }

  public IEnumerable<long> GetSecrets(long secret, long n) {
    for(var i = 0; i < n; i++) {
      secret = Prune(Mix(secret, secret * 64));
      secret = Prune(Mix(secret, secret / 32));
      secret = Prune(Mix(secret, secret * 2048));
      yield return secret;
    }
  }

  public long Mix(long n1, long n2) => n1 ^ n2;
  public long Prune(long n) => n % 16777216;

  private static List<long> FormatInput(List<string> input)
  {
    return P.Long.ParseMany(input);
  }
}
