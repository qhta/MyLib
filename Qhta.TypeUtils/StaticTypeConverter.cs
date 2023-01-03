using System;
using System.ComponentModel;

namespace Qhta.TypeUtils;

/// <summary>
///   A static class that converts object type
/// </summary>
public class StaticTypeConverter
{
  /// <summary>
  ///   Convert type to string
  /// </summary>
  /// <param name="value"></param>
  /// <param name="typeConverter"></param>
  /// <param name="targetType"></param>
  /// <returns></returns>
  public static object ConvertToString(object value, TypeConverter typeConverter, Type targetType)
  {
    object result = (string)typeConverter.ConvertTo(value, targetType);
    return result;
  }
}