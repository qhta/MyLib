using System;

namespace Qhta.SystemUtils;

/// <summary>
/// A bunch of string utility methods
/// </summary>
public static class DateTimeUtils
{
  /// <summary>
  /// Truncates the specified date time to whole seconds.
  /// </summary>
  /// <param name="dateTime">Truncated date time</param>
  /// <returns></returns>
  public static DateTime Truncate(this DateTime dateTime) => Truncate(dateTime, TimeSpan.FromSeconds(1));

  /// <summary>
  /// Truncates the specified date time to the specified time span.
  /// Usage:
  /// <list type="button">
  /// <item>
  ///   dateTime = dateTime.Truncate(TimeSpan.FromMilliseconds(1)); // Truncate to whole ms
  /// </item>
  /// <item>
  ///   dateTime = dateTime.Truncate(TimeSpan.FromSeconds(1)); // Truncate to whole second
  /// </item>
  /// <item>
  ///   dateTime = dateTime.Truncate(TimeSpan.FromMinutes(1)); // Truncate to whole minute
  /// </item>
  /// </list>
  /// </summary>
  /// <param name="dateTime">Truncated date time</param>
  /// <param name="timeSpan">Specifies unit of truncation</param>
  /// <returns></returns>
  public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
  {
    if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException
    if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return dateTime; // do not modify "guard" values
    return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
  }

}