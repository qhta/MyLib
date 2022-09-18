using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion
{
  public class ValueTypeConverter : TypeConverter
  {
    protected readonly TypeConverter InternalConverter;
    protected readonly Type ObjectType;

    public ValueTypeConverter(Type objectType)
    {
      InternalConverter = TypeDescriptor.GetConverter(objectType);
      ObjectType = objectType;
    }


    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
      return destinationType == typeof(string);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      return InternalConverter.ConvertToInvariantString(value);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value is string str)
        return InternalConverter.ConvertFromInvariantString(str);
      throw new InvalidOperationException($"ValueTypeConverter supports only conversion to/from string");
    }
  }
}