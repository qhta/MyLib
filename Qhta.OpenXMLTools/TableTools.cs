using System;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;

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
  public static string GetText(this Table table, GetTextOptions? options)
  {
    options ??= GetTextOptions.Default;
    StringBuilder sb = new();
    foreach (var element in table.Elements())
    {
      if (element is TableRow row)
      {
        sb.Append(options.TableRowStartTag);
        sb.Append(row.GetText(options));
        sb.Append(options.TableRowEndTag);
      }
    }
    return sb.ToString();
  }

  /// <summary>
  /// Gets the text of all cells in the table row.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this TableRow row, GetTextOptions? options)
  {
    options ??= GetTextOptions.Default;
    StringBuilder sb = new();
    foreach (var element in row.Elements())
    {
      if (element is TableCell cell)
      {
        sb.Append(options.TableCellStartTag);
        sb.Append(cell.GetText(options));
        sb.Append(options.TableCellEndTag);
      }
    }
    return sb.ToString();
  }

  /// <summary>
  /// Gets the text of all paragraph in the table cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this TableCell cell, GetTextOptions? options)
  {
    options ??= GetTextOptions.Default;
    StringBuilder sb = new ();
    foreach (var element in cell.Elements())
    {
      if (element is Paragraph paragraph)
      {
        sb.Append (options.ParagraphStartTag);
        sb.Append(paragraph.GetText(options));
        sb.Append(options.ParagraphEndTag);
      }
    }
    return sb.ToString();
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