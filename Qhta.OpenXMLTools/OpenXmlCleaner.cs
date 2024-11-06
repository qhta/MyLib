using System;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for cleaning OpenXml document.
/// </summary>
public class OpenXmlCleaner
{
  /// <summary>
  /// Determines the level of verbosity of the cleaner.
  /// </summary>
  public int VerboseLevel { get; set; }

  /// <summary>
  /// Cleans the document using all the methods available.
  /// </summary>
  /// <param name="fileName"></param>
  public void CleanDocument(string fileName)
  {
    using var wordDoc = DXPack.WordprocessingDocument.Open(fileName, true);
    TrimParagraphs(wordDoc);
    RemoveEmptyParagraphs(wordDoc);
    RemoveFakeHeadersAndFooters(wordDoc);
    ResetHeadingsFormat(wordDoc);
    ReplaceSymbolEncoding(wordDoc);
    RepairBulletContainedParagraph(wordDoc);
    FixInternalTables(wordDoc);
    JoinSiblingTables(wordDoc);
    FixDividedTables(wordDoc);
    FixLongWords(wordDoc);
  }

  /// <summary>
  /// Trims all the paragraphs in the document removing starting and ending whitespaces.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void TrimParagraphs(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nTrimming paragraphs");
    var body = wordDoc.GetBody();
    var count = TrimParagraphs(body);
    var headers = wordDoc.GetHeaders().ToList();
    foreach (var header in headers)
    {
      count += TrimParagraphs(header);
    }
    var footers = wordDoc.GetFooters().ToList();
    foreach (var footer in wordDoc.GetFooters())
    {
      count += TrimParagraphs(footer);
    }
    if (VerboseLevel > 0)
      Console.WriteLine($" {count} paragraphs trimmed.");
  }

  /// <summary>
  /// Removes all the empty paragraphs from the document.
  /// </summary>
  /// <param name="body"></param>
  /// <returns>count of trimmed paragraphs</returns>
  public int TrimParagraphs(DX.OpenXmlCompositeElement body)
  {
    var paragraphs = body.Descendants<DXW.Paragraph>().ToList();
    var count = 0;
    foreach (var paragraph in paragraphs)
      if (TryTrimParagraph(paragraph))
        count++;
    return count;
  }

  /// <summary>
  /// Trims the paragraph removing ending whitespaces.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns>true if the trimming was successful, false if it was not needed.</returns>
  public bool TryTrimParagraph(DXW.Paragraph paragraph)
  {
    //var paraText = paragraph.GetText();
    //if (paraText.StartsWith("1. Scope"))
    //  Debug.Assert(true);
    bool done = false;
    var lastElement = paragraph.MemberElements().LastOrDefault();
    while (lastElement != null)
    {
      var previousElement = lastElement.PreviousSibling();
      if (lastElement is DXW.BookmarkEnd)
      {
        // ignore;
      }
      else
      if (lastElement is DXW.Run run)
      {
        if (run.TryTrim())
        {
          done = true;
          if (run.IsEmpty())
            run.Remove();
          else
            break;
        }
        else
          break;
      }
      else if (lastElement is DXW.Hyperlink hyperlink)
      {
        if (!hyperlink.TryTrim())
          break;
      }
      lastElement = previousElement;
    }
    return done;
  }

  /// <summary>
  /// Removes all the empty paragraphs from the document.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void RemoveEmptyParagraphs(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nRemoving empty paragraphs");
    var body = wordDoc.GetBody();
    var count = RemoveEmptyParagraphs(body);
    var headers = wordDoc.GetHeaders().ToList();
    foreach (var header in headers)
    {
      count += RemoveEmptyParagraphs(header);
    }
    var footers = wordDoc.GetFooters().ToList();
    foreach (var footer in wordDoc.GetFooters())
    {
      count += RemoveEmptyParagraphs(footer);
    }
    if (VerboseLevel > 0)
      Console.WriteLine($" {count} paragraphs removed.");
  }

  /// <summary>
  /// Removes all the empty paragraphs from the document.
  /// </summary>
  /// <param name="body"></param>
  /// <returns>count of removed paragraphs</returns>
  public int RemoveEmptyParagraphs(DX.OpenXmlCompositeElement body)
  {
    var removed = 0;
    var emptyParagraphs = body.Descendants<DXW.Paragraph>().Where(p => p.IsEmpty()).ToList();
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
  /// Insert an optional hyphens into long words, especially URL's.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void FixLongWords(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nFixing long words");
    var body = wordDoc.GetBody();
    var count = FixLongWords(body);
    var headers = wordDoc.GetHeaders().ToList();
    foreach (var header in headers)
    {
      count += FixLongWords(header);
    }
    var footers = wordDoc.GetFooters().ToList();
    foreach (var footer in wordDoc.GetFooters())
    {
      count += FixLongWords(footer);
    }
    if (VerboseLevel > 0)
      Console.WriteLine($" {count} words fixed.");
  }

  /// <summary>
  /// Insert an optional hyphens into long words, especially URL's.
  /// </summary>
  /// <param name="body"></param>
  /// <returns>count of fixed paragraphs</returns>
  public int FixLongWords(DX.OpenXmlCompositeElement body)
  {
    var runs = body.Descendants<DXW.Run>().ToList();
    var count = 0;
    foreach (var run in runs)
      if (TryFixLongWords(run))
        count++;
    return count;
  }

  /// <summary>
  /// Insert an optional hyphens into long words, especially URL's.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns>true if the fixing was successful, false if it was not needed.</returns>
  public bool TryFixLongWords(DXW.Paragraph paragraph)
  {
    var done = false;
    foreach (var element in paragraph.MemberElements())
    {
      if (element is DXW.Run run)
      {
        if (TryFixLongWords(run))
          done = true;
      }
      else if (element is DXW.Hyperlink hyperlink)
      {
        if (TryFixLongWords(hyperlink))
          done = true;
      }
    }
    return done;
  }


  /// <summary>
  /// Insert an optional hyphens into long words, especially URL's.
  /// </summary>
  /// <param name="hyperlink"></param>
  /// <returns>true if the fixing was successful, false if it was not needed.</returns>
  public bool TryFixLongWords(DXW.Hyperlink hyperlink)
  {
    var done = false;
    var hyperlinkElements = hyperlink.MemberElements();
    foreach (var element in hyperlinkElements)
    {
      if (element is DXW.Run run)
      {
        if (TryFixLongWords(run))
          done = true;
      }
    }
    return done;
  }

  /// <summary>
  /// Insert an optional hyphens into long words, especially URL's.
  /// </summary>
  /// <param name="run"></param>
  /// <returns>true if the fixing was successful, false if it was not needed.</returns>
  public bool TryFixLongWords(DXW.Run run)
  {
    var done = false;
    var runElements = run.MemberElements();
    foreach (var element in runElements)
    {
      if (element is DXW.Text text)
      {
        var textValue = text.Text;
        var newTextValue = FixLongWordsInText(textValue);
        if (newTextValue != textValue)
        {
          var textItems = newTextValue.Split('\u00AD');
          text.Text = textItems[0];
          var prevText = text;
          for (int i = 1; i < textItems.Length; i++)
          {
            var textItem = textItems[i];
            var t1 = text.InsertAfterSelf(new DXW.SoftHyphen());
            text = t1.InsertAfterSelf(new DXW.Text(textItem));
          }
          done = true;
        }
      }
    }
    return done;
  }

  private string FixLongWordsInText(string textValue)
  {
    var newTextValue = textValue;
    var words = textValue.Split(' ');
    foreach (var word in words)
    {
      if (word.Length > 30)
      {
        var newWord = FixLongWord(word);
        newTextValue = newTextValue.Replace(word, newWord);
      }
    }
    return newTextValue;
  }

  private string FixLongWord(string word)
  {
    var chars = word.ToList();
    for (int i = 10; i < chars.Count - 10; i++)
    {
      if (chars[i] == '/' && char.IsLetter(chars[i + 1]))
        chars.Insert(i + 1, '\u00AD');
      else if (char.IsUpper(chars[i]) && char.IsLower(chars[i - 1]))
        chars.Insert(i, '\u00AD');
    }
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Removes paragraphs that are same as headers or footers but are contained in body.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void RemoveFakeHeadersAndFooters(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nRemoving fake header/footer paragraphs");
    var removed = 0;
    HashSet<string> headers = new();
    var allHeaders = wordDoc.GetHeaders().Select(h => h.GetText(null)).ToArray();
    foreach (var str in allHeaders)
      if (!string.IsNullOrEmpty(str))
      {
        var s = CleanWhitespace(TryRemoveNumbering(str!));
        if (s.Length > 0 && !IsNumber(s))
          headers.Add(s);
      }
    var allFooters = wordDoc.GetFooters().Select(f => f.GetText(null)).ToArray();
    foreach (var str in allFooters)
      if (!string.IsNullOrEmpty(str))
      {
        var s = CleanWhitespace(TryRemoveNumbering(str!));
        if (s.Length > 0 && !IsNumber(s))
          headers.Add(s);
      }
    var body = wordDoc.GetBody();
    var headings1 = body.Elements<DXW.Paragraph>().Where(p => p.HeadingLevel() == 1)
      .Select(h => h.GetText()).ToList();
    foreach (var str in headings1)
      if (!string.IsNullOrEmpty(str))
      {
        var s = CleanWhitespace(TryRemoveNumbering(str!));
        if (s.Length > 0 && !IsNumber(s))
          headers.Add(s);
      }
    var paragraphs = body.Elements<DXW.Paragraph>().Where(p => !p.IsHeading()).ToList();
    foreach (var paragraph in paragraphs)
    {
      var paraText = paragraph.GetText();
      var s = CleanWhitespace(TryRemoveNumbering(paraText));
      if (headers.Contains(s))
      {
        paragraph.Remove();
        removed++;
      }
    }
    if (VerboseLevel > 0)
      Console.WriteLine($"  {removed} paragraphs removed");
  }

  /// <summary>
  /// Resets format of heading paragraphs.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void ResetHeadingsFormat(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nResetting headings format");
    var reset = 0;
    var body = wordDoc.GetBody();
    var paragraphs = body.Elements<DXW.Paragraph>().Where(p => p.IsHeading()).ToList();
    foreach (var paragraph in paragraphs)
    {
      if (ResetParagraphFormat(paragraph))
        reset++;
    }
    if (VerboseLevel > 0)
      Console.WriteLine($"  {reset} paragraphs format reset");
  }

  /// <summary>
  /// Reset paragraph format by removing all the properties except the style id.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns>true if any properties were removed</returns>
  private bool ResetParagraphFormat(DXW.Paragraph paragraph)
  {
    bool done = false;
    var properties = paragraph.ParagraphProperties;
    if (properties != null)
    {
      foreach (var item in properties.Elements().ToList())
      {
        if (item is not DXW.ParagraphStyleId)
        {
          item.Remove();
          done = true;
        }
      }
    }
    foreach (var run in paragraph.Elements<DXW.Run>())
    {
      var runProperties = run.RunProperties;
      if (runProperties != null)
      {
        foreach (var item in runProperties.Elements().ToList())
        {
          if (item is not DXW.RunStyle)
          {
            item.Remove();
            done = true;
          }
        }
      }
    }
    return done;
  }

  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void ReplaceSymbolEncoding(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nReplacing symbol encoding characters with Unicode");
    var body = wordDoc.GetBody();
    var count = ReplaceSymbolEncoding(body);
    foreach (var header in wordDoc.GetHeaders())
      count += ReplaceSymbolEncoding(header);
    foreach (var footer in wordDoc.GetFooters())
      count += ReplaceSymbolEncoding(footer);
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} symbols replaced");
  }


  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="body"></param>
  /// <returns>count of replaced symbols</returns>
  public int ReplaceSymbolEncoding(DX.OpenXmlCompositeElement body)
  {
    int count = 0;
    foreach (var paragraph in body.Descendants<DXW.Paragraph>())
    {
      count += ReplaceSymbolEncoding(paragraph);
    }
    return count;
  }

  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns> count of replaced symbols</returns>
  public int ReplaceSymbolEncoding(DXW.Paragraph paragraph)
  {
    int count = 0;
    foreach (var run in paragraph.Descendants<DXW.Run>())
    {
      count += ReplaceSymbolEncoding(run);
    }
    return count;
  }

  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="run"></param>
  /// <returns> count of replaced symbols</returns>
  public int ReplaceSymbolEncoding(DXW.Run run)
  {
    var count = 0;
    foreach (var text in run.Elements<DXW.Text>())
    {
      var oldText = text.Text;
      var newText = oldText.ReplaceSymbolEncoding();
      var oldChars = oldText.ToCharArray();
      var newChars = newText.ToCharArray();
      if (oldChars.Length != newChars.Length)
        throw new Exception("The length of the text has changed.");
      var cnt = oldChars.Where((c, i) => c != newChars[i]).Count();
      count += cnt;
      if (cnt > 0)
        text.Text = newText;
    }
    return count;
  }

  /// <summary>
  /// Detect paragraphs that contain a bullet and enter a new new paragraph with bullet numbering.
  /// replace the bullet with a corresponding unicode character.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void RepairBulletContainedParagraph(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nRepairing bullet contained paragraphs");
    var defaultParagraphProperties = GetMostFrequentNumberingParagraphProperties(wordDoc);
    if (defaultParagraphProperties == null)
    {
      var abstractNumbering = wordDoc.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?
        .Elements<DXW.AbstractNum>().FirstOrDefault(a =>
          a.MultiLevelType?.Val?.Value == DXW.MultiLevelValues.HybridMultilevel
          && a.Elements<DXW.Level>().FirstOrDefault(l => l.LevelText?.Val?.Value == "•") != null);
      if (abstractNumbering != null)
      {
        var numbering = wordDoc.MainDocumentPart?.NumberingDefinitionsPart?.Numbering?
          .Elements<DXW.NumberingInstance>()
          .FirstOrDefault(n => n.AbstractNumId?.Val?.Value == abstractNumbering.AbstractNumberId?.Value);
        if (numbering != null)
        {
          defaultParagraphProperties = new DXW.ParagraphProperties
          {
            NumberingProperties = new DXW.NumberingProperties
            {
              NumberingLevelReference = new DXW.NumberingLevelReference { Val = 0 },
              NumberingId = new DXW.NumberingId { Val = numbering.NumberID }
            }
          };
        }
      }
    }
    int count = 0;
    var paragraphs = wordDoc.GetBody().Descendants<DXW.Paragraph>().ToList();
    for (int i = 0; i < paragraphs.Count; i++)
    {
      var paragraph = paragraphs[i];
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
          var newParagraphProperties = GetNearbyNumberingParagraphProperties(paragraph)
                                       ?? (DXW.ParagraphProperties?)defaultParagraphProperties?.CloneNode(true);
          if (run.PreviousSibling() != null && run.PreviousSibling() is not DXW.ParagraphProperties)
          {
            var newParagraph = new DXW.Paragraph();
            newParagraph.ParagraphProperties = newParagraphProperties;
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
              newParagraph.Append(item);
            }
            TryTrimParagraph(paragraph);
            var priorParagraph = paragraph.PreviousSibling<DXW.Paragraph>();
            if (priorParagraph != null && priorParagraph.ParagraphProperties?.NumberingProperties != null)
              paragraph.ParagraphProperties =
                (DXW.ParagraphProperties)priorParagraph.ParagraphProperties.CloneNode(true);
            TryTrimParagraph(newParagraph);
            paragraph.InsertAfterSelf(newParagraph);
            paragraphs.Insert(i + 1, newParagraph);
          }
          else // if it is the first run in the paragraph then do not create a new paragraph.
          {
            paragraph.ParagraphProperties = newParagraphProperties;
            TryTrimParagraph(paragraph);
            i--;
          }
        }
      }
    }
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} bullet contained paragraphs changed to list items");
  }

  private DXW.ParagraphProperties? GetMostFrequentNumberingParagraphProperties(DXPack.WordprocessingDocument wordDoc)
  {
    Dictionary<int, int> numberingFrequency = new();
    Dictionary<int, DXW.ParagraphProperties> numberingPropertiesIndex = new();
    foreach (var paragraph in wordDoc.GetBody().Descendants<DXW.Paragraph>())
    {
      var paragraphProperties = paragraph.ParagraphProperties;
      if (paragraphProperties != null)
      {
        var numberingProperties = paragraphProperties.NumberingProperties;
        if (numberingProperties != null)
        {
          var numberingLevel = numberingProperties.NumberingLevelReference?.Val;
          if (numberingLevel == null || numberingLevel != 0)
            continue;
          var numberingId = numberingProperties.NumberingId?.Val;
          if (numberingId == null)
            continue;
          if (numberingFrequency.ContainsKey(numberingId))
            numberingFrequency[numberingId]++;
          else
          {
            numberingPropertiesIndex[numberingId] = paragraphProperties;
            numberingFrequency[numberingId] = 1;
          }
        }
      }
    }
    if (numberingFrequency.Count == 0)
      return null;
    var mostFrequentNumberingId = numberingFrequency.OrderByDescending(kvp => kvp.Value).First().Key;
    return (DXW.ParagraphProperties)numberingPropertiesIndex[mostFrequentNumberingId].CloneNode(true);
    ;
  }

  private DXW.ParagraphProperties? GetNearbyNumberingParagraphProperties(DXW.Paragraph paragraph)
  {
    var paragraphProperties = paragraph.ParagraphProperties;
    if (paragraphProperties != null && paragraphProperties.NumberingProperties != null)
      return (DXW.ParagraphProperties)paragraphProperties.CloneNode(true);
    paragraphProperties = (paragraph.PreviousSibling() as DXW.Paragraph)?.ParagraphProperties;
    if (paragraphProperties != null && paragraphProperties.NumberingProperties != null)
      return (DXW.ParagraphProperties)paragraphProperties.CloneNode(true);
    paragraphProperties = (paragraph.NextSibling() as DXW.Paragraph)?.ParagraphProperties;
    if (paragraphProperties != null && paragraphProperties.NumberingProperties != null)
      return (DXW.ParagraphProperties)paragraphProperties.CloneNode(true);
    return null;
  }

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
  /// Joins sibling tables that have the same number of columns.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void JoinSiblingTables(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nJoining sibling tables");
    var body = wordDoc.GetBody();
    var count = 0;
    var tables = body.Descendants<DXW.Table>().ToList();
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
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} tables appended to previous ones");
  }


  /// <summary>
  /// Fix page-divided tables.
  /// Page-divided table is (usually long) table that have been split across consecutive pages
  /// so that the headings of the table are repeated on each page.
  /// Sometimes there are no repeating headings but the table is divided.
  /// After <see cref="JoinSiblingTables"/> it should be a single table.
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
    while (k < str.Length)
    {
      str = str.TrimStart();
      k = str.IndexOfAny(['.', '!', '?', ':'], k);
      if (k == -1)
        break;
      if (k == str.Length - 1)
      {
        var s1 = str.Substring(k + 1);
        sentences.Add(s1);
        break;
      }
      if (k + 1 < str.Length && str[k + 1] == ' ')

      {
        var s1 = str.Substring(0, k + 1);
        sentences.Add(s1);
        str = str.Substring(k + 1);
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

  private bool IsNumber(string str)
  {
    return str.ToCharArray().All(char.IsDigit);
  }

  /// <summary>
  /// Replaces all the whitespaces with a single space.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  private string NormalizeWhitespace(string str)
  {
    var chars = str.ToList();
    var wasChar = false;
    for (int i = 0; i < chars.Count; i++)
    {
      if (char.IsWhiteSpace(chars[i]))
      {
        chars[i] = ' ';
        wasChar = false;
      }
      if (chars[i] == ' ')
      {
        if (wasChar)
        {
          chars.RemoveAt(i);
          i--;
        }
        else
          wasChar = true;
      }
      else
        wasChar = false;
    }
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Removes all the whitespaces.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  private string CleanWhitespace(string str)
  {
    var chars = str.ToList();
    for (int i = 0; i < chars.Count; i++)
    {
      if (char.IsWhiteSpace(chars[i]))
      {
        chars.RemoveAt(i);
        i--;
      }
    }
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Removes a numbering from the string (if it begins the string).
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  private string TryRemoveNumbering(string str)
  {
    var numStr = str.GetNumberingString();
    if (numStr != null)
    {
      //if (str.Length > numStr.Length && str[numStr.Length - 1] == '.')
      //  str = str.Remove(numStr.Length - 1, 1);
      str = str.Substring(numStr.Length).TrimStart();
    }
    return str;
  }

}