using System;

using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Borders element.
/// </summary>
public static class BordersTools
{

  /// <summary>
  /// Convert a list of <see cref="BorderType"/> elements
  /// to a typed OpenXml borders element.
  /// </summary>
  /// <param name="borderList">Enumeration of source <see cref="BorderType"/> elements</param>
  /// <typeparam name="OpenXmlElementType">Target OpenXml element type. It must be a composite element</typeparam>
  /// <returns>New <typeparamref name="OpenXmlElementType"/> element</returns>
  public static OpenXmlElementType ToOpenXmlBorders<OpenXmlElementType>(this IEnumerable<BorderType> borderList) 
    where OpenXmlElementType : DX.OpenXmlCompositeElement, new()
  {
    var bordersElement = new OpenXmlElementType();
    foreach (var border in borderList)
    {
     bordersElement.Append(border);
    }
    return bordersElement;
  }

  /// <summary>
  /// Determines whether the border is visible.
  /// </summary>
  /// <param name="border"></param>
  /// <returns></returns>
  public static bool IsVisible(this BorderType? border)
  {
    return border!= null && border.Val?.Value != BorderValues.Nil;
  }

  /// <summary>
  /// Get the border of the specific type to the borders element.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="bordersElement"></param>
  public static T? GetBorder<T>(this DX.OpenXmlElement bordersElement) where T : DXW.BorderType
  {
    var bordersType = bordersElement.GetType();
    if (!bordersType.Name.EndsWith("Borders"))
      throw new ArgumentException($"The element {bordersElement.GetType().Name} is not a borders element");
    var property = bordersType.GetProperty(typeof(T).Name);
    if (property == null)
      throw new ArgumentException($"The element {bordersElement.GetType().Name} does not contain a property of type {typeof(T).Name}");
    if (property.GetValue(bordersElement) is T oldBorder)
      return oldBorder;
    return null;
  }

  /// <summary>
  /// Set the border of the specific type to the borders element.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="bordersElement"></param>
  /// <param name="value"></param>
  public static void SetBorder<T>(this DX.OpenXmlElement bordersElement, DXW.BorderType? value) where T : DXW.BorderType
  {
    var bordersType = bordersElement.GetType();
    if (!bordersType.Name.EndsWith("Borders"))
      throw new ArgumentException($"The element {bordersElement.GetType().Name} is not a borders element");
    var property = bordersType.GetProperty(typeof(T).Name);
    if (property==null)
      throw new ArgumentException($"The element {bordersElement.GetType().Name} does not contain a property of type {typeof(T).Name}");
    if (property.GetValue(bordersElement) is BorderType oldBorder)
      oldBorder.Remove();
    if (value != null)
    {
      if (value is not T)
      {
        var newValue = (T)Activator.CreateInstance(typeof(T));
        value.CopyProperties(newValue);
        value = newValue;
      }
      else
      {
        if (value.Parent != null)
          value = (T)value.CloneNode(true);
      }
      bordersElement.Append(value);
    }
  }

  /// <summary>
  /// Copies the properties of a <see cref="BorderType"/> element to another.
  /// </summary>
  /// <param name="source"></param>
  /// <param name="target"></param>
  public static void CopyProperties(this DXW.BorderType source, DXW.BorderType target)
  {
    target.Color = source.Color;
    target.Space = source.Space;
    target.Val = source.Val;
    target.ThemeColor = source.ThemeColor;
    target.ThemeShade = source.ThemeShade;
    target.ThemeTint = source.ThemeTint;
    target.Size = source.Size;
    target.Shadow = source.Shadow;
    target.Frame = source.Frame;
  }

  /// <summary>
  /// Check if any of the borders elements is visible.
  /// </summary>
  /// <param name="bordersElement"></param>
  /// <returns></returns>
  public static bool IsVisible(this DX.OpenXmlElement bordersElement)
  {
    foreach (var border in bordersElement.Elements<DXW.BorderType>())
    {
      if (border.Val?.Value != BorderValues.Nil)
        return true;
    }
    return false;
  }
}