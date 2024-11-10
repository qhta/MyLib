using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

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
      if (paragraph.Trim())
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
      var runProps = run.RunProperties;
      var nextRunProps = nextRun.RunProperties;
      if (runProps == null && nextRunProps != null)
      {
        foreach (var item in nextRun.MemberElements().ToList())
        {
          item.Remove();
          run.AppendChild(item);
        }
        nextRun.Remove();
        count++;
        i++;
      }
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
          count++;
          i++;
        }
      }
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
