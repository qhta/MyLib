using System.ComponentModel;
using System.Globalization;

namespace Qhta.Conversion;

public enum ByteArrayConversionMode
{
  Default,
  Base64Binary,
  HexBinary,
}

public class ArrayTypeConverter : ValueTypeConverter
{
  public ArrayTypeConverter(): base(typeof (byte))
  {
  }

  //public ArrayTypeConverter(Type objectType, string? dataType, string format, CultureInfo culture, ConversionOptions options) :
  //  base(objectType, dataType, format, culture, options)
  //{
  //}

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (value is byte[] bytes)
    {
      if (Mode == ByteArrayConversionMode.Base64Binary)
        return Convert.ToBase64String(bytes);
      if (Mode == ByteArrayConversionMode.HexBinary)
        return Convert.ToHexString(bytes);
    }
    var list = new List<string?>();
    if (value is Array array)
    {
      foreach (var item in array)
        list.Add((string?)base.ConvertTo(context, culture, item, ExpectedType));
    }
    return String.Join(", ", list);
  }

  public ByteArrayConversionMode Mode 
  { 
    get => _Mode;
    set
    {
      _Mode = value;
      if (value != ByteArrayConversionMode.Default)
        ExpectedType = typeof(bool);
    }
  }
  protected ByteArrayConversionMode _Mode;

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
    {
      if (Mode == ByteArrayConversionMode.Base64Binary)
        return Convert.FromBase64String(str);
      if (Mode == ByteArrayConversionMode.HexBinary)
        return Convert.FromHexString(str);
      var strs = str.Split(", ");
      var result = Array.CreateInstance(ExpectedType, strs.Length);
      for (int i = 0; i < strs.Length; i++)
        result.SetValue(base.ConvertFrom(context, culture, strs[i]), i);
      return result;
    }
    return null;
  }
}