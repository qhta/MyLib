using System;
using System.ComponentModel;

namespace Qhta.TypeUtils;

/// <summary>
///   A class that helps using <see cref="System.ComponentModel.TypeConverter"/> to change an object type in the runtime.
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
  ///   Wraps TypeConverter.ConvertFrom and returns <c>null</c> when the input value is <c>null</c>.
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