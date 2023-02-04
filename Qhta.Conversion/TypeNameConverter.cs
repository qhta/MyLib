using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Qhta.Conversion;

public class TypeNameConverter: BaseTypeConverter
{
  public TypeNameConverter()
  {
  }

  public TypeNameConverter(IEnumerable<Type> types)
  {
    if (KnownTypes == null)
      KnownTypes = new();
    foreach (var type in types)
    {
      if (type.FullName != null)
        KnownTypes.Add(type.FullName, type);
    }
  }

  public TypeNameConverter(Dictionary<string, Type> types)
  {
    KnownTypes = types;
  }

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (value is Type type)
      return type.FullName;
    return base.ConvertTo(context, culture, value, destinationType);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value == null)
      return null;
    if (value is string str)
    {
       if (str == String.Empty)
         return null;
       if (KnownTypes != null && KnownTypes.TryGetValue(str, out var type))
         return type;
       type = Type.GetType(str);
       return type;
    }
    return base.ConvertFrom(context, culture, value);
  }
}