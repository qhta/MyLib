using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Media;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Helper methods to convert dash style to string.
  /// Used in <see cref="DashStyleConverter"/>.
  /// </summary>
  public static class DashStyleUtils
  {
    /// <summary>
    /// Checks if a dash style can be serialized.
    /// </summary>
    /// <param name="style"></param>
    /// <returns></returns>
    public static bool CanSerializeToString(this DashStyle style)
    {
      foreach (var property in typeof(DashStyles).GetProperties(BindingFlags.Public | BindingFlags.Static))
      {
        if (property.GetValue(null)==style)
          return true;
      }
      return false;
    }

    /// <summary>
    /// Checks if a dash style can be converter do string.
    /// </summary>
    /// <param name="style"></param>
    /// <param name="format"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static string? ConvertToString(this DashStyle style, string? format, IFormatProvider? provider)
    {
      foreach (var property in typeof(DashStyles).GetProperties(BindingFlags.Public | BindingFlags.Static))
      {
        if (property.GetValue(null)==style)
          return property.Name;
      }
      return null;
    }

    /// <summary>
    /// Checks if a string can be parsed to dash style.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static DashStyle? Parse(string value, ITypeDescriptorContext? context)
    {
      foreach (var property in typeof(DashStyles).GetProperties(BindingFlags.Public | BindingFlags.Static))
      {
        if (property.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
          return property.GetValue(null) as DashStyle;
      }
      return null;
    }

  }
}

