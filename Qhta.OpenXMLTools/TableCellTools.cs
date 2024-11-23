using System;
using System.Text;

using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Options for selecting paragraphs in a cell.
/// </summary>
public enum WhichParagraphs
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  All,
  First,
  Last
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

/// <summary>
/// Tools for working with table cells in OpenXml documents.
/// </summary>
public static class TableCellTools
{

  /// <summary>
  /// Get all the member elements of the cell (except <c>TableCellProperties</c>).
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static IEnumerable<DX.OpenXmlElement> GetMembers(this DX.OpenXmlElement cell)
  {
    foreach (var element in cell.Elements())
    {
      if (element is not DXW.TableCellProperties)
        yield return element;
    }
  }

  /// <summary>
  /// Determines if the cell content is empty or contains only empty paragraphs.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static bool HasSimpleContent(this DXW.TableCell cell)
  {
    var members = cell.GetMembers().ToList();
    return members.Count==0 || (members.Count == 1 && members[0] is DXW.Paragraph singleParagraph);
  }

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
  /// Sets the keep with next property for the paragraphs in the cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="value">value to set</param>
  /// <param name="mode">specifies which paragraphs to set</param>
  public static void SetKeepWithNext(this DXW.TableCell cell, bool value, WhichParagraphs mode)
  {
    if (mode == WhichParagraphs.All)
    {
      foreach (var paragraph in cell.Elements<DXW.Paragraph>())
      {
        paragraph.GetParagraphProperties().SetKeepNext(value);
      }
    }
    else if (mode == WhichParagraphs.First)
    {
      var firstParagraph = cell.Elements<DXW.Paragraph>().FirstOrDefault();
      if (firstParagraph != null)
      {
        firstParagraph.GetParagraphProperties().SetKeepNext(value);
      }
    }
    else if (mode == WhichParagraphs.Last)
    {
      var lastParagraph = cell.Elements<DXW.Paragraph>().LastOrDefault();
      if (lastParagraph != null)
      {
        lastParagraph.GetParagraphProperties().SetKeepNext(value);
      }
    }
  }

  /// <summary>
  /// Determines if the cell contains a long text or non text elements.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static bool IsLong(this DXW.TableCell cell)
  {
    var members = cell.GetMembers().ToList();
    var isLong = members.Any(e => e is not DXW.Paragraph);
    if (!isLong)
    {
      var sb = new StringBuilder();
      int parNumber = 0;
      foreach (var element in members)
      {
        sb.Append(element.GetText());
        parNumber++;
        if (parNumber < members.Count)
          sb.Append("\r\n");
      }
      var text = sb.ToString();
      //if (text.Contains("wordprocessingml.endnotes+xml"))
      //  Debug.Assert(true);
      isLong = text.Length > 500;
    }
    return isLong;
  }

  /// <summary>
  /// Returns the width of the cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static int? GetWidth(this DXW.TableCell cell)
  {
    if (cell.TableCellProperties?.TableCellWidth?.Type?.Value == TableWidthUnitValues.Dxa)
      if (int.TryParse(cell.TableCellProperties?.TableCellWidth?.Width, out var val))
        return val;
    return null;
  }

  /// <summary>
  /// Sets the width of the cell.
  /// If value is null, the width element is removed.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetWidth(this DXW.TableCell cell, int? value)
  {
    var tableCellWidth = cell.TableCellProperties?.TableCellWidth;
    if (value <= 1)
    {
      if (tableCellWidth != null)
        tableCellWidth.Remove();
    }
    else
    {
      if (tableCellWidth == null)
      {
        tableCellWidth = new TableCellWidth();
        cell.GetTableCellProperties().Append(tableCellWidth);
      }
      tableCellWidth.Width = value.ToString();
      tableCellWidth.Type = TableWidthUnitValues.Dxa;
    }
  }

  /// <summary>
  /// Returns the number of columns merged in the cell.
  /// If the cell is not spanned, the returned value is 1.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static int GetSpan(this DXW.TableCell cell)
  {
    return cell.TableCellProperties?.GridSpan?.Val?.Value ?? 1;
  }

  /// <summary>
  /// Sets the number of columns merged in the cell.
  /// if value is 1 or less, the grid span element is removed.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetSpan(this DXW.TableCell cell, int value)
  {
    var gridSpan = cell.TableCellProperties?.GridSpan;
    if (value <= 1)
    {
      if (gridSpan != null)
        gridSpan.Remove();
    }
    else
    {
      if (gridSpan == null)
      {
        gridSpan = new GridSpan();
        cell.GetTableCellProperties().Append(gridSpan);
      }
      gridSpan.Val = value;
    }
  }
  /// <summary>
  /// Returns the border of the table.
  /// </summary>
  /// <param name="Table"></param>
  /// <returns></returns>
  public static T? GetBorder<T>(this DXW.TableCell Table) where T : DXW.BorderType
  {
    return Table.GetTableCellProperties()?.TableCellBorders?.GetBorder<T>();
  }

  /// <summary>
  /// Set the border of the cell.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="cell"></param>
  /// <param name="value"></param>
  public static void SetBorder<T>(this DXW.TableCell cell, DXW.BorderType? value) where T : DXW.BorderType
  {
    cell.GetTableCellProperties().GetTableCellBorders().SetBorder<T>(value);
  }


  /// <summary>
  /// Sets justification for all paragraphs in the cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="value"></param>
  public static void SetJustification(this DXW.TableCell cell, JustificationValues value)
  {
    foreach (var paragraph in cell.Elements<DXW.Paragraph>())
    {
      paragraph.SetJustification(value);
    }
  }

  /// <summary>
  /// Set the cell background color.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="backgroundColor"></param>
  public static void SetBackgroundColor(this DXW.TableCell cell, int backgroundColor)
  {
    cell.GetTableCellProperties().SetShading(ShadingPatternValues.Clear, "auto", backgroundColor.ToString("X6"));
  }

}