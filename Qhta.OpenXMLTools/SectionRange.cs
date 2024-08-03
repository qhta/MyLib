using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;


namespace Qhta.OpenXmlTools;

/// <summary>
/// Class to represent a range of elements in a Word section of the document.
/// </summary>
public class SectionRange
{
  /// <summary>
  /// First element in the range.
  /// </summary>
  public OpenXmlElement Start { get; internal set; } = null!;
  /// <summary>
  /// Last element in the range.
  /// </summary>
  public OpenXmlElement End { get; internal set; } = null!;
  /// <summary>
  /// Section properties of the range.
  /// </summary>
  public SectionProperties SectionProperties { get; internal set; } = null!;

  /// <summary>
  /// Gets the elements of the specified type in the range.
  /// If the start element is not initialized, it will return an empty list.
  /// If the end element is not initialized, it will return all elements from the start to the end of the section.
  /// </summary>
  public IEnumerable<OpenXmlElement> GetElements()
  {
    List<OpenXmlElement> result = new List<OpenXmlElement>();
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (Start != null)
    {
      var element = Start;
      while (element != null)
      {
        if (element == SectionProperties)
          break;
        if (element.Elements().Count() == 1)
        {
          var firstChild = element.Elements().First();
          if (firstChild == SectionProperties)
            break;
          if (firstChild.Elements().Count() == 1)
          {
            firstChild = firstChild.Elements().First();
            if (firstChild == SectionProperties)
              break;
          }
        }
        result.Add(element);
        if (element != End)
          element = element.NextSibling();
        else
          break;
      }
    }
    return result;
  }
}