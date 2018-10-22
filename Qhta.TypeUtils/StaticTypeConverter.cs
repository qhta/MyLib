using System;
using System.ComponentModel;

namespace Qhta.TypeUtils
{
  /// <summary>
  /// A static class that converts object type
  /// </summary>
  public class StaticTypeConverter
  {
    public static object ConvertToString(object value, TypeConverter typeConverter, Type targetType)
    {
      object result = (string)typeConverter.ConvertTo(value, targetType);
      return result;
    }
  }
}
