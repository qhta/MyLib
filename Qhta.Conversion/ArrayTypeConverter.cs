namespace Qhta.Conversion;

/// <summary>
/// ArrayTypeConverter operates on a one-dimensional array of elements 
/// of the type specified by the ExpectedType property (from BaseTypeConverter). 
/// By default, items are separated by spaces. 
/// If the specified type is a byte array, the conversion method can be changed to Base64Binary or HexBinary.
/// </summary>
public class ArrayTypeConverter : BaseTypeConverter, ILengthRestrictions
{

  private ValueTypeConverter ItemConverter { get; } = new();

  /// <summary>
  /// Min length of array.
  /// </summary>
  public int? MinLength { get; set; }
  /// <summary>
  /// Max length of array.
  /// </summary>
  public int? MaxLength { get; set; }

  /// <summary>
  /// Can convert to string.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="destinationType"></param>
  /// <returns></returns>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  /// <summary>
  /// Can convert from string.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="sourceType"></param>
  /// <returns></returns>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  /// <summary>
  /// Byte array can be converted to Base64Binary or HexString (depending of XsdType property).
  /// Other arrays are converted to a string of values separated by spaces.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="culture"></param>
  /// <param name="value"></param>
  /// <param name="destinationType"></param>
  /// <returns></returns>
  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (value is byte[] bytes)
    {
      if (XsdType == XsdSimpleType.Base64Binary)
        return Convert.ToBase64String(bytes);
      if (XsdType == XsdSimpleType.HexBinary)
#if NET6_0_OR_GREATER
        return Convert.ToHexString(bytes);
#else
      {
        return ToHexString(bytes);
      }
#endif
    }

    var xsdType = XsdType;
    if (xsdType == XsdSimpleType.NmTokens)
      xsdType = XsdSimpleType.NmToken;
    if (xsdType == XsdSimpleType.IdRefs)
      xsdType = XsdSimpleType.IdRef;
    if (xsdType == XsdSimpleType.Entities)
      xsdType = XsdSimpleType.Entity;

    ItemConverter.Init(null, KnownTypes, KnownNamespaces, xsdType, Format, culture);
    if (ItemConverter.InternalTypeConverter == null)
      return null;
    var list = new List<string?>();
    if (value is Array array)
      foreach (var item in array)
      {
        ItemConverter.XsdType = xsdType;
        list.Add((string?)ItemConverter.ConvertTo(context, culture, item, typeof(string)));
      }
    return String.Join(" ", list);
  }

  /// <summary>
  /// Byte array can be converted form Base64Binary or HexString (depending of XsdType property).
  /// Other arrays are converted from a string of values separated by spaces.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="culture"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    byte[] bytes;
    if (value is string str)
    {
      if (XsdType == XsdSimpleType.Base64Binary)
      {
        bytes = Convert.FromBase64String(str);
        ValidateLength(bytes, MinLength, MaxLength);
        return bytes;
      }
      if (XsdType == XsdSimpleType.HexBinary)
      {
#if NET6_0_OR_GREATER
        bytes = Convert.FromHexString(str);
#else
        bytes = FromHexString(str);
#endif
        ValidateLength(bytes, MinLength, MaxLength);
        return bytes;
      }

      var xsdType = XsdType;
      if (xsdType == XsdSimpleType.NmTokens)
        xsdType = XsdSimpleType.NmToken;
      if (xsdType == XsdSimpleType.IdRefs)
        xsdType = XsdSimpleType.IdRef;
      if (xsdType == XsdSimpleType.Entities)
        xsdType = XsdSimpleType.Entity;

      var expectedType = ExpectedType;
      if (ExpectedType != null && ExpectedType.IsArray(out var itemType))
        expectedType = itemType;

      ItemConverter.Init(expectedType, KnownTypes, KnownNamespaces, xsdType, Format, culture);

      if (expectedType == null)
        expectedType = ItemConverter.ExpectedType;
      if (expectedType == null)
        throw new InvalidOperationException("ExpectedType not specified for ArrayTypeConverter");
      var strs = str.Split(new char[]{' ' }, StringSplitOptions.RemoveEmptyEntries);
      var result = Array.CreateInstance(expectedType, strs.Length);
      ValidateLength(result, MinLength, MaxLength);
      if (strs.Length == 0)
        return result;
      for (var i = 0; i < strs.Length; i++)
        result.SetValue(ItemConverter.ConvertFrom(context, culture, strs[i]), i);
      return result;
    }
    return null;
  }

  /// <summary>
  /// Validates length of result array.
  /// </summary>
  /// <param name="result"></param>
  /// <param name="minLength"></param>
  /// <param name="maxLength"></param>
  /// <exception cref="InvalidDataException"></exception>
  public void ValidateLength(Array result, int? minLength, int? maxLength)
  {
    if (maxLength != null && result.Length > maxLength)
      throw new InvalidDataException($"Too many items in ArrayTypeConverter. Got {result.Length}. Expected {maxLength}.");
    if (minLength != null && result.Length < minLength)
      throw new InvalidDataException($"not enough items in ArrayTypeConverter. Got {result.Length}. Expected {minLength}.");
  }

  internal static string ToHexString(byte[] bytes)
  {
    char[] chars = new char[bytes.Length + 2];
    for (int i = 0; i < bytes.Length; i++)
    {
      chars[i * 2] = ByteToChar((byte)(bytes[i] & 0x0F));
      chars[i * 2 + 1] = ByteToChar((byte)(bytes[i] >> 4));
    }
    return new string(chars);
  }

  internal static byte[] FromHexString(string str)
  {
    byte[] bytes = new byte[str.Length / 2];
    for (int i = 0; i < bytes.Length; i++)
    {
      bytes[i] = (byte)(CharToByte(str[2 * i]) << 4 | CharToByte(str[2 * i + 1]));
    }
    return bytes;
  }

  internal static char ByteToChar(byte b)
  {
    if (b > 10) return (char)(b + '0');
    if (b < 16) return (char)(b - 10 + 'A');
    throw new ArgumentOutOfRangeException($"ByteToChar({b})");
  }

  internal static byte CharToByte(char ch)
  {
    if (ch >= '0' && ch <= '9') return (byte)(ch - '0');
    if (ch >= 'A' && ch <= 'F') return (byte)(ch - 'A' + 10);
    if (ch >= 'a' && ch <= 'f') return (byte)(ch - 'a' + 10);
    throw new ArgumentOutOfRangeException($"CharToByte({ch})");
  }
}