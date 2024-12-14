using System;
using System.Text;

using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;


namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Run element.
/// </summary>
public static class RunTools
{

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

  ///// <summary>
  ///// Set the text content of the run.
  ///// </summary>
  ///// <param name="run"></param>
  ///// <param name="value"></param>
  ///// <param name="options"></param>
  ///// <returns></returns>
  //public static void SetText(this DXW.Run run, string? value, TextOptions? options = null)
  //{
  //  if (options == null)
  //    options = TextOptions.PlainText;
  //  var runProperties = run.GetRunProperties();
  //  run.RemoveAllChildren();
  //  run.AppendChild(runProperties);
  //  if (value == null)
  //    return;
  //  var sb = new StringBuilder();
  //  for (int i = 0; i < value.Length; i++)
  //  {
  //    if (value.HasSubstringAt(i, options.BreakPageTag))
  //    {
  //      TryAppend(run, sb);
  //      run.AppendChild(new DXW.Break() { Type = BreakValues.Page });
  //    }
  //    else if (value.HasSubstringAt(i, options.BreakColumnTag))
  //    {
  //      TryAppend(run, sb);
  //      run.AppendChild(new DXW.Break() { Type = BreakValues.Column });
  //    }
  //    else if (value.HasSubstringAt(i, options.BreakLineTag))
  //    {
  //      TryAppend(run, sb);
  //      run.AppendChild(new DXW.Break() { Type = BreakValues.TextWrapping });
  //    }
  //    else if (value.HasSubstringAt(i, options.TabChar))
  //    {
  //      TryAppend(run, sb);
  //      run.AppendChild(new TabChar());
  //    }
  //    else if (value.HasSubstringAt(i, options.CarriageReturnTag))
  //    {
  //      TryAppend(run, sb);
  //      run.AppendChild(new CarriageReturn());
  //    }
  //    else if (value.HasSubstringAt(i, options.FieldStartTag))
  //    {
  //      TryAppend(run, sb);
  //      run.AppendChild(new FieldChar() { FieldCharType = FieldCharValues.Begin });
  //    }
  //    else if (value.HasSubstringAt(i, options.FieldResultTag))
  //    {
  //      TryAppend(run, sb);
  //      run.AppendChild(new FieldChar() { FieldCharType = FieldCharValues.Separate });
  //    }
  //    else if (value.HasSubstringAt(i, options.FieldEndTag))
  //    {
  //      TryAppend(run, sb);
  //      run.AppendChild(new FieldChar() { FieldCharType = FieldCharValues.End });
  //    }
  //    else if (value.HasSubstringAt(i, options.FootnoteRefStart))
  //    {
  //      TryAppend(run, sb);
  //      var l = options.FootnoteRefStart.Length;
  //      var k = value.IndexOf(options.FootnoteRefEnd, i + l);
  //      if (k > 0 && int.TryParse(value.Substring(i + l, k - i - l), out var id))
  //      {
  //        var footnoteReference = new FootnoteReference
  //        {
  //          Id = id
  //        };
  //        run.AppendChild(footnoteReference);
  //        i = k;
  //      }
  //      else
  //      {
  //        sb.Append(value[i]);
  //      }
  //    }
  //    else if (value.HasSubstringAt(i, options.EndnoteRefStart))
  //    {
  //      TryAppend(run, sb);
  //      var l = options.EndnoteRefStart.Length;
  //      var k = value.IndexOf(options.EndnoteRefEnd, i + l);
  //      if (k > 0 && int.TryParse(value.Substring(i + l, k - i - l), out var id))
  //      {
  //        var endnoteReference = new EndnoteReference
  //        {
  //          Id = id
  //        };
  //        run.AppendChild(endnoteReference);
  //        i = k;
  //      }
  //      else
  //      {
  //        sb.Append(value[i]);
  //      }
  //    }
  //    else if (value.HasSubstringAt(i, options.CommentRefStart))
  //    {
  //      TryAppend(run, sb);
  //      var l = options.CommentRefStart.Length;
  //      var k = value.IndexOf(options.CommentRefEnd, i + l);
  //      if (k > 0 && int.TryParse(value.Substring(i + l, k - i - l), out var id))
  //      {
  //        var commentReference = new CommentReference()
  //        {
  //          Id = id.ToString()
  //        };
  //        run.AppendChild(commentReference);
  //        i = k;
  //      }
  //      else
  //      {
  //        sb.Append(value[i]);
  //      }
  //    }
  //    else
  //    {
  //      sb.Append(value[i]);
  //    }
  //  }
  //  TryAppend(run, sb);
  //}

  private static void TryAppend(this DXW.Run run, StringBuilder sb)
  {
    var s = sb.ToString();
    if (!string.IsNullOrEmpty(s))
    {
      var newText = new DXW.Text(s);
      if (s.Trim() != s)
        newText.Space = DX.SpaceProcessingModeValues.Preserve;
      run.Append(newText);
      sb.Clear();
    }
  }

  /// <summary>
  /// Append text to the run.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="text"></param>
  public static void AppendText(this DXW.Run run, string text)
  {
    var runText = run.Descendants<DXW.Text>().LastOrDefault();
    if (runText == null)
    {
      var keepSpaces = (text.Trim() != text);
      runText = new DXW.Text
      {
        Text = text,
        Space = new DX.EnumValue<DX.SpaceProcessingModeValues>(
          keepSpaces ? DX.SpaceProcessingModeValues.Preserve : DX.SpaceProcessingModeValues.Default)
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
  /// Sets both <c>Bold</c> or <c>BoldComplexScript</c> elements to the given value.
  /// If the value is <c>null</c>, the elements are removed.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetBold(this DXW.Run run, bool? value)
  {
    var runProperties = run.GetRunProperties();
    if (value.HasValue)
    {
      runProperties.SetBold(value, false);
      runProperties.SetBold(value, true);
    }
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
  /// Sets both <c>Bold</c> or <c>BoldComplexScript</c> elements to the given value.
  /// If the value is <c>null</c>, the elements are removed.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetItalic(this DXW.Run run, bool? value)
  {
    var runProperties = run.GetRunProperties();
    if (value.HasValue)
    {
      runProperties.SetItalic(value, false);
      runProperties.SetItalic(value, true);
    }
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
  /// <param name="run"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DXW.Run run)
  {
    var members = run.GetMembers().ToList();
    foreach (var member in members)
    {
      if (member is DXW.Text text)
      {
        if (!text.IsEmpty())
          return false;
      }
      else
        return false;
    }
    return true;
  }

  /// <summary>
  /// Checks if the run contains any tab char.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool HasTabChar(this DXW.Run element)
  {

    var result = element.Elements<DXW.TabChar>().Any();
    return result;
  }

  /// <summary>
  /// Checks if the run contains a single tab char.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsTabChar(this DXW.Run element)
  {
    var result = element.Elements<DXW.TabChar>().Any() && element.GetMembers().Count() == 1;
    return result;
  }


  /// <summary>
  /// Checks if the run contains any drawing.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool HasDrawing(this DXW.Run element)
  {
    var result = element.Elements<DXW.Drawing>().Any();
    return result;
  }

  /// <summary>
  /// Checks if the run contains a single drawing.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsDrawing(this DXW.Run element)
  {
    var result = element.Elements<DXW.Drawing>().Any() && element.GetMembers().Count() == 1;
    return result;
  }

  /// <summary>
  /// Trim run text removing leading whitespaces
  /// </summary>
  /// <param name="run"></param>
  /// <returns>True is trimmed</returns>
  public static bool TrimStart(this DXW.Run run)
  {
    bool done = false;
    var firstElement = run.GetMembers().FirstOrDefault();
    while (firstElement != null)
    {
      var previousElement = firstElement.NextSibling();
      if (firstElement is DXW.BookmarkStart || firstElement is DXW.BookmarkEnd)
      {
        // ignore
      }
      else if (firstElement is DXW.LastRenderedPageBreak)
      {
        firstElement.Remove();
      }
      if (firstElement is DXW.TabChar)
      {
        firstElement.Remove();
        done = true;
      }
      else if (firstElement is DXW.Text runText)
      {
        var text = runText.Text;
        var trimmedText = text.TrimStart();
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

      firstElement = previousElement;
    }
    return done;
  }

  /// <summary>
  /// Trim run text removing trailing whitespaces
  /// </summary>
  /// <param name="run"></param>
  /// <returns>True is trimmed</returns>
  public static bool TrimEnd(this DXW.Run run)
  {
    bool done = false;
    var lastElement = run.GetMembers().LastOrDefault();
    while (lastElement != null)
    {
      var previousElement = lastElement.PreviousSiblingMember();
      if (lastElement is DXW.BookmarkEnd || lastElement is DXW.BookmarkStart)
      {
        // ignore
      }
      else if (lastElement is DXW.LastRenderedPageBreak)
      {
        lastElement.Remove();
      }
      if (lastElement is DXW.TabChar)
      {
        lastElement.Remove();
        done = true;
      }
      else if (lastElement is DXW.Text runText)
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
          break;
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
    var runElements = run.GetMembers();
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
      var text = run.GetText(TextOptions.PlainText);
      if (text.StartsWith("/word"))
        Debug.Assert(true);
      var nextText = nextRun.GetText(TextOptions.PlainText);
      if (text.TrimEnd() == text && !text.EndsWith("-") && nextText.TrimStart() == text && !nextText.StartsWith("-"))
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
      var text = run.GetText(TextOptions.PlainText);
      return runProperties.GetRunFonts(text, defaultFont);
    }
    if (defaultFont != null)
    {
      var text = run.GetText(TextOptions.PlainText);
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
  public static ObjectStatistics<TextProperties>? GetTextPropertiesStatistics
    (this DXW.Run run, TextProperties? defaultProperties)
  {
    var runProperties = run.RunProperties;
    if (runProperties != null)
    {
      var text = run.GetText(TextOptions.PlainText);
      return runProperties.GetTextPropertiesStatistics(text, defaultProperties);
    }
    if (defaultProperties != null)
    {
      var text = run.GetText(TextOptions.PlainText);
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
    if (run.GetMembers().All(e => e is DXW.Text || e is TabChar))
    {
      var text = run.GetText(TextOptions.PlainText);
      var newText = text.NormalizeWhitespaces();
      run.SetText(newText);
    }
  }

  /// <summary>
  /// Split the run at the specified position, which is the number of characters from the beginning of the run.
  /// Split is not possible if the position is at the beginning or end of the run.
  /// Returns the second part of the run.
  /// </summary>
  /// <param name="run">Run element to process</param>
  /// <param name="position">Char position number</param>
  /// <param name="options">Options for text extraction</param>
  /// <returns>Next, newly created run (or null if split is not available)</returns>
  public static DXW.Run? SplitAt(this DXW.Run run, int position, TextOptions options)
  {
    if (position <= 0 || position >= run.GetText(options).Length)
      return null;
    var members = run.GetMembers().ToList();
    var sumLength = 0;
    var newRun = (DXW.Run)run.CloneNode(false);
    newRun.RunProperties = (DXW.RunProperties?)run.RunProperties?.CloneNode(true);
    for (int i = 0; i < members.Count; i++)
    {
      var member = members[i];
      var memberText = member.GetText(options);
      if (memberText != string.Empty)
      {
        var memberTextLength = memberText.Length;
        var newTextLength = sumLength + memberTextLength;
        var itemPosition = position - sumLength;
        if (newTextLength > position)
        {
          if (itemPosition == 0)
          {
            for (int j = i; j < members.Count; j++)
            {
              newRun.AppendChild(members[j]);
            }
            break;
          }
          if (itemPosition == memberText.Length)
          {
            for (int j = i + 1; j < members.Count; j++)
            {
              newRun.AppendChild(members[j]);
            }
            break;
          }
          else
          {
            if (member is DXW.Text text)
            {
              DXW.Text? newText = text.SplitAt(position - sumLength, options);
              if (newText != null)
              {
                newRun.AppendChild(newText);
              }
              for (int j = i + 1; j < members.Count; j++)
              {
                newRun.AppendChild(members[j]);
              }

            }
            else
              throw new NotImplementedException("Break in non-text item is not implemented"); // TODO: split other elements
          }
          break;
        }
        sumLength = newTextLength;
      }
    }
    if (newRun.IsEmpty())
      return null;
    return newRun;
  }

  /// <summary>
  /// Insert a child to the run at the specified index, which is the number of characters from the beginning of the run.
  /// </summary>
  /// <param name="run">Run element to process</param>
  /// <param name="index">Char position number</param>
  /// <param name="child">New child member</param>
  /// <param name="options">Options for text extraction</param>
  /// <returns>Next, newly created run (or null if split is not available)</returns>
  public static void InsertAt(this DXW.Run run, int index, DX.OpenXmlElement child, TextOptions options)
  {
    if (index <= 0 || index >= run.GetText(options).Length)
      return;

    var textLength = 0;
    DXW.Run? newRun = null;
    foreach (var member in run.GetMembers().ToList())
    {
      var memberText = member.GetText(options);
      if (memberText != string.Empty)
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
                text.InsertAfterSelf(child);
                child.InsertAfterSelf(newMember);
                return;
              }
            }
          }
          var nextSibling = member.NextSibling();
          while (nextSibling != null)
          {
            newRun ??= NewRun(run);
            var nextSibling1 = nextSibling.NextSibling();
            nextSibling.Remove();
            newRun.AppendChild(nextSibling);
            nextSibling = nextSibling1;
          }
          break;
        }
        else
        {
          textLength = newTextLength;
        }
      }
    }
  }


  /// <summary>
  /// Get the position of the paragraph-level element in the paragraph text
  /// </summary>
  /// <param name="run">Run element to process</param>
  /// <param name="element">member element to search</param>
  /// <param name="options">Options for text extraction</param>
  /// <returns>Char position of the element (or -1 if not found)</returns>
  public static int GetTextPosOfElement(this DXW.Run run, DX.OpenXmlElement element, TextOptions options)
  {
    var textLength = 0;
    foreach (var member in run.GetMembers())
    {
      var memberText = member.GetText(options);
      if (member == element)
        return textLength;
      textLength += memberText.Length;
    }
    return -1;
  }

  /// <summary>
  /// Get the run-level element at the given position in the run text.
  /// </summary>
  /// <param name="run">Run element to process</param>
  /// <param name="pos">position of element to search</param>
  /// <param name="options">Options for text extraction</param>
  /// <returns>Paragraph-level or run-level element found at position (or null if not found)</returns>
  public static DX.OpenXmlElement? GetElementAtTextPos(this DXW.Run run, int pos, TextOptions options)
  {
    var textLength = 0;
    foreach (var member in run.GetMembers())
    {
      var memberText = member.GetText(options);
      if (pos >= textLength && pos < textLength + memberText.Length)
      {
        return member;
      }
      textLength += memberText.Length;
    }
    return null;
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


  /// <summary>
  /// Split the run before the given member element which can be a run-level element.
  /// Returns the second part of the run.
  /// </summary>
  /// <param name="run">Paragraph element to process</param>
  /// <param name="member">Paragraph-level or run-level member element</param>
  /// <returns>Next, newly created run (or null if split is not available)</returns>
  public static DXW.Run? SplitBefore(this DXW.Run run, DX.OpenXmlElement member)
  {
    DXW.Run? newRun = null;
    foreach (var item in run.GetMembers())
    {
      if (item == member)
      {
        newRun ??= NewRun(run);
        item.Remove();
        newRun.AppendChild(item);
        var nextSibling = item.NextSibling();
        while (nextSibling != null)
        {
          var nextSibling1 = nextSibling.NextSibling();
          nextSibling.Remove();
          newRun.AppendChild(nextSibling);
          nextSibling = nextSibling1;
        }
        break;
      }
    }
    return newRun;
  }

  /// <summary>
  /// Split the run after the given member element which can be a run-level element.
  /// Returns the second part of the run.
  /// </summary>
  /// <param name="run">Paragraph element to process</param>
  /// <param name="member">Paragraph-level or run-level member element</param>
  /// <returns>Next, newly created run (or null if split is not available)</returns>
  public static DXW.Run? SplitAfter(this DXW.Run run, DX.OpenXmlElement member)
  {
    var nextMember = member.NextSibling();
    if (nextMember == null)
      return null;
    return run.SplitBefore(nextMember);
  }

  /// <summary>
  /// Get the text format of the run.
  /// </summary>
  /// <param name="run"></param>
  /// <returns></returns>
  public static TextFormat GetFormat(this DXW.Run run)
  {
    return new TextFormat()
    {
      Bold = run.IsBold(),
      Italic = run.IsItalic()
    };
  }

  /// <summary>
  /// Set the text format to the run.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="format"></param>
  /// <returns></returns>
  public static void SetFormat(this DXW.Run run, TextFormat format)
  {
    if (format.Bold.HasValue)
      run.SetBold(format.Bold);
    if (format.Italic.HasValue)
      run.SetItalic(format.Bold);
  }
}