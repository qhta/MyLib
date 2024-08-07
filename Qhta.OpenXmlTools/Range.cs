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
  public OpenXmlElement? Start { get; internal set; } = start;

  /// <summary>
  /// Last element in the range.
  /// </summary>
  public OpenXmlElement? End { get; internal set; } = end;

  /// <summary>
  /// Gets all the elements in the range.
  /// </summary>
  public OpenXmlElement[] GetElements()
  {
    List<OpenXmlElement> result = new();
    var element = Start;
    while (element != null)
    {
      result.Add(element);
      if (element == End)
        break;
      element = element.NextSibling();
    }
    return result.ToArray();
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
  public DXW.Paragraph[] GetParagraphs()
  {
    List<DXW.Paragraph> result = new();
    var element = Start;
    while (element != null)
    {
      if (element is DXW.Paragraph paragraph)
        result.Add(paragraph);
      if (element == End)
        break;
      element = element.NextSibling();
    }
    return result.ToArray();
  }

  /// <summary>
  /// Gets all tables in the range.
  /// </summary>
  public DXW.Table[] GetTables()
  {
    List<DXW.Table> result = new();
    var element = Start;
    while (element != null)
    {
      if (element is DXW.Table table)
        result.Add(table);
      if (element == End)
        break;
      element = element.NextSibling();
    }
    return result.ToArray();
  }

  /// <summary>
  /// Get the text content of the run.
  /// </summary>
  /// <param name="options"></param>
  /// <returns></returns>
  public string GetText(GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    List<string> sl = new();
    var element = Start;
    while (element != null)
    {
      if (element is Paragraph paragraph)
      {
        sl.Add(options.ParagraphStartTag);
        sl.Add(paragraph.GetText(options));
        sl.Add(options.ParagraphEndTag);
      }
      if (element is Table table)
      {
        if (options.TableInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine)==false)
          sl.Add(options.NewLine);
        sl.Add(options.TableStartTag);
        sl.Add(table.GetText(options));
        if (options.TableInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine) == false)
          sl.Add(options.NewLine);
        sl.Add(options.TableEndTag);
      }
      if (element == End)
        break;
      element = element.NextSibling();
    }
    return string.Join("",sl);
  }
}