using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
  //public static RunProperties GetRunProperties (this Run run) { return run.Elements<RunProperties>().FirstOrDefault();} 
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
  public static RunProperties GetRunProperties(this Run run)
  {
    if (run.RunProperties == null)
      run.RunProperties = new RunProperties();
    return run.RunProperties;
  }

  /// <summary>
  /// Get the text content of the run.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this Run run, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    StringBuilder sb = new();
    foreach (var element in run.Elements())
    {
      if (element is Text text)
      {
        sb.Append(text.Text);
      }
      else if (element is Break @break)
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
  public static void SetText(this Run run, string? value, GetTextOptions? options = null)
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
        run.AppendChild(new Break() { Type = BreakValues.Page });
      }
      else if (value.HasSubstringAt(i, options.BreakColumnTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new Break() { Type = BreakValues.Column });
      }
      else if (value.HasSubstringAt(i, options.BreakLineTag))
      {
        TryAppend(run, sb);
        run.AppendChild(new Break() { Type = BreakValues.TextWrapping });
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

  private static void TryAppend(this Run run, StringBuilder sb)
  {
    var s = sb.ToString();
    if (!string.IsNullOrEmpty(s))
    {
      run.AppendChild(new Text(s));
      sb.Clear();
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
      return underline.Val?.Value != UnderlineValues.None;
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
  /// Checks if the run text is empty.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DXW.Text? element)
  {
    if (element == null)
      return true;
    return element.Text.Trim()=="";
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
}