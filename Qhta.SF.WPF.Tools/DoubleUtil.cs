namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Utility class for comparing double values with a specified tolerance.
/// </summary>
public static class DoubleUtil
{
  /// <summary>
  /// Determines whether two double values are close to each other within a specified tolerance.
  /// </summary>
  /// <param name="a"></param>
  /// <param name="b"></param>
  /// <param name="tolerance"></param>
  /// <returns></returns>
  public static bool AreClose(double a, double b, double tolerance = 1e-10)
  {
    return Math.Abs(a - b) < tolerance;
  }

  /// <summary>
  /// Determines whether the first double value is less than or equal to the second value, considering a specified tolerance.
  /// </summary>
  /// <param name="a"></param>
  /// <param name="b"></param>
  /// <param name="tolerance"></param>
  /// <returns></returns>
  public static bool GreaterThanOrClose(double a, double b, double tolerance = 1e-10)
  {
    return a > b || AreClose(a, b, tolerance);
  }
}