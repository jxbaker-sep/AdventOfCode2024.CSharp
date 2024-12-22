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
  [InlineData("Day22", 1614)]
  public void Part2(string file, long expected)
  {
    var codes = FormatInput(AoCLoader.LoadLines(file));
    var d = new Dictionary<(long,long,long,long), long>();
    foreach (var code in codes) GetSequences(code, d);
    d.Values.Max().Should().Be(expected);
  }

  private static void GetSequences(long code, Dictionary<(long,long,long,long), long> sellPrices)
  {
    var closed = new HashSet<(long,long,long,long)>();
    foreach(var w in GetSecrets(code, 2000).Select(it => it % 10).Windows(5)) {
      var key = (w[1] - w[0], w[2] - w[1], w[3] - w[2], w[4] - w[3]);
      if (!closed.Add(key)) continue;
      sellPrices[key] = sellPrices.GetValueOrDefault(key) + w[4];
    }
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
    var s = new Dictionary<(long,long,long,long), long>();
    GetSequences(123, s);
    s.Should().Contain(KeyValuePair.Create((-1L, -1L, 0L, 2L), 6L));
  }

  public static IEnumerable<long> GetSecrets(long secret, long n) {
    yield return secret;
    for(var i = 0; i < n; i++) {
      secret = Prune(Mix(secret, secret * 64));
      secret = Prune(Mix(secret, secret / 32));
      secret = Prune(Mix(secret, secret * 2048));
      yield return secret;
    }
  }

  public static long Mix(long n1, long n2) => n1 ^ n2;
  public static long Prune(long n) => n % 16777216;

  private static List<long> FormatInput(List<string> input)
  {
    return P.Long.ParseMany(input);
  }
}
