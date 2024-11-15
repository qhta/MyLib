using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with OpenXml composite elements.
/// </summary>
public static class OpenXmlCompositeElementTools
{

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
    for (int i=0;i<runList.Count; i++)
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
      else
      if (runProps != null && nextRunProps != null)
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
  /// <param name="element">Processed element</param>
  /// <returns>number of tables fixed</returns>
  public static int FixTablesWithInvalidColumns(this DX.OpenXmlCompositeElement element)
  {
    var count = 0;
    var tables = element.Descendants<DXW.Table>().ToList();
    for (int i = 0; i < tables.Count; i++)
    {
      var table = tables[i];
      if (table.Elements<DXW.TableRow>().Count() <= 3)
        continue;
      var firstRow = table.GetFirstChild<DXW.TableRow>();
      var firstCell = firstRow?.GetFirstChild<DXW.TableCell>();
      var borders = firstCell?.TableCellProperties?.GetFirstChild<DXW.TableCellBorders>();
      if (borders == null)
        continue;
      if (borders.LeftBorder?.Val?.Value == BorderValues.Nil)
        continue;
      var nextRow = firstRow?.NextSibling() as DXW.TableRow;
      firstCell = nextRow?.GetFirstChild<DXW.TableCell>();
      var firstParagraph = firstCell?.GetFirstChild<DXW.Paragraph>();
      if (firstParagraph != null)
        Debug.WriteLine($"{i + 1}: {firstParagraph.GetText()}");
      if (table.TryFixInvalidColumns())
        count++;
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
      var nextTable = table.NextSibling() as DXW.Table;
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
        var newTableRows = table.Elements<DXW.TableRow>().ToList();
      }
      nextTable.Remove();
      i--;
      count++;
    }
    return count;
  }
  
}
