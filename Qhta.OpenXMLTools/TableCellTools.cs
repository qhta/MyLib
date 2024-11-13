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
  /// Gets the text of all paragraph in the table cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string? GetText(this TableCell cell, GetTextOptions? options = null)
  {
    List<string> sl = new();
    var indentLevel = options?.IndentLevel ?? 0;
    string indentStr = "";
    if (options!= null && options.IndentTableContent && options.TableRowInSeparateLine)
    {
      options.IndentLevel++;
      indentStr = options.Indent.Duplicate(options.IndentLevel) ?? "";
    }
    foreach (var element in cell.Elements())
    {
      if (element is Paragraph paragraph)
      {
        sl.Add(indentStr);
        if (options != null)
          sl.Add(options.ParagraphStartTag);
        sl.Add(indentStr);
        sl.Add(paragraph.GetText(options));
        sl.Add(indentStr);
        if (options != null)
          sl.Add(options.ParagraphEndTag);
      }
      else
      {
        var aText = element.GetText(options);
        if (aText != null)
          sl.Add(aText);
      }
    }
    if (options != null)
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
    var members = cell.MemberElements().ToList();
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
  public static int GetGridSpan(this DXW.TableCell cell)
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
  public static void SetGridSpan(this DXW.TableCell cell, int value)
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
  /// Returns the left border of the cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static DXW.BorderType? GetLeftBorder(this DXW.TableCell cell)
  {
    return cell.TableCellProperties?.TableCellBorders?.LeftBorder;
  }

  /// <summary>
  /// Sets the left border of the cell.
  /// If set value has a parent, its clone is used.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetLeftBorder(this DXW.TableCell cell, DXW.BorderType? value)
  {
    var oldBorder = cell.TableCellProperties?.TableCellBorders?.LeftBorder;
    if (oldBorder != null)
      oldBorder.Remove();
    if (value != null)
    {
      if (value is not LeftBorder)
        value = new LeftBorder
        {
          Val = value.Val,
          Color = value.Color,
          ThemeColor = value.ThemeColor,
          ThemeShade = value.ThemeShade,
          ThemeTint = value.ThemeTint,
          Size = value.Size,
          Space = value.Space,
          Shadow = value.Shadow,
          Frame = value.Frame,
        };
      else
      {
        if (value.Parent != null)
          value = (LeftBorder)value.CloneNode(true);
      }
      cell.GetTableCellProperties().GetTableCellBorders().Append(value);
    }
  }

  /// <summary>
  /// Returns the right border of the cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <returns></returns>
  public static DXW.BorderType? GetRightBorder(this DXW.TableCell cell)
  {
    return cell.TableCellProperties?.TableCellBorders?.RightBorder;
  }

  /// <summary>
  /// Sets the right border of the cell.
  /// If set value has a parent, its clone is used.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetRightBorder(this DXW.TableCell cell, DXW.BorderType? value)
  {
    var oldBorder = cell.TableCellProperties?.TableCellBorders?.RightBorder;
    if (oldBorder != null)
      oldBorder.Remove();
    if (value != null)
    {
      if (value is not RightBorder)
        value = new RightBorder
        { 
          Val = value.Val, 
          Color = value.Color, 
          ThemeColor = value.ThemeColor,
          ThemeShade = value.ThemeShade,
          ThemeTint = value.ThemeTint,
          Size = value.Size,
          Space = value.Space,
          Shadow = value.Shadow,
          Frame = value.Frame,
        };
      else
      {
        if (value.Parent!=null)
          value = (RightBorder)value.CloneNode(true);
      }
      cell.GetTableCellProperties().GetTableCellBorders().Append(value);
    }
  }
}