using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Extension methods for the Body class.
/// </summary>
public static class BodyTools
{
  /// <summary>
  /// Gets the section properties of the body.
  /// </summary>
  /// <param name="body"></param>
  /// <returns></returns>
  public static SectionProperties? GetSectionProperties(this Body body)
  {
    return body.Descendants<SectionProperties>().FirstOrDefault();
  }

  /// <summary>
  /// Gets all the sections
  /// </summary>
  /// <param name="body"></param>
  /// <returns></returns>
  public static IEnumerable<SectionRange> GetSections(this Body body)
  {
    List<SectionProperties> sectionProps = body.Descendants<SectionProperties>().ToList();
    List<SectionRange> sections = new List<SectionRange>();
    for (int i = 0; i < sectionProps.Count; i++)
    {
      SectionRange newSection = new SectionRange (sectionProps[i]);
      if (i == 0)
        newSection.Start = body.Elements().FirstOrDefault();
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