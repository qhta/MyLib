using System;

using DocumentFormat.OpenXml;

using static Qhta.WordInteropOpenXmlConverter.NumberConverter;

using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.WordInteropOpenXmlConverter;
public static class PropertiesConverter
{

  public static String? NotEmptyString(string value)
  {
    if (!string.IsNullOrEmpty(value))
      return value;
    return null;
  }

  public static XType? GetOnOffTypeElement<XType>(int wordValue, int? defaultValue) where XType : W.OnOffType, new()
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
      return new XType { Val = OnOffValue.FromBoolean(wordValue != 0) };
    return null;
  }

  public static OnOffValue? GetOnOffValue(int wordValue, int? defaultValue)
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
      return OnOffValue.FromBoolean(wordValue != 0);
    return null;
  }

  public static XType? GetStringValTypeElement<XType>(int wordValue, int? defaultValue) where XType : OpenXmlLeafElement, new()
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var valProperty = typeof(XType).GetProperty("Val");
      var element = new XType();
      var valStr = wordValue.ToString();
      valProperty?.SetValue(element, valStr);
      return element;
    }
    return null;
  }

  public static int? GetTwipsValue(float wordValue, float? defaultValue)
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var result = PointsToTwips(wordValue);
      return result;
    }
    return null;
  }

  public static int? GetCharsNumber(float wordValue, float? defaultValue)
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var result = (int)wordValue;
      return result;
    }
    return null;
  }

  public static int? GetLinesNumber(float wordValue, float? defaultValue)
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var result = (int)wordValue;
      return result;
    }
    return null;
  }

  public static XType? GetIntValTypeElement<XType>(float wordValue, float? defaultValue) where XType : OpenXmlLeafElement, new()
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var valProperty = typeof(XType).GetProperty("Val");
      var element = new XType();
      valProperty?.SetValue(element, new Int32Value((int)wordValue));
      return element;
    }
    return null;
  }

  public static XType? GetFontSizeTypeElement<XType>(float wordValue, float? defaultValue) where XType : OpenXmlLeafElement, new()
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var valProperty = typeof(XType).GetProperty("Val");
      var element = new XType();
      valProperty?.SetValue(element, new Int32Value(FontSizeToHps(wordValue)));
      return element;
    }
    return null;
  }

  public static W.VerticalTextAlignment? GetVerticalTextAlignment(W.VerticalPositionValues positionValues, int wordValue, int? defaultValue)
  {
    if (wordValue != wdUndefined && wordValue != defaultValue)
    {
      var element = new W.VerticalTextAlignment { Val = positionValues };
      return element;
    }
    return null;
  }

}
