using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A composite tool for cleaning a Wordprocessing document.
/// </summary>
public partial class DocumentCleaner
{
  /// <summary>
  /// Find multi-column rows in tables and make them internal tables.
  /// Pseudo-tables are tables that are created by using tabs and spaces.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void FixInternalTables(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nFixing internal tables");
    var body = wordDoc.GetBody();
    var count = 0;
    var tables = body.Descendants<DXW.Table>().ToList();
    foreach (var table in tables)
    {
      if (TryFixInternalTable(table))
        count++;
    }
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} internal tables created");
  }

  /// <summary>
  /// Find multi-column rows in tables and make them internal tables.
  /// Pseudo-tables are tables that are created by using tabs and spaces.
  /// </summary>
  /// <param name="table"></param>
  public bool TryFixInternalTable(DXW.Table table)
  {
    DXW.TableGrid tableGrid = table.GetTableGrid();
    var gridColumns = tableGrid.Elements<DXW.GridColumn>().ToList();
    if (gridColumns.Count < 2)
      return false;
    var tableRows = table.Elements<DXW.TableRow>().ToList();
    if (tableRows.Count < 2)
      return false;
    var firstRow = tableRows[0];
    var firstRowCells = firstRow.Elements<DXW.TableCell>().ToList();
    if (firstRowCells.Count == gridColumns.Count)
      return false;
    var multiColumnRows = new List<DXW.TableRow>();
    var firstMultiColumnRow = -1;
    var lastMultiColumnRow = -1;
    for (int i = 1; i < tableRows.Count; i++)
    {
      var row = tableRows[i];
      var cells = row.Elements<DXW.TableCell>().ToList();
      if (cells.Count == gridColumns.Count)
      {
        if (firstMultiColumnRow < 0)
          firstMultiColumnRow = i;
        multiColumnRows.Add(row);
      }
      else
      {
        if (lastMultiColumnRow < 0)
          lastMultiColumnRow = i;
        else
        {
          break;
        }
      }
    }
    if (multiColumnRows.Count == 0)
      return false;
    if (firstMultiColumnRow < 0)
      return false;

    var firstNonEmptyColumn = -1;
    var lastNonEmptyColumn = -1;
    foreach (var row in multiColumnRows)
    {
      var cells = row.Elements<DXW.TableCell>().ToList();
      for (int i = 0; i < cells.Count; i++)
      {
        var cellText = cells[i].GetText();
        if (!String.IsNullOrWhiteSpace(cellText))
        {
          if (lastNonEmptyColumn < 0)
            firstNonEmptyColumn = i;
          lastNonEmptyColumn = i;
        }
      }
    }
    if (firstNonEmptyColumn < 0)
      return false;

    var internalTable = new DXW.Table();
    var internalTableGrid = internalTable.GetTableGrid();
    for (int i = firstNonEmptyColumn; i <= lastNonEmptyColumn; i++)
    {
      internalTableGrid.AppendChild(gridColumns[i].CloneNode((true)));
    }

    foreach (var row in multiColumnRows)
    {
      row.Remove();
      tableRows.Remove(row);
      for (int i = lastNonEmptyColumn + 1; i < row.Elements<DXW.TableCell>().Count(); i++)
      {
        row.Elements<DXW.TableCell>().ElementAt(i).Remove();
      }
      for (int i = 0; i < firstNonEmptyColumn; i++)
      {
        row.Elements<DXW.TableCell>().ElementAt(0).Remove();
      }
      internalTable.AppendChild(row);
    }

    var tableGridList = tableGrid.Elements<DXW.GridColumn>().ToList();
    var mergeColumnNdx = (firstNonEmptyColumn < firstRowCells.Count) ? firstNonEmptyColumn : firstRowCells.Count - 1;
    var mergeColumn = tableGridList[mergeColumnNdx];
    if (firstNonEmptyColumn > 0)
    {
      var indent = tableGridList[firstNonEmptyColumn - 1].GetWidth();
      if (indent != null)
      {
        internalTable.GetTableProperties().TableIndentation = new DXW.TableIndentation { Width = indent, Type = DXW.TableWidthUnitValues.Dxa };
      }
    }
    for (int i = tableGridList.Count - 1; i >= firstNonEmptyColumn; i--)
    {
      mergeColumn.SetWidth(mergeColumn.GetWidth() + tableGridList[i].GetWidth());
      tableGridList[i].Remove();
      tableGridList.Remove(tableGridList[i]);
    }

    var mergeRowNdx = firstMultiColumnRow - 1;
    var mergeRow = tableRows[mergeRowNdx];
    var mergeRowCells = mergeRow.Elements<DXW.TableCell>().ToList();
    var mergeCell = mergeRowCells[mergeColumnNdx];
    mergeCell.AppendChild(internalTable);
    var afterTableParaAdded = false;
    var nextRow = mergeRow.NextSibling<DXW.TableRow>();
    if (nextRow != null)
    {
      var nextRowCells = nextRow.Elements<DXW.TableCell>().ToList();
      nextRow.Remove();
      tableRows.Remove(nextRow);
      var nextRowText = nextRow.GetText();
      int j = 0;
      for (int i = 0; i < nextRowCells.Count; i++)
      {
        var nextRowCell = nextRowCells[i];
        var mergeRowCell = mergeRowCells[j];
        var vMerge = mergeRowCell.TableCellProperties?.VerticalMerge;
        if (vMerge != null)
          vMerge.Remove();
        var nextCellText = nextRowCell.GetText();
        var nextCellContent = nextRowCell.MemberElements().ToList();
        if (!(nextCellContent.Count == 1 && nextCellContent[0] is DXW.Paragraph paragraph && paragraph.IsEmpty()))
        {
          for (int k = 0; k < nextCellContent.Count; k++)
          {
            var nextCellContentItem = nextCellContent[k];
            nextCellContentItem.Remove();
            nextCellContent.Remove(nextCellContentItem);
            mergeRowCell.AppendChild(nextCellContentItem);
            if (nextCellContentItem is DXW.Paragraph && j == mergeColumnNdx)
              afterTableParaAdded = true;
          }
        }
        if (j < mergeRowCells.Count - 1)
          j++;
      }
    }
    if (!afterTableParaAdded)
      mergeCell.AppendChild(new DXW.Paragraph());
    return true;
  }

  /// <summary>
  /// Joins adjacent tables that have the same number of columns.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void JoinAdjacentTables(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nJoining adjacent tables");
    var body = wordDoc.GetBody();
    var count = body.JoinAdjacentTables();
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} tables appended to previous ones");
  }


  /// <summary>
  /// Fix page-divided tables.
  /// Page-divided table is (usually long) table that have been split across consecutive pages
  /// so that the headings of the table are repeated on each page.
  /// Sometimes there are no repeating headings but the table is divided.
  /// After <see cref="JoinAdjacentTables"/> it should be a single table.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void FixDividedTables(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nFixing divided tables");
    var fixedTables = 0;
    int removedRows = 0;
    int joinedRows = 0;
    foreach (var table in wordDoc.GetBody().Descendants<DXW.Table>())
    {
      if (FixTableWithRepeatedHeaders(table, out var removed, out var joined))
      {
        fixedTables++;
        removedRows += removed;
        joinedRows += joined;
      }
      else if (FixTableWithWithDividedRows(table, out joined))
      {
        fixedTables++;
        joinedRows += joined;
      }
    }
    if (VerboseLevel > 0)
    {
      Console.WriteLine($"  {fixedTables} tables fixed");
      Console.WriteLine($"  {removedRows} repeated headings removed");
      Console.WriteLine($"  {joinedRows} divided rows joined");
    }
  }

  /// <summary>
  /// Check if the table has first (heading) row repeated in the middle of the table.
  /// These repeating heading rows should be removed.
  /// Then the last row of the first part of the table
  /// should be joined with the first row of the second part.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="removedRepeatedHeadings">count of removed repeated headings</param>
  /// <param name="joinedRows">count of joined rows</param>
  /// <returns>true when a table was divided</returns>
  public bool FixTableWithRepeatedHeaders(DXW.Table table, out int removedRepeatedHeadings, out int joinedRows)
  {
    removedRepeatedHeadings = 0;
    joinedRows = 0;
    var done = false;
    var tableRows = table.Elements<DXW.TableRow>().ToList();
    var headingText = tableRows[0].GetText();
    for (int i = 1; i < tableRows.Count; i++)
    {
      var row = tableRows[i];
      var rowText = row.GetText();
      if (rowText != headingText)
        continue;
      // remove repeating heading row
      row.Remove();
      tableRows.RemoveAt(i);
      i--;
      done = true;
      removedRepeatedHeadings++;
      if (i < 1 || i >= tableRows.Count-1)
        continue;

      var priorRow = tableRows[i];
      var nextRow = tableRows[i+1];
      if (TryJoinDividedRows(priorRow, nextRow))
      {
        nextRow.Remove();
        tableRows.RemoveAt(i+1);
        joinedRows++;
        i--;
      }
    }
    return done;
  }

  /// <summary>
  /// If the table does not have repeating headings, it can still be divided.
  /// We must check if two adjacent rows should be joined.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="joinedRows">count of joined rows</param>
  /// <returns>true when a table was divided</returns>
  public bool FixTableWithWithDividedRows(DXW.Table table, out int joinedRows)
  {
    joinedRows = 0;
    var done = false;
    var tableRows = table.Elements<DXW.TableRow>().ToList();
    for (int i = 1; i < tableRows.Count; i++)
    {
      var priorRow = tableRows[i - 1];
      var nextRow = tableRows[i];
      if (TryJoinDividedRows(priorRow, nextRow))
      {
        nextRow.Remove();
        tableRows.RemoveAt(i);
        joinedRows++;
        done = true;
        i--;
      }
    }
    return done;
  }

  /// <summary>
  /// Try to join two rows in page-divided table.
  /// Page-divided table is (usually long) table that have been split across consecutive pages
  /// so that the headings of the table are repeated on each page.
  /// The repeating heading rows has been removed.
  /// Now we try to join the last row of the first part of the table
  /// with the first row of the second part.
  /// </summary>
  /// <param name="upperRow">Upper table row.</param>
  /// <param name="lowerRow">Lower table row.</param>
  private bool TryJoinDividedRows(DXW.TableRow upperRow, DXW.TableRow lowerRow)
  {
    if (!ShouldBeJoined(upperRow, lowerRow))
      return false;
    var upperCells = upperRow.Elements<DXW.TableCell>().ToList();
    var lowerCells = lowerRow.Elements<DXW.TableCell>().ToList();
    if (upperCells.Count != lowerCells.Count)
      return false;
    for (int i = 0; i < upperCells.Count; i++)
    {
      var upperCell = upperCells[i];
      var lowerCell = lowerCells[i];
      if (!JoinDividedCells(upperCell, lowerCell))
        return false;
    }
    return true;
  }

  /// <summary>
  /// Check if two sibling rows in a table should be joined.
  /// First check if the number of cells in the rows are the same.
  /// Then check the corresponding cells.
  /// </summary>
  /// <param name="upperRow">Upper table row.</param>
  /// <param name="lowerRow">Lower table row.</param>
  private bool ShouldBeJoined(DXW.TableRow upperRow, DXW.TableRow lowerRow)
  {
    var upperCells = upperRow.Elements<DXW.TableCell>().ToList();
    var lowerCells = lowerRow.Elements<DXW.TableCell>().ToList();
    if (upperCells.Count != lowerCells.Count)
      return false;
    var check = 0;
    for (int i = 0; i < upperCells.Count; i++)
    {
      var upperCell = upperCells[i];
      var lowerCell = lowerCells[i];
      check += ShouldBeJoined(upperCell, lowerCell);
    }
    return check > 0;
  }

  /// <summary>
  /// Check if two corresponding cells in sibling table rows should be joined.
  /// Check the last paragraph in the upper cell and the first paragraph in the lower cell.
  /// Returns 2 if the cells should definitely be joined.
  /// Returns 1 if the cells should rather be joined.
  /// Returns -1 if the cells should rather not be joined,
  /// Returns -2 if the cells should definitely not be joined.
  /// Returns 0 if the algorithm can't tell. 
  /// </summary>
  /// <param name="upperCell">Upper table row.</param>
  /// <param name="lowerCell">Lower table row.</param>
  private int ShouldBeJoined(DXW.TableCell upperCell, DXW.TableCell lowerCell)
  {
    var upperPara = upperCell.Elements<DXW.Paragraph>().LastOrDefault();
    var lowerPara = lowerCell.Elements<DXW.Paragraph>().FirstOrDefault();
    if (upperPara == null || lowerPara == null)
      return 0;
    // If the lower paragraph is empty then the cells should definitely be joined
    // because it is very probable that the cells were created by division.
    if (lowerPara.IsEmpty())
      return 2;

    // Last paragraph in the upper cell should end with a run element
    // and the first paragraph in the lower cell should start with a run element
    // to be possibly parts of one cell. If they are not then we can't tell.
    if (upperPara.Elements().LastOrDefault() is not DXW.Run && lowerPara.Elements().FirstOrDefault() is not DXW.Run)
      return 0;

    var upperText = upperPara.GetText().Trim();
    var lowerText = lowerPara.GetText().Trim();

    // If the last paragraph in the upper cell ends with a comma
    // then it is very possible that the cells were created by division.
    var upperTextEndsWithSentenceDivMark = upperText.EndsWith(",");
    if (upperTextEndsWithSentenceDivMark)
      return 2;

    // If the last paragraph in the upper cell ends with a dot, exclamation mark, question mark or colon
    // then it is possible that the cells were created by division, but be can't tell it.
    var upperTextEndsWithSentenceEndMark = upperText.EndsWith(".")
                                           || upperText.EndsWith("!")
                                           || upperText.EndsWith("?")
                                           || upperText.EndsWith(":");
    if (upperTextEndsWithSentenceEndMark)
      return 0;

    var upperSentences = GetSentences(upperText);
    var lowerSentences = GetSentences(lowerText);

    // If the last paragraph in the upper cell and the first paragraph in the lower cell
    // both do not contain any sentence than it is rather not possible that the cells were created by division.
    if (upperSentences.Count == 0 && lowerSentences.Count == 0)
      return -1;

    // if the first paragraph in the lower cell
    // starts with a non-letter character then the cells were rather not divided.
    if (!char.IsLetter(lowerText.FirstOrDefault()))
      return -1;
    return 0;
  }

  /// <summary>
  /// Get the list of sentences from the string.
  /// Sentences are separated by '.', '!', '?' or ':'
  /// followed by a space (or standing at the end of the string).
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  private List<string> GetSentences(string str)
  {
    var sentences = new List<string>();
    var k = 0;
    str = str.Trim();
    while (k>=0 && k < str.Length)
    {
      k = str.IndexOfAny(['.', '!', '?', ':'], k);
      if (k == -1)
        break;
      if (k == str.Length - 1)
      {
        var s1 = str.Substring(k + 1).TrimStart();
        sentences.Add(s1);
        break;
      }
      if (k + 1 < str.Length && str[k + 1] == ' ')

      {
        var s1 = str.Substring(0, k + 1);
        sentences.Add(s1);
        str = str.Substring(k + 1).TrimStart();
      }
      else
        k = k + 1;
    }
    return sentences;
  }

  /// <summary>
  /// Join two corresponding cells in possibly divided row.
  /// Last paragraph in the upper cell and the first paragraph in the lower cell should be joined.
  /// If it is not possible to join the cells then return false.
  /// </summary>
  /// <param name="upperCell">Cell taken from the upper table row.</param>
  /// <param name="lowerCell">Cell taken from the lower table row.</param>
  public bool JoinDividedCells(DXW.TableCell upperCell, DXW.TableCell lowerCell)
  {
    var upperPara = upperCell.Elements<DXW.Paragraph>().LastOrDefault();
    var lowerPara = lowerCell.Elements<DXW.Paragraph>().FirstOrDefault();
    if (upperPara == null || lowerPara == null)
      return false;
    if (lowerPara.IsEmpty())
    {
      return true;
    }
    var lastElement = upperPara.MemberElements().LastOrDefault();
    var firstElement = lowerPara.MemberElements().FirstOrDefault();
    if (lastElement == null || firstElement == null)
      return true;
    if (lastElement is DXW.Hyperlink lastHyperlink && firstElement is DXW.Hyperlink firstHyperlink)
    {
      if (lastHyperlink.GetRel().IsEqual(firstHyperlink.GetRel()))
      {
        lastHyperlink.SetText(lastHyperlink.GetText() + firstHyperlink.GetText());
        firstHyperlink.Remove();
      }
      foreach (var item in lowerPara.MemberElements().ToList())
      {
        item.Remove();
        upperPara.AppendChild(item);
      }
      lowerPara.Remove();
    }
    else
    {
      var upperText = upperPara.GetText();
      var lowerText = lowerPara.GetText();
      if (char.IsLower(lowerText.FirstOrDefault()))
      {
        if (!upperText.EndsWith(" ") && !lowerText.StartsWith(" "))
        {
          upperPara.AppendChild(new DXW.Run(new DXW.Text(" ")));
        }
        foreach (var item in lowerPara.MemberElements().ToList())
        {
          item.Remove();
          upperPara.AppendChild(item);
        }
        lowerPara.Remove();
      }
      else
      {
        if (!upperText.EndsWith(" ") && !lowerText.StartsWith(" "))
        {
          upperPara.AppendChild(new DXW.Run(new DXW.Text(" ")));
        }
        upperPara.AppendChild(new DXW.Run(new DXW.Text(" ")));
        foreach (var item in lowerPara.MemberElements().ToList())
        {
          item.Remove();
          upperPara.AppendChild(item);
        }
        lowerPara.Remove();
      }
    }
    var restOfParagraphs = lowerCell.Elements<DXW.Paragraph>().ToList();
    foreach (var paragraph in restOfParagraphs)
    {
      paragraph.Remove();
      upperCell.AppendChild(paragraph);
    }
    return true;
  }

}