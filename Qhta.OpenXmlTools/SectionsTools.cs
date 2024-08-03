using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing sections.
/// </summary>
public static class SectionsTools
{
  /// <summary>
  /// Check if the document has section properties defined.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument</param>
  /// <returns>True if the document has more than one section properties defined</returns>
  public static bool HasSectionProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.MainDocumentPart?.Document?.Body?.Descendants<SectionProperties>().Any()==true;
  }

  /// <summary>
  /// Gets all the sections properties from the document.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument</param>
  /// <returns>Instance of the sections element</returns>
  public static DXW.SectionProperties[] GetSectionProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    var body = wordDoc.GetBody();
    return body.Descendants<SectionProperties>().ToArray();
  }

  /// <summary>
  /// Gets the section properties from the document body. If the body does not have a section properties element, it is created.
  /// </summary>
  /// <param name="body">Body of the WordprocessingDocument</param>
  /// <returns>Instance of the sections element</returns>
  public static DXW.SectionProperties GetSectionProperties(this DXW.Body body)
  {
    var result = body.Last() as SectionProperties;
    if (result == null)
    {
      result = new SectionProperties();
      body.Append(result);
    }
    return result;
  }

  /// <summary>
  /// Gets all the section ranges from the document body.
  /// </summary>
  /// <param name="body">Body of the WordprocessingDocument</param>
  /// <returns>Instance of the sections element</returns>
  public static SectionRange[] GetSectionRanges(this DXW.Body body)
  {
    var result = new List<SectionRange>();
    
    var sectionProperties = body.Descendants<SectionProperties>().ToArray();
    DX.OpenXmlElement? prevElement = null;
    foreach (var sectPr in sectionProperties)
    {
      var range = new SectionRange { SectionProperties = sectPr };
      prevElement ??= body.First();
      range.Start = prevElement;
      if (sectPr.Parent is ParagraphProperties parentParaProps)
      {
        range.End = parentParaProps.Parent!;
      }
      else
      {
        range.End = sectPr;
      }
      prevElement = range.End.NextSibling();
      result.Add(range);
    }
    return result.ToArray();
  }
}