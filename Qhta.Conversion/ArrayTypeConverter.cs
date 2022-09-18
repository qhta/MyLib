using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public class ArrayTypeConverter : ValueTypeConverter
{
  public ArrayTypeConverter(Type objectType) : base(objectType)
  {
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    var list = new List<string?>();
    if (value is Array array)
    {
      foreach (var item in array)
        list.Add((string?)base.ConvertTo(context, culture, item, ObjectType));
    }
    return String.Join(", ", list);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
    {
      var strs = str.Split(", ");
      var result = Array.CreateInstance(ObjectType, strs.Length);
      for (int i = 0; i < strs.Length; i++)
        result.SetValue(base.ConvertFrom(context, culture, strs[i]), i);
      return result;
    }
    return null;
  }
}