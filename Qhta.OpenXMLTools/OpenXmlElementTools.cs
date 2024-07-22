using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Globalization;

using VT = DocumentFormat.OpenXml.VariantTypes;
using System.Collections;
using DocumentFormat.OpenXml.VariantTypes;

namespace Qhta.OpenXmlTools;

public static class OpenXmlElementTools
{
  public static OpenXmlPart? GetDocumentPart(this OpenXmlElement? xmlElement)
  {
    if (xmlElement == null)
    {
      return null;
    }

    if (xmlElement is Document document)
    {
      return document.MainDocumentPart;
    }

    if (xmlElement is Header header)
    {
      return header.HeaderPart;
    }

    if (xmlElement is Footer footer)
    {
      return footer.FooterPart;
    }

    return GetDocumentPart(xmlElement.Parent);
  }

  public static Document? GetParentDocument(OpenXmlElement element)
  {
    while (element is not Document)
    {
      if (element.Parent == null)
        return null;
      element = element.Parent;
    }

    return element as Document;
  }

  public static byte[]? GetBlobData(DocumentFormat.OpenXml.VariantTypes.VTBlob? blob)
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
    throw new InvalidDataException($"Non-conformed VTBlob data");
  }

  public static DocumentFormat.OpenXml.VariantTypes.VTBlob CreateBlob(byte[] data)
  {
    var dataBytes = new byte[data.Length + 4];
    Array.Copy(data, 0, dataBytes, 4, data.Length);
    var countBytes = BitConverter.GetBytes(data.Length);
    Array.Copy(countBytes, 0, dataBytes, 0, 4);
    return new DocumentFormat.OpenXml.VariantTypes.VTBlob(Convert.ToBase64String(dataBytes));
  }

  public static void SetElementStringValue<ElementType>(this OpenXmlCompositeElement root, string? value) where ElementType : OpenXmlLeafTextElement, new()
  {
    var element = root.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      var val = (string)value;
      if (element != null && element.Text != val)
      {
        element.Remove();
        root.AddChild(new ElementType { Text = val });
      }
    }
    else
      element?.Remove();
  }

  public static int? GetElementInt32Value<ElementType>(this OpenXmlCompositeElement root) where ElementType : OpenXmlLeafTextElement
  {
    var text = root.Elements<ElementType>().FirstOrDefault()?.Text;
    if (text == null)
      return null;
    var val = int.Parse(text);
    return val;
  }

  public static void SetElementInt32Value<ElementType>(this OpenXmlCompositeElement root, int? value) where ElementType : OpenXmlLeafTextElement, new()
  {
    var element = root.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      var val = ((int)value).ToString();
      if (element != null && element.Text != val)
      {
        element.Remove();
        root.AddChild(new ElementType { Text = val });
      }
    }
    else
      element?.Remove();
  }

  public static bool? GetElementBoolValue<ElementType>(this OpenXmlCompositeElement root) where ElementType : OpenXmlLeafTextElement
  {
    var text = root.Elements<ElementType>().FirstOrDefault()?.Text;
    if (text == null)
      return null;
    var val = bool.Parse(text);
    return val;
  }

  public static void SetElementBoolValue<ElementType>(this OpenXmlCompositeElement root, bool? value) where ElementType : OpenXmlLeafTextElement, new()
  {
    var element = root.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      var val = ((bool)value).ToString().ToLower();
      if (element != null && element.Text != val)
      {
        element.Remove();
        root.AddChild(new ElementType { Text = val });
      }
    }
    else
      element?.Remove();
  }

  public static byte[] GetBlobData(this VT.VTOBlob blob)
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
    throw new InvalidDataException($"Non-conformed VTOBlob data");
  }

  public static string? GetElementStringValue<ElementType>(this OpenXmlCompositeElement root) where ElementType : OpenXmlLeafTextElement
  {
    var text = root.Elements<ElementType>().FirstOrDefault()?.Text;
    if (string.IsNullOrEmpty(text))
      return null;
    return text;
  }

  public static object[]? GetVectorData(this VT.VTVector? vector)
  {
    if (vector == null)
      return null;
    var variants = new List<Object>();
    foreach (var element in vector.Elements())
    {
      if (element is VT.Variant variant)
      {
        var item = variant.Elements().FirstOrDefault();
        if (item != null)
        {
          var varItem = GetVariantValue(item);
          if (varItem != null)
            variants.Add(varItem);
        }
      }
      else
      {
        var varItem = GetVariantValue(element);
        if (varItem != null)
          variants.Add(varItem);
      }
    }
    return variants.ToArray();
  }

  public static object? GetVariantValue(this OpenXmlElement? element)
  {
    if (element is VT.Variant variant)
      return GetVariantValue(variant.InnerVariant);
    if (element is VT.VTBool vbool)
      return bool.Parse(vbool.InnerText);
    if (element is VT.VTLPSTR lpStr)
      return lpStr.InnerText;
    if (element is VT.VTLPWSTR lpwStr)
      return lpwStr.InnerText;
    if (element is VT.VTBString bStr)
      return bStr.InnerText;
    if (element is VT.VTInteger vint)
      return Int32.Parse(vint.InnerText);
    if (element is VT.VTUnsignedInteger vuint)
      return Int32.Parse(vuint.InnerText);
    if (element is VT.VTInt32 i4)
      return Int32.Parse(i4.InnerText);
    if (element is VT.VTInt64 i8)
      return Int64.Parse(i8.InnerText);
    if (element is VT.VTUnsignedInt32 ui4)
      return UInt32.Parse(ui4.InnerText);
    if (element is VT.VTUnsignedInt64 ui8)
      return UInt64.Parse(ui8.InnerText);
    if (element is VT.VTByte vb)
      return SByte.Parse(vb.InnerText);
    if (element is VT.VTUnsignedByte ub)
      return Byte.Parse(ub.InnerText);
    if (element is VT.VTShort vsh)
      return Int16.Parse(vsh.InnerText);
    if (element is VT.VTUnsignedShort ush)
      return UInt16.Parse(ush.InnerText);
    if (element is VT.VTDate vdate)
      return DateTime.Parse(vdate.InnerText);
    if (element is VT.VTFileTime vtime)
      return DateTime.Parse(vtime.InnerText);
    if (element is VT.VTFloat vfloat)
      return Single.Parse(vfloat.InnerText, System.Globalization.CultureInfo.InvariantCulture);
    if (element is VT.VTDouble vdouble)
      return Double.Parse(vdouble.InnerText, System.Globalization.CultureInfo.InvariantCulture);
    if (element is VT.VTCurrency vcurrency)
      return Decimal.Parse(vcurrency.InnerText, System.Globalization.CultureInfo.InvariantCulture);
    if (element is VT.VTDecimal vdecimal)
      return Decimal.Parse(vdecimal.InnerText, System.Globalization.CultureInfo.InvariantCulture);
    if (element is VT.VTNull)
      return null;
    if (element is VT.VTEmpty)
      return new System.Object();
    if (element is VT.VTError vError)
      return Int32.Parse(vError.InnerText, NumberStyles.AllowHexSpecifier);
    if (element is VT.VTClassId vClsId)
      return Guid.Parse(vClsId.InnerText);

    if (element is VT.VTBlob vBlob)
      return OpenXmlElementTools.GetBlobData(vBlob);
    if (element is VT.VTOBlob oBlob)
      return OpenXmlElementTools.GetBlobData(oBlob);
    if (element is VT.VTStreamData vStream)
      return vStream;
    if (element is VT.VTOStreamData oStream)
      return oStream;
    if (element is VT.VTVStreamData vvStream)
      return vvStream;
    if (element is VT.VTStorage vStorage)
      return vStorage;
    if (element is VT.VTOStorage oStorage)
      return oStorage;

    if (element is VT.VTArray vArray)
      return vArray;
    if (element is VT.VTVector vVector)
      return vVector;

    throw new InvalidDataException($"Variant type{element?.GetType()} not recognized");
  }

  //private static byte[]? GetData(OpenXmlLeafElement? data)
  //{
  //  if (data == null)
  //    return null;
  //  if (String.IsNullOrEmpty(data.InnerText))
  //    return null;
  //  var bytes = Convert.FromBase64String(data.InnerText);
  //  return bytes;
  //}

  //private static byte[]? GetVTOStreamData(VariantTypes.VTOStreamData? data)
  //{
  //  if (data == null)
  //    return null;
  //  if (String.IsNullOrEmpty(data.InnerText))
  //    return null;
  //  var bytes = Convert.FromBase64String(data.InnerText);
  //  return bytes;
  //}

  //private static (Guid, byte[])? GetVTVStreamData(VariantTypes.VTVStreamData? data)
  //{
  //  if (data == null)
  //    return null;
  //  if (String.IsNullOrEmpty(data.InnerText))
  //    return null;
  //  var bytes = Convert.FromBase64String(data.InnerText);
  //  Guid guid;
  //  if (data.Version?.Value != null)
  //    guid = Guid.Parse(data.Version.Value);
  //  return (guid, bytes);
  //}

  //private static int[]? ReadVTArrayBounds(string? str)
  //{
  //  if (str == null)
  //    return null;
  //  var ss = str.Split(',');
  //  var result = new int[ss.Length];
  //  for (int i = 0; i < ss.Length; i++)
  //    result[i] = Convert.ToInt32(ss[i]);
  //  return result;
  //}

  //private static System.Array? GetVTArray(VariantTypes.VTArray? vArray)
  //{
  //  if (vArray == null)
  //    return null;
  //  if (vArray.BaseType == null)
  //    throw new InvalidDataException($"Unknown VTArray base type");
  //  var baseType = DM.VariantTypeMapping.ArrayBaseTypeToType[vArray.BaseType];
  //  var lBounds = ReadVTArrayBounds(vArray.LowerBounds?.InnerText);
  //  var uBounds = ReadVTArrayBounds(vArray.UpperBounds?.InnerText);
  //  if (lBounds == null || uBounds == null || lBounds.Length != uBounds.Length)
  //    throw new InvalidDataException($"Non-comformed VTArray bounds");
  //  var lengths = new int[uBounds.Length];
  //  for (int i = 0; i < lengths.Length; i++)
  //    lengths[i] = uBounds[i] - lBounds[i] + 1;
  //  var array = System.Array.CreateInstance(baseType, lengths, lBounds);
  //  int index = 0;
  //  foreach (var itemElement in vArray.Elements())
  //  {
  //    var item = GetVariantValue(itemElement);
  //    array.SetValue(item, index++);
  //  }
  //  return array;
  //}

  //private static ICollection? ReadVTVector(VariantTypes.VTVector? vVector)
  //{
  //  if (vVector == null)
  //    return null;
  //  if (vVector.BaseType == null)
  //    throw new InvalidDataException($"Unknown VTVector base type");
  //  var baseType = DM.VariantTypeMapping.VectorBaseTypeToType[vVector.BaseType];
  //  var vectorType = typeof(List<>).MakeGenericType(new Type[] { baseType });
  //  var vector = vectorType.GetConstructor(new Type[0])?.Invoke(new object[0]);
  //  foreach (var itemElement in vVector.Elements())
  //  {
  //    var item = GetVariantValue(itemElement);
  //    vectorType.GetMethod("Add")?.Invoke(vector, new object[] { item });
  //  }
  //  return vector as ICollection;
  //}

  //public static Guid? GetGuidAttribute(this OpenXmlElement element, string attrName)
  //{
  //}

  public static OpenXmlElement CreateVariant(object value, string? format = null)
  {
    if (format == "error")
      return new VTError(((Int32)value).ToString("X8"));
    if (format == "cy")
      return new VTCurrency(((decimal)value).ToString("F", CultureInfo.InvariantCulture));

    if (value is VT.Variant variant)
      return new VT.Variant { InnerVariant = variant };
    if (value is bool boolValue)
      return new VTBool(boolValue.ToString().ToLowerInvariant());
    if (value is string str)
      return new VTLPSTR(str);
    if (value is Int32 int32value)
      return new VTInt32(int32value.ToString());
    if (value is Int64 int64value)
      return new VTInt64(int64value.ToString());
    if (value is UInt32 uint32value)
      return new VTUnsignedInt32(uint32value.ToString());
    if (value is UInt64 uint64value)
      return new VTUnsignedInt64(uint64value.ToString());
    if (value is SByte int8value)
      return new VTByte(int8value.ToString());
    if (value is byte uint8value)
      return new VTUnsignedByte(uint8value.ToString());
    if (value is Int16 int16value)
      return new VTShort(int16value.ToString());
    if (value is UInt16 uint16value)
      return new VTShort(uint16value.ToString());
    if (value is DateTime datetimeValue)
      return new VTFileTime(datetimeValue.ToUniversalTime().ToString("s") + "Z");
    if (value is float floatValue)
      return new VTFloat(floatValue.ToString(CultureInfo.InvariantCulture));
    if (value is double doubleValue)
      return new VTDouble(doubleValue.ToString(CultureInfo.InvariantCulture));
    if (value is decimal decimalValue)
      return new VTDecimal(decimalValue.ToString(CultureInfo.InvariantCulture));
    if (value == null)
      return new VT.VTNull();
    if (value is Guid guidValue)
      return new VT.VTClassId(guidValue.ToString("B").ToUpperInvariant());

    throw new InvalidDataException($"Value of type {value.GetType()} cannot be converted to VT.VariantType");
  }

}