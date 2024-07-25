using System.Collections.ObjectModel;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Body element.
/// </summary>
public static class BodyTools
{
  /// <summary>
  /// Get the SectionProperties element of the Body element.
  /// </summary>
  /// <param name="body">Element to browse</param>
  /// <returns>SectionProperties element or null</returns>
  public static SectionProperties? GetSectionProperties(this Body body)
  {
    return body.Descendants<SectionProperties>().FirstOrDefault();
  }

  /// <summary>
  /// Get the collection of <see cref="SectionRange"/> element of the Body element.
  /// Each element of the collection contains a <see cref="SectionProperties"/> element
  /// and the start and end elements of the section which refers to the first and last paragraphs
  /// (or other OpenXmlElements) of the section.
  /// </summary>
  /// <param name="body">Element to browse</param>
  /// <returns>Observable collection of section range elements</returns>
  public static Collection<SectionRange> GetSections(this Body body)
  {
    List<SectionProperties> sectionProps = body.Descendants<SectionProperties>().ToList();
    Collection<SectionRange> sections = new ();
    for (int i = 0; i < sectionProps.Count; i++)
    {
      SectionRange newSection = new SectionRange { SectionProperties = sectionProps[i] };
      if (i == 0)
        newSection.Start = body.Elements().FirstOrDefault()!;
      else
      {
        var sectionStart=  sections[i - 1].End?.NextSibling();
        if (sectionStart!=null)
          newSection.Start = sectionStart;
      }
      Debug.Assert(newSection.Start != null, "Section start is null");
      var endElement = sectionProps[i].Parent;
      if (endElement is ParagraphProperties)
        endElement = endElement.Parent;
      if (endElement != null)
        newSection.End = endElement;
      Debug.Assert(newSection.End != null, "Section end is null");
      sections.Add(newSection);
    }
    return sections;
  }
}