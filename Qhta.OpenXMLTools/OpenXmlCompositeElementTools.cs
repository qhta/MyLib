using System;

using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with OpenXml composite elements.
/// </summary>
public static class OpenXmlCompositeElementTools
{

  /// <summary>
  /// Gets the text of the composite element.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DX.OpenXmlCompositeElement element, TextOptions options)
  {
    var sl = new List<String>();
    var members = element.GetMembers().ToList();
    for (int i = 0; i < members.Count; i++)
    {
      var member = members[i];
      if (member is DXW.Paragraph paragraph)
      {
        var paraText = paragraph.GetText(options);
        if (options.UseIndenting)
        {
          if (i > 0)
          {
            sl.Add(options.NewLine);
            sl.Add(options.GetIndent());
          }
        }
        if (options.UseHtmlParagraphs)
        {
          if (paraText == string.Empty)
            sl.Add(options.ParagraphSeparator);
          else
          {
            sl.Add(options.ParagraphStartTag);
            sl.Add(paraText);
            sl.Add(options.ParagraphEndTag);
          }
        }
        else
        {
          if (paraText == string.Empty)
          {
            if (!options.IgnoreEmptyParagraphs)
              sl.Add(options.ParagraphSeparator);
          }
          else
          {
            sl.Add(paraText);
            sl.Add(options.ParagraphSeparator);
          }
        }
      }
      else if (member is DXW.Table table)
      {
        if (options.IgnoreTableContents)
        {
          if (options.UseHtmlTables)
          {
            if (options.UseIndenting)
            {
              sl.Add(options.NewLine);
              sl.Add(options.GetIndent());
            }
            sl.Add(options.TableSubstituteTag);
          }
        }
        else
        {
          if (options.UseIndenting)
          {
            sl.Add(options.NewLine);
            sl.Add(options.GetIndent());
          }
          if (options.UseHtmlTables)
          {
            sl.Add(options.TableStartTag);
            if (options.UseIndenting)
            {
              sl.Add(options.NewLine);
            }
            sl.Add(table.GetText(options));
            if (options.UseIndenting)
            {
              sl.Add(options.NewLine);
            }
            if (options.UseHtmlTables)
              sl.Add(options.TableEndTag);
            else
              sl.Add(options.TableSeparator);
          }
          else
          {
            var tableText = table.GetText(options);
            sl.Add(tableText);
            if (i == members.Count - 1)
              sl.Add(Environment.NewLine);
          }
        }
      }
      else if (member is DXW.Run run)
      {
        var runText = run.GetText();
        if (options.UseIndenting && options.IndentLevel > 0)
          runText = options.GetIndent() + runText;
        sl.Add(runText);
      }
      else if (member is DXW.Text text)
      {
        if (options.UseHtmlEntities)
          sl.Add(text.Text.HtmlEncode());
        else
          sl.Add(text.Text);
      }
      else if (member is DXW.Break @break)
      {
        if (@break.Type?.Value == BreakValues.Page)
          sl.Add(options.BreakPageTag);
        else if (@break.Type?.Value == BreakValues.Column)
          sl.Add(options.BreakColumnTag);
        else if (@break.Type?.Value == BreakValues.TextWrapping)
          sl.Add(options.BreakLineTag);
      }
      else if (member is TabChar)
      {
        sl.Add(options.TabTag);
      }
      else if (member is CarriageReturn)
      {
        sl.Add(options.CarriageReturnTag);
      }
      else if (member is FieldCode fieldCode && options.IncludeFieldFormula)
      {
        sl.Add(fieldCode.Text);
      }
      else if (member is SymbolChar symbolChar)
      {
        if (int.TryParse(symbolChar.Char!.Value, out var symbolVal))
        {
          sl.Add(new String((char)symbolVal, 1));
        }
      }
      else if (member is PositionalTab)
      {
        sl.Add(options.TabTag);
      }
      else if (member is FieldChar fieldChar)
      {
        if (fieldChar.FieldCharType?.Value == FieldCharValues.Begin && options.IncludeFieldFormula)
        {
          sl.Add(options.FieldStartTag);
        }
        else if (fieldChar.FieldCharType?.Value == FieldCharValues.Separate && options.IncludeFieldFormula)
        {
          sl.Add(options.FieldResultTag);
        }
        else if (fieldChar.FieldCharType?.Value == FieldCharValues.End && options.IncludeFieldFormula)
        {
          sl.Add(options.FieldEndTag);
        }
      }
      else if (member is Ruby ruby)
      {
        sl.Add(ruby.GetPlainText());
      }
      else if (member is FootnoteReference footnoteReference)
      {
        sl.Add(options.FootnoteRefStart + footnoteReference.Id + options.FootnoteRefEnd);
      }
      else if (member is EndnoteReference endnoteReference)
      {
        sl.Add(options.EndnoteRefStart + endnoteReference.Id + options.EndnoteRefEnd);
      }
      else if (member is CommentReference commentReference)
      {
        sl.Add(options.CommentRefStart + commentReference.Id + options.CommentRefEnd);
      }
      else if (member is DXW.Drawing drawing)
      {
        if (options.IncludeDrawings)
          sl.Add(drawing.GetText(options));
      }
      else
      {
        if (options.IncludeOtherMembers)
          sl.Add(member.OuterXml.IndentString(options.IndentLevel, options.IndentUnit, options.NewLine));
      }
    }
    return string.Join("", sl);
  }

  /// <summary>
  /// Removes all the empty paragraphs from the document.
  /// </summary>
  /// <param name="body"></param>
  /// <param name="descendants">should remove in descendants or at elements</param>
  /// <returns>count of removed paragraphs</returns>
  public static int RemoveEmptyParagraphs(this DX.OpenXmlCompositeElement body, bool descendants)
  {
    var removed = 0;
    var emptyParagraphs = descendants
      ? body.Descendants<DXW.Paragraph>().Where(p => p.IsEmpty()).ToList()
      : body.Elements<DXW.Paragraph>().Where(p => p.IsEmpty()).ToList();
    foreach (var paragraph in emptyParagraphs)
    {
      if (paragraph.Ancestors<DXW.Table>().Any())
      {
        if (paragraph.Parent is DXW.TableCell tableCell)
        {
          if (tableCell.Elements<DXW.Paragraph>().Count() > 1)
          {
            if (paragraph.PreviousSibling() is DXW.Table && paragraph.NextSibling() == null)
            {
              Debug.Assert(true);
              // Do not remove the last paragraph in the table cell when the table is the previous sibling.
            }
            else
            {
              paragraph.Remove();
              removed++;
            }
          }
          else
          {
            Debug.Assert(true);
            // Do not remove the single paragraph in the table cell.
          }
        }
        else
        {
          Debug.Assert(true);
          // Do not remove the paragraph when it is not in a table cell.
        }
      }
      else
      {
        paragraph.Remove();
        removed++;
      }
    }
    return removed;
  }

  /// <summary>
  /// Removes all <c>ProofError</c> and <c>ProofState</c> members from the element.
  /// </summary>
  /// <param name="element">Processed element</param>
  /// <returns>count of trimmed paragraphs</returns>
  public static int RemoveProofErrors(this DX.OpenXmlCompositeElement element)
  {
    var proofErrors = element.Descendants<DXW.ProofError>().ToList();
    var count = proofErrors.Count;
    foreach (var proofError in proofErrors)
      proofError.Remove();
    var proofStates = element.Descendants<DXW.ProofState>().ToList();
    count += proofStates.Count;
    foreach (var proofState in proofStates)
      proofState.Remove();
    return count;
  }

  /// <summary>
  /// Trimming all paragraphs endings.
  /// </summary>
  /// <param name="element">Processed element</param>
  /// <returns>count of trimmed paragraphs</returns>
  public static int TrimParagraphs(this DX.OpenXmlCompositeElement element)
  {
    var paragraphs = element.Descendants<DXW.Paragraph>().ToList();
    var count = 0;
    foreach (var paragraph in paragraphs)
      if (paragraph.TrimEnd())
        count++;
    return count;
  }

  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="element">Processed element</param>
  /// <returns>number of replaced symbols</returns>
  public static int ReplaceSymbolEncoding(this DX.OpenXmlCompositeElement element)
  {
    int count = 0;
    foreach (var run in element.Descendants<DXW.Run>())
    {
      count += run.ReplaceSymbolEncoding();
    }
    return count;
  }

  /// <summary>
  /// Insert an optional hyphens into long words, especially URL's.
  /// </summary>
  /// <param name="element">Processed element</param>
  /// <returns>number of fixed runs in the element</returns>
  public static int FixLongWords(this DX.OpenXmlCompositeElement element)
  {
    int count = 0;
    foreach (var run in element.Descendants<DXW.Run>())
    {
      if (run.TryFixLongWords())
        count++;
    }
    return count;
  }

  /// <summary>
  /// Join all adjacent runs that have the same run properties.
  /// </summary>
  /// <param name="element">Processed element</param>
  /// <returns>number of joins</returns>
  public static int JoinAdjacentRuns(this DX.OpenXmlCompositeElement element)
  {
    int count = 0;
    var runList = element.Descendants<DXW.Run>().ToList();
    for (int i = 0; i < runList.Count; i++)
    {
      var run = runList[i];
      var nextRun = run.NextSibling() as DXW.Run;
      if (nextRun == null)
        continue;
      var runText = run.GetText();
      var nextRunText = nextRun.GetText();
      //if (runText.StartsWith("/word/comments"))
      //  Debug.Assert(true);
      var runProps = run.RunProperties;
      var nextRunProps = nextRun.RunProperties;
      if (runProps == null && nextRunProps == null)
      {
        foreach (var item in nextRun.GetMembers().ToList())
        {
          item.Remove();
          run.AppendChild(item);
        }
        nextRun.Remove();
        runList.Remove(nextRun);
        i--;
        count++;
      }
      else if (runProps != null && nextRunProps != null)
      {
        if (runProps.IsEqual(nextRunProps))
        {
          foreach (var item in nextRun.GetMembers().ToList())
          {
            item.Remove();
            run.AppendChild(item);
          }
          nextRun.Remove();
          runList.Remove(nextRun);
          i--;
          count++;
        }
      }
    }
    return count;
  }

  /// <summary>
  /// Removes all members from the element.
  /// </summary>
  /// <param name="element"/>/param>
  public static void RemoveContent(this DX.OpenXmlCompositeElement element)
  {
    var members = element.GetMembers().ToList();
    foreach (var item in members)
    {
      item.Remove();
    }
  }

  /// <summary>
  /// Find tables that have invalid columns and fix them.
  /// Such tables have rows filled with empty cells.
  /// </summary>
  /// <param name="body">Processed body</param>
  /// <returns>number of cells fixed</returns>
  public static int JoinParagraphsInFirstColumn(this DX.OpenXmlCompositeElement body)
  {
    var count = 0;
    var tables = body.Descendants<DXW.Table>().ToList();
    foreach (var table in tables)
    {
      //if (table.Elements<DXW.TableRow>().RowsCount() <= 3)
      //  continue;
      var firstRow = table.GetFirstChild<DXW.TableRow>();
      var firstCell = firstRow?.GetFirstChild<DXW.TableCell>();
      var borders = firstCell?.TableCellProperties?.GetFirstChild<DXW.TableCellBorders>();
      if (borders == null)
        continue;
      if (borders.LeftBorder?.Val?.Value == BorderValues.Nil)
        continue;
      var shading = firstCell?.TableCellProperties?.GetFirstChild<DXW.Shading>();
      if (shading == null || shading.Fill != "C0C0C0")
        continue;
      //var nextRow = firstRow?.NextSibling() as DXW.TableRow;
      //firstCell = nextRow?.GetFirstChild<DXW.TableCell>();
      //var firstParagraph = firstCell?.GetFirstChild<DXW.Paragraph>();
      //if (firstParagraph != null)
      //  Debug.WriteLine($"{i + 1}: {firstParagraph.GetText()}");
      count += table.TryJoinFirstColumnParagraphs();
    }
    return count;
  }

  /// <summary>
  /// Browse paragraphs and break them before the specified string.
  /// </summary>
  /// <param name="body">Processed body</param>
  /// <param name="str">string to break paragraphs before</param>
  /// <returns>Number of broken paragraphs</returns>
  public static int BreakParagraphsBefore(this DX.OpenXmlCompositeElement body, string str)
  {
    var count = 0;
    var paragraphs = body.Descendants<DXW.Paragraph>().ToList();
    foreach (var paragraph in paragraphs)
    {
      var paraText = paragraph.GetText();
      if (paraText.Contains(str))
      {
        if (paragraph.BreakBefore(str))
          count++;
      }
    }
    return count;
  }

}
