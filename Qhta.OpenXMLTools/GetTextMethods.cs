using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Methods to get text from OpenXml elements.
/// </summary>
public static class GetTextMethods
{

  /// <summary>
  /// Gets the text of the collection of elements
  /// </summary>
  /// <param name="elements"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this IEnumerable<DX.OpenXmlElement> elements, TextOptions options)
  {
    var sb = new StringBuilder();
    foreach (var element in elements)
    {
      var text = DispatchGetText(element, options);
      if (text != null)
        sb.Append(text);
      else if (element is DXW.Drawing drawing)
      {
        if (options.IncludeDrawings)
          sb.Append(drawing.GetText(options));
      }
      else
      {
        if (options.IncludeOtherMembers) 
          sb.Append(element.GetText(options));
      }
    }
    return sb.ToString();
  }

  /// <summary>
  /// Gets the text of the element.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DX.OpenXmlElement element, TextOptions options)
  {
    var text = DispatchGetText(element, options);
    if (text != null)
      return text;
    if (element is DX.OpenXmlCompositeElement compositeElement)
    {
      var sb = new StringBuilder();
      var memberTag = element.GetType().Name.ToLowerFirst();
      if (memberTag == "blipFill")
        Debug.Assert(true);
      var members1 = compositeElement.GetMembers().ToList();
      if (members1.Any())
      {
        sb.Append($"<{memberTag}>");
        foreach (var item in members1)
        {
          sb.Append(item.GetText(options));
        }
        sb.Append($"</{memberTag}>");
        return sb.ToString();
      }
      else
        return $"<{memberTag}/>";
    }
    else
    {
      var memberTag = element.GetType().Name.ToLowerFirst();
      return $"<{memberTag}/>";
    }
  }

  private static string? DispatchGetText(DX.OpenXmlElement element, TextOptions options)
  {
    if (element is DXW.Text text)
      return text.GetText(options);
    else if (element is DXW.Run run)
      return run.GetText(options);
    else if (element is DXW.Paragraph paragraph)
      return paragraph.GetText(options);
    else if (element is DXW.Hyperlink hyperlink)
      return hyperlink.GetText(options);
    else if (element is DX.OpenXmlLeafTextElement textElement)
      return textElement.GetText(options);
    else if (element is DXW.Table table)
      return table.GetText(options);
    else if (element is DXW.Break @break)
      return @break.GetText(options);
    else if (element is TabChar)
      return options.TabTag;
    else if (element is CarriageReturn)
      return options.CarriageReturnTag;
    else if (element is FieldCode fieldCode)
        return fieldCode.GetText(options);
    else if (element is SymbolChar symbolChar)
      return symbolChar.GetText(options);
    else if (element is PositionalTab)
      return options.TabTag;
    else if (element is FieldChar fieldChar)
      return fieldChar.GetText(options);
    else if (element is Ruby ruby)
      return ruby.GetPlainText();
    else if (element is FootnoteReference footnoteReference)
      return footnoteReference.GetText(options);
    else if (element is EndnoteReference endnoteReference)
      return endnoteReference.GetText(options);
    else if (element is CommentReference commentReference)
      return commentReference.GetText(options);
    else if (element is DXD.Blip blip)
      return blip.GetText(options);
    return null;
  }

  /// <summary>
  /// Get the text content of the run.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.Run run, TextOptions options)
  {
    StringBuilder sb = new();
    if (options.UseHtmlFormatting && options.UseHtmlEntities)
    {
      if (run.RunProperties?.GetBold(false) == true)
        sb.Append(options.BoldStartTag);
      if (run.RunProperties?.GetItalic(false) == true)
        sb.Append(options.ItalicStartTag);
      if (run.RunProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Superscript)
        sb.Append(options.SuperscriptStartTag);
      if (run.RunProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Subscript)
        sb.Append(options.SubscriptStartTag);
    }
    sb.Append(run.GetMembers().GetText(options));
    if (options.UseHtmlFormatting && options.UseHtmlEntities)
    {
      if (run.RunProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Subscript)
        sb.Append(options.SubscriptEndTag);
      if (run.RunProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Superscript)
        sb.Append(options.SuperscriptEndTag);
      if (run.RunProperties?.GetItalic(false) == true)
        sb.Append(options.ItalicEndTag);
      if (run.RunProperties?.GetBold(false) == true)
        sb.Append(options.BoldEndTag);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Get the text of the run text element.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.Text text, TextOptions options)
  {
    if (options.UseHtmlEntities)
      return text.Text.HtmlEncode();
    else
      return text.Text;
  }

  /// <summary>
  /// Get the text of any textElement element.
  /// </summary>
  /// <param name="textElement"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DX.OpenXmlLeafTextElement textElement, TextOptions options)
  {
    if (options.UseHtmlEntities)
      return textElement.Text.HtmlEncode();
    else
      return textElement.Text;
  }

  /// <summary>
  /// Get the text of the Break element.
  /// </summary>
  /// <param name="break"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.Break @break, TextOptions options)
  {
    if (@break.Type?.Value == BreakValues.Page)
      return options.BreakPageTag;
    else if (@break.Type?.Value == BreakValues.Column)
      return options.BreakColumnTag;
    else if (@break.Type?.Value == BreakValues.TextWrapping)
      return options.BreakLineTag;
    return String.Empty;
  }

  /// <summary>
  /// Get the text of the SymbolChar element.
  /// </summary>
  /// <param name="symbolChar"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.SymbolChar symbolChar, TextOptions options)
  {
    if (int.TryParse(symbolChar.Char!.Value, out var symbolVal))
    {
      return new String((char)symbolVal, 1);
    }
    return String.Empty;
  }

  /// <summary>
  /// Get the text of the FieldChar element.
  /// </summary>
  /// <param name="fieldChar"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.FieldChar fieldChar, TextOptions options)
  {
    if (fieldChar.FieldCharType?.Value == FieldCharValues.Begin && options.IncludeFieldFormula)
    {
      return options.FieldStartTag;
    }
    else if (fieldChar.FieldCharType?.Value == FieldCharValues.Separate && options.IncludeFieldFormula)
    {
      return options.FieldResultTag;
    }
    else if (fieldChar.FieldCharType?.Value == FieldCharValues.End && options.IncludeFieldFormula)
    {
      return options.FieldEndTag;
    }
    return String.Empty;
  }

  /// <summary>
  /// Get the text of the FieldCode element.
  /// </summary>
  /// <param name="fieldCode"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.FieldCode fieldCode, TextOptions options)
  {
    return fieldCode.Text;
  }

  /// <summary>
  /// Get the text of the FootnoteReference element.
  /// </summary>
  /// <param name="footnoteReference"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.FootnoteReference footnoteReference, TextOptions options)
  {
    return options.FootnoteRefStart + footnoteReference.Id + options.FootnoteRefEnd;
  }

  /// <summary>
  /// Get the text of the EndnoteReference element.
  /// </summary>
  /// <param name="endnoteReference"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.EndnoteReference endnoteReference, TextOptions options)
  {
    return options.EndnoteRefStart + endnoteReference.Id + options.EndnoteRefEnd;
  }

  /// <summary>
  /// Get the text of the CommentReference element.
  /// </summary>
  /// <param name="commentReference"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.CommentReference commentReference, TextOptions options)
  {
    return options.CommentRefStart + commentReference.Id + options.CommentRefEnd;
  }

  /// <summary>
  /// Get the text of the paragraph.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  public static string GetText(this Paragraph paragraph, TextOptions options)
  {
    var sb = new StringBuilder();
    var paraText = paragraph.GetInnerText(options);
    if (options.UseHtmlParagraphs)
    {
      if (paraText == string.Empty)
        sb.Append(options.ParagraphSeparator);
      else
      {
        sb.Append(options.ParagraphStartTag);
        sb.Append(paraText);
        sb.Append(options.ParagraphEndTag);
      }
    }
    else
    {
      if (paraText == string.Empty)
      {
        if (!options.IgnoreEmptyParagraphs)
          sb.Append(options.ParagraphSeparator);
      }
      else
      {
        sb.Append(paraText);
        sb.Append(options.ParagraphSeparator);
      }
    }
    return sb.ToString();
  }

  /// <summary>
  /// Get the text of the paragraph members elements.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  public static string GetInnerText(this Paragraph paragraph, TextOptions options)
  {
    var result = String.Join("", paragraph.GetMembers().Select(item => item.GetText(options)));
    if (options.IncludeParagraphNumbering)
      result = paragraph.GetNumberingString(options) + result;
    return result;
  }

  /// <summary>
  /// Gets the text table including table tags
  /// </summary>
  /// <param name="table"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this Table table, TextOptions options)
  {
    var sb = new StringBuilder();
    if (options.IgnoreTableContents)
    {
      if (options.UseHtmlTables)
      {
        sb.Append(options.TableSubstituteTag);
      }
    }
    else
    {
      if (options.UseHtmlTables)
      {
        sb.Append(options.TableStartTag);
        sb.Append(table.GetInnerText(options));
        if (options.UseHtmlTables)
          sb.Append(options.TableEndTag);
        else
          sb.Append(options.TableSeparator);
      }
      else
      {
        var tableText = table.GetInnerText(options);
        sb.Append(tableText);
      }
    }
    return sb.ToString();
  }
  /// <summary>
  /// Gets the text of all rows in the table.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetInnerText(this Table table, TextOptions options)
  {
    var sb = new StringBuilder();
    var rows = table.GetRows().ToList();
    foreach (var row in rows)
    {
      if (options.UseHtmlTables)
        sb.Append(options.TableRowStartTag);
      sb.Append(row.GetText(options));
      if (options.UseHtmlTables)
        sb.Append(options.TableRowEndTag);
      else
        sb.Append(options.TableRowSeparator);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Gets the text of all cells in the table row.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this TableRow row, TextOptions options)
  {
    List<string> sl = new();
    var cells = row.GetCells().ToList();
    for (var i = 0; i < cells.Count; i++)
    {
      var cell = cells[i];
      if (options.UseHtmlTables)
      {
        sl.Add(options.TableCellStartTag);
        if (!cell.HasSimpleContent())
        {
          var aText = cell.GetInnerText(options);
          sl.Add(aText);
        }
        else
        {
          var aText = cell.GetInnerText(options);
          sl.Add(aText);
        }
      }
      else
      {
        var aText = cell.GetInnerText(options);
        sl.Add(aText);
      }
      if (options.UseHtmlTables)
        sl.Add(options.TableCellEndTag);
      else if (i < cells.Count - 1)
        sl.Add(options.TableCellSeparator);
    }
    return string.Join("", sl);
  }

  /// <summary>
  /// Gets inner text the table cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetInnerText(this TableCell cell, TextOptions options)
  {
    var members = cell.GetMembers().ToList();
    if (members.Any())
    {
      if (members.Count() == 1 && members[0] is DXW.Paragraph singleParagraph)
      {
        return singleParagraph.GetInnerText(options);
      }
      else
      {
        return (cell.GetMembers()).GetText(options);
      }
    }
    return string.Empty;
  }

  /// <summary>
  /// Get the text for the drawing element.
  /// </summary>
  /// <param name="drawing"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.Drawing drawing, TextOptions options)
  {
    if (options.IncludeDrawings)
    {
      if (options.IgnoreDrawingContents)
      {
        return options.DrawingSubstituteTag;
      }
      else
      {
        return options.DrawingStartTag + (drawing.GetMembers()).GetText(options) + options.DrawingEndTag;
      }
    }
    return string.Empty;
  }

  /// <summary>
  /// Get the text for the Blip element.
  /// </summary>
  /// <param name="blip"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXD.Blip blip, TextOptions options)
  {
    var str = options.BlipTag;
    var embed = blip.Embed?.Value;
    if (embed != null)
      str = str.Insert(str.Length - 2, $" embed=\"{embed}\"");
    var link = blip.Link?.Value;
    if (link != null)
      str = str.Insert(str.Length - 2, $" link=\"{link}\"");
    return str;
  }
}