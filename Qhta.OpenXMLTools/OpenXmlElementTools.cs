using System;
using System.IO;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml elements.
/// </summary>
public static class OpenXmlElementTools
{

  /// <summary>
  /// Checks if the element is empty.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DX.OpenXmlElement? element)
  {
    if (element == null)
      return true;
    var result = element.ChildElements.Count == 0 && !element.HasAttributes;
    return result;
  }

  /// <summary>
  /// Get child elements (without properties) of the OpenXmlElement.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static IEnumerable<DX.OpenXmlElement> GetMembers(this DX.OpenXmlCompositeElement element)
  {
    foreach (var child in element.ChildElements)
    {
      if (!child.GetType().Name.EndsWith("Properties"))
        yield return child;
    }
  }

  /// <summary>
  /// Get the document part of the OpenXmlElement. Works specially for Document, Header and Footer elements.
  /// </summary>
  /// <param name="xmlElement">Checked source element</param>
  /// <returns>Returned document part or null</returns>
  public static DXPack.OpenXmlPart? GetDocumentPart(this DX.OpenXmlElement? xmlElement)
  {
    var rootElement = GetRootElement(xmlElement);
    if (rootElement != null)
    {
      return rootElement.OpenXmlPart;
    }
    return null;
  }

  /// <summary>
  /// Get the <c>WordprocessingDocument</c> of the OpenXmlElement. 
  /// </summary>
  /// <param name="xmlElement">Checked source element</param>
  /// <returns>Returned document part or null</returns>
  public static DXPack.WordprocessingDocument? GetWordprocessingDocument(this DX.OpenXmlElement? xmlElement)
  {
    var part = GetDocumentPart(xmlElement);
    return part?.OpenXmlPackage as DXPack.WordprocessingDocument;
  }

  /// <summary>
  /// Recursively get the root element of the OpenXmlElement.
  /// </summary>
  /// <param name="xmlElement">Checked source element</param>
  /// <returns>Returned document part or null</returns>
  public static DX.OpenXmlPartRootElement? GetRootElement(this DX.OpenXmlElement? xmlElement)
  {
    if (xmlElement?.Parent is DX.OpenXmlPartRootElement rootElement)
    {
      return rootElement;
    }
    if (xmlElement?.Parent != null)
      return xmlElement.Parent.GetRootElement();
    return null;
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
  /// Get the string value of the first child element of the specified type of the <c>StringType</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>string value or null (on parse error)</result>
  public static string? GetFirstElementStringTypeValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.StringType
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Val?.Value;
    return text;
  }

  /// <summary>
  /// Set the value of the first child element of the specified type of the <c>StringType</c> to the string value.
  /// </summary>
  /// <typeparam name="ElementType">Type of the element</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">string value to set</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstElementStringTypeValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement, string? value)
    where ElementType : DXW.StringType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element?.Val?.Value != value)
      {
        if (element != null)
          element.Val = value;
        else
          xmlElement.Append(new ElementType { Val = value });
      }
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
        valProperty.SetValue(newElement, new DX.Int32Value(value.Value));
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
  /// Get the <see cref="Twips"/> <c>Val</c> property of the first child element of the specified type of the <c>TwipsMeasureType</c> element.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>integer value or null (on parse error)</result>
  public static Twips? GetFirstTwipsMeasureTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
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
  ///Set the <c>Val</c> property of the first child element of the specified type of the <c>TwipsMeasureType</c> element to the <see cref="Twips"/> value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">integer value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstTwipsMeasureTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, Twips? value)
    where ElementType : DXW.TwipsMeasureType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value is not null)
    {
      var val = ((int?)value).ToString();
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
  /// Get the <see cref="Twips"/> <c>Val</c> property of the first child element of the specified type of the <c>TwipsMeasureType</c> element.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>integer value or null (on parse error)</result>
  public static Twips? GetFirstTwipsMeasureMathTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXM.TwipsMeasureType
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Val;
    if (text == null)
      return null;
    if (int.TryParse(text, out var val))
      return val;
    return null;
  }

  /// <summary>
  ///Set the <c>Val</c> property of the first child element of the specified type of the <c>TwipsMeasureType</c> element to the <see cref="Twips"/> value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">integer value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstTwipsMeasureMathTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, Twips? value)
    where ElementType : DXM.TwipsMeasureType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value is not null)
    {
      var val = (uint?)value;
      if (element?.Val != null)
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
  public static bool? GetFirstOnOffTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
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
  public static void SetFirstOnOffTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, bool? value)
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
  /// Get the boolean value of the first child element of the specified type of the <c>Math.OnOffType</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>boolean value or null (on parse error)</result>
  /// <remarks>
  ///   boolean value can be "true" or "false" (case-insensitive) or "1" or "0".
  /// </remarks>
  public static DXM.BooleanValues? GetFirstOnOffMathTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXM.OnOffType
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element == null)
      return null;
    var value = element.Val?.Value;
    return value;
  }

  /// <summary>
  /// Set the text content of the first child element of the specified type of the <c>Math.OnOffType</c> to the bool value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">bool value (or null)</param>
  /// <remarks>
  /// If the value is null, the text content is removed.
  /// </remarks>
  public static void SetFirstOnOffMathTypeElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, DXM.BooleanValues? value)
    where ElementType : DXM.OnOffType, new()
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
  public static string? GetFirstElementStringVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      return element.GetValValue<DX.StringValue>()?.Value;
    }
    return null;
  }

  /// <summary>
  /// Set the <c>Val</c> property value of the first specified type element in the <c>OpenXmlCompositeElement</c> as a string.
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  public static void SetFirstElementStringVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, string? value)
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
      SetValValue(element, new DX.StringValue(value));
      xmlElement.Append(element);
    }
  }

  /// <summary>
  /// Set the <c>Val</c> property value of the first specified type element in the <c>OpenXmlCompositeElement</c> as an object.
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  public static void SetFirstElementVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, object? value)
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
  public static HexInt? GetFirstElementHexIntVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      var val = element.GetValValue<DX.HexBinaryValue>()?.Value;
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
    where ElementType : DX.OpenXmlLeafElement, new()
  {
    if (value != null)
      SetFirstElementVal<ElementType>(xmlElement, new DX.HexBinaryValue(((int)value).ToString("X8")));
    else
      SetFirstElementVal<ElementType>(xmlElement, null);
  }


  /// <summary>
  /// Get the <c>Val</c> property value of the first specified type element of the <c>OpenXmlCompositeElement</c> as a Guid value.
  /// </summary>
  /// <param name="xmlElement"></param>
  public static Guid? GetFirstElementGuidVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      var val = element.GetValValue<DX.StringValue>()?.Value;
      if (val != null)
        return Guid.Parse(val);
    }
    return null;
  }

  /// <summary>
  /// Set the <c>Val</c> property value of the first specified type element in the <c>OpenXmlCompositeElement</c> as a Guid value.
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  public static void SetFirstElementGuidVal<ElementType>(this DX.OpenXmlCompositeElement xmlElement, Guid? value)
    where ElementType : DX.OpenXmlElement, new()
  {
    if (value != null)
      SetFirstElementStringVal<ElementType>(xmlElement, value.Value.ToString("B"));
    else
      SetFirstElementStringVal<ElementType>(xmlElement, null);
  }

  /// <summary>
  /// Get the fixed point real number value of the first child element of the specified type of the <c>OpenXmlLeafTextElement</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>decimal value or null (on parse error)</result>
  public static decimal? GetFirstElementDecimalValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlLeafTextElement
  {
    var text = xmlElement.Elements<ElementType>().FirstOrDefault()?.Text;
    if (text == null)
      return null;
    if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var val))
      return val;
    return null;
  }

  /// <summary>
  /// Set the text content of the first child element of the specified type  of the <c>OpenXmlLeafTextElement</c> to the fixed point real value.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">decimal value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstElementDecimalValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement, decimal? value)
    where ElementType : DX.OpenXmlLeafTextElement, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      var val = ((decimal)value).ToString(CultureInfo.InvariantCulture);
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
  /// Get the integer value of the first child element of the specified type of the <c>DecimalNumberType</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>decimal value or null (on parse error)</result>
  public static int? GetFirstElementDecimalNumberValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DXW.DecimalNumberType
  {
    var val = xmlElement.Elements<ElementType>().FirstOrDefault()?.Val?.Value;
    return val;
  }

  /// <summary>
  /// Set the value of the first child element of the specified type  of the <c>DecimalNumberType</c> to integer val.
  /// </summary>
  /// <typeparam name="ElementType">element to set</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">integer value (or null)</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstElementDecimalNumberValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement, int? value)
    where ElementType : DXW.DecimalNumberType, new()
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        if (element.Val?.Value != value)
          element.Val = new DX.Int32Value(value);
      }
      else
        xmlElement.Append(new ElementType { Val = new DX.Int32Value(value) });
    }
    else
      element?.Remove();
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
  /// Get the <c>Val</c> value of the first specified type element of type <c>IEnumValueFactory</c> from the <c>OpenXmlCompositeElement</c>.
  /// </summary>
  /// <param name="xmlElement"></param>
  public static ElementValuesType? GetFirstEnumTypeElementVal<ElementType, ElementValuesType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlElement, new()
    where ElementValuesType : struct, DX.IEnumValue, DX.IEnumValueFactory<ElementValuesType>
  {
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element != null)
    {
      ;
      return GetValValue<DX.EnumValue<ElementValuesType>>(element)?.Value;
    }
    return null;
  }


  /// <summary>
  /// Set the value of the first specified type element of type <c>IEnumValueFactory</c>  in the <c>OpenXmlCompositeElement</c>.
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <param name="value"></param>
  public static void SetFirstEnumTypeElementVal<ElementType, ElementValuesType>(this DX.OpenXmlCompositeElement xmlElement, ElementValuesType? value)
    where ElementType : DX.OpenXmlElement, new()
    where ElementValuesType : struct, DX.IEnumValue, DX.IEnumValueFactory<ElementValuesType>
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
      SetValValue(element, new DX.EnumValue<ElementValuesType>(value));
      xmlElement.Append(element);
    }
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
  /// Get the relationship of the specified by <c>relationshipType</c> in the <c>OpenXmlPartRootElement</c>
  /// </summary>
  /// <param name="rootElement"></param>
  /// <param name="relationshipType"></param>
  /// <param name="rId"></param>
  public static string? GetRelationshipValue(this DX.OpenXmlPartRootElement rootElement, string relationshipType, string rId)
  {
    DXPack.OpenXmlPart part = rootElement.OpenXmlPart!;
    var externalRelationships = part.ExternalRelationships.ToList();
    var externalRelationship = externalRelationships
      .FirstOrDefault(r => r.RelationshipType == relationshipType && r.Id == rId);
    if (externalRelationship != null)
    {
      return externalRelationship.Uri.ToString();
    }
    return null;
  }

  /// <summary>
  /// Set the relationship of the specified by <c>relationshipType</c> in the <c>OpenXmlPartRootElement</c> to the specified value.
  /// </summary>
  /// <param name="rootElement"></param>
  /// <param name="relationshipType"></param>
  /// <param name="value"></param>
  public static void SetRelationshipValue(this DX.OpenXmlPartRootElement rootElement, string relationshipType, string? value)
  {
    DXPack.OpenXmlPart part = rootElement.OpenXmlPart!;
    if (value != null)
    {
      var uri = new Uri(value);
      string? rId = null;

      var externalRelationships = part.ExternalRelationships.ToList();
      var externalRelationship = externalRelationships
        .FirstOrDefault(r => r.RelationshipType == relationshipType);
      if (externalRelationship != null)
      {
        if (externalRelationship.Uri.ToString() == uri.ToString())
          rId = externalRelationship.Id;
        else
        {
          part.DeleteExternalRelationship(externalRelationship);
          externalRelationships.Remove(externalRelationship);
        }
      }
      if (rId == null)
      {
        for (int i = 1; ; i++)
        {
          rId = $"rId{i}";
          if (externalRelationships.FirstOrDefault(r => r.Id == rId) == null)
            break;
        }
        part.AddExternalRelationship(relationshipType, uri, rId);
      }
      rootElement.SetFirstRelationshipElementId<DXW.AttachedTemplate>(rId);
    }
    else
    {
      var externalRelationships = part.ExternalRelationships.ToList();
      var externalRelationship = externalRelationships.FirstOrDefault(r => r.RelationshipType == relationshipType);
      if (externalRelationship != null)
      {
        part.DeleteExternalRelationship(externalRelationship);
        externalRelationships.Remove(externalRelationship);
      }
    }
  }

  ///// <summary>
  ///// Get the DocPartReference value of the <c>OpenXmlCompositeElement</c>.
  ///// </summary>
  ///// <param name="xmlElement"></param>
  //public static string? GetFirstElementDocPartReference<ElementType, ChildType>(this DX.OpenXmlCompositeElement xmlElement)
  //  where ElementType : DX.OpenXmlCompositeElement
  //{
  //  var element = xmlElement.Elements<ChildType>().FirstOrDefault();
  //  if (element == null)
  //    return null;
  //  var value = element.DocPartReference?.Val?.Value;
  //  return value;
  //}

  ///// <summary>
  ///// Set the DocPartReference value of the <c>OpenXmlCompositeElement</c> to the specified value.
  ///// </summary>
  ///// <param name="xmlElement"></param>
  ///// <param name="value"></param>
  //public static void SetFirstElementDocPartReference<ElementType, ChildType>(this DX.OpenXmlCompositeElement xmlElement, string? value)
  //  where ElementType : DX.OpenXmlCompositeElement
  //  where ChildType : DX.OpenXmlElement
  //{
  //  var element = xmlElement.Elements<DXW.SdtPlaceholder>().FirstOrDefault();
  //  if (value != null)
  //  {
  //    if (element != null)
  //    {
  //      if (element.DocPartReference?.Val?.Value != value)
  //        element.DocPartReference = new DXW.DocPartReference { Val = new DX.StringValue(value) };
  //    }
  //    else
  //      xmlElement.Append(new DXW.DocPartReference { Val = new DX.StringValue(value) });
  //  }
  //  else
  //    element?.Remove();
  //}

  /// <summary>
  /// Get the <c>VTBlob</c> value of the first child element of the specified type of the <c>OpenXmlCompositeElement</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>value or null</result>
  public static DXVT.VTBlob? GetFirstElementVTBlobValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlCompositeElement
  {
    var vtBlobProperty = typeof(ElementType).GetProperty("VTBlob");
    if (vtBlobProperty == null)
      throw new InvalidDataException($"Property VTBlob not found in {typeof(ElementType)}");
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element == null)
      return null;
    return (DXVT.VTBlob?)vtBlobProperty.GetValue(element);
  }

  /// <summary>
  /// Set the content of the first child element of the specified type of the <c>OpenXmlCompositeElement</c> to the <c>VTBlob</c> value.
  /// </summary>
  /// <typeparam name="ElementType">Type of the element</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">value to set</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstElementVTBlobValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement, DXVT.VTBlob? value)
    where ElementType : DX.OpenXmlCompositeElement, new()
  {
    var vtBlobProperty = typeof(ElementType).GetProperty("VTBlob");
    if (vtBlobProperty == null)
      throw new InvalidDataException($"Property VTBlob not found in {typeof(ElementType)}");
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        var val = (DXVT.VTBlob?)vtBlobProperty.GetValue(element);
        if (val != value)
          vtBlobProperty.SetValue(element, value);
      }
      else
      {
        var newElement = new ElementType();
        vtBlobProperty.SetValue(newElement, value);
        xmlElement.Append(newElement);
      }
    }
    else
      element?.Remove();
  }

  /// <summary>
  /// Get the <c>VTVector</c> value of the first child element of the specified type of the <c>OpenXmlCompositeElement</c>.
  /// </summary>
  /// <param name="xmlElement">checked element</param>
  /// <result>value or null</result>
  public static DXVT.VTVector? GetFirstElementVTVectorValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement)
    where ElementType : DX.OpenXmlCompositeElement
  {
    var VTVectorProperty = typeof(ElementType).GetProperty("VTVector");
    if (VTVectorProperty == null)
      throw new InvalidDataException($"Property VTVector not found in {typeof(ElementType)}");
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (element == null)
      return null;
    return (DXVT.VTVector?)VTVectorProperty.GetValue(element);
  }

  /// <summary>
  /// Set the content of the first child element of the specified type of the <c>OpenXmlCompositeElement</c> to the <c>VTVector</c> value.
  /// </summary>
  /// <typeparam name="ElementType">Type of the element</typeparam>
  /// <param name="xmlElement">element to set</param>
  /// <param name="value">value to set</param>
  /// <remarks>
  /// If the value is null, the existing element is removed.
  /// </remarks>
  public static void SetFirstElementVTVectorValue<ElementType>(this DX.OpenXmlCompositeElement xmlElement, DXVT.VTVector? value)
    where ElementType : DX.OpenXmlCompositeElement, new()
  {
    var VTVectorProperty = typeof(ElementType).GetProperty("VTVector");
    if (VTVectorProperty == null)
      throw new InvalidDataException($"Property VTVector not found in {typeof(ElementType)}");
    var element = xmlElement.Elements<ElementType>().FirstOrDefault();
    if (value != null)
    {
      if (element != null)
      {
        var val = (DXVT.VTVector?)VTVectorProperty.GetValue(element);
        if (val != value)
          VTVectorProperty.SetValue(element, value);
      }
      else
      {
        var newElement = new ElementType();
        VTVectorProperty.SetValue(newElement, value);
        xmlElement.Append(newElement);
      }
    }
    else
      element?.Remove();
  }

  /// <summary>
  /// Gets the <c>Range</c> element from any composite element.
  /// </summary>
  /// <param name="xmlElement"></param>
  /// <returns></returns>
  /// <exception cref="InvalidDataException"></exception>
  public static Range GetRange(this DX.OpenXmlElement xmlElement)
  {
    // ReSharper disable once MergeIntoPattern
    var result = new Range(xmlElement.FirstChild, xmlElement.LastChild);
    return result;
  }
}