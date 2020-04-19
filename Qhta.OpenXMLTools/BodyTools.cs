using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OpenXMLTools
{
  public static class BodyTools
  {
    public static SectionProperties GetSectionProperties(this Body body)
    {
      return body.Descendants<SectionProperties>().FirstOrDefault();
    }

    public static IEnumerable<SectionRange> GetSections(this Body body)
    {
      List<SectionProperties> sectionProps = body.Descendants<SectionProperties>().ToList();
      List<SectionRange> sections = new List<SectionRange>();
      for (int i = 0; i < sectionProps.Count; i++)
      {
        SectionRange newSection = new SectionRange { SectionProperties = sectionProps[i] };
        if (i == 0)
          newSection.Start = body.Elements().FirstOrDefault();
        else
        {
          newSection.Start = sections[i - 1].End.NextSibling();
        }
        var endElement = sectionProps[i].Parent;
        if (endElement is ParagraphProperties)
          endElement = endElement.Parent;
        newSection.End = endElement;
        sections.Add(newSection);
      }
      return sections;
    }
  }

}
