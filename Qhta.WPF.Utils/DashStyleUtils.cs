using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public static class DashStyleUtils
  {
    public static bool CanSerializeToString(this DashStyle style)
    {
      foreach (var property in typeof(DashStyles).GetProperties(BindingFlags.Public | BindingFlags.Static))
      {
        if (property.GetValue(null)==style)
          return true;
      }
      return false;
    }

    public static string ConvertToString(this DashStyle style, string format, IFormatProvider provider)
    {
      foreach (var property in typeof(DashStyles).GetProperties(BindingFlags.Public | BindingFlags.Static))
      {
        if (property.GetValue(null)==style)
          return property.Name;
      }
      return null;
    }

    public static DashStyle Parse(string value, ITypeDescriptorContext context)
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

