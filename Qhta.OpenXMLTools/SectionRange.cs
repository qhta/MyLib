using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;


namespace OpenXMLTools
{
  public class SectionRange
  {
    public OpenXmlElement Start { get; set; }
    public OpenXmlElement End { get; set; }
    public SectionProperties SectionProperties { get; set; }

    public IEnumerable<OpenXmlElement> Elements
    {
      get
      {
        List<OpenXmlElement> result = new List<OpenXmlElement>();
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
  }
}
