namespace Qhta.UnicodeBuild.Helpers;

public static class DoubleUtil
{
  public static bool AreClose(double a, double b, double tolerance = 1e-10)
  {
    return Math.Abs(a - b) < tolerance;
  }

  public static bool GreaterThanOrClose(double a, double b, double tolerance = 1e-10)
  {
    return a > b || AreClose(a, b, tolerance);
  }
}