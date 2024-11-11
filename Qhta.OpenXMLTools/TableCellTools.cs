using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table cells in OpenXml documents.
/// </summary>
public static class TableCellTools
{


  /// <summary>
  /// Gets the text of all paragraph in the table cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string? GetText(this TableCell cell, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    List<string> sl = new();
    var indentLevel = options.IndentLevel;
    string indentStr = "";
    if (options.IndentTableContent && options.TableRowInSeparateLine)
    {
      options.IndentLevel++;
      indentStr = options.Indent.Duplicate(options.IndentLevel) ?? "";
    }
    foreach (var element in cell.Elements())
    {
      if (element is Paragraph paragraph)
      {
        sl.Add(indentStr);
        sl.Add(options.ParagraphStartTag);
        sl.Add(indentStr);
        sl.Add(paragraph.GetText(options));
        sl.Add(indentStr);
        sl.Add(options.ParagraphEndTag);
      }
      else
      {
        var aText = element.GetText(options);
        if (aText != null)
          sl.Add(aText);
      }
    }
    options.IndentLevel = indentLevel;
    return string.Join("", sl);
  }

  /// <summary>
  /// Return all elements that are not <c>TableCellProperties</c>
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static IEnumerable<DX.OpenXmlElement> MemberElements(this TableCell cell)
    => cell.Elements().Where(e => e is not TableCellProperties);

  /// <summary>
  /// Get the <c>TableCellProperties</c> element of the cell. If it does not exist, it will be created.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static TableCellProperties GetTableCellProperties(this TableCell cell)
  {
    var cellProperties = cell.TableCellProperties;
    if (cellProperties == null)
    {
      cellProperties = new TableCellProperties();
      cell.Append(cellProperties);
    }
    return cellProperties;
  }
  
  /// <summary>
  /// Sets the keep with next property for the last property in the cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="value"></param>
  public static void SetKeepWithNext(this DXW.TableCell cell, bool value)
  {
    var lastParagraph = cell.Elements<DXW.Paragraph>().LastOrDefault();
    if (lastParagraph!=null)
    {
      lastParagraph.GetParagraphProperties().SetKeepNext(value);
    }
  }

  /// <summary>
  /// Determines if the cell contains a long text or non text elements.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static bool IsLong(this DXW.TableCell cell)
  {
    var members = cell.MemberElements().ToList();
    var isLong = members.Any(e => e is not DXW.Paragraph)
                 || members.Count(e => e is DXW.Paragraph) > 1
                 || members.Any(p => p.GetText()?.Length > 500);
    return isLong;
  }
}