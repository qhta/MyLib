﻿using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A composite tool for cleaning a Wordprocessing document.
/// </summary>
public partial class DocumentCleaner
{
  /// <summary>
  /// Determines the level of verbosity of the cleaner.
  /// </summary>
  public int VerboseLevel { get; set; }

  /// <summary>
  /// Determines the font used for examples.
  /// </summary>
  public string ExampleFont { get; set; } = "Courier New";

  /// <summary>
  /// Cleans the document using all the methods available.
  /// </summary>
  /// <param name="fileName"></param>
  public void CleanDocument(string fileName)
  {
    using var wordDoc = DXPack.WordprocessingDocument.Open(fileName, true);
    RemoveProofErrors(wordDoc);
    TrimParagraphs(wordDoc);
    RemoveEmptyParagraphs(wordDoc);
    RemoveFakeHeadersAndFooters(wordDoc);
    ResetHeadingsFormat(wordDoc);
    ReplaceSymbolEncoding(wordDoc);
    FixParagraphNumbering(wordDoc);
    FixInternalTables(wordDoc);
    FixTablesWithInvalidColumns(wordDoc);
    JoinAdjacentTables(wordDoc);
    FixDividedTables(wordDoc);
    JoinAdjacentRuns(wordDoc);
    FixLongWords(wordDoc);
    RepairXmlExamples(wordDoc);
    JoinDividedSentences(wordDoc);
    JoinParagraphsInFirstColumn(wordDoc);
    BreakParagraphsBefore(wordDoc, "Namespace:");
    FormatTables(wordDoc);
  }

  /// <summary>
  /// Removes all <c>ProofError</c> and <c>ProofState</c> elements from the document.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void RemoveProofErrors(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nRemoving proofing errors");
    var body = wordDoc.GetBody();
    var count = body.RemoveProofErrors();
    var headers = wordDoc.GetHeaders().ToList();
    foreach (var header in headers)
    {
      count += header.RemoveProofErrors();
    }
    var footers = wordDoc.GetFooters().ToList();
    foreach (var footer in wordDoc.GetFooters())
    {
      count += footer.RemoveProofErrors();
    }
    if (VerboseLevel > 0)
      Console.WriteLine($" {count} error tags removed.");
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
    var count = body.TrimParagraphs();
    var headers = wordDoc.GetHeaders().ToList();
    foreach (var header in headers)
    {
      count += header.TrimParagraphs();
    }
    var footers = wordDoc.GetFooters().ToList();
    foreach (var footer in wordDoc.GetFooters())
    {
      count += footer.TrimParagraphs();
    }
    if (VerboseLevel > 0)
      Console.WriteLine($" {count} paragraphs trimmed.");
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
  /// Joins adjacent tables that have the same number of columns.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void JoinAdjacentRuns(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nJoining adjacent runs");
    var body = wordDoc.GetBody();
    var count = body.JoinAdjacentRuns();
    foreach (var header in wordDoc.GetHeaders().ToList())
      count += header.JoinAdjacentRuns();
    foreach (var footer in wordDoc.GetFooters().ToList())
      count += footer.JoinAdjacentRuns();
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} runs appended to previous ones");
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
    var count = body.FixLongWords();
    foreach (var header in wordDoc.GetHeaders().ToList())
      count += header.FixLongWords();
    foreach (var footer in wordDoc.GetFooters().ToList())
      count += footer.FixLongWords();
    if (VerboseLevel > 0)
      Console.WriteLine($" {count} words fixed.");
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
      if (str != null)
      {
        str.TryRemoveNumbering(out var s);
        s = s.RemoveWhitespaces();
        if (s.Length > 0 && !s.IsNumber())
          headers.Add(s);
      }
    var allFooters = wordDoc.GetFooters().Select(f => f.GetText(null)).ToArray();
    foreach (var str in allFooters)
      if (str != null)
      {
        str.TryRemoveNumbering(out var s);
        s = s.RemoveWhitespaces();
        if (s.Length > 0 && !s.IsNumber())
          headers.Add(s);
      }
    var body = wordDoc.GetBody();
    var headings1 = body.Elements<DXW.Paragraph>().Where(p => p.HeadingLevel() == 1)
      .Select(h => h.GetText()).ToList();
    foreach (var str in headings1)
      if (str != null)
      {
        str.TryRemoveNumbering(out var s);
        s = s.RemoveWhitespaces();
        if (s.Length > 0 && !s.IsNumber())
          headers.Add(s);
      }
    var paragraphs = body.Elements<DXW.Paragraph>().Where(p => !p.IsHeading()).ToList();
    foreach (var paragraph in paragraphs)
    {
      var str = paragraph.GetText();
      str.TryRemoveNumbering(out var s);
      s = s.RemoveWhitespaces();
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
  /// Join sentences that are divided into multiple paragraphs.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void JoinDividedSentences(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nJoining divided sentences");
    var body = wordDoc.GetBody();
    var count = JoinDividedSentences(body);
    foreach (var header in wordDoc.GetHeaders().ToList())
      count += JoinDividedSentences(header);
    foreach (var footer in wordDoc.GetFooters().ToList())
      count += JoinDividedSentences(footer);
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} sentences joined");
  }

  /// <summary>
  /// Join sentences divided between adjacent paragraphs.
  /// </summary>
  /// <param name="element">Processed element</param>
  /// <returns>number of joins</returns>
  public int JoinDividedSentences(DX.OpenXmlCompositeElement element)
  {
    int count = 0;
    var paragraphs = element.Descendants<DXW.Paragraph>().ToList();
    for (int paraNdx = 0; paraNdx < paragraphs.Count; paraNdx++)
    {
      var para = paragraphs[paraNdx];
      var font = para.GetFont(null);
      if (font == ExampleFont)
        continue;
      var priorPara = para.PreviousSibling() as DXW.Paragraph;
      var paraText = para.GetText().Trim();
      if (paraText == "end note]" || paraText == "end example]" || paraText == "the results are:")
        continue;
      if (priorPara == null)
        continue;
      font = priorPara.GetFont(null);
      if (font == ExampleFont)
        continue;
      var priorParaText = priorPara.GetText().Trim();
      var priorSentences = priorParaText.GetSentences();
      var lastSentence = priorSentences.LastOrDefault() ?? priorParaText;

      var lastChar = lastSentence.LastOrDefault();
      if (char.IsUpper(lastSentence.FirstOrDefault()) && !".!?:".Contains(lastChar) &&
          char.IsLower(paraText.FirstOrDefault()))
      {
        Debug.WriteLine($"Join \"{priorParaText}\" & \"{paraText}\"");
        priorPara.JoinNextParagraph(para);
        para.Remove();
        count++;
      }
    }
    return count;
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
      if ((paragraph.TryResetFormat(true)))
        reset++;
    }
    if (VerboseLevel > 0)
      Console.WriteLine($"  {reset} paragraphs format reset");
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
    var count = body.ReplaceSymbolEncoding();
    foreach (var header in wordDoc.GetHeaders())
      count += body.ReplaceSymbolEncoding();
    foreach (var footer in wordDoc.GetFooters())
      count += body.ReplaceSymbolEncoding();
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} symbols replaced");
  }






  /// <summary>
  /// Find paragraphs that have the font of XML examples and track XML start and end tags.
  /// Each start tag should begin the paragraph and each end tag should be placed in a separate line.
  /// If a paragraph does not begin with a start tag, its content is moved to the previous paragraph.
  /// Whitespaces are normalized.
  /// Embedded XML tags are indented by adding two spaces on left.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void RepairXmlExamples(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nRepairing XML end tag containing paragraphs");
    int count = 0;
    var paragraphs = wordDoc.GetBody().Descendants<DXW.Paragraph>().ToList();
    //int xmlIndent = 0;
    for (int i = 0; i < paragraphs.Count; i++)
    {
      var paragraph = paragraphs[i];

      var font = paragraph.GetFont("defaultFont");
      if (font != ExampleFont)
        continue;

      var text = paragraph.GetText();

      var indent = text.LeftIndentLength();
      if (IsXmlExample(text))
      {
        NormalizeXmlParagraph(paragraph, indent);
      }
      text = paragraph.GetText();
      indent = text.LeftIndentLength();
      if (text.Contains("Target=\"http://www.ecma-international.org/\""))
        Debug.Assert((true));

      if (indent < text.Length && text[indent] != '<')
      { // if the paragraph does not start with an XML tag, append its content to the previous paragraph.
        var priorParagraph = paragraph.PreviousSibling<DXW.Paragraph>();
        if (priorParagraph != null && priorParagraph.GetText().Trim().StartsWith("<"))
        {
          var priorText = priorParagraph.GetText();
          text = text.Trim();
          if (!priorText.EndsWith(" "))
            text = " " + text;
          priorParagraph.AppendText(text);
          paragraph.Remove();
          paragraphs.RemoveAt(i);
          i -= 2;
          if (i < 0)
            i = 0;
          count++;
        }

      }
      else if (text.Length > indent + 1)
      {
        var k = text.IndexOf("<", indent + 1);
        if (k != -1)
        {
          if (k > 0 && k < text.Length - 1 && (text[k + 1] == '/' || char.IsLetter(text[k + 1])))
          {
            // if the paragraph contains an XML tag but not in the beginning, split it to a new paragraph.
            //Console.WriteLine(text);
            var newParagraph = paragraph.SplitAt(k);
            paragraph.TrimEnd();
            if (newParagraph != null)
            {
              var spacing = paragraph.GetParagraphProperties().GetSpacingBetweenLines();
              spacing.After = "8";
              paragraph.InsertAfterSelf(newParagraph);
              paragraphs.Add(newParagraph);
            }
            i--;
            count++;
            if (i < 0)
              i = 0;
          }
          else
          {
            //NormalizeXmlParagraph(paragraph, indent);
          }
        }
        else
        {
          //NormalizeXmlParagraph(paragraph, indent);
        }
      }
    }

    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} XML paragraphs repaired");
  }


  /// <summary>
  /// Check if the text contains an XML example.
  /// </summary>
  /// <param name="text"></param>
  /// <returns></returns>
  private bool IsXmlExample(string text)
  {
    var k = text.IndexOf('<');
    if (k == -1)
      return false;
    if (k < text.Length - 1 && (text[k + 1] == '/' || char.IsLetter(text[k + 1])))
      return true;
    return false;
  }

  /// <summary>
  /// Normalize the whitespaces in the XML paragraph. Keep the spaces indentation.
  /// Set paragraph indent to the specified value of characters (plus two).
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="indent"></param>
  private void NormalizeXmlParagraph(DXW.Paragraph paragraph, int indent)
  {
    var text = paragraph.GetText();
    var newText = text.NormalizeWhitespaces();
    var newIndent = newText.LeftIndentLength();
    if (newIndent < indent)
      newText = new String(' ', indent - newIndent) + newText;
    paragraph.SetText(newText);
    var indentation = paragraph.GetIndentation();
    var left = indentation.GetLeft() ?? 0;
    var hanging = ((indent + 2) * 12 * 20);
    var newLeft = hanging;
    indentation.SetLeft(newLeft);
    indentation.SetHanging(hanging);
  }
}