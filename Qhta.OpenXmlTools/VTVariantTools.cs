using System;
using System.IO;
using System.Text;

namespace Qhta.OpenXmlTools;
/// <summary>
/// Tools for working with variant types.
/// </summary>
public static class VTVariantTools
{

  /// <summary>
  /// Get the value of the VTVariant element.
  /// </summary>
  /// <param name="element">Source element</param>
  /// <returns>simple type result</returns>
  /// <exception cref="InvalidDataException"></exception>
  public static object? GetVariantValue(this DX.OpenXmlElement? element)
  {
    if (element == null)
      return null;
    var typeName = element.GetType().Name;
    switch (typeName)
    {
      case "VTNull":
        return null;
      case "VTEmpty":
        return new Object();
      case "VTBool":
        return bool.Parse(((DXVT.VTBool)element).InnerText);
      case "VTLPSTR":
        return ((DXVT.VTLPSTR)element).InnerText;
      case "VTLPWSTR":
        return ((DXVT.VTLPWSTR)element).InnerText;
      case "VTBString":
        return DecodeBstr(((DXVT.VTBString)element).InnerText);
      case "VTInteger":
        return Int32.Parse(((DXVT.VTInteger)element).InnerText);
      case "VTUnsignedInteger":
        return UInt32.Parse(((DXVT.VTUnsignedInteger)element).InnerText);
      case "VTInt32":
        return Int32.Parse(((DXVT.VTInt32)element).InnerText);
      case "VTInt64":
        return Int64.Parse(((DXVT.VTInt64)element).InnerText);
      case "VTUnsignedInt32":
        return UInt32.Parse(((DXVT.VTUnsignedInt32)element).InnerText);
      case "VTUnsignedInt64":
        return UInt64.Parse(((DXVT.VTUnsignedInt64)element).InnerText);
      case "VTByte":
        return SByte.Parse(((DXVT.VTByte)element).InnerText);
      case "VTUnsignedByte":
        return Byte.Parse(((DXVT.VTUnsignedByte)element).InnerText);
      case "VTShort":
        return Int16.Parse(((DXVT.VTShort)element).InnerText);
      case "VTUnsignedShort":
        return UInt16.Parse(((DXVT.VTUnsignedShort)element).InnerText);
      case "VTDate":
        return DateTime.Parse(((DXVT.VTDate)element).InnerText);
      case "VTFileTime":
        return DateTime.Parse(((DXVT.VTFileTime)element).InnerText);
      case "VTFloat":
        return Single.Parse(((DXVT.VTFloat)element).InnerText, CultureInfo.InvariantCulture);
      case "VTDouble":
        return Double.Parse(((DXVT.VTDouble)element).InnerText, CultureInfo.InvariantCulture);
      case "VTCurrency":
        return Decimal.Parse(((DXVT.VTCurrency)element).InnerText, CultureInfo.InvariantCulture);
      case "VTDecimal":
        return Decimal.Parse(((DXVT.VTDecimal)element).InnerText, CultureInfo.InvariantCulture);
      case "VTError":
        return Int32.Parse(((DXVT.VTError)element).InnerText, NumberStyles.AllowHexSpecifier);
      case "VTClassId":
        return Guid.Parse(((DXVT.VTClassId)element).InnerText);
      case "VTBlob":
        return GetData((DXVT.VTBlob)element);
      case "VTOBlob":
        return GetData((DXVT.VTOBlob)element);
      case "VTStreamData":
        return GetData((DXVT.VTStreamData)element);
      case "VTOStreamData":
        return GetData((DXVT.VTOStreamData)element);
      case "VTVStreamData":
        return GetData((DXVT.VTVStreamData)element);
      case "VTStorage":
        return GetData((DXVT.VTStorage)element);
      case "VTOStorage":
        return GetData((DXVT.VTOStorage)element);
      case "VTArray":
        return GetData((DXVT.VTArray)element);
      case "VTVector":
        return GetData((DXVT.VTVector)element);
      case "Variant":
        return GetVariantValue(((DXVT.Variant)element).InnerVariant)
               ?? GetVariantValue(((DXVT.Variant)element).FirstChild);
      case "OpenXmlUnknownElement":
        switch (element.LocalName)
        {
          case "lpwstr":
            return element.InnerText;
        }
        throw new InvalidDataException($"Variant type {element.LocalName} in GetVariantValue not supported");
    }
    throw new InvalidDataException($"Variant type {typeName} in GetVariantValue not supported");
  }

  /// <summary>
  /// Decodes the BSTR string
  /// </summary>
  /// <param name="bstr"></param>
  /// <returns></returns>
  /// <remarks>
  /// Unicode characters that cannot be directly represented in XML as defined by the XML 1.0 specification,
  /// shall be escaped using the Unicode numerical character representation escape character format _xHHHH_,
  /// where H represents a hexadecimal character in the character's value.
  /// [Example: The Unicode character 8 is not permitted in an XML 1.0 document, so it shall be escaped as _x0008_. end example]
  /// To store the literal form of an escape sequence, the initial underscore shall itself be escaped (i.e. stored as _x005F_).
  /// [Example: The string literal _x0008_ would be stored as _x005F_x0008_. end example]
  /// </remarks>
  public static string? DecodeBstr(this string? bstr)
  {
    if (bstr == null)
      return null;
    var k = bstr.IndexOf("_x");
    while (k >= 0)
    {
      if (k + 2 >= bstr.Length)
        break;
      int j = bstr.IndexOf("_", k + 2);
      if (j < 0)
        break;
      var hex = bstr.Substring(k + 2, j - k - 2);
      var ch = (char)Convert.ToInt32(hex, 16);
      bstr = bstr.Substring(0, k) + ch + bstr.Substring(j + 1);
      k = bstr.IndexOf("_x", k + 1);
    }
    return bstr;
  }

  /// <summary>
  /// Decodes the BSTR string
  /// </summary>
  /// <param name="bstr"></param>
  /// <returns></returns>
  /// <remarks>
  /// Unicode characters that cannot be directly represented in XML as defined by the XML 1.0 specification,
  /// shall be escaped using the Unicode numerical character representation escape character format _xHHHH_,
  /// where H represents a hexadecimal character in the character's value.
  /// [Example: The Unicode character 8 is not permitted in an XML 1.0 document, so it shall be escaped as _x0008_. end example]
  /// To store the literal form of an escape sequence, the initial underscore shall itself be escaped (i.e. stored as _x005F_).
  /// [Example: The string literal _x0008_ would be stored as _x005F_x0008_. end example]
  /// </remarks>
  public static string? EncodeBstr(this string? bstr)
  {
    if (bstr == null)
      return null;
    StringBuilder encodedString = new StringBuilder();
    foreach (char ch in bstr)
    {
      if (NeedsEncoding(ch))
      {
        encodedString.AppendFormat("_x{0:X4}_", (int)ch);
      }
      else
      {
        encodedString.Append(ch);
      }
    }
    return encodedString.ToString();

  }
  private static bool NeedsEncoding(char ch)
  {
    return ch < 32 || ch > 126 || ch == '_' || ch == '<' || ch == '>' || ch == '&' || ch == '"';
  }

  /// <summary>
  /// Get byte array from the <c>VTStreamData</c> element.
  /// </summary>
  /// <param name="data"></param>
  /// <returns></returns>
  public static byte[]? GetData(this DXVT.VTStreamData? data)
  {
    if (data == null)
      return null;
    if (String.IsNullOrEmpty(data.InnerText))
      return null;
    var bytes = Convert.FromBase64String(data.InnerText);
    return bytes;
  }

  /// <summary>
  /// Get byte array from the <c>VTOStreamData</c> element.
  /// </summary>
  /// <param name="data"></param>
  /// <returns></returns>
  public static byte[]? GetData(this DXVT.VTOStreamData? data)
  {
    if (data == null)
      return null;
    if (String.IsNullOrEmpty(data.InnerText))
      return null;
    var bytes = Convert.FromBase64String(data.InnerText);
    return bytes;
  }

  /// <summary>
  /// Get byte array from the <c>VTVStreamData</c> element.
  /// </summary>
  /// <param name="data"></param>
  /// <returns></returns>
  public static byte[]? GetData(this DXVT.VTVStreamData? data)
  {
    if (data == null)
      return null;
    if (String.IsNullOrEmpty(data.InnerText))
      return null;
    var bytes = Convert.FromBase64String(data.InnerText);
    return bytes;
  }

  /// <summary>
  /// Get byte array from the <c>VTStorage</c> element.
  /// </summary>
  /// <param name="data"></param>
  /// <returns></returns>
  public static byte[]? GetData(this DXVT.VTStorage? data)
  {
    if (data == null)
      return null;
    if (String.IsNullOrEmpty(data.InnerText))
      return null;
    var bytes = Convert.FromBase64String(data.InnerText);
    return bytes;
  }


  /// <summary>
  /// Get byte array from the <c>VTOStorage</c> element.
  /// </summary>
  /// <param name="data"></param>
  /// <returns></returns>
  public static byte[]? GetData(this DXVT.VTOStorage? data)
  {
    if (data == null)
      return null;
    if (String.IsNullOrEmpty(data.InnerText))
      return null;
    var bytes = Convert.FromBase64String(data.InnerText);
    return bytes;
  }

  /// <summary>
  /// Creates the <c>VTArray</c> element from the array data.
  /// </summary>
  /// <param name="array"></param>
  /// <returns></returns>
  /// <exception cref="InvalidDataException"></exception>
  public static DXVT.VTArray? CreateVTArray(this Array? array)
  {
    if (array == null)
      return null;
    // Get the type of the array
    Type arrayType = array.GetType();

    // Get the element type of the array
    Type? elementType = arrayType.GetElementType();
    var baseType = TypeToArrayBaseType[elementType];

    var lBounds = new int[array.Rank];
    var uBounds = new int[array.Rank];
    for (int i = 0; i < array.Rank; i++)
    {
      lBounds[i] = array.GetLowerBound(i);
      uBounds[i] = array.GetUpperBound(i);
    }
    var vArray = new DXVT.VTArray
    {
      BaseType = baseType,
    };
    if (lBounds.Length == 1)
      vArray.LowerBounds = lBounds[0];
    if (uBounds.Length == 1)
      vArray.UpperBounds = uBounds[0];
    var size = array.Length;
    for (int i = 0; i < size; i++)
    {
      var item = array.GetValue(i);
      vArray.AppendChild(CreateVariant(ArrayBaseTypeToVTType[baseType], item));
    }
    return vArray;
  }

  private static int[]? DecodeVTArrayBounds(string? str)
  {
    if (str == null)
      return null;
    var ss = str.Split(',');
    var result = new int[ss.Length];
    for (int i = 0; i < ss.Length; i++)
      result[i] = Convert.ToInt32(ss[i]);
    return result;
  }

  /// <summary>
  /// Get the array from the <c>VTArray</c> element.
  /// </summary>
  /// <param name="vArray"></param>
  /// <returns></returns>
  /// <exception cref="InvalidDataException"></exception>
  public static object? GetData(this DXVT.VTArray? vArray)
  {
    if (vArray == null)
      return null;
    if (vArray.BaseType == null)
      throw new InvalidDataException("Unknown VTArray base type");
    var baseType = ArrayBaseTypeToType[vArray.BaseType];
    var lBounds = DecodeVTArrayBounds(vArray.LowerBounds?.InnerText);
    var uBounds = DecodeVTArrayBounds(vArray.UpperBounds?.InnerText);
    Array array;
    if (lBounds != null && uBounds != null && lBounds.Length == uBounds.Length)
    {
      var lengths = new int[uBounds.Length];
      for (int i = 0; i < lengths.Length; i++)
        lengths[i] = uBounds[i] - lBounds[i] + 1;
      array = Array.CreateInstance(baseType, lengths, lBounds);
    }
    else
    {
      var size = vArray.Elements().Count();
      array = Array.CreateInstance(baseType, size);
    }
    int index = 0;
    foreach (var itemElement in vArray.Elements())
    {
      var item = GetVariantValue(itemElement);
      array.SetValue(item, index++);
    }
    return array;
  }

  /// <summary>
  /// Dictionary mapping the <c>ArrayBaseValues</c> to the data type.
  /// </summary>
  public static readonly Dictionary<DXVT.ArrayBaseValues, Type> ArrayBaseTypeToType = new()
  {
    { DXVT.ArrayBaseValues.Bool, typeof(bool) },
    { DXVT.ArrayBaseValues.Bstr, typeof(string) },
    { DXVT.ArrayBaseValues.Currency, typeof(decimal) },
    { DXVT.ArrayBaseValues.Date, typeof(DateTime) },
    { DXVT.ArrayBaseValues.Decimal, typeof(decimal) },
    { DXVT.ArrayBaseValues.EightBytesReal, typeof(double) },
    { DXVT.ArrayBaseValues.Error, typeof(string) },
    { DXVT.ArrayBaseValues.FourBytesReal, typeof(float) },
    { DXVT.ArrayBaseValues.FourBytesSignedInteger, typeof(int) },
    { DXVT.ArrayBaseValues.FourBytesUnsignedInteger, typeof(uint) },
    { DXVT.ArrayBaseValues.Integer, typeof(long) },
    { DXVT.ArrayBaseValues.OneByteSignedInteger, typeof(sbyte) },
    { DXVT.ArrayBaseValues.OneByteUnsignedInteger, typeof(byte) },
    { DXVT.ArrayBaseValues.TwoBytesSignedInteger, typeof(short) },
    { DXVT.ArrayBaseValues.TwoBytesUnsignedInteger, typeof(ushort) },
    { DXVT.ArrayBaseValues.UnsignedInteger, typeof(ulong) },
    { DXVT.ArrayBaseValues.Variant, typeof(object) },
  };

  /// <summary>
  /// Dictionary mapping data type to the <c>ArrayBaseValues</c>.
  /// </summary>
  public static readonly Dictionary<Type, DXVT.ArrayBaseValues> TypeToArrayBaseType = new()
  {
    { typeof(bool), DXVT.ArrayBaseValues.Bool },
    { typeof(byte), DXVT.ArrayBaseValues.OneByteUnsignedInteger },
    { typeof(DateTime), DXVT.ArrayBaseValues.Date },
    { typeof(decimal), DXVT.ArrayBaseValues.Currency },
    { typeof(double), DXVT.ArrayBaseValues.EightBytesReal },
    { typeof(float), DXVT.ArrayBaseValues.FourBytesReal },
    { typeof(int), DXVT.ArrayBaseValues.FourBytesSignedInteger },
    { typeof(long), DXVT.ArrayBaseValues.Integer },
    { typeof(object), DXVT.ArrayBaseValues.Variant },
    { typeof(sbyte), DXVT.ArrayBaseValues.OneByteSignedInteger },
    { typeof(short), DXVT.ArrayBaseValues.TwoBytesSignedInteger },
    { typeof(string), DXVT.ArrayBaseValues.Bstr },
    { typeof(uint), DXVT.ArrayBaseValues.FourBytesUnsignedInteger },
    { typeof(ulong), DXVT.ArrayBaseValues.UnsignedInteger },
    { typeof(ushort), DXVT.ArrayBaseValues.TwoBytesUnsignedInteger },
  };

  /// <summary>
  /// Dictionary mapping the <c>ArrayBaseValues</c> to the Variant type.
  /// </summary>
  public static readonly Dictionary<DXVT.ArrayBaseValues, Type> ArrayBaseTypeToVTType = new()
  {
    { DXVT.ArrayBaseValues.Bool, typeof(DXVT.VTBool) },
    { DXVT.ArrayBaseValues.Bstr, typeof(DXVT.VTBString) },
    { DXVT.ArrayBaseValues.Currency, typeof(DXVT.VTDecimal) },
    { DXVT.ArrayBaseValues.Date, typeof(DXVT.VTFileTime) },
    { DXVT.ArrayBaseValues.Decimal, typeof(DXVT.VTDecimal) },
    { DXVT.ArrayBaseValues.EightBytesReal, typeof(DXVT.VTDouble) },
    { DXVT.ArrayBaseValues.Error, typeof(DXVT.VTError) },
    { DXVT.ArrayBaseValues.FourBytesReal, typeof(DXVT.VTFloat) },
    { DXVT.ArrayBaseValues.FourBytesSignedInteger, typeof(DXVT.VTInt32) },
    { DXVT.ArrayBaseValues.FourBytesUnsignedInteger, typeof(DXVT.VTUnsignedInt32) },
    { DXVT.ArrayBaseValues.Integer, typeof(DXVT.VTInt64) },
    { DXVT.ArrayBaseValues.OneByteSignedInteger, typeof(DXVT.VTByte) },
    { DXVT.ArrayBaseValues.OneByteUnsignedInteger, typeof(DXVT.VTUnsignedByte) },
    { DXVT.ArrayBaseValues.TwoBytesSignedInteger, typeof(DXVT.VTShort) },
    { DXVT.ArrayBaseValues.TwoBytesUnsignedInteger, typeof(DXVT.VTUnsignedShort) },
    { DXVT.ArrayBaseValues.UnsignedInteger, typeof(DXVT.VTUnsignedInt64) },
    { DXVT.ArrayBaseValues.Variant, typeof(DXVT.Variant) },
  };

  /// <summary>
  /// Dictionary mapping the <c>VectorBaseValues</c> type to the data type.
  /// </summary>
  public static readonly Dictionary<DXVT.VectorBaseValues, Type> VectorBaseTypeToType = new()
  {
    { DXVT.VectorBaseValues.Bool, typeof(bool) },
    { DXVT.VectorBaseValues.Bstr, typeof(string) },
    { DXVT.VectorBaseValues.ClassId, typeof(Guid)},
    { DXVT.VectorBaseValues.ClipboardData, typeof(object) },
    { DXVT.VectorBaseValues.Currency, typeof(string) },
    { DXVT.VectorBaseValues.Date, typeof(DateTime) },
    { DXVT.VectorBaseValues.EightBytesReal, typeof(double) },
    { DXVT.VectorBaseValues.Error, typeof(string) },
    { DXVT.VectorBaseValues.FourBytesReal, typeof(float) },
    { DXVT.VectorBaseValues.FourBytesSignedInteger, typeof(int) },
    { DXVT.VectorBaseValues.FourBytesUnsignedInteger, typeof(uint) },
    { DXVT.VectorBaseValues.Lpstr, typeof(string) },
    { DXVT.VectorBaseValues.Lpwstr, typeof(string) },
    { DXVT.VectorBaseValues.OneByteSignedInteger, typeof(sbyte) },
    { DXVT.VectorBaseValues.OneByteUnsignedInteger, typeof(byte) },
    { DXVT.VectorBaseValues.TwoBytesSignedInteger, typeof(short) },
    { DXVT.VectorBaseValues.TwoBytesUnsignedInteger, typeof(ushort) },
    { DXVT.VectorBaseValues.Variant, typeof(object) },
  };


  /// <summary>
  /// Dictionary mapping the variant type to the data type.
  /// </summary>
  public static readonly Dictionary<Type, Type> VTTypeToType = new()
  {
    { typeof(DXVT.VTBool), typeof(bool)},
    { typeof(DXVT.VTLPSTR), typeof(string)},
    { typeof(DXVT.VTLPWSTR), typeof(string)},
    { typeof(DXVT.VTBString), typeof(string)},
    { typeof(DXVT.VTInteger), typeof(int)},
    { typeof(DXVT.VTUnsignedInteger), typeof(uint)},
    { typeof(DXVT.VTInt32), typeof(int)},
    { typeof(DXVT.VTInt64), typeof(long)},
    { typeof(DXVT.VTUnsignedInt32), typeof(uint)},
    { typeof(DXVT.VTUnsignedInt64), typeof(ulong)},
    { typeof(DXVT.VTByte), typeof(sbyte)},
    { typeof(DXVT.VTUnsignedByte), typeof(byte)},
    { typeof(DXVT.VTShort), typeof(short)},
    { typeof(DXVT.VTUnsignedShort), typeof(ushort)},
    { typeof(DXVT.VTDate), typeof(DateTime)},
    { typeof(DXVT.VTFileTime), typeof(DateTime)},
    { typeof(DXVT.VTFloat), typeof(float)},
    { typeof(DXVT.VTDouble), typeof(double)},
    { typeof(DXVT.VTCurrency), typeof(decimal)},
    { typeof(DXVT.VTDecimal), typeof(decimal)},
    { typeof(DXVT.VTError), typeof(int)},
    { typeof(DXVT.VTClassId), typeof(Guid)},
    { typeof(DXVT.VTBlob), typeof(byte[])},
    { typeof(DXVT.VTOBlob), typeof(byte[])},
    { typeof(DXVT.VTStreamData), typeof(byte[])},
    { typeof(DXVT.VTOStreamData), typeof(byte[])},
    { typeof(DXVT.VTVStreamData), typeof(byte[])},
    { typeof(DXVT.VTStorage), typeof(byte[])},
    { typeof(DXVT.VTOStorage), typeof(byte[])},
    { typeof(DXVT.VTArray), typeof(Array)},
    { typeof(DXVT.VTVector), typeof(Array)},
    { typeof(DXVT.Variant), typeof(object)},
  };



  /// <summary>
  /// Get the array from the <c>VTVector</c> element.
  /// </summary>
  /// <param name="vector"></param>
  /// <returns></returns>
  /// <exception cref="InvalidDataException"></exception>
  public static object? GetData(this DXVT.VTVector? vector)
  {
    if (vector == null)
      return null;
    if (vector.BaseType == null)
      throw new InvalidDataException("Unknown VTVector base type");
    var baseType = (vector.BaseType.HasValue) ?
      VectorBaseTypeToType[vector.BaseType] : typeof(object);
    var size = Convert.ToInt32(vector.Size?.Value ?? (uint)vector.Elements().Count());
    var array = Array.CreateInstance(baseType, size);
    int index = 0;
    foreach (var itemElement in vector.Elements())
    {
      var item = GetVariantValue(itemElement);
      array.SetValue(item, index++);
    }
    return array;
  }

  /// <summary>
  /// Get byte array from the VTBlob element.
  /// </summary>
  /// <param name="blob">Source blob element</param>
  /// <returns>byte array or null</returns>
  /// <exception cref="InvalidDataException"></exception>
  /// <remarks>
  /// Blob must be encoded in base64 with the first 4 bytes representing the length of the data.
  /// </remarks>
  public static byte[]? GetData(DXVT.VTBlob? blob)
  {
    if (blob == null)
      return null;
    if (String.IsNullOrEmpty(blob.InnerText))
      return null;
    var bytes = Convert.FromBase64String(blob.InnerText);
    if (bytes.Length >= 4)
    {
      var countBytes = new byte[4];
      Array.Copy(bytes, 0, countBytes, 0, countBytes.Length);
      var count = BitConverter.ToInt32(countBytes, 0);
      if (count == bytes.Length - 4)
      {
        var dataBytes = new byte[count];
        Array.Copy(bytes, 4, dataBytes, 0, dataBytes.Length);
        return dataBytes;
      }
    }
    throw new InvalidDataException("Non-conformed VTBlob data");
  }

  /// <summary>
  /// Create a VTBlob element from a byte array.
  /// Result is encoded in base64 with the first 4 bytes representing the length of the data.
  /// </summary>
  /// <param name="data">Source byte array</param>
  /// <returns>VTBlob result</returns>
  public static DXVT.VTBlob CreateBlob(byte[] data)
  {
    var dataBytes = new byte[data.Length + 4];
    Array.Copy(data, 0, dataBytes, 4, data.Length);
    var countBytes = BitConverter.GetBytes(data.Length);
    Array.Copy(countBytes, 0, dataBytes, 0, 4);
    return new DXVT.VTBlob(Convert.ToBase64String(dataBytes));
  }

  /// <summary>
  /// Get byte array from the VTOBlob element.
  /// </summary>
  /// <param name="blob">Source blob element</param>
  /// <returns>byte array or null</returns>
  /// <exception cref="InvalidDataException"></exception>
  /// <remarks>
  /// Blob must be encoded in base64 with the first 4 bytes representing the length of the data.
  /// </remarks>
  public static byte[] GetData(this DXVT.VTOBlob blob)
  {
    var bytes = Convert.FromBase64String(blob.InnerText);
    if (bytes.Length >= 4)
    {
      var countBytes = new byte[4];
      Array.Copy(bytes, 0, countBytes, 0, countBytes.Length);
      var count = BitConverter.ToInt32(countBytes, 0);
      if (count == bytes.Length - 4)
      {
        var dataBytes = new byte[count];
        Array.Copy(bytes, 4, dataBytes, 0, dataBytes.Length);
        return dataBytes;
      }
    }
    throw new InvalidDataException("Non-conformed VTOBlob data");
  }

  /// <summary>
  /// Create a VTOBlob element from a byte array.
  /// Result is encoded in base64 with the first 4 bytes representing the length of the data.
  /// </summary>
  /// <param name="data">Source byte array</param>
  /// <returns>VTOBlob result</returns>
  public static DXVT.VTBlob CreateOBlob(byte[] data)
  {
    var dataBytes = new byte[data.Length + 4];
    Array.Copy(data, 0, dataBytes, 4, data.Length);
    var countBytes = BitConverter.GetBytes(data.Length);
    Array.Copy(countBytes, 0, dataBytes, 0, 4);
    return new DXVT.VTBlob(Convert.ToBase64String(dataBytes));
  }

  /// <summary>
  /// Create variant element from the value.
  /// </summary>
  /// <param name="value">value to set </param>
  /// <param name="variantType">optional type of the variant</param>
  /// <param name="format">optional format of the value</param>
  /// <returns>variant with the specified value</returns>
  /// <exception cref="InvalidDataException">When the value cannot be converter to variant type</exception>

  public static DX.OpenXmlElement? CreateVariant(Type variantType, object? value, string? format = null)
  {
    switch (variantType.Name)
    {
      case "VTNull":
        return new DXVT.VTNull();
      case "VTEmpty":
        return new DXVT.VTEmpty();
    }
    if (value == null)
      return new DXVT.VTNull();
    switch (variantType.Name)
    {
      case "VTBool":
        return new DXVT.VTBool(value.ToString().ToLowerInvariant());
      case "VTBString":
        return new DXVT.VTBString(EncodeBstr(value.ToString()) ?? String.Empty);
      case "VTLPSTR":
      case "VTLPWSTR":
        return new DXVT.VTLPWSTR(value.ToString() ?? String.Empty);
      case "VTInt32":
        return new DXVT.VTInt32(IntegerToString(value, format) ?? FloatToString(value, format) ?? value.ToString());
      case "VTUnsignedInt32":
        return new DXVT.VTUnsignedInt32(IntegerToString(value, format) ?? FloatToString(value, format) ?? value.ToString());
      case "VTInt64":
        return new DXVT.VTInt64(IntegerToString(value, format) ?? FloatToString(value, format) ?? value.ToString());
      case "VTUnsignedInt64":
        return new DXVT.VTUnsignedInt64(IntegerToString(value, format) ?? FloatToString(value, format) ?? value.ToString());
      case "VTByte":
        return new DXVT.VTByte(IntegerToString(value, format) ?? FloatToString(value, format) ?? value.ToString());
      case "VTUnsignedByte":
        return new DXVT.VTUnsignedByte(IntegerToString(value, format) ?? FloatToString(value, format) ?? value.ToString());
      case "VTShort":
        return new DXVT.VTShort(IntegerToString(value, format) ?? FloatToString(value, format) ?? value.ToString());
      case "VTUnsignedShort":
        return new DXVT.VTUnsignedShort(IntegerToString(value, format) ?? FloatToString(value, format) ?? value.ToString());
      case "VTDate":
        return new DXVT.VTDate(DateTimeToString(value, format) ?? value.ToString());
      case "VTFileTime":
        return new DXVT.VTFileTime(DateTimeToString(value, format) ?? value.ToString());
      case "VTFloat":
        return new DXVT.VTFloat(FloatToString(value, format) ?? value.ToString());
      case "VTDouble":
        return new DXVT.VTDouble(FloatToString(value, format) ?? value.ToString());
      case "VTDecimal":
        return new DXVT.VTDecimal(DecimalToString(value, format) ?? value.ToString());
      case "VTCurrency":
        return new DXVT.VTCurrency(DecimalToString(value, format) ?? value.ToString());

      case "VTError":
        if (value is int errCode)
          return new DXVT.VTError(errCode.ToString(format ?? "X8"));
        return new DXVT.VTError(value?.ToString() ?? String.Empty);
      case "VTClassId":
        if (value is Guid guidVal)
          return new DXVT.VTClassId(guidVal.ToString(format));
        return new DXVT.VTClassId(value?.ToString() ?? String.Empty);
      case "VTBlob":
        return CreateBlob((byte[])value);
      case "VTOBlob":
        return CreateOBlob((byte[])value);
      case "VTStreamData":
        return new DXVT.VTStreamData(value.ToString());
      case "VTOStreamData":
        return new DXVT.VTOStreamData(value.ToString());
      case "VTVStreamData":
        return new DXVT.VTVStreamData(value.ToString());
      case "VTStorage":
        return new DXVT.VTStorage(value.ToString());
      case "VTOStorage":
        return new DXVT.VTOStorage(value.ToString());
      case "VTArray":
        return CreateVTArray(value as Array);
      case "VTVector":
        var vector = new DXVT.VTVector
        {
          BaseType = DXVT.VectorBaseValues.Variant,
          Size = (UInt32)((object[])value).Length
        };
        foreach (var obj in (object[])value)
        {
          var childElement = new DXVT.Variant();
          childElement.AppendChild(CreateVariant(obj));
          vector.AppendChild(childElement);
        }
        return vector;
      case "Variant":
        return new DXVT.Variant { InnerVariant = value as DXVT.Variant };
    }

    throw new InvalidDataException($"Variant type {variantType.Name} not supported in VariantTools.CreateVariant");
  }

  /// <summary>
  /// Create variant element from the value.
  /// </summary>
  /// <param name="value">value to set </param>
  /// <param name="format">optional format of the value</param>
  /// <returns>variant with the specified value</returns>
  /// <exception cref="InvalidDataException">When the value cannot be converter to variant type</exception>

  public static DX.OpenXmlElement CreateVariant(object value, string? format = null)
  {
    if (value is DXVT.Variant variant)
      return new DXVT.Variant { InnerVariant = variant };

    if (value is bool boolValue)
      return new DXVT.VTBool(boolValue.ToString().ToLowerInvariant());
    if (value is string str)
    {
      return new DXVT.VTLPWSTR(str);
    }
    if (value is Int32 int32value)
      return new DXVT.VTInt32(int32value.ToString(format));
    if (value is Int64 int64value)
      return new DXVT.VTInt64(int64value.ToString(format));
    if (value is UInt32 uint32value)
      return new DXVT.VTUnsignedInt32(uint32value.ToString(format));
    if (value is UInt64 uint64value)
      return new DXVT.VTUnsignedInt64(uint64value.ToString(format));
    if (value is SByte int8value)
      return new DXVT.VTByte(int8value.ToString(format));
    if (value is byte uint8value)
      return new DXVT.VTUnsignedByte(uint8value.ToString(format));
    if (value is Int16 int16value)
      return new DXVT.VTShort(int16value.ToString(format));
    if (value is UInt16 uint16value)
      return new DXVT.VTUnsignedShort(uint16value.ToString(format));
    if (value is DateTime datetimeValue)
      return new DXVT.VTFileTime(datetimeValue.ToUniversalTime().ToString("s") + "Z");
    if (value is float floatValue)
      return new DXVT.VTFloat(floatValue.ToString(CultureInfo.InvariantCulture));
    if (value is double doubleValue)
      return new DXVT.VTDouble(doubleValue.ToString(CultureInfo.InvariantCulture));
    if (value is decimal decimalValue)
      return new DXVT.VTDecimal(decimalValue.ToString(format, CultureInfo.CurrentCulture));


    if (value is Guid guidValue)
      return new DXVT.VTClassId(guidValue.ToString(format ?? "B").ToUpperInvariant());

    if (value is string[] strArray)
    {
      var array = new DXVT.VTArray();
      array.BaseType = DXVT.ArrayBaseValues.Bstr;
      foreach (var s1 in strArray)
        array.AppendChild(CreateVariant(s1));
      return array;
    }

    if (value is object[] objArray)
    {
      var vector = new DXVT.VTVector();
      vector.Size = (UInt32)objArray.Length;
      vector.BaseType = DXVT.VectorBaseValues.Variant;
      foreach (var obj in objArray)
      {
        var childElement = new DXVT.Variant();
        childElement.AppendChild(CreateVariant(obj));
        vector.AppendChild(childElement);
      }
      return vector;
    }

    throw new InvalidDataException($"Value of type {value.GetType()} cannot be converted to VT.VariantType");
  }


  /// <summary>
  /// Convert integer value to string with the specified format.
  /// If the format is null, the default format is used.
  /// If the value is not integer, the null value is returned.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="format"></param>
  /// <returns></returns>
  public static string? IntegerToString(object value, string? format)
  {
    if (value is int intValue)
      return intValue.ToString(format);
    if (value is uint uintValue)
      return uintValue.ToString(format);
    if (value is long longValue)
      return longValue.ToString(format);
    if (value is ulong ulongValue)
      return ulongValue.ToString(format);
    if (value is short shortValue)
      return shortValue.ToString(format);
    if (value is ushort ushortValue)
      return ushortValue.ToString(format);
    if (value is byte byteValue)
      return byteValue.ToString(format);
    if (value is sbyte sbyteValue)
      return sbyteValue.ToString(format);
    return null;
  }

  /// <summary>
  /// Convert float, double or decimal value to string with the specified format with the invariant culture.
  /// If the format is null, the default format is used.
  /// If the value is not  float, double nor decimal, it is converted to decimal.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="format"></param>
  /// <returns></returns>
  public static string? FloatToString(object value, string? format)
  {
    if (value is float floatValue)
      return floatValue.ToString(format, CultureInfo.InvariantCulture);
    if (value is double doubleValue)
      return doubleValue.ToString(format, CultureInfo.InvariantCulture);
    if (value is decimal decimalValue)
      return decimalValue.ToString(format, CultureInfo.InvariantCulture);
    var decimalValue2 = Convert.ToDecimal(value);
    return decimalValue2.ToString(format, CultureInfo.CurrentCulture);
  }

  /// <summary>
  /// Convert float, double or decimal value to string with the specified format with the current culture.
  /// If the format is null, the <c>F</c> format is used.
  /// If the value is not  float, double nor decimal, it is converted to decimal.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="format"></param>
  /// <returns></returns>
  public static string? DecimalToString(object value, string? format)
  {
    if (value is float floatValue)
      return floatValue.ToString(format ?? "F", CultureInfo.CurrentCulture);
    if (value is double doubleValue)
      return doubleValue.ToString(format ?? "F", CultureInfo.CurrentCulture);
    if (value is decimal decimalValue)
      return decimalValue.ToString(format ?? "F", CultureInfo.CurrentCulture);
    var decimalValue2 = Convert.ToDecimal(value);
    return decimalValue2.ToString(format ?? "F", CultureInfo.CurrentCulture);
  }
  /// <summary>
  /// Convert DateTime value to string with the specified format.
  /// If the format is not specified, the <c>"yyyy-MM-ddTHH:mm:sszzz"</c> format is used.
  /// if the value is not DateTime, returns null.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="format"></param>
  /// <returns></returns>
  public static string? DateTimeToString(object value, string? format)
  {
    if (value is DateTime dateTimeValue)
      return dateTimeValue.ToString("yyyy-MM-ddTHH:mm:sszzz");
    return null;
  }


}
