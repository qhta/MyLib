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
}