namespace Qhta.Conversion;

/// <summary>
/// Converts TimeSpan values to a string and backward.
/// Uses TimeSpanStyle as defined in System.Globalization.
/// </summary>
public class TimeSpanTypeConverter : TypeConverter
{
  /// <summary>
  ///   Specifies format for ConvertTo method.
  /// </summary>
  public string? Format { get; set; }

  /// <summary>
  /// TimeSpanStyle as defined in System.Globalization.
  /// </summary>
  public TimeSpanStyles TimeSpanStyle { get; set; }

  /// <inheritdoc/>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  /// <inheritdoc/>
  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (destinationType == typeof(string) && value is TimeSpan ts)
    {
      var format = Format;
      if (format == "D")
        return DurationToString(ts);
      return ts.ToString(format, CultureInfo.InvariantCulture);
    }
    return base.ConvertTo(context, culture, value, destinationType);
  }

  /// <inheritdoc/>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <inheritdoc/>
  public new object? ConvertFrom(object value)
  {
    return ConvertFrom(null, null, value);
  }

  /// <inheritdoc/>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (value == null)
      return null;
    if (value is string str)
    {
      str = str.Trim();
      if (str == String.Empty)
        return null;
      var style = TimeSpanStyle;
      var format = Format;
      TimeSpan result;
      if (format == "D")
        result = ParseDuration(str);
      else if (format != null)
        result = TimeSpan.ParseExact(str, format, CultureInfo.InvariantCulture, style);
      else
        result = TimeSpan.Parse(str, CultureInfo.InvariantCulture);
      return result;
    }
    return base.ConvertFrom(context, culture, value);
  }

  /// <summary>
  /// Converts duration to string.
  /// </summary>
  /// <param name="ts"></param>
  /// <returns></returns>
  public string DurationToString(TimeSpan ts)
  {
    var sb = new StringBuilder();
    if (ts.TotalMilliseconds < 0)
      sb.Append('-');
    sb.Append('P');
    var started = false;
    var days = Math.Abs(ts.Days);
    if (days > 0)
    {
      started = true;
      sb.Append(days + "D");
    }
    var hours = Math.Abs(ts.Hours);
    if (hours > 0 || started)
    {
      started = true;
      sb.Append(hours.ToString("D") + "H");
    }
    var minutes = Math.Abs(ts.Minutes);
    if (minutes > 0 || started)
    {
      started = true;
      sb.Append(minutes.ToString("D") + "M");
    }
    var seconds = Math.Abs(ts.Seconds);
    var milliseconds = Math.Abs(ts.Milliseconds);
    if (seconds > 0 || milliseconds > 0 || started)
    {
      started = true;
      sb.Append(seconds.ToString(CultureInfo.InvariantCulture));
      if (milliseconds > 0)
        sb.Append("." + milliseconds.ToString("D3"));
      sb.Append('S');
    }
    return sb.ToString();
  }

  /// <summary>
  /// Parses duration string and returns TimeSpan result.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public TimeSpan ParseDuration(string str)
  {
    var neg = false;
    var years = 0;
    var months = 0;
    var days = 0;
    var hours = 0;
    var minutes = 0;
    var seconds = 0;
    var milliseconds = 0;
    var i = 0;
    int j;
    if (str.Length > i)
    {
      if (str[i] == '-')
      {
        neg = true;
        i++;
      }
      if (str[i] == 'P')
        i++;
      else
        throw new InvalidOperationException($"Invalid duration string {str}");
      if (str.IndexOf('-', i) > 0)
        throw new InvalidOperationException($"Invalid duration string {str}");
      if (str.Length > i)
      {
        j = str.IndexOf('Y', i);
        if (j > i)
        {
          if (!Int32.TryParse(str.Substring(i, j - i), out years))
            throw new InvalidOperationException($"Invalid duration string {str}");
          i = j + 1;
        }
      }
      if (str.Length > i)
      {
        j = str.IndexOf('M', i);
        if (j > i)
          if (Int32.TryParse(str.Substring(i, j - i), out months))
            i = j + 1;
      }
      if (str.Length > i)
      {
        j = str.IndexOf('D', i);
        if (j > i)
        {
          if (!Int32.TryParse(str.Substring(i, j - i), out days))
            throw new InvalidOperationException($"Invalid duration string {str}");
          i = j + 1;
        }
      }
      if (str.Length > i)
      {
        j = str.IndexOf('H', i);
        if (j > i)
        {
          if (!Int32.TryParse(str.Substring(i, j - i), out hours))
            throw new InvalidOperationException($"Invalid duration string {str}");
          i = j + 1;
        }
      }
      if (str.Length > i)
      {
        j = str.IndexOf('M', i);
        if (j > i)
        {
          if (!Int32.TryParse(str.Substring(i, j - i), out minutes))
            throw new InvalidOperationException($"Invalid duration string {str}");
          i = j + 1;
        }
      }
      var hasMilliseconds = false;
      if (str.Length > i)
      {
        j = str.IndexOf('.', i);
        if (j > i)
        {
          if (!Int32.TryParse(str.Substring(i, j - i), out seconds))
            throw new InvalidOperationException($"Invalid duration string {str}");
          i = j + 1;
          hasMilliseconds = true;
        }
      }
      if (str.Length > i)
      {
        j = str.IndexOf('S', i);
        if (j > i)
        {
          if (!Int32.TryParse(str.Substring(i, j - i), out milliseconds))
            throw new InvalidOperationException($"Invalid duration string {str}");
          //i = j + 1;
          if (!hasMilliseconds)
          {
            seconds = milliseconds;
            milliseconds = 0;
          }
        }
      }
    }
    var result = new TimeSpan(years * 365 + months * 30 + days, hours, minutes, seconds, milliseconds);
    if (neg || TimeSpanStyle.HasFlag(TimeSpanStyles.AssumeNegative))
      result = -result;
    return result;
  }
}