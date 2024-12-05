using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Class to represent a range of elements in a Word document.
/// </summary>
public class Range(OpenXmlElement? start, OpenXmlElement? end)
{
  /// <summary>
  /// First element in the range.
  /// </summary>
  public OpenXmlElement? Start { get; set; } = start;

  /// <summary>
  /// Last element in the range.
  /// </summary>
  public OpenXmlElement? End { get; set; } = end;

  /// <summary>
  /// Gets all the member elements in the range.
  /// </summary>
  public IEnumerable<OpenXmlElement> GetMembers()
  {
    var element = Start;
    while (element != null)
    {
      if (element.IsMember())
        yield return element;
      if (element == End)
        break;
      element = element.NextSibling();
    }
  }


  /// <summary>
  /// Gets elements of the specified type in the range.
  /// </summary>
  public ElementType[] GetElements<ElementType>() where ElementType : DX.OpenXmlElement
  {
    List<ElementType> result = new();
    var element = Start;
    while (element != null)
    {
      if (element is ElementType acceptedElement)
        result.Add(acceptedElement);
      if (element == End)
        break;
      element = element.NextSibling();
    }
    return result.ToArray();
  }


  /// <summary>
  /// Gets paragraphs in the range.
  /// </summary>
  public IEnumerable<DXW.Paragraph> GetParagraphs(bool getDescendant)
  {
    var element = Start;
    while (element != null)
    {
      if (element is DXW.Paragraph paragraph)
        yield return paragraph;
      else if (getDescendant)
      {
        foreach (var para in element.Descendants<DXW.Paragraph>())
          yield return para;
      }
      if (element == End)
        break;
      element = element.NextSibling();
    }
  }

  /// <summary>
  /// Gets all tables in the range.
  /// </summary>
  public IEnumerable<DXW.Table> GetTables(bool getDescendant)
  {
    var element = Start;
    while (element != null)
    {
      if (element is DXW.Table Table)
        yield return Table;
      else if (getDescendant)
      {
        foreach (var para in element.Descendants<DXW.Table>())
          yield return para;
      }
      if (element == End)
        break;
      element = element.NextSibling();
    }
  }

  /// <summary>
  /// Get the text content of the run.
  /// </summary>
  /// <param name="options"></param>
  /// <returns></returns>
  public string GetText(TextOptions options)
  {
    
    List<string> sl = new();
    var element = Start;
    while (element != null)
    {
      var aText = element.GetText(options with { OuterText = true});
      sl.Add(aText);
      if (element == End)
        break;
      element = element.NextSibling();
    }
    return string.Join("",sl);
  }
}