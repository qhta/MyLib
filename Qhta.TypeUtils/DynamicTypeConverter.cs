using System;
using System.ComponentModel;

namespace Qhta.TypeUtils;

/// <summary>
///   A class that helps using <c>TypeConverter</c> to change an object type in the runtime.
/// </summary>
public class DynamicTypeConverter
{
  private Type TargetType;
  private readonly TypeConverter TypeConverter;

  /// <summary>
  ///   Initializing constructor
  /// </summary>
  /// <param name="typeConverter"></param>
  /// <param name="targetType"></param>
  public DynamicTypeConverter(TypeConverter typeConverter, Type targetType)
  {
    TypeConverter = typeConverter;
    TargetType = targetType;
  }

  /// <summary>
  ///   Wraps TypeConverter.ConvertFrom
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public object? Convert(object? value)
  {
    if (value != null)
      return TypeConverter.ConvertFrom(value);
    return null;
  }
}