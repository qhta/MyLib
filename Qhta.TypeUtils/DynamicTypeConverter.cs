using System;
using System.ComponentModel;

namespace Qhta.TypeUtils
{
  /// <summary>
  /// A class that helps using <c>TypeConverter</c> to change an object type in the runtime.
  /// </summary>
  public class DynamicTypeConverter
  {
    private TypeConverter TypeConverter;
    private Type TargetType;

    public DynamicTypeConverter(TypeConverter typeConverter, Type targetType)
    {
      TypeConverter = typeConverter;
      TargetType = targetType;
    }

    public object Convert(object value)
    {
      return TypeConverter.ConvertFrom(value);
    }
  }
}
