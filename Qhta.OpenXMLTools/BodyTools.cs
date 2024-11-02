using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Extension methods for the Body class.
/// </summary>
public static class BodyTools
{

  /// <summary>
  /// Gets the last <c>SectionProperties</c> element of the <c>Body</c> element of the document.
  /// If the document does not have a <c>SectionProperties</c>, it can be created.
  /// </summary>
  /// <param name="body"></param>
  /// <param name="create">Option to create a <c>SectionProperties</c>, if it does not exist.</param>
  /// <returns></returns>
  public static DXW.SectionProperties? GetLastSectionProperties(this DXW.Body body, bool create = false)
  {
    var sectionProperties = body.Elements<DXW.SectionProperties>().LastOrDefault();
    if (sectionProperties == null && create)
    {
      sectionProperties = new DXW.SectionProperties();
      body.AppendChild(sectionProperties);
    }
    return sectionProperties;
  }

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