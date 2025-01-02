using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Attribute name-value pair.
/// </summary>
/// <param name="name"></param>
/// <param name="value"></param>
public struct AttributeValue(string name, string? value)
{
  /// <summary>
  /// Attribute name.
  /// </summary>
  public readonly string Name = name;
  /// <summary>
  /// Attribute value.
  /// </summary>
  public readonly string? Value = value;
}


/// <summary>
/// Class to format attribute name-value pairs.
/// </summary>
public abstract class AttributeFormatter
{
  ///// <summary>
  ///// Method to get the text formatted of element attributes.
  ///// </summary>
  ///// <param name="element"></param>
  ///// <returns></returns>
  //public abstract string GetIncludedAttributesText(DX.OpenXmlElement element);

  /// <summary>
  /// Method to get the text formatted of several included attributes.
  /// </summary>
  /// <param name="attributes"></param>
  /// <returns></returns>
  public abstract string GetIncludedAttributesText(AttributeValue[] attributes);

  /// <summary>
  /// Method to get the text formatted of one attribute value.
  /// </summary>
  /// <param name="attributeValue"></param>
  /// <returns></returns>
  public abstract string GetAttributeText(AttributeValue attributeValue);

  /// <summary>
  /// Method to get the attribute values of several included attributes.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public abstract AttributeValue[] GetIncludedAttributesValue(string str);

  /// <summary>
  /// Method to get attribute value of a string.
  /// </summary>
  /// <returns></returns>
  public abstract AttributeValue GetAttributeValue(string str);
}

/// <summary>
/// Class to format attribute name-value pairs in plain text.
/// </summary>
public class PlainAttributeFormatter: AttributeFormatter
{
  /// <summary>
  /// Method to get the text formatted of several included attributes.
  /// </summary>
  /// <param name="attributes"></param>
  /// <returns></returns>
  public override string GetIncludedAttributesText(AttributeValue[] attributes)
  {
    var sl = new List<string>();
    foreach (var a in attributes)
    {
      var text = GetAttributeText(a);
      sl.Add(text);
    }
    return "{" + String.Join(",", sl) + "}";
  }

  /// <summary>
  /// Method to get the text formatted of one attribute.
  /// </summary>
  /// <param name="attributeValue"></param>
  /// <returns></returns>
  public override string GetAttributeText(AttributeValue attributeValue)
  {
    if (attributeValue.Value == null)
      return string.Empty;
    return attributeValue.Name + "=" +attributeValue.Value;
  }

  /// <summary>
  /// Method to get the attribute values of several included attributes.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public override AttributeValue[] GetIncludedAttributesValue(string str)
  {
    var result = new List<AttributeValue>();
    var ss = str.Split(',');
    foreach (var s in ss)
    {
      var av = GetAttributeValue(s);
      result.Add(av);
    }
    return result.ToArray();
  }

  /// <summary>
  /// Method to get attribute value of a string.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public override AttributeValue GetAttributeValue(string str)
  {
    var ss = str.Split('=');
    if (ss.Length == 1)
      return new AttributeValue(ss[0], null);
    return new AttributeValue(ss[0], ss[1]);
  }
}
