using System;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;


namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Run element.
/// </summary>
public static class RunTools
{
  //public static AnnotationReferenceMark GetAnnotationReferenceMark (this Run run) { return run.Elements<AnnotationReferenceMark>().FirstOrDefault();} 
  //public static FootnoteReferenceMark GetFootnoteReferenceMark (this Run run) { return run.Elements<FootnoteReferenceMark>().FirstOrDefault();} 
  //public static EndnoteReferenceMark GetEndnoteReferenceMark (this Run run) { return run.Elements<EndnoteReferenceMark>().FirstOrDefault();} 
  //public static LastRenderedPageBreak GetLastRenderedPageBreak (this Run run) { return run.Elements<LastRenderedPageBreak>().FirstOrDefault();} 
  //public static Break GetBreak (this Run run) { return run.Elements<Break>().FirstOrDefault();} 
  //public static Text GetText (this Run run) { return run.Elements<Text>().FirstOrDefault();} 
  //public static DeletedText GetDeletedText (this Run run) { return run.Elements<DeletedText>().FirstOrDefault();} 
  //public static FieldCode GetFieldCode (this Run run) { return run.Elements<FieldCode>().FirstOrDefault();} 
  //public static DeletedFieldCode GetDeletedFieldCode (this Run run) { return run.Elements<DeletedFieldCode>().FirstOrDefault();} 
  //public static NoBreakHyphen GetNoBreakHyphen (this Run run) { return run.Elements<NoBreakHyphen>().FirstOrDefault();} 
  //public static SoftHyphen GetSoftHyphen (this Run run) { return run.Elements<SoftHyphen>().FirstOrDefault();} 
  //public static DayShort GetDayShort (this Run run) { return run.Elements<DayShort>().FirstOrDefault();} 
  //public static MonthShort GetMonthShort (this Run run) { return run.Elements<MonthShort>().FirstOrDefault();} 
  //public static YearShort GetYearShort (this Run run) { return run.Elements<YearShort>().FirstOrDefault();} 
  //public static DayLong GetDayLong (this Run run) { return run.Elements<DayLong>().FirstOrDefault();} 
  //public static MonthLong GetMonthLong (this Run run) { return run.Elements<MonthLong>().FirstOrDefault();} 
  //public static YearLong GetYearLong (this Run run) { return run.Elements<YearLong>().FirstOrDefault();} 
  //public static EmbeddedObject GetEmbeddedObject (this Run run) { return run.Elements<EmbeddedObject>().FirstOrDefault();} 
  //public static PositionalTab GetPositionalTab (this Run run) { return run.Elements<PositionalTab>().FirstOrDefault();} 
  //public static SeparatorMark GetSeparatorMark (this Run run) { return run.Elements<SeparatorMark>().FirstOrDefault();} 
  //public static ContinuationSeparatorMark GetContinuationSeparatorMark (this Run run) { return run.Elements<ContinuationSeparatorMark>().FirstOrDefault();} 
  //public static SymbolChar GetSymbolChar (this Run run) { return run.Elements<SymbolChar>().FirstOrDefault();} 
  //public static PageNumber GetPageNumber (this Run run) { return run.Elements<PageNumber>().FirstOrDefault();} 
  //public static CarriageReturn GetCarriageReturn (this Run run) { return run.Elements<CarriageReturn>().FirstOrDefault();} 
  //public static TabChar GetTabChar (this Run run) { return run.Elements<TabChar>().FirstOrDefault();} 
  //public static RunProperties GetTextProperties (this Run run) { return run.Elements<RunProperties>().FirstOrDefault();} 
  //public static Picture GetPicture (this Run run) { return run.Elements<Picture>().FirstOrDefault();} 
  //public static FieldChar GetFieldChar (this Run run) { return run.Elements<FieldChar>().FirstOrDefault();} 
  //public static Ruby GetRuby (this Run run) { return run.Elements<Ruby>().FirstOrDefault();} 
  //public static FootnoteReference GetFootnoteReference (this Run run) { return run.Elements<FootnoteReference>().FirstOrDefault();} 
  //public static EndnoteReference GetEndnoteReference (this Run run) { return run.Elements<EndnoteReference>().FirstOrDefault();} 
  //public static CommentReference GetCommentReference (this Run run) { return run.Elements<CommentReference>().FirstOrDefault();} 
  //public static Drawing GetDrawing (this Run run) { return run.Elements<Drawing>().FirstOrDefault();} 

  /// <summary>
  /// Get the <c>RunProperties</c> element of the run. If it is null, create a new one.
  /// </summary>
  /// <param name="run"></param>
  /// <returns></returns>
  public static DXW.RunProperties GetRunProperties(this DXW.Run run)
  {
    if (run.RunProperties == null)
      run.RunProperties = new DXW.RunProperties();
    return run.RunProperties;
  }

  /// <summary>
  /// Get the text content of the run.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.Run run, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    StringBuilder sb = new();
    foreach (var element in run.Elements())
    {
      if (element is DXW.Text text)
      {
        sb.Append(text.Text);
      }
      else if (element is DXW.Break @break)
      {
        if (@break.Type?.Value == BreakValues.Page)
          sb.Append(options.BreakPageTag);
        else if (@break.Type?.Value == BreakValues.Column)
          sb.Append(options.BreakColumnTag);
        else if (@break.Type?.Value == BreakValues.TextWrapping)
          sb.Append(options.BreakLineTag);
      }
      else if (element is TabChar)
      {
        sb.Append(options.TabTag);
      }
      else if (element is CarriageReturn)
      {
        sb.Append(options.CarriageReturnTag);
      }
      else if (element is FieldChar fieldChar)
      {
        if (fieldChar.FieldCharType?.Value == FieldCharValues.Begin && options.IncludeFieldFormula)
        {
          sb.Append(options.FieldStartTag);
        }
        else if (fieldChar.FieldCharType?.Value == FieldCharValues.Separate && options.IncludeFieldFormula)
        {
          sb.Append(options.FieldResultTag);
        }
        else if (fieldChar.FieldCharType?.Value == FieldCharValues.End && options.IncludeFieldFormula)
        {
          sb.Append(options.FieldEndTag);
        }
      }
      else if (element is FieldCode fieldCode && options.IncludeFieldFormula)
      {
        sb.Append(fieldCode.Text);
      }
      else if (element is SymbolChar symbolChar)
      {
        if (int.TryParse(symbolChar.Char!.Value, out var symbolVal))
        {
          sb.Append((char)symbolVal);
        }
      }
      else if (element is PositionalTab)
      {
        sb.Append(options.TabTag);
      }
      else if (element is Ruby ruby)
      {
        sb.Append(ruby.GetPlainText());
      }
      else if (element is FootnoteReference footnoteReference)
      {
        sb.Append(options.FootnoteRefStart + footnoteReference.Id + options.FootnoteRefEnd);
      }
      else if (element is EndnoteReference endnoteReference)
      {
        sb.Append(options.EndnoteRefStart + endnoteReference.Id + options.EndnoteRefEnd);
      }
      else if (element is CommentReference commentReference)
      {
        sb.Append(options.CommentRefStart + commentReference.Id + options.CommentRefEnd);
      }
    }

    return sb.ToString();
  }

  /// <summary>
  /// Set the text content of the run.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="value"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static void SetText(this DXW.Run run, string? value, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    var runProperties = run.GetRunProperties();
    run.RemoveAllChildren();
    run.AppendChild(runProperties);
    if (value == null)
      return;
    var sb = new StringBuilder();
    for (int i = 0; i < value.Length; i++)
    {
      if (value.HasSubstringAt(i, options.BreakPageTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new DXW.Break() { Type = BreakValues.Page });
      }
      else if (value.HasSubstringAt(i, options.BreakColumnTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new DXW.Break() { Type = BreakValues.Column });
      }
      else if (value.HasSubstringAt(i, options.BreakLineTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new DXW.Break() { Type = BreakValues.TextWrapping });
      }
      else if (value.HasSubstringAt(i, options.TabTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new TabChar());
      }
      else if (value.HasSubstringAt(i, options.CarriageReturnTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new CarriageReturn());
      }
      else if (value.HasSubstringAt(i, options.FieldStartTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new FieldChar() { FieldCharType = FieldCharValues.Begin });
      }
      else if (value.HasSubstringAt(i, options.FieldResultTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new FieldChar() { FieldCharType = FieldCharValues.Separate });
      }
      else if (value.HasSubstringAt(i, options.FieldEndTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new FieldChar() { FieldCharType = FieldCharValues.End });
      }
      else if (value.HasSubstringAt(i, options.FootnoteRefStart))
      {
        TryAppend(run, sb);
        var l = options.FootnoteRefStart.Length;
        var k = value.IndexOf(options.FootnoteRefEnd, i + l);
        if (k > 0 && int.TryParse(value.Substring(i + l, k - i - l), out var id))
        {
          var footnoteReference = new FootnoteReference
          {
            Id = id
          };
          run.AppendChild(footnoteReference);
          i = k;
        }
        else
        {
          sb.Append(value[i]);
        }
      }
      else if (value.HasSubstringAt(i, options.EndnoteRefStart))
      {
        TryAppend(run, sb);
        var l = options.EndnoteRefStart.Length;
        var k = value.IndexOf(options.EndnoteRefEnd, i + l);
        if (k > 0 && int.TryParse(value.Substring(i + l, k - i - l), out var id))
        {
          var endnoteReference = new EndnoteReference
          {
            Id = id
          };
          run.AppendChild(endnoteReference);
          i = k;
        }
        else
        {
          sb.Append(value[i]);
        }
      }
      else if (value.HasSubstringAt(i, options.CommentRefStart))
      {
        TryAppend(run, sb);
        var l = options.CommentRefStart.Length;
        var k = value.IndexOf(options.CommentRefEnd, i + l);
        if (k > 0 && int.TryParse(value.Substring(i + l, k - i - l), out var id))
        {
          var commentReference = new CommentReference()
          {
            Id = id.ToString()
          };
          run.AppendChild(commentReference);
          i = k;
        }
        else
        {
          sb.Append(value[i]);
        }
      }
      else
      {
        sb.Append(value[i]);
      }
    }
    TryAppend(run, sb);
  }

  private static void TryAppend(this DXW.Run run, StringBuilder sb)
  {
    var s = sb.ToString();
    if (!string.IsNullOrEmpty(s))
    {
      var newText = new DXW.Text(s);
      if (s.Trim()!=s)
        newText.Space = DX.SpaceProcessingModeValues.Preserve;
      run.Append(newText);
      sb.Clear();
    }
  }

  /// <summary>
  /// Append text to the paragraph.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="text"></param>
  public static void AppendText(this DXW.Run run, string text)
  {
    var runText = run.Descendants<DXW.Text>().LastOrDefault();
    if (runText == null)
    {
      runText = new DXW.Text
      {
        Text = text
      };
      run.AppendChild(runText);
    }
    else
    {
      runText.Text += text;
    }
  }

  /// <summary>
  /// Checks if the run properties contain a <c>Bold</c> or <c>BoldComplexScript</c> element and returns the value of the <c>Val</c> attribute.
  /// </summary>
  /// <param name="run"></param>
  /// <returns></returns>
  public static bool IsBold(this DXW.Run run)
  {
    var bold = run.GetRunProperties().Bold;
    if (bold != null)
    {
      return bold.Val?.Value ?? true;
    }
    var boldCS = run.GetRunProperties().BoldComplexScript;
    if (boldCS != null)
    {
      return boldCS.Val?.Value ?? true;
    }
    return false;
  }

  /// <summary>
  /// Checks if the run properties contain a <c>Italic</c> or <c>ItalicComplexScript</c> element and returns the value of the <c>Val</c> attribute.
  /// </summary>
  /// <param name="run"></param>
  /// <returns></returns>
  public static bool IsItalic(this DXW.Run run)
  {
    var italic = run.GetRunProperties().Italic;
    if (italic != null)
    {
      return italic.Val?.Value ?? true;
    }
    var italicCS = run.GetRunProperties().ItalicComplexScript;
    if (italicCS != null)
    {
      return italicCS.Val?.Value ?? true;
    }
    return false;
  }

  /// <summary>
  /// Checks if the run properties contain a <c>Underline</c> element and checks if the value of the <c>Val</c> attribute is not <c>None</c>.
  /// </summary>
  /// <param name="run"></param>
  /// <returns></returns>
  public static bool IsUnderline(this DXW.Run run)
  {
    var underline = run.GetRunProperties().Underline;
    if (underline != null)
    {
      return underline.Val?.Value != DXW.UnderlineValues.None;
    }
    return false;
  }

  /// <summary>
  /// Checks if the run is empty.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DXW.Run? element)
  {
    if (element == null)
      return true;
    foreach (var e in element.MemberElements())
    {
      if (e is DXW.Text runText)
      {
        if (!runText.IsEmpty())
          return false;
      }
      else
      if (e is DXW.TabChar or DXW.LastRenderedPageBreak)
      {
        // ignore
      }
      else
        return false;
    }
    return true;
  }

  /// <summary>
  /// Try to trim run text.
  /// </summary>
  /// <param name="run"></param>
  /// <returns></returns>
  public static bool TryTrim(this DXW.Run run)
  {
    bool done = false;
    var lastElement = run.MemberElements().LastOrDefault();
    while (lastElement != null)
    {
      var previousElement = lastElement.PreviousSibling();
      if (lastElement is DXW.BookmarkEnd)
      {
        // ignore
      }
      else
      if (lastElement is DXW.LastRenderedPageBreak)
      {
        lastElement.Remove();
      }
      if (lastElement is DXW.TabChar)
      {
        lastElement.Remove();
        done = true;
      }
      else
      if (lastElement is DXW.Text runText)
      {
        var text = runText.Text;
        var trimmedText = text.TrimEnd();
        if (trimmedText == "")
        {
          runText.Remove();
          done = true;
        }

        if (text != trimmedText)
        {
          runText.Text = trimmedText;
          done = true;
        }
        else
        {
          break;
        }
      }

      lastElement = previousElement;
    }
    return done;
  }

  /// <summary>
  /// Reset run format by removing all the properties except the run style.
  /// </summary>
  /// <param name="run"></param>
  /// <returns>true if any properties were removed</returns>
  public static bool TryResetFormat(this DXW.Run run)
  {
    bool done = false;
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
    return done;
  }

  /// <summary>
  /// Insert an optional hyphens into long words, especially URL's.
  /// </summary>
  /// <param name="run"></param>
  /// <returns>true if the fixing was successful, false if it was not needed.</returns>
  public static bool TryFixLongWords(this DXW.Run run)
  {
    var done = false;
    var runElements = run.MemberElements();
    foreach (var element in runElements)
    {
      if (element is DXW.Text text)
      {
        var textValue = text.Text;
        var newTextValue = textValue.FixLongWords();
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
    if (run.NextSibling() is DXW.Run nextRun)
    {
      var text = run.GetText();
      if (text.StartsWith("/word"))
        Debug.Assert(true);
      var nextText = nextRun.GetText();
      if (text.TrimEnd()==text && !text.EndsWith("-") && nextText.TrimStart() == text && !nextText.StartsWith("-"))
      {
        run.AppendChild(new DXW.SoftHyphen());
        done = true;
      }
    }
    return done;
  }

  /// <summary>
  /// Replace characters with a code between F000 and F0DD to corresponding unicode characters according to the symbol encoding.
  /// </summary>
  /// <param name="run"></param>
  /// <returns>number of replaced symbols</returns>
  public static int ReplaceSymbolEncoding(this DXW.Run run)
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
  /// Get the name of the font most frequently used in run text.
  /// If there is no font defined, return default font statistics.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="defaultFont">Font name used when there is no runFonts element</param>
  public static string? GetFont(this DXW.Run run, string? defaultFont)
  {
    var runFonts = run.RunProperties?.RunFonts;
    if (runFonts != null)
    {
      var fonts = run.GetRunFonts(defaultFont);
      return fonts?.MostFrequent();
    }
    return null;
  }

  /// <summary>
  /// Get statistics of fonts used for run text. If there is no font defined, return default font statistics.
  /// </summary>
  /// <param name="run">Run element to examine</param>
  /// <param name="defaultFont">Font name used when there is no runFonts element</param>
  public static StringStatistics? GetRunFonts(this DXW.Run run, string? defaultFont)
  {
    var runProperties = run.RunProperties;
    if (runProperties != null)
    {
      var text = run.GetText();
      return runProperties.GetRunFonts(text, defaultFont);
    }
    if (defaultFont != null)
    {
      var text = run.GetText();
      var fonts = new StringStatistics();
      fonts.Add(defaultFont, text.Length);
      return fonts;
    }
    return null;
  }

  /// <summary>
  /// Get statistics of properties used for run text. Each entry in the dictionary represents a property fullName and the number of times it is used.
  /// The fullName of the property consists of the property element fullName (prefix+":"+localName), attribute fullName and the text value of the attribute.
  /// Format is: elementFullName attributeFullName=attributeValue
  /// </summary>
  /// <param name="run">Run element to examine</param>
  /// <param name="defaultProperties">Default font properties used when RunProperties are not found</param>
  public static ObjectStatistics<TextProperties>? GetTextPropertiesStatistics(this DXW.Run run, TextProperties? defaultProperties)
  {
    var runProperties = run.RunProperties;
    if (runProperties != null)
    {
      var text = run.GetText();
      return runProperties.GetTextPropertiesStatistics(text, defaultProperties);
    }
    if (defaultProperties != null)
    {
      var text = run.GetText();
      var stats = new ObjectStatistics<TextProperties>();
      stats.Add(defaultProperties, text.Length);
      return stats;
    }
    return null;
  }


  /// <summary>
  /// Set the text properties to the run.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="properties"></param>
  public static void SetTextProperties(this DXW.Run run, TextProperties properties)
  {
    var runProperties = run.GetRunProperties();
    runProperties.SetTextProperties(properties);
  }

  /// <summary>
  /// Normalize whitespaces in the run text. Run element is modified when it contains only text or tab elements.
  /// In the effect, there is only one text run with all whitespaces replaced by a single space.
  /// All other elements are removed.
  /// </summary>
  /// <param name="run">Run element to process</param>
  public static void NormalizeWhitespaces(this DXW.Run run)
  {
    if (run.MemberElements().All(e => e is DXW.Text || e is TabChar))
    {
      var text = run.GetText();
      var newText = text.NormalizeWhitespaces();
      run.SetText(newText);
    }
  }

  /// <summary>
  /// Split the run at the specified index, which is the number of characters from the beginning of the run.
  /// Split is not possible if the index is at the beginning or end of the run.
  /// Returns the second part of the run.
  /// </summary>
  /// <param name="run">Run element to process</param>
  /// <param name="index">Char position number</param>
  /// <param name="options">Options for text extraction</param>
  /// <returns>Next, newly created run (or null) if split is not available</returns>
  public static DXW.Run? SplitAt(this DXW.Run run, int index, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    if (index <= 0 || index >= run.GetText(options).Length)
      return null;

    var textLength = 0;
    DXW.Run? newRun = null;
    foreach (var member in run.MemberElements().ToList())
    {
      var memberText = member.GetText(options);
      if (memberText != null)
      {
        var memberTextLength = memberText.Length;
        var newTextLength = textLength + memberTextLength;
        if (index <= newTextLength)
        {
          if (index < newTextLength)
          {
            if (member is DXW.Text text)
            {
              DX.OpenXmlElement? newMember = text.SplitAt(index - textLength, options);
              if (newMember != null)
              {
                newRun ??= NewRun(run);
                newRun.AppendChild(newMember);
              }
            }
            else
              return null;
          }
          var nextSibling = member.NextSibling();
          while (nextSibling != null)
          {
            newRun ??= NewRun(run);
            nextSibling.Remove();
            newRun.AppendChild(nextSibling);
            nextSibling = nextSibling.NextSibling();
          }
          break;
        }
        else
        {
          textLength = newTextLength;
        }
      }
    }
    return newRun;
  }

  /// <summary>
  /// Create a new run with the same properties as the source run.
  /// </summary>
  /// <param name="run">Run element to process</param>
  public static DXW.Run NewRun(this DXW.Run run)
  {
    var newRun = new DXW.Run();
    var properties = run.RunProperties;
    if (properties != null)
    {
      newRun.RunProperties = (DXW.RunProperties)properties.CloneNode(true);
    }
    return newRun;
  }


}