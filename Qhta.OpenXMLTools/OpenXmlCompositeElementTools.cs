using System;
using System.Xml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with OpenXml composite elements.
/// </summary>
public static class OpenXmlCompositeElementTools
{

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
        foreach (var item in nextRun.MemberElements().ToList())
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
          foreach (var item in nextRun.MemberElements().ToList())
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

  /// <summary>
  /// Joins adjacent tables that have the same number of columns.
  /// </summary>
  /// <param name="element">Processed element</param>
  /// <returns>number of joins</returns>
  public static int JoinAdjacentTables(this DX.OpenXmlCompositeElement element)
  {
    var count = 0;
    var tables = element.Descendants<DXW.Table>().ToList();
    for (int i = 0; i < tables.Count; i++)
    {
      var table = tables[i];
      var nextElement = table.NextSibling();
      var nextTable = nextElement as DXW.Table;
      if (nextTable == null)
        continue;

      var tableGrid = table.GetTableGrid();
      var nextTableGrid = nextTable.GetTableGrid();
      var tableGridColumns = tableGrid.Elements<DXW.GridColumn>().ToList();
      var nextTableGridColumns = nextTableGrid.Elements<DXW.GridColumn>().ToList();
      if (tableGridColumns.Count != nextTableGridColumns.Count)
        continue;

      var nextTableRows = nextTable.Elements<DXW.TableRow>().ToList();
      foreach (var row in nextTableRows)
      {
        row.Remove();
        table.AppendChild(row);
        //var newTableRows = table.Elements<DXW.TableRow>().ToList();
      }
      nextTable.Remove();
      i--;
      count++;
    }
    return count;
  }

  /// <summary>
  /// Fix paragraphs with bullets.
  /// If the paragraph starts with a bullet, the bullet is removed and the paragraph is bulleted.
  /// If the paragraph contains a bullet inside text, the paragraph is divided and new paragraph is bulleted.
  /// </summary>
  /// <param name="body"></param>
  /// <returns></returns>
  public static int FixParagraphsWithBullets(this DX.OpenXmlCompositeElement body)
  {
    var numbering = body.GetMainDocumentPart()!.GetNumberingDefinitions();
    var abstractNumbering = body.FindMostFrequentBulletedAbstractNumbering();
    if (abstractNumbering == null)
      abstractNumbering = numbering.GetDefaultBulletedAbstractNumbering();

    int abstractNumId = abstractNumbering.AbstractNumberId!;
    var numberingStatistic = body.GetNumberingInstanceStatistics(abstractNumId);
    var numberingInstance = numberingStatistic.MostFrequent();
    DXW.ParagraphProperties? defaultParagraphProperties = null;
    if (numberingInstance != null)
      defaultParagraphProperties = numberingInstance.Parent as DXW.ParagraphProperties;
    if (defaultParagraphProperties == null)
    {
      if (numberingInstance == null)
      {
        numberingInstance = numbering.GetNumberingInstance(abstractNumId);
      }
      defaultParagraphProperties = new DXW.ParagraphProperties();
      defaultParagraphProperties.SetNumbering(numberingInstance.NumberID?.Value);
    }
    int count = 0;
    var paragraphs = body.Descendants<DXW.Paragraph>().ToList();
    for (int i = 0; i < paragraphs.Count; i++)
    {
      var paragraph = paragraphs[i];
      //var paraText = paragraph.GetText();
      //if (paraText.Contains("Video (\u00a715.2.17)"))
      //  Debug.Assert(true);
      foreach (var run in paragraph.Elements<DXW.Run>())
      {
        var text = run.GetText();
        if (text.Contains("•"))
        {
          var textItem = run.Descendants<DXW.Text>().FirstOrDefault(t => t.Text.TrimStart().StartsWith("•"));
          if (textItem == null)
            continue;
          textItem.Text = text.Replace("•", "");
          count++;
          DXW.Paragraph? bulletedParagraph = null;
          if (paragraph.IsBulleted())
            bulletedParagraph = paragraph;
          else
          {
            bulletedParagraph = paragraph.GetPreviousNumberedParagraph();
            if (bulletedParagraph == null)
              bulletedParagraph = paragraph.GetNextNumberedParagraph();
          }
          DXW.ParagraphProperties? numberingParagraphProperties =
            bulletedParagraph?.ParagraphProperties;
          if (numberingParagraphProperties == null)
            numberingParagraphProperties = defaultParagraphProperties;
          numberingParagraphProperties = (DXW.ParagraphProperties)numberingParagraphProperties.CloneNode(true);

          var prevSibling = run.PreviousSibling();
          if (prevSibling != null && prevSibling is not DXW.ParagraphProperties &&
              !String.IsNullOrEmpty((prevSibling as DXW.Run)?.GetText()))
          {
            var newParagraph = new DXW.Paragraph();
            newParagraph.ParagraphProperties = numberingParagraphProperties;
            var tailItems = new List<DX.OpenXmlElement>();
            tailItems.Add(run);
            var siblingItem = run.NextSibling();
            while (siblingItem != null)
            {
              tailItems.Add(siblingItem);
              siblingItem = siblingItem.NextSibling();
            }
            foreach (var item in tailItems)
            {
              item.Remove();
              if (item is DXW.Run runItem && newParagraph.IsEmpty())
                runItem.TrimStart();
              newParagraph.Append(item);
            }
            paragraph.TrimEnd();
            newParagraph.TrimStart();
            newParagraph.TrimEnd();
            if (paragraph.IsEmpty())
            {
              numberingParagraphProperties?.Remove();
              paragraph.ParagraphProperties = numberingParagraphProperties;
              var priorParagraph = paragraph.PreviousSibling<DXW.Paragraph>();
              var after = priorParagraph?.ParagraphProperties?.SpacingBetweenLines?.After;
              if (after != null)
                paragraph.GetParagraphProperties().GetSpacingBetweenLines().After = after;
              foreach (var item in newParagraph.MemberElements())
              {
                item.Remove();
                paragraph.AppendChild(item);
              }
            }
            else
            {
              var priorParagraph = paragraph.PreviousSibling<DXW.Paragraph>();
              if (priorParagraph != null && priorParagraph.ParagraphProperties?.NumberingProperties != null)
                paragraph.ParagraphProperties =
                  (DXW.ParagraphProperties)priorParagraph.ParagraphProperties.CloneNode(true);
              newParagraph.TrimEnd();
              paragraph.InsertAfterSelf(newParagraph);
              paragraphs.Insert(i + 1, newParagraph);
              //if (paragraph.IsEmpty())
              //{
              //  paragraph.Remove();
              //  paragraphs.RemoveAt(i);
              //  i--;
              //}
            }
          }
          else // if it is the first run in the paragraph then do not create a new paragraph.
          {
            paragraph.ParagraphProperties = numberingParagraphProperties;
            paragraph.TrimStart();
            paragraph.TrimEnd();
            i--;
          }
        }
      }
    }
    return count;
  }


  /// <summary>
  /// Fix paragraphs with numbering.
  /// Numbering is a digit or a sequence of digits, a letter or two letters followed by a dot or closing parenthesis.
  /// If the paragraph starts with a numbering, the numbering is removed and the paragraph is numbered.
  /// </summary>
  /// <param name="body"></param>
  /// <returns></returns>
  public static int FixParagraphsWithNumbers(this DX.OpenXmlCompositeElement body)
  {
    var numbering = body.GetMainDocumentPart()!.GetNumberingDefinitions();
    //var abstractNumbering = body.FindMostFrequentBulletedAbstractNumbering();
    //if (abstractNumbering == null)
    //  abstractNumbering = numbering.GetDefaultBulletedAbstractNumbering();

    //int abstractNumId = abstractNumbering.AbstractNumberId!;
    //var numberingStatistic = body.GetNumberingInstanceStatistics(abstractNumId);
    //var numberingInstance = numberingStatistic.MostFrequent();
    //DXW.ParagraphProperties? defaultParagraphProperties = null;
    //if (numberingInstance != null)
    //  defaultParagraphProperties = numberingInstance.Parent as DXW.ParagraphProperties;
    //if (defaultParagraphProperties == null)
    //{
    //  if (numberingInstance == null)
    //  {
    //    numberingInstance = numbering.GetNumberingInstance(abstractNumId);
    //  }
    //  defaultParagraphProperties = new DXW.ParagraphProperties();
    //  defaultParagraphProperties.SetNumbering(numberingInstance.NumberID?.Value);
    //}
    int count = 0;
    var paragraphs = body.Descendants<DXW.Paragraph>().ToList();
    for (int i = 0; i < paragraphs.Count; i++)
    {
      var paragraph = paragraphs[i];
      var paraText = paragraph.GetText();
      var numberingString = paraText.GetNumberingString();
      if (numberingString == null)
        continue;
      //if (paraText.Contains("Video (\u00a715.2.17)"))
      //  Debug.Assert(true);
      var run = paragraph.Elements<DXW.Run>().FirstOrDefault();
      if (run == null)
        continue;
      {
        var text = run.GetText();
        if (text.StartsWith(numberingString))
        {
          DXW.AbstractNum? abstractNumbering = null;
          DXW.Level? numLevel = null;
          foreach (var abstractNum in numbering.Elements<AbstractNum>().ToList())
          {
            foreach (var level in abstractNum.Elements<Level>().ToList())
            {
              if (level.IsCompatibleWith(numberingString))
              {
                abstractNumbering = abstractNum;
                numLevel = level;
                break;
              }
            }
          }
          if (abstractNumbering != null && numLevel!=null)
          {
            var tailText = text.Substring(numberingString.Length).TrimStart();
            if (tailText.Length == 0)
              run.Remove();
            else
              run.SetText(tailText);
            paragraph.SetNumbering(abstractNumbering, numLevel);
            paragraph.TrimStart();
            count++;
          }
        }
      }
    }
    return count;
  }

}
