using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using System;
using DocumentFormat.OpenXml;

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
      var text = DispatchGetText(element, options with { OuterText = true });
      if (text != null)
        sb.Append(text);
      else if (element is DXW.Drawing drawing)
      {
        if (options.IncludeDrawings)
          sb.Append(drawing.GetTextOf(options));
        else
          sb.Append(options.ObjectReplacement);
      }
      else
      {
        if (options.IncludeOtherMembers) 
          sb.Append(element.GetText(options));
        else
          sb.Append(options.ObjectReplacement);
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
    if (element is DX.OpenXmlCompositeElement compositeElement && !options.IgnoreOtherMembersContent)
    {
      var sb = new StringBuilder();
      var memberTag = element.GetType().Name.ToLowerFirst();
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
    if (options.IncludeOtherMembers)
    {                    
      var memberTag = element.GetType().Name.ToLowerFirst();
      return $"<{memberTag}/>";
    }
    return "";
  }

  private static string? DispatchGetText(DX.OpenXmlElement element, TextOptions options)
  {
    if (element is DXW.Text text)
      return text.GetTextOf(options);
    else if (element is DXW.Run run)
      return run.GetTextOf(options);
    else if (element is DXW.Paragraph paragraph)
      return paragraph.GetTextOf(options);
    else if (element is DXW.Hyperlink hyperlink)
      return hyperlink.GetTextOf(options);
    else if (element is DX.OpenXmlLeafTextElement textElement)
      return textElement.GetTextOf(options);
    else if (element is DXW.Table table)
      return table.GetTextOf(options);
    else if (element is DXW.TableRow tableRow)
      return tableRow.GetTextOf(options);
    else if (element is DXW.TableCell tableCell)
      return tableCell.GetTextOf(options);
    else if (element is DXW.Break @break)
      return @break.GetTextOf(options);
    else if (element is TabChar)
      return options.TabChar;
    else if (element is CarriageReturn)
      return options.CarriageReturnTag;
    else if (element is FieldCode fieldCode)
        return fieldCode.GetTextOf(options);
    else if (element is SymbolChar symbolChar)
      return symbolChar.GetTextOf(options);
    else if (element is PositionalTab)
      return options.TabChar;
    else if (element is FieldChar fieldChar)
      return fieldChar.GetTextOf(options);
    else if (element is Ruby ruby)
      return ruby.GetPlainText();
    else if (element is FootnoteReference footnoteReference)
      return footnoteReference.GetTextOf(options);
    else if (element is EndnoteReference endnoteReference)
      return endnoteReference.GetTextOf(options);
    else if (element is CommentReference commentReference)
      return commentReference.GetTextOf(options);
    //else if (element is AnnotationReferenceMark annotationReferenceMark)
    //  return annotationReferenceMark.GetTextOf(options);
    //typeof(DXW.AnnotationReferenceMark),
    //typeof(DXW.Break),
    //typeof(DXW.CommentReference),
    //typeof(DXW.ContentPart),
    //typeof(DXW.ContinuationSeparatorMark),
    //typeof(DXW.CarriageReturn),
    //typeof(DXW.DayLong),
    //typeof(DXW.DayShort),
    //typeof(DXW.DeletedFieldCode),
    //typeof(DXW.DeletedText),
    //typeof(DXW.Drawing),
    //typeof(DXW.EndnoteReferenceMark),
    //typeof(DXW.EndnoteReference),
    //typeof(DXW.FieldChar),
    //typeof(DXW.FootnoteReferenceMark),
    //typeof(DXW.FootnoteReference),
    //typeof(DXW.FieldCode),
    //typeof(DXW.LastRenderedPageBreak),
    //typeof(DXW.MonthLong),
    //typeof(DXW.MonthShort),
    //typeof(DXW.NoBreakHyphen),
    //typeof(DXW.EmbeddedObject),
    //typeof(DXW.PageNumber),
    //typeof(DXW.PositionalTab),
    //typeof(DXW.Ruby),
    //typeof(DXW.SeparatorMark),
    //typeof(DXW.SoftHyphen),
    //typeof(DXW.SymbolChar),
    //typeof(DXW.Text),
    //typeof(DXW.TabChar),
    //typeof(DXW.YearLong),
    //typeof(DXW.YearShort),
    else if (element is Drawing drawing)
      return drawing.GetTextOf(options);
    else if (element is DXD.Blip blip)
      return blip.GetTextOf(options);

    return null;
  }

  /// <summary>
  /// Get the text content of the run.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.Run run, TextOptions options)
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
    var members = run.GetMembers();
    foreach (var member in members)
    {
      if (member is DXW.Text text)
        sb.Append(text.GetTextOf(options));
      else
        sb.Append(member.GetText(options));
    }
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
  private static string GetTextOf(this DXW.Text text, TextOptions options)
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
  private static string GetTextOf(this DX.OpenXmlLeafTextElement textElement, TextOptions options)
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
  private static string GetTextOf(this DXW.Break @break, TextOptions options)
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
  private static string GetTextOf(this DXW.SymbolChar symbolChar, TextOptions options)
  {
    if (int.TryParse(symbolChar.Char!.Value, out var symbolVal))
    {
      return new String((char)symbolVal, 1);
    }
    return String.Empty;
  }

  /// <summary>
  /// Get the text of the hyperlink run elements.
  /// </summary>
  /// <param name="hyperlink">source hyperlink</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  public static string GetTextOf(this DXW.Hyperlink hyperlink, TextOptions? options)
  {
    if (options == null)
      options = TextOptions.PlainText;
    var sb = new StringBuilder();
    foreach (var item in hyperlink.Elements())
    {
      var text = item.GetText(options);
      sb.Append(text);
    }
    var result = sb.ToString();
    return result;
  }
  /// <summary>
  /// Get the text of the FieldChar element.
  /// </summary>
  /// <param name="fieldChar"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.FieldChar fieldChar, TextOptions options)
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
  private static string GetTextOf(this DXW.FieldCode fieldCode, TextOptions options)
  {
    return fieldCode.Text;
  }

  /// <summary>
  /// Get the text of the FootnoteReference element.
  /// </summary>
  /// <param name="footnoteReference"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.FootnoteReference footnoteReference, TextOptions options)
  {
    return options.FootnoteRefStart + footnoteReference.Id + options.FootnoteRefEnd;
  }

  /// <summary>
  /// Get the text of the EndnoteReference element.
  /// </summary>
  /// <param name="endnoteReference"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.EndnoteReference endnoteReference, TextOptions options)
  {
    return options.EndnoteRefStart + endnoteReference.Id + options.EndnoteRefEnd;
  }

  /// <summary>
  /// Get the text of the CommentReference element.
  /// </summary>
  /// <param name="commentReference"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.CommentReference commentReference, TextOptions options)
  {
    return options.CommentRefStart + commentReference.Id + options.CommentRefEnd;
  }

  /// <summary>
  /// Get the text of the paragraph using ParaText options.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <returns>joined text</returns>
  public static string GetText(this Paragraph paragraph)
    => paragraph.GetTextOf(TextOptions.ParaText);

  /// <summary>
  /// Get the text of the paragraph.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  private static string GetTextOf(this Paragraph paragraph, TextOptions options)
  {

    var paraText = paragraph.GetInnerText(options);
    if (!options.OuterText)
      return paraText;
    var sb = new StringBuilder();
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
  private static string GetInnerText(this Paragraph paragraph, TextOptions options)
  {
    var sl = new List<string>();
    var members = paragraph.GetMembers().ToList();
    foreach (var member in members)
    {
      var text = member.GetText(options);
      sl.Add(text);
    }
    var result = string.Join("", sl);
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
  private static string GetTextOf(this Table table, TextOptions options)
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
      sb.Append(row.GetTextOf(options));
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
  private static string GetTextOf(this TableRow row, TextOptions options)
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

  private static string GetTextOf(this TableCell cell, TextOptions options)
  {
    var text = cell.GetInnerText(options);
    return text;
  }

/// <summary>
/// Gets inner text the table cell.
/// </summary>
/// <param name="cell"></param>
/// <param name="options"></param>
/// <returns></returns>
private static string GetInnerText(this TableCell cell, TextOptions options)
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
  private static string GetTextOf(this DXW.Drawing drawing, TextOptions options)
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
  private static string GetTextOf(this DXD.Blip blip, TextOptions options)
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