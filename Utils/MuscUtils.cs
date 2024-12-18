public static class MiscUtils {
  public static long? BinarySearch(long max, Func<long, bool> action) {
    long min = 0;

    if (action(0)) return 0;
    if (!action(max)) return null;

    while (min < max) {
      var attempt = (min + max) / 2;
      if (action(attempt)) {
        if (attempt == min + 1) return attempt;
        max = attempt;
      } else {
        if (attempt == max - 1) return max;
        min = attempt;
      }
    }
    return max;
  }
}