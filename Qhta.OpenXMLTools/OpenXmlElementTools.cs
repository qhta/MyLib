using System;
using System.IO;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml elements.
/// </summary>
public static class OpenXmlElementTools
{
  /// <summary>
  /// Get the document part of the OpenXmlElement. Works specially for Document, Header and Footer elements.
  /// </summary>
  /// <param name="xmlElement">Checked source element</param>
  /// <returns>Returned document part or null</returns>
  public static DXPack.OpenXmlPart? GetDocumentPart(this DX.OpenXmlElement? xmlElement)
  {
    if (xmlElement == null)
    {
      return null;
    }

    if (xmlElement is DXW.Document document)
    {
      return document.MainDocumentPart;
    }

    if (xmlElement is DXW.Header header)
    {
      return header.HeaderPart;
    }

    if (xmlElement is DXW.Footer footer)
    {
      return footer.FooterPart;
    }

    return GetDocumentPart(xmlElement.Parent);
  }

  /// <summary>
  /// Recursively get the parent document of the OpenXmlElement.
  /// </summary>
  /// <param name="xmlElement">Checked source element</param>
  /// <returns>Returned document part or null</returns>
  public static DXW.Document? GetParentDocument(DX.OpenXmlElement? xmlElement)
  {
    while (xmlElement is not DXW.Document)
    {
      if (xmlElement?.Parent == null)
        return null;
      xmlElement = xmlElement.Parent;
    }

    return xmlElement as DXW.Document;
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
  public static byte[]? GetBlobData(DXVT.VTBlob? blob)
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
  public static byte[] GetOBlobData(this DXVT.VTOBlob blob)
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
  /// Get the string value of the first child element of the specified type of the <c>OpenXmlLeafTextElement</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>string value or null (on parse error)</result>
  public static string? GetFirstElementStringValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlLeafTextElement
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Text;
    if (string.IsNullOrEmpty(text))
      return null;
    return text;
  }

  /// <summary>
  /// Set the text content of the first child element of the specified type of the <c>OpenXmlLeafTextElement</c> to the string value.
  /// </summary>
  /// <typeparam name="ElementType">Type of the element</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">string value to set</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstElementStringValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement, string? value)
    where ElementType : DX.OpenXmlLeafTextElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      var val = value;
      if (element != null)
      {
        if (element.Text != val)
          element.Text = val;
      }
      else
        xmlElement.Append(new ElementType { Text = val });
    }
    else
      element?.Remove();
  }

  /// <summary>
  /// Get the integer value of the first child element of the specified type of the <c>OpenXmlLeafTextElement</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>integer value or null (on parse error)</result>
  public static int? GetFirstElementIntValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlLeafTextElement
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Text;
    if (text == null)
      return null;
    if (int.TryParse(text, out var val))
      return val;
    return null;
  }

  /// <summary>
  /// Set the text content of the first child element of the specified type  of the <c>OpenXmlLeafTextElement</c> to the integer value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">integer value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstElementIntValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement, int? value)
    where ElementType : DX.OpenXmlLeafTextElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      var val = ((int)value).ToString();
      if (element != null)
      {
        if (element.Text != val)
          element.Text = val;
      }
      else
        xmlElement.Append(new ElementType { Text = val });
    }
    else
      element?.Remove();
  }


  /// <summary>
  /// Get the integer <c>Val</c> value of the first child element of the specified type of the <c>OpenXmlLeafElement</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>integer value or null</result>
  public static int? GetFirstElementIntVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlLeafElement
  {
    var valProperty = typeof(ElementType).GetProperty("Val");
    if (valProperty == null)
      throw new InvalidDataException($"Property Val not found in {typeof(ElementType)}");
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element == null)
      return null;
    var val = (DX.Int32Value?)valProperty.GetValue(element);
    return val?.Value;
  }

  /// <summary>
  /// Set the <c>Val</c> property of the first child element of the specified type  of the <c>OpenXmlLeafElement</c> to the integer value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">integer value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstElementIntVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, int? value)
    where ElementType : DX.OpenXmlLeafElement, new()
  {
    var valProperty = typeof(ElementType).GetProperty("Val");
    if (valProperty == null)
      throw new InvalidDataException($"Property Val not found in {typeof(ElementType)}");
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        var val = (DX.Int32Value?)valProperty.GetValue(element);
        var valValue = val?.Value;
        if (valValue != value)
          valProperty.SetValue(element, value);
      }
      else
      {
        var newElement = new ElementType();
        valProperty.SetValue(newElement, value);
        xmlElement.Append(newElement);
      }
    }
    else
      element?.Remove();
  }
  /// <summary>
  /// Get the short integer <c>Val</c> value of the first child element of the specified type of the <c>NonNegativeShortType</c> element.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>integer value or null</result>
  public static short? GetFirstNonNegativeShortTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.NonNegativeShortType
  {
    var val = xmlElement.Elements<ElementType>().FirstOrDefault()?.Val?.Value;
    return val;
  }

  /// <summary>
  /// Set the short integer <c>Val</c> value of the first child element of the specified type of the <c>NonNegativeShortType</c> element.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">integer value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstNonNegativeShortTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, short? value)
    where ElementType : DXW.NonNegativeShortType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        if (element.Val?.Value != value)
          element.Val = value;
      }
      else
        xmlElement.Append(new ElementType { Val = value });
    }
    else
      element?.Remove();
  }


  /// <summary>
  /// Get the short unsigned integer <c>Val</c> value of the first child element of the specified type of the <c>OpenXmlLeafTextElement</c> element.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>integer value or null</result>
  public static ushort? GetFirstElementUShortVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlLeafElement
  {
    var valProperty = typeof(ElementType).GetProperty("Val");
    if (valProperty == null)
      throw new InvalidDataException($"Property Val not found in {typeof(ElementType)}");
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element == null)
      return null;
    var val = (DX.UInt16Value?)valProperty.GetValue(element);
    return val?.Value;
  }

  /// <summary>
  /// Set the short unsigned integer <c>Val</c> value of the first child element of the specified type of the <c>OpenXmlLeafTextElement</c> element.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">integer value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstElementUShortVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, ushort? value)
    where ElementType : DX.OpenXmlLeafElement, new()
  {
    var valProperty = typeof(ElementType).GetProperty("Val");
    if (valProperty == null)
      throw new InvalidDataException($"Property Val not found in {typeof(ElementType)}");
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        var val = (DX.UInt16Value?)valProperty.GetValue(element);
        var valValue = val?.Value;
        if (valValue != value)
          valProperty.SetValue(element, value);
      }
      else
      {
        var newElement = new ElementType();
        valProperty.SetValue(newElement, value);
        xmlElement.Append(newElement);
      }
    }
    else
      element?.Remove();
  }

  /// <summary>
  /// Get the integer <c>Val</c> property of the first child element of the specified type of the <c>TwipsMeasureType</c> element.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>integer value or null (on parse error)</result>
  public static int? GetFirstTwipsMeasureTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.TwipsMeasureType
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Val;
    if (text == null)
      return null;
    if (int.TryParse(text, out var val))
      return val;
    return null;
  }

  /// <summary>
  ///Set the <c>Val</c> property of the first child element of the specified type of the <c>TwipsMeasureType</c> element to the integer value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">integer value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstTwipsMeasureTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, int? value)
    where ElementType : DXW.TwipsMeasureType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      var val = ((int)value).ToString();
      if (element != null)
      {
        if (element.Val != val)
          element.Val = val;
      }
      else
        xmlElement.Append(new ElementType { Val = val });
    }
    else
      element?.Remove();
  }


  /// <summary>
  /// Get the byte <c>Val</c> property of the first child element of the specified type of the <c>UnsignedInt7Type</c> element.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>integer value or null (on parse error)</result>
  public static byte? GetFirstUnsignedInt7TypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.UnsignedInt7Type
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Val;
    if (text == null)
      return null;
    if (byte.TryParse(text, out var val))
      return val;
    return null;
  }

  /// <summary>
  ///Set the <c>Val</c> property of the first child element of the specified type of the <c>UnsignedInt7Type</c> element to the byte value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">integer value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstUnsignedInt7TypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, byte? value)
    where ElementType : DXW.UnsignedInt7Type, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        if (element.Val?.Value != value)
          element.Val = value;
      }
      else
        xmlElement.Append(new ElementType { Val = value });
    }
    else
      element?.Remove();
  }

  /// <summary>
  /// Get the string <c>Val</c> property of the first child element of the specified type of the <c>StringType</c> element.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>string value or null</result>
  public static string? GetFirstStringTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.StringType
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Val;
    return text?.Value;
  }

  /// <summary>
  /// Set the <c>Val</c> property of the first child element of the specified type of the <c>StringType</c> element to the string value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">string value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstStringTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, string? value)
    where ElementType : DXW.StringType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        if (element.Val != value)
          element.Val = value;
      }
      else
        xmlElement.Append(new ElementType { Val = value });
    }
    else
      element?.Remove();
  }

  /// <summary>
  /// Get the string <c>Val</c> property of the first child element of the specified type of the <c>String253Type</c> element.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>string value or null</result>
  public static string? GetFirstString253TypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.String253Type
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Val;
    return text?.Value;
  }

  /// <summary>
  /// Set the <c>Val</c> property of the first child element of the specified type of the <c>String253Type</c> element to the string value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">string value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstString253TypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, string? value)
    where ElementType : DXW.String253Type, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        if (element.Val != value)
          element.Val = value;
      }
      else
        xmlElement.Append(new ElementType { Val = value });
    }
    else
      element?.Remove();
  }

  /// <summary>
  /// Get the boolean value of the first child element of the specified type of the <c>OpenXmlLeadTextElement</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>boolean value or null (on parse error)</result>
  /// <remarks>
  ///   boolean value can be "true" or "false" (case-insensitive) or "1" or "0".
  /// </remarks>
  public static bool? GetFirstElementBoolValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlLeafTextElement
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Text;
    if (text == null)
      return null;
    if (bool.TryParse(text, out var val))
      return val;
    if (int.TryParse(text, out var n))
      return n != 0;
    return null;
  }

  /// <summary>
  /// Set the text content of the first child element of the specified type of the <c>OpenXmlLeadTextElement</c> to the bool value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">bool value (or null)</param>
  /// <remarks>
  /// If the value is null, the text content is removed.
  /// </remarks>
  public static void SetFirstElementBoolValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement, bool? value)
    where ElementType : DX.OpenXmlLeafTextElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      var val = ((bool)value).ToString().ToLower();
      if (element != null)
      {
        if (element.Text != val)
          element.Text = val;
      }
      else
        xmlElement.Append(new ElementType { Text = val });
    }
    else
      element?.Remove();
  }


  /// <summary>
  /// Get the boolean value of the first child element of the specified type of the <c>OnOffType</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>boolean value or null (on parse error)</result>
  /// <remarks>
  ///   boolean value can be "true" or "false" (case-insensitive) or "1" or "0".
  /// </remarks>
  public static bool? GetFirstOnOffElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.OnOffType
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element == null)
      return null;
    var value = element.Val?.Value;
    return value;
  }

  /// <summary>
  /// Set the text content of the first child element of the specified type of the <c>OnOffType</c> to the bool value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">bool value (or null)</param>
  /// <remarks>
  /// If the value is null, the text content is removed.
  /// </remarks>
  public static void SetFirstOnOffElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, bool? value)
    where ElementType : DXW.OnOffType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        if (element.Val?.Value != value)
          element.Val = value;
      }
      else
        xmlElement.Append(new ElementType { Val = value });
    }
    else
      element?.Remove();
  }

  /// <summary>
  /// Get the <c>Val</c> property of the first child element of the specified type of the <c>Office2010.Word.OnOffType</c> as the <c>OnOffValues</c> value.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>boolean value or null (on parse error)</result>
  /// <remarks>
  ///   boolean value can be "true" or "false" (case-insensitive) or "1" or "0".
  /// </remarks>
  public static DXO10W.OnOffValues? GetFirstOnOffValuesElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXO10W.OnOffType
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element == null)
      return null;
    var value = element.Val?.Value;
    return value;
  }

  /// <summary>
  /// Set the <c>Val</c> property of the first child element of the specified type of the <c>Office2010.Word.OnOffType</c> to the <c>OnOffValues</c> value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">bool value (or null)</param>
  /// <remarks>
  /// If the value is null, the text content is removed.
  /// </remarks>
  public static void SetFirstOnOffValuesElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, DXO10W.OnOffValues? value)
    where ElementType : DXO10W.OnOffType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        if (element.Val?.Value != value)
          element.Val = value;
      }
      else
        xmlElement.Append(new ElementType { Val = value });
    }
    else
      element?.Remove();
  }


  /// <summary>
  /// Get the <c>Val</c> property of the first child element of the specified type of the <c>Office2013.Word.OnOffType</c> as the <c>OnOffValue</c> value.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>boolean value or null (on parse error)</result>
  /// <remarks>
  ///   boolean value can be "true" or "false" (case-insensitive) or "1" or "0".
  /// </remarks>
  public static DX.OnOffValue? GetFirstOnOffValueElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXO13W.OnOffType
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element == null)
      return null;
    var value = element.Val?.Value;
    return value;
  }

  /// <summary>
  /// Set the <c>Val</c> property of the first child element of the specified type of the <c>Office2013.Word.OnOffType</c> to the <c>OnOffValue</c> value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">bool value (or null)</param>
  /// <remarks>
  /// If the value is null, the text content is removed.
  /// </remarks>
  public static void SetFirstOnOffValueElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, DX.OnOffValue? value)
    where ElementType : DXO13W.OnOffType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        if (element.Val?.Value != value)
          element.Val = value;
      }
      else
        xmlElement.Append(new ElementType { Val = value });
    }
    else
      element?.Remove();
  }
  /// <summary>
  /// Get the boolean value indicating that a child <c>EmptyType</c> element exists.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>boolean value (true or false)</result>
  public static bool GetFirstEmptyTypeElementAsBoolean<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.EmptyType
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    return element != null;
  }

  /// <summary>
  /// Appends or removes a child <c>EmptyType</c> element depending on boolean value. 
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">bool value (or null)</param>
  /// <remarks>
  /// If the value is null, nothing is changed.
  /// If the value is true and the element of the specified type is not in the collection, it is added.
  /// If the value is false and the element of the specified type is in the collection, it is removed.
  /// </remarks>
  public static void SetFirstEmptyTypeElementAsBoolean<ElementType>(this DX.OpenXmlCompositeElement xmlElement, bool? value)
    where ElementType : DXW.EmptyType, new()
  {
    if (value != null)
    {
      var element = xmlElement.Elements<ElementType>().FirstOrDefault();
      if (value.Value)
      {
        if (element == null)
          xmlElement.Append(new ElementType());
      }
      else
        element?.Remove();
    }
  }

  /// <summary>
  /// Get first element of the specified type from the <c>OpenXmlCompositeElement</c>. 
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <remarks>
  /// </remarks>
  public static ElementType? GetFirstElement<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlElement
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    return element;
  }

  /// <summary>
  /// Set first element of the specified type in the <c>OpenXmlCompositeElement</c>. 
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  /// <remarks>
  /// If the value is null and the element of the specified type is already in the collection, it is removed.
  /// If the value equals existing element, nothing is changed.
  /// Otherwise, the existing element is removed and the new element is added.
  /// </remarks>
  public static void SetFirstElement<ElementType>(this DX.OpenXmlCompositeElement xmlElement, ElementType? value)
    where ElementType : DX.OpenXmlElement
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        if (element != value)
        {
          element.Remove();
          xmlElement.Append(value);
        }
      }
      else
        xmlElement.Append(value);
    }
    else
      element?.Remove();
  }

  /// <summary>
  /// Get the <c>Val</c> property value of the first specified type element of the <c>OpenXmlCompositeElement</c> as a string.
  /// </summary>
  /// <param name="xmlElement"></param>
  public static string? GetFirstElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      return element.GetValValue<string>();
    }
    return null;
  }

  /// <summary>
  /// Set the <c>Val</c> property value of the first specified type element in the <c>OpenXmlCompositeElement</c> as a string.
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  public static void SetFirstElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, string? value)
    where ElementType : DX.OpenXmlElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      if (value == null)
        element.Remove();
      else
        SetValValue(element, value);
    }
    else if (value != null)
    {
      element = new ElementType();
      SetValValue(element, value);
      xmlElement.Append(element);
    }
  }


  /// <summary>
  /// Get the <c>Val</c> property value of the first specified type element of the <c>OpenXmlCompositeElement</c> as a hex integer.
  /// </summary>
  /// <param name="xmlElement"></param>
  public static int? GetFirstElementHexIntVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      var val = element.GetValValue<string>();
      if (val != null)
        return int.Parse(val, NumberStyles.HexNumber);
    }
    return null;
  }

  /// <summary>
  /// Set the <c>Val</c> property value of the first specified type element in the <c>OpenXmlCompositeElement</c> as a hex integer.
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  public static void SetFirstElementHexIntVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, int? value)
    where ElementType : DX.OpenXmlElement, new()
  {
    if (value != null)
      SetFirstElementVal<ElementType>(xmlElement, value.Value.ToString("X8"));
    else
      SetFirstElementVal<ElementType>(xmlElement, null);
  }

  /// <summary>
  /// Set all elements of the first specified type element in the <c>OpenXmlCompositeElement</c>.
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  public static void SetAllElements<ElementType>(this DX.OpenXmlCompositeElement xmlElement, IEnumerable<ElementType>? value)
    where ElementType : DX.OpenXmlElement
  {
    foreach (var item in xmlElement.Elements<ElementType>().ToArray())
      item.Remove();
    if (value != null)
      foreach (var item in value)
        xmlElement.Append(item);
  }

  /// <summary>
  /// Get the <c>Val</c> value of the first specified type element from the <c>OpenXmlCompositeElement</c>.
  /// </summary>
  /// <param name="xmlElement"></param>
  public static ElementValuesType? GetFirstElementVal<ElementType, ElementValuesType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlElement, new()
    where ElementValuesType : struct
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      return GetValValue<ElementValuesType>(element);
    }
    return null;
  }

  /// <summary>
  /// Set the value of the first specified type element in the <c>OpenXmlCompositeElement</c>.
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  public static void SetFirstElementVal<ElementType, ElementValuesType>(this DX.OpenXmlCompositeElement xmlElement, ElementValuesType? value)
    where ElementType : DX.OpenXmlElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      if (value == null)
        element.Remove();
      else
        SetValValue(element, value);
    }
    else if (value != null)
    {
      element = new ElementType();
      SetValValue(element, value);
      xmlElement.Append(element);
    }
  }

  /// <summary>
  /// Get the <c>Val</c> property of the specified type int the <c>OpenXmlLeafElement</c>.
  /// </summary>
  /// <typeparam name="ValType"></typeparam>
  /// <param name="xmlElement"></param>
  /// <exception cref="InvalidDataException"></exception>
  public static ValType? GetValValue<ValType>(this DX.OpenXmlElement xmlElement)
  {
    var valProperty = xmlElement.GetType().GetProperty("Val");
    if (valProperty == null)
      throw new InvalidDataException($"Property Val not found in {xmlElement.GetType()}");
    return (ValType?)valProperty.GetValue(xmlElement);
  }

  /// <summary>
  /// Set the <c>Val</c> property of the specified type int the <c>OpenXmlLeafElement</c>.
  /// </summary>
  /// <typeparam name="ValType"></typeparam>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  /// <exception cref="InvalidDataException"></exception>
  public static void SetValValue<ValType>(this DX.OpenXmlElement xmlElement, ValType? value)
  {
    var valProperty = xmlElement.GetType().GetProperty("Val");
    if (valProperty == null)
      throw new InvalidDataException($"Property Val not found in {xmlElement.GetType()}");
    valProperty.SetValue(xmlElement, value);
  }

  /// <summary>
  /// Get the <c>Id</c> property of the first specified type from the <c>OpenXmlLeafElement</c>.
  /// </summary>
  /// <typeparam name="ElementType"></typeparam>
  /// <param name="xmlElement"></param>
  /// <exception cref="InvalidDataException"></exception>
  public static string? GetFirstRelationshipElementId<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.RelationshipType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      return element.Id;
    }
    return null;
  }

  /// <summary>
  /// Set the <c>Id</c> property of the first specified type in the <c>OpenXmlLeafElement</c>.
  /// </summary>
  /// <typeparam name="ElementType"></typeparam>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  /// <exception cref="InvalidDataException"></exception>
  public static void SetFirstRelationshipElementId<ElementType>(this DX.OpenXmlElement xmlElement, string? value)
  where ElementType : DXW.RelationshipType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      if (value == null)
        element.Remove();
      else
        element.Id = value;
    }
    else if (value != null)
    {
      // ReSharper disable once UseObjectOrCollectionInitializer
      element = new ElementType();
      element.Id = value;
      xmlElement.Append(element);
    }
  }

  /// <summary>
  /// Get the object array from the VTArray element.
  /// </summary>
  /// <param name="vector">source vector</param>
  /// <returns>array of objects</returns>
  public static object[]? GetVectorData(this DXVT.VTVector? vector)
  {
    if (vector == null)
      return null;
    var variants = new List<Object>();
    foreach (var element in vector.Elements())
    {
      if (element is DXVT.Variant variant)
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

  /// <summary>
  /// Get the value of the VTVariant element.
  /// </summary>
  /// <param name="element">Source element</param>
  /// <returns>simple type result</returns>
  /// <exception cref="InvalidDataException"></exception>
  public static object? GetVariantValue(this DX.OpenXmlElement? element)
  {
    if (element is DXVT.Variant variant)
      return GetVariantValue(variant.InnerVariant);
    if (element is DXVT.VTBool vbool)
      return bool.Parse(vbool.InnerText);
    if (element is DXVT.VTLPSTR lpStr)
      return lpStr.InnerText;
    if (element is DXVT.VTLPWSTR lpwStr)
      return lpwStr.InnerText;
    if (element is DXVT.VTBString bStr)
      return bStr.InnerText;
    if (element is DXVT.VTInteger vint)
      return Int32.Parse(vint.InnerText);
    if (element is DXVT.VTUnsignedInteger vuint)
      return Int32.Parse(vuint.InnerText);
    if (element is DXVT.VTInt32 i4)
      return Int32.Parse(i4.InnerText);
    if (element is DXVT.VTInt64 i8)
      return Int64.Parse(i8.InnerText);
    if (element is DXVT.VTUnsignedInt32 ui4)
      return UInt32.Parse(ui4.InnerText);
    if (element is DXVT.VTUnsignedInt64 ui8)
      return UInt64.Parse(ui8.InnerText);
    if (element is DXVT.VTByte vb)
      return SByte.Parse(vb.InnerText);
    if (element is DXVT.VTUnsignedByte ub)
      return Byte.Parse(ub.InnerText);
    if (element is DXVT.VTShort vsh)
      return Int16.Parse(vsh.InnerText);
    if (element is DXVT.VTUnsignedShort ush)
      return UInt16.Parse(ush.InnerText);
    if (element is DXVT.VTDate vdate)
      return DateTime.Parse(vdate.InnerText);
    if (element is DXVT.VTFileTime vtime)
      return DateTime.Parse(vtime.InnerText);
    if (element is DXVT.VTFloat vfloat)
      return Single.Parse(vfloat.InnerText, CultureInfo.InvariantCulture);
    if (element is DXVT.VTDouble vdouble)
      return Double.Parse(vdouble.InnerText, CultureInfo.InvariantCulture);
    if (element is DXVT.VTCurrency vcurrency)
      return Decimal.Parse(vcurrency.InnerText, CultureInfo.InvariantCulture);
    if (element is DXVT.VTDecimal vdecimal)
      return Decimal.Parse(vdecimal.InnerText, CultureInfo.InvariantCulture);
    if (element is DXVT.VTNull)
      return null;
    if (element is DXVT.VTEmpty)
      return new Object();
    if (element is DXVT.VTError vError)
      return Int32.Parse(vError.InnerText, NumberStyles.AllowHexSpecifier);
    if (element is DXVT.VTClassId vClsId)
      return Guid.Parse(vClsId.InnerText);

    if (element is DXVT.VTBlob vBlob)
      return GetBlobData(vBlob);
    if (element is DXVT.VTOBlob oBlob)
      return oBlob.GetOBlobData();
    if (element is DXVT.VTStreamData vStream)
      return vStream;
    if (element is DXVT.VTOStreamData oStream)
      return oStream;
    if (element is DXVT.VTVStreamData vvStream)
      return vvStream;
    if (element is DXVT.VTStorage vStorage)
      return vStorage;
    if (element is DXVT.VTOStorage oStorage)
      return oStorage;

    if (element is DXVT.VTArray vArray)
      return vArray;
    if (element is DXVT.VTVector vVector)
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

  /// <summary>
  /// Create variant element from the value.
  /// </summary>
  /// <param name="value">value to set </param>
  /// <param name="format">format of the value</param>
  /// <returns></returns>
  /// <exception cref="InvalidDataException">When the value cannot be converter to variant type</exception>
  public static DX.OpenXmlElement CreateVariant(object? value, string? format = null)
  {
    if (value == null)
      return new DXVT.VTNull();
    if (format == "error")
      return new DXVT.VTError(((Int32)value).ToString("X8"));
    if (format == "cy")
      return new DXVT.VTCurrency(((decimal)value).ToString("F", CultureInfo.InvariantCulture));

    if (value is DXVT.Variant variant)
      return new DXVT.Variant { InnerVariant = variant };
    if (value is bool boolValue)
      return new DXVT.VTBool(boolValue.ToString().ToLowerInvariant());
    if (value is string str)
      return new DXVT.VTLPSTR(str);
    if (value is Int32 int32value)
      return new DXVT.VTInt32(int32value.ToString());
    if (value is Int64 int64value)
      return new DXVT.VTInt64(int64value.ToString());
    if (value is UInt32 uint32value)
      return new DXVT.VTUnsignedInt32(uint32value.ToString());
    if (value is UInt64 uint64value)
      return new DXVT.VTUnsignedInt64(uint64value.ToString());
    if (value is SByte int8value)
      return new DXVT.VTByte(int8value.ToString());
    if (value is byte uint8value)
      return new DXVT.VTUnsignedByte(uint8value.ToString());
    if (value is Int16 int16value)
      return new DXVT.VTShort(int16value.ToString());
    if (value is UInt16 uint16value)
      return new DXVT.VTShort(uint16value.ToString());
    if (value is DateTime datetimeValue)
      return new DXVT.VTFileTime(datetimeValue.ToUniversalTime().ToString("s") + "Z");
    if (value is float floatValue)
      return new DXVT.VTFloat(floatValue.ToString(CultureInfo.InvariantCulture));
    if (value is double doubleValue)
      return new DXVT.VTDouble(doubleValue.ToString(CultureInfo.InvariantCulture));
    if (value is decimal decimalValue)
      return new DXVT.VTDecimal(decimalValue.ToString(CultureInfo.InvariantCulture));

    if (value is Guid guidValue)
      return new DXVT.VTClassId(guidValue.ToString("B").ToUpperInvariant());

    throw new InvalidDataException($"Value of type {value.GetType()} cannot be converted to VT.VariantType");
  }

}