using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with tables in OpenXml documents.
/// </summary>
public static class TableTools
{
  //public static Table? FindParagraph(DXPack.WordprocessingDocument document, string paraId)
  //{
  //  return document.MainDocumentPart?.Document?.Body?.Elements<Paragraph>().FirstOrDefault(p => p.ParagraphId?.Value == paraId);
  //}

  //public static Table? FindParagraph(DX.OpenXmlCompositeElement compositeElement, string paraId)
  //{
  //  return compositeElement.Elements<Paragraph>().FirstOrDefault(p => p.ParagraphId?.Value == paraId);
  //}

  /// <summary>
  /// Gets the text of all rows in the table.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this Table table, GetTextOptions? options=null)
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
    foreach (var element in table.Elements())
    {
      if (element is TableRow row)
      {
        if (options.TableRowInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine) != true)
          sl.Add(options.NewLine);
        sl.Add(indentStr);
        sl.Add(options.TableRowStartTag);
        var aText = row.GetText(options);
        if (aText != null)
          sl.Add(aText);
        if (options.TableRowInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine) != true)
          sl.Add(options.NewLine);
        sl.Add(indentStr);
        sl.Add(options.TableRowEndTag);
      }
    }
    options.IndentLevel = indentLevel;
    return string.Join("", sl);
  }

  /// <summary>
  /// Gets the text of all cells in the table row.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string? GetText(this TableRow row, GetTextOptions? options = null)
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
    foreach (var element in row.Elements())
    {
      if (element is TableCell cell)
      {
        if (options.TableCellInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine) != true)
          sl.Add(options.NewLine);
        sl.Add(indentStr);
        sl.Add(options.TableCellStartTag);
        var aText = cell.GetText(options);
        if (aText != null)
          sl.Add(aText);
        if (options.TableCellInSeparateLine && sl.LastOrDefault()?.EndsWith(options.NewLine) != true)
          sl.Add(options.NewLine);
        sl.Add(indentStr);
        sl.Add(options.TableCellEndTag);
      }
    }
    options.IndentLevel = indentLevel;
    return string.Join("", sl);
  }

  /// <summary>
  /// Gets the text of all paragraph in the table cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string? GetText(this TableCell cell, GetTextOptions? options=null)
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
    }
    options.IndentLevel = indentLevel;
    return string.Join("", sl);
  }

  /// <summary>
  /// Gets the style name of the table.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public static string? Style(this Table table)
  {
    return table.Elements<TableProperties>().FirstOrDefault()?.TableStyle?.Val?.Value;
  }
}