namespace Qhta.Conversion;

/// <summary>
/// Converts array of bytes to Base64String and vice/versa.
/// On backward conversion error it tries to convert array of bytes from HexString.
/// </summary>
public class Base64TypeConverter : BaseTypeConverter
{
  /// <summary>
  /// Sets ExpectedType to byte[]
  /// </summary>
  public Base64TypeConverter()
  {
    ExpectedType = typeof(byte[]);
  }

  /// <inheritdoc/>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  /// <inheritdoc/>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <inheritdoc/>
  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (value is byte[] bytes) return Convert.ToBase64String(bytes);
    return null;
  }


  /// <inheritdoc/>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
    {
      try
      {
        return Convert.FromBase64String(str);
      }
      // Base64String is the default encoder for bytes[] type, however sometimes byte array can be encoded with HexString.
      catch 
      {
        return ArrayTypeConverter.FromHexString(str);
      }
    }
    return null;
  }
}