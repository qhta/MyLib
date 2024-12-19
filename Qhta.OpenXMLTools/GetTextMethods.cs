using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Methods to get text from OpenXml elements.
/// </summary>
public static class GetTextMethods
{

  /// <summary>
  /// Gets the text of the collection of elements
  /// </summary>
  /// <param name="elements"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this IEnumerable<DX.OpenXmlElement> elements, TextOptions options)
  {
    var sb = new StringBuilder();
    foreach (var element in elements)
    {
      var text = DispatchGetText(element, options with { OuterText = true });
      if (text != null)
        sb.Append(text);
      else
      {
        if (options.IncludeOtherMembers)
          sb.Append(element.GetText(options));
        else
          sb.Append(options.OtherObjectSubstituteTag);
      }
    }
    return sb.ToString();
  }

  /// <summary>
  /// Gets the text of the element.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DX.OpenXmlElement element, TextOptions options)
  {
    var text = DispatchGetText(element, options);
    if (text != null)
      return text;
    if (element is DX.OpenXmlCompositeElement compositeElement && !options.IgnoreOtherMembersContent)
    {
      var sb = new StringBuilder();
      var memberTag = element.GetType().Name.ToLowerFirst();
      var members1 = compositeElement.GetMembers().ToList();
      if (members1.Any())
      {
        sb.Append($"<{memberTag}>");
        foreach (var item in members1)
        {
          sb.Append(item.GetText(options));
        }
        sb.Append($"</{memberTag}>");
        return sb.ToString();
      }
      else
        return $"<{memberTag}/>";
    }
    else
    if (options.IncludeOtherMembers)
    {
      var memberTag = element.GetType().Name.ToLowerFirst();
      return $"<{memberTag}/>";
    }
    return "";
  }

  /// <summary>
  /// Dispatch the get text method for the element.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string? DispatchGetText(DX.OpenXmlElement element, TextOptions options)
  {
    switch (element.GetType())
    {
      case var type when type == typeof(DXW.Text):
        return (element as DXW.Text)!.GetTextOf(options);
      case var type when type == typeof(TabChar):
        return options.TabChar;
      case var type when type == typeof(DXW.Run):
        return (element as DXW.Run)!.GetTextOf(options);
      case var type when type == typeof(DXW.Paragraph):
        return (element as DXW.Paragraph)!.GetTextOf(options);
      case var type when type == typeof(DXW.Hyperlink):
        return (element as DXW.Hyperlink)!.GetTextOf(options);
      case var type when type == typeof(DXW.Table):
        return (element as DXW.Table)!.GetTextOf(options);
      case var type when type == typeof(DXW.TableRow):
        return (element as DXW.TableRow)!.GetTextOf(options);
      case var type when type == typeof(DXW.TableCell):
        return (element as DXW.TableCell)!.GetTextOf(options);
      case var type when type == typeof(DXW.Break):
        return (element as DXW.Break)!.GetTextOf(options);
      case var type when type == typeof(CarriageReturn):
        return options.CarriageReturnTag;
      case var type when type == typeof(SoftHyphen):
        return options.SoftHyphen;
      case var type when type == typeof(NoBreakHyphen):
        return options.NoBreakHyphenTag;
      case var type when type == typeof(LastRenderedPageBreak):
        return options.LastRenderedPageBreak;
      case var type when type == typeof(PageNumber):
        return options.PageNumber;
      case var type when type == typeof(FieldCode):
        return (element as FieldCode)!.GetTextOf(options);
      case var type when type == typeof(SymbolChar):
        return (element as SymbolChar)!.GetTextOf(options);
      case var type when type == typeof(PositionalTab):
        return options.TabChar;
      case var type when type == typeof(FieldChar):
        return (element as FieldChar)!.GetTextOf(options);
      case var type when type == typeof(Ruby):
        return (element as Ruby)!.GetTextOf(options);
      case var type when type == typeof(FootnoteReference):
        return (element as FootnoteReference)!.GetTextOf(options);
      case var type when type == typeof(EndnoteReference):
        return (element as EndnoteReference)!.GetTextOf(options);
      case var type when type == typeof(CommentReference):
        return (element as CommentReference)!.GetTextOf(options);
      case var type when type == typeof(FootnoteReferenceMark):
        return (element as FootnoteReferenceMark)!.GetTextOf(options);
      case var type when type == typeof(EndnoteReferenceMark):
        return (element as EndnoteReferenceMark)!.GetTextOf(options);
      case var type when type == typeof(AnnotationReferenceMark):
        return (element as AnnotationReferenceMark)!.GetTextOf(options);
      case var type when type == typeof(SeparatorMark):
        return (element as SeparatorMark)!.GetTextOf(options);
      case var type when type == typeof(ContinuationSeparatorMark):
        return (element as ContinuationSeparatorMark)!.GetTextOf(options);
      case var type when type == typeof(DayLong):
        return (element as DayLong)!.GetTextOf(options);
      case var type when type == typeof(DayShort):
        return (element as DayShort)!.GetTextOf(options);
      case var type when type == typeof(MonthLong):
        return (element as MonthLong)!.GetTextOf(options);
      case var type when type == typeof(MonthShort):
        return (element as MonthShort)!.GetTextOf(options);
      case var type when type == typeof(YearLong):
        return (element as YearLong)!.GetTextOf(options);
      case var type when type == typeof(YearShort):
        return (element as YearShort)!.GetTextOf(options);
      case var type when type == typeof(DXW.Drawing):
        return (element as DXW.Drawing)!.GetTextOf(options);
      case var type when type == typeof(DXD.Blip):
        return (element as DXD.Blip)!.GetTextOf(options);
      case var type when type == typeof(DXW.EmbeddedObject):
        return (element as DXW.EmbeddedObject)!.GetTextOf(options);
      case var type when type == typeof(DXW.ContentPart):
        return (element as DXW.ContentPart)!.GetTextOf(options);
      case var type when type == typeof(DXW.DeletedText):
        return (element as DXW.DeletedText)!.GetTextOf(options);
      case var type when type == typeof(DXW.DeletedFieldCode):
        return (element as DXW.DeletedFieldCode)!.GetTextOf(options);
    }
    //if (element is DX.OpenXmlLeafTextElement textElement)
    //  return textElement.GetTextOf(options);
    return null;
  }


  /// <summary>
  /// Get the text content of the run.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.Run run, TextOptions options)
  {
    StringBuilder sb = new();
    if (options.UseHtmlFormatting)
    {
      if (run.RunProperties?.GetBold(false) == true)
        sb.Append(options.BoldStartTag);
      if (run.RunProperties?.GetItalic(false) == true)
        sb.Append(options.ItalicStartTag);
      if (run.RunProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Superscript)
        sb.Append(options.SuperscriptStartTag);
      if (run.RunProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Subscript)
        sb.Append(options.SubscriptStartTag);
    }
    var members = run.GetMembers();
    foreach (var member in members)
    {
      if (member is DXW.Text text)
        sb.Append(text.GetTextOf(options));
      else
        sb.Append(member.GetText(options));
    }
    if (options.UseHtmlFormatting)
    {
      if (run.RunProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Subscript)
        sb.Append(options.SubscriptEndTag);
      if (run.RunProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Superscript)
        sb.Append(options.SuperscriptEndTag);
      if (run.RunProperties?.GetItalic(false) == true)
        sb.Append(options.ItalicEndTag);
      if (run.RunProperties?.GetBold(false) == true)
        sb.Append(options.BoldEndTag);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Get the run-in text content of the run.
  /// These run members, which can be converted to the plain text, are concatenated to single Text element.
  /// Others are contained as themselves.
  /// </summary>
  /// <param name="run"></param>
  /// <returns></returns>
  public static RunInText GetRunInText(this DXW.Run run)
  {
    RunInText result = new();

    var members = run.GetMembers();
    foreach (var member in members)
    {
      if (member is DXW.Text text)
        result.Append(text.Text);
      else if (member is DXW.TabChar)
        result.Append(TextOptions.PlainText.TabChar);
      else if (member is DXW.CarriageReturn)
        result.Append(TextOptions.PlainText.CarriageReturnTag);
      else if (member is DXW.SoftHyphen)
        result.Append(TextOptions.PlainText.SoftHyphen);
      else if (member is DXW.NoBreakHyphen)
        result.Append(TextOptions.PlainText.NoBreakHyphenTag);
      else
      if (member is DXW.Break breakElement)
      {
        if (breakElement.Type?.Value == BreakValues.Page)
          result.Append(TextOptions.PlainText.BreakPageTag);
        else if (breakElement.Type?.Value == BreakValues.Column)
          result.Append(TextOptions.PlainText.BreakColumnTag);
        else if (breakElement.Type?.Value == BreakValues.TextWrapping)
          result.Append(TextOptions.PlainText.BreakLineTag);
        else
          result.Add(member);
      }
      else if (member is DXW.FootnoteReferenceMark)
        result.Append(TextOptions.PlainText.FootnoteReferenceMark);
      else if (member is DXW.EndnoteReferenceMark)
        result.Append(TextOptions.PlainText.EndnoteReferenceMark);
      else if (member is DXW.SeparatorMark)
        result.Append(TextOptions.PlainText.SeparatorMark);
      else if (member is DXW.ContinuationSeparatorMark)
        result.Append(TextOptions.PlainText.ContinuationSeparatorMark);
      else if (member is DXW.AnnotationReferenceMark)
        result.Append(TextOptions.PlainText.AnnotationReferenceMark);
      else 
        result.Add(member);
    }
    return result;
  }

  /// <summary>
  /// Get the text of the run SearchText element.
  /// </summary>
  /// <param name="runText"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetText(this DXW.Text runText, TextOptions options)
  => runText.GetTextOf(options);

  /// <summary>
  /// Get the text of the run SearchText element.
  /// </summary>
  /// <param name="runText"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.Text runText, TextOptions options)
  {
    if (options.UseHtmlEntities)
      return runText.Text.HtmlEncode();
    else
      return runText.Text;
  }

  /// <summary>
  /// Get the element of the DeletedText element.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.DeletedText element, TextOptions options)
  {
    string text = element.Text;
    if (options.UseHtmlEntities)
      text = text.HtmlEncode();
    return options.DeletedTextStartTag + text + options.DeletedTextEndTag;
  }

  ///// <summary>
  ///// Set the text of the DeletedText element.
  ///// </summary>
  ///// <param name="element"></param>
  ///// <param name="text"></param>
  ///// <param name="options"></param>
  //private static bool SetTextOf(this DXW.DeletedText element, string text, TextOptions options)
  //{
  //  var l = options.DeletedTextStartTag.Length;
  //  var k = options.DeletedTextEndTag.Length;
  //  if (k > 0)
  //    k = text.IndexOf(options.DeletedTextEndTag, l);
  //  else
  //    k = l;
  //  if (k >= l)
  //  {
  //    text = text.Substring(l, k - l);
  //    if (options.UseHtmlEntities)
  //      text = text.HtmlDecode();
  //    element.SearchText = text;
  //    return true;
  //  }
  //  return false;
  //}

  /// <summary>
  /// Get the element of the deleted field code element.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.DeletedFieldCode element, TextOptions options)
  {
    return options.DeletedTextStartTag + element.Text + options.DeletedTextEndTag;
  }

  /// <summary>
  /// Get the text of the Break element.
  /// </summary>
  /// <param name="break"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.Break @break, TextOptions options)
  {
    if (@break.Type?.Value == BreakValues.Page)
      return options.BreakPageTag;
    else if (@break.Type?.Value == BreakValues.Column)
      return options.BreakColumnTag;
    else if (@break.Type?.Value == BreakValues.TextWrapping)
      return options.BreakLineTag;
    return String.Empty;
  }

  /// <summary>
  /// Get the text of the SymbolChar element.
  /// </summary>
  /// <param name="symbolChar"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.SymbolChar symbolChar, TextOptions options)
  {
    if (int.TryParse(symbolChar.Char!.Value, out var symbolVal))
    {
      return new String((char)symbolVal, 1);
    }
    return String.Empty;
  }

  /// <summary>
  /// Get the text of the hyperlink run elements.
  /// </summary>
  /// <param name="hyperlink">source hyperlink</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  public static string GetTextOf(this DXW.Hyperlink hyperlink, TextOptions? options)
  {
    if (options == null)
      options = TextOptions.PlainText;
    var sb = new StringBuilder();
    foreach (var item in hyperlink.Elements())
    {
      var text = item.GetText(options);
      sb.Append(text);
    }
    var result = sb.ToString();
    return result;
  }

  /// <summary>
  /// Get the text of the FieldChar element.
  /// </summary>
  /// <param name="fieldChar"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.FieldChar fieldChar, TextOptions options)
  {
    string tag = String.Empty;
    if (fieldChar.FieldCharType?.Value == FieldCharValues.Begin && options.IncludeFieldFormula)
    {
      tag = options.FieldStartTag;
    }
    else if (fieldChar.FieldCharType?.Value == FieldCharValues.Separate && options.IncludeFieldFormula)
    {
      tag = options.FieldResultTag;
    }
    else if (fieldChar.FieldCharType?.Value == FieldCharValues.End && options.IncludeFieldFormula)
    {
      tag = options.FieldEndTag;
    }
    return tag;
  }

  ///// <summary>
  ///// Set the text of the FieldChar element.
  ///// </summary>
  ///// <param name="fieldChar"></param>
  ///// <param name="text">text to check</param>
  ///// <param name="options"></param>
  ///// <returns></returns>
  //private static bool SetTextOf(this DXW.FieldChar fieldChar, string text, TextOptions options)
  //{
  //  if (text == options.FieldStartTag)
  //  {
  //    fieldChar.FieldCharType = new DX.EnumValue<DXW.FieldCharValues>(FieldCharValues.Begin);
  //    return true;
  //  }
  //  if (text == options.FieldResultTag)
  //  {
  //    fieldChar.FieldCharType = new DX.EnumValue<DXW.FieldCharValues>(FieldCharValues.Separate);
  //    return true;
  //  }
  //  if (text == options.FieldEndTag)
  //  {
  //    fieldChar.FieldCharType = new DX.EnumValue<DXW.FieldCharValues>(FieldCharValues.End);
  //    return true;
  //  }
  //  return false;
  //}

  /// <summary>
  /// Get the text of the FieldCode element.
  /// </summary>
  /// <param name="fieldCode"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.FieldCode fieldCode, TextOptions options)
  {
    if (options.IncludeFieldFormula) return options.FieldCodeStart + fieldCode.Text + options.FieldCodeEnd;
    return string.Empty;
  }

  /// <summary>
  /// Get the text of the DayLong element.
  /// </summary>
  /// <param name="dayLong"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.DayLong dayLong, TextOptions options)
  {
    if (options.IncludeFieldFormula)
      return options.FieldStartTag + options.FieldCodeStart + "DayLong" + options.FieldCodeEnd +
             options.FieldResultTag + GetSystemLongDay(dayLong.GetMainDocumentPart()!) + options.FieldEndTag;
    return GetSystemLongDay(dayLong.GetMainDocumentPart()!);
  }

  private static string GetSystemLongDay(DXPack.MainDocumentPart mainDocumentPart)
  {
    CultureInfo culture = CultureInfo.CurrentCulture;
    var languages = mainDocumentPart.StyleDefinitionsPart?.Styles?.Descendants<DXW.Languages>().FirstOrDefault();
    if (languages != null)
    {
      var lang = languages.Val;
      if (lang != null)
      {
        culture = new System.Globalization.CultureInfo(lang!);
      }
    }
    return DateTime.Now.Date.ToString("DDDD", culture);
  }

  /// <summary>
  /// Get the text of the DayShort element.
  /// </summary>
  /// <param name="dayShort"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.DayShort dayShort, TextOptions options)
  {
    if (options.IncludeFieldFormula)
      return options.FieldStartTag + options.FieldCodeStart + "DayShort" + options.FieldCodeEnd +
             options.FieldResultTag + GetSystemShortDay(dayShort.GetMainDocumentPart()!) + options.FieldEndTag;
    return GetSystemShortDay(dayShort.GetMainDocumentPart()!);
  }

  private static string GetSystemShortDay(DXPack.MainDocumentPart mainDocumentPart)
  {
    CultureInfo culture = CultureInfo.CurrentCulture;
    var languages = mainDocumentPart.StyleDefinitionsPart?.Styles?.Descendants<DXW.Languages>().FirstOrDefault();
    if (languages != null)
    {
      var lang = languages.Val;
      if (lang != null)
      {
        culture = new System.Globalization.CultureInfo(lang!);
      }
    }
    return DateTime.Now.Date.ToString("DD", culture);
  }

  /// <summary>
  /// Get the text of the MonthLong element.
  /// </summary>
  /// <param name="MonthLong"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.MonthLong MonthLong, TextOptions options)
  {
    if (options.IncludeFieldFormula)
      return options.FieldStartTag + options.FieldCodeStart + "MonthLong" + options.FieldCodeEnd +
             options.FieldResultTag + GetSystemLongMonth(MonthLong.GetMainDocumentPart()!) + options.FieldEndTag;
    return GetSystemLongMonth(MonthLong.GetMainDocumentPart()!);
  }

  private static string GetSystemLongMonth(DXPack.MainDocumentPart mainDocumentPart)
  {
    CultureInfo culture = CultureInfo.CurrentCulture;
    var languages = mainDocumentPart.StyleDefinitionsPart?.Styles?.Descendants<DXW.Languages>().FirstOrDefault();
    if (languages != null)
    {
      var lang = languages.Val;
      if (lang != null)
      {
        culture = new System.Globalization.CultureInfo(lang!);
      }
    }
    return DateTime.Now.Date.ToString("MMMM", culture);
  }

  /// <summary>
  /// Get the text of the MonthShort element.
  /// </summary>
  /// <param name="MonthShort"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.MonthShort MonthShort, TextOptions options)
  {
    if (options.IncludeFieldFormula)
      return options.FieldStartTag + options.FieldCodeStart + "MonthShort" + options.FieldCodeEnd +
             options.FieldResultTag + GetSystemShortMonth(MonthShort.GetMainDocumentPart()!) + options.FieldEndTag;
    return GetSystemShortMonth(MonthShort.GetMainDocumentPart()!);
  }

  private static string GetSystemShortMonth(DXPack.MainDocumentPart mainDocumentPart)
  {
    CultureInfo culture = CultureInfo.CurrentCulture;
    var languages = mainDocumentPart.StyleDefinitionsPart?.Styles?.Descendants<DXW.Languages>().FirstOrDefault();
    if (languages != null)
    {
      var lang = languages.Val;
      if (lang != null)
      {
        culture = new System.Globalization.CultureInfo(lang!);
      }
    }
    return DateTime.Now.Date.ToString("MM", culture);
  }

  /// <summary>
  /// Get the text of the YearLong element.
  /// </summary>
  /// <param name="YearLong"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.YearLong YearLong, TextOptions options)
  {
    if (options.IncludeFieldFormula)
      return options.FieldStartTag + options.FieldCodeStart + "YearLong" + options.FieldCodeEnd +
             options.FieldResultTag + GetSystemLongYear(YearLong.GetMainDocumentPart()!) + options.FieldEndTag;
    return GetSystemLongYear(YearLong.GetMainDocumentPart()!);
  }

  private static string GetSystemLongYear(DXPack.MainDocumentPart mainDocumentPart)
  {
    CultureInfo culture = CultureInfo.CurrentCulture;
    var languages = mainDocumentPart.StyleDefinitionsPart?.Styles?.Descendants<DXW.Languages>().FirstOrDefault();
    if (languages != null)
    {
      var lang = languages.Val;
      if (lang != null)
      {
        culture = new System.Globalization.CultureInfo(lang!);
      }
    }
    return DateTime.Now.Date.ToString("YYYY", culture);
  }

  /// <summary>
  /// Get the text of the YearShort element.
  /// </summary>
  /// <param name="YearShort"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.YearShort YearShort, TextOptions options)
  {
    if (options.IncludeFieldFormula)
      return options.FieldStartTag + options.FieldCodeStart + "YearShort" + options.FieldCodeEnd +
             options.FieldResultTag + GetSystemShortYear(YearShort.GetMainDocumentPart()!) + options.FieldEndTag;
    return GetSystemShortYear(YearShort.GetMainDocumentPart()!);
  }

  private static string GetSystemShortYear(DXPack.MainDocumentPart mainDocumentPart)
  {
    CultureInfo culture = CultureInfo.CurrentCulture;
    var languages = mainDocumentPart.StyleDefinitionsPart?.Styles?.Descendants<DXW.Languages>().FirstOrDefault();
    if (languages != null)
    {
      var lang = languages.Val;
      if (lang != null)
      {
        culture = new System.Globalization.CultureInfo(lang!);
      }
    }
    return DateTime.Now.Date.ToString("YY", culture);
  }

  /// <summary>
  /// Get the text of the FootnoteReference element.
  /// </summary>
  /// <param name="footnoteReference"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.FootnoteReference footnoteReference, TextOptions options)
  {
    return options.FootnoteRefStart + footnoteReference.Id + options.FootnoteRefEnd;
  }

  /// <summary>
  /// Get the text of the EndnoteReference element.
  /// </summary>
  /// <param name="endnoteReference"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.EndnoteReference endnoteReference, TextOptions options)
  {
    return options.EndnoteRefStart + endnoteReference.Id + options.EndnoteRefEnd;
  }

  /// <summary>
  /// Get the text of the CommentReference element.
  /// </summary>
  /// <param name="commentReference"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.CommentReference commentReference, TextOptions options)
  {
    return options.CommentRefStart + commentReference.Id + options.CommentRefEnd;
  }

  /// <summary>
  /// Get the text of the FootnoteReferenceMark element.
  /// </summary>
  /// <param name="footnoteReferenceMark"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.FootnoteReferenceMark footnoteReferenceMark, TextOptions options)
  {
    return options.FootnoteReferenceMark;
  }

  /// <summary>
  /// Get the text of the EndnoteReferenceMark element.
  /// </summary>
  /// <param name="endnoteReferenceMark"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.EndnoteReferenceMark endnoteReferenceMark, TextOptions options)
  {
    return options.EndnoteReferenceMark;
  }

  /// <summary>
  /// Get the text of the annotationReferenceMark element.
  /// </summary>
  /// <param name="annotationReferenceMark"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.AnnotationReferenceMark annotationReferenceMark, TextOptions options)
  {
    return options.AnnotationReferenceMark;
  }

  /// <summary>
  /// Get the text of the SeparatorMark element.
  /// </summary>
  /// <param name="SeparatorMark"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.SeparatorMark SeparatorMark, TextOptions options)
  {
    return options.SeparatorMark;
  }

  /// <summary>
  /// Get the text of the continuationSeparatorMark element.
  /// </summary>
  /// <param name="continuationSeparatorMark"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.ContinuationSeparatorMark continuationSeparatorMark, TextOptions options)
  {
    return options.ContinuationSeparatorMark;
  }

  /// <summary>
  /// Get the text of the paragraph using ParaText options.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <returns>joined text</returns>
  public static string GetText(this Paragraph paragraph)
    => paragraph.GetTextOf(TextOptions.ParaText);

  /// <summary>
  /// Get the text of the paragraph.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  private static string GetTextOf(this Paragraph paragraph, TextOptions options)
  {

    var paraText = paragraph.GetInnerText(options);
    if (!options.OuterText)
      return paraText;
    var sb = new StringBuilder();
    if (options.UseHtmlParagraphs)
    {
      if (paraText == string.Empty)
        sb.Append(options.ParagraphSeparator);
      else
      {
        sb.Append(options.ParagraphStartTag);
        sb.Append(paraText);
        sb.Append(options.ParagraphEndTag);
      }
    }
    else
    {
      if (paraText == string.Empty)
      {
        if (!options.IgnoreEmptyParagraphs)
          sb.Append(options.ParagraphSeparator);
      }
      else
      {
        sb.Append(paraText);
        sb.Append(options.ParagraphSeparator);
      }
    }
    return sb.ToString();
  }

  /// <summary>
  /// Get the text of the paragraph members elements.
  /// </summary>
  /// <param name="paragraph">source paragraph</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  private static string GetInnerText(this Paragraph paragraph, TextOptions options)
  {
    var sl = new List<string>();
    var members = paragraph.GetMembers().ToList();
    foreach (var member in members)
    {
      var text = member.GetText(options);
      sl.Add(text);
    }
    var result = string.Join("", sl);
    if (options.IncludeParagraphNumbering)
      result = paragraph.GetNumberingString(options) + result;
    return result;
  }

  /// <summary>
  /// Gets the text table including table tags
  /// </summary>
  /// <param name="table"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.Table table, TextOptions options)
  {
    var sb = new StringBuilder();
    if (options.IgnoreTableContents)
    {
      if (options.UseHtmlTables)
      {
        sb.Append(options.TableSubstituteTag);
      }
    }
    else
    {
      if (options.UseHtmlTables)
      {
        sb.Append(options.TableStartTag);
        sb.Append(table.GetInnerText(options));
        if (options.UseHtmlTables)
          sb.Append(options.TableEndTag);
        else
          sb.Append(options.TableSeparator);
      }
      else
      {
        var tableText = table.GetInnerText(options);
        sb.Append(tableText);
      }
    }
    return sb.ToString();
  }
  /// <summary>
  /// Gets the text of all rows in the table.
  /// </summary>
  /// <param name="table"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static string GetInnerText(this DXW.Table table, TextOptions options)
  {
    var sb = new StringBuilder();
    var rows = table.GetRows().ToList();
    foreach (var row in rows)
    {
      if (options.UseHtmlTables)
        sb.Append(options.TableRowStartTag);
      sb.Append(row.GetTextOf(options));
      if (options.UseHtmlTables)
        sb.Append(options.TableRowEndTag);
      else
        sb.Append(options.TableRowSeparator);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Gets the text of all cells in the table row.
  /// </summary>
  /// <param name="row"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this TableRow row, TextOptions options)
  {
    List<string> sl = new();
    var cells = row.GetCells().ToList();
    for (var i = 0; i < cells.Count; i++)
    {
      var cell = cells[i];
      if (options.UseHtmlTables)
      {
        sl.Add(options.TableCellStartTag);
        if (!cell.HasSimpleContent())
        {
          var aText = cell.GetInnerText(options);
          sl.Add(aText);
        }
        else
        {
          var aText = cell.GetInnerText(options);
          sl.Add(aText);
        }
      }
      else
      {
        var aText = cell.GetInnerText(options);
        sl.Add(aText);
      }
      if (options.UseHtmlTables)
        sl.Add(options.TableCellEndTag);
      else if (i < cells.Count - 1)
        sl.Add(options.TableCellSeparator);
    }
    return string.Join("", sl);
  }

  private static string GetTextOf(this TableCell cell, TextOptions options)
  {
    var text = cell.GetInnerText(options);
    return text;
  }

  /// <summary>
  /// Gets inner text the table cell.
  /// </summary>
  /// <param name="cell"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetInnerText(this TableCell cell, TextOptions options)
  {
    var members = cell.GetMembers().ToList();
    if (members.Any())
    {
      if (members.Count() == 1 && members[0] is DXW.Paragraph singleParagraph)
      {
        return singleParagraph.GetInnerText(options);
      }
      else
      {
        return (cell.GetMembers()).GetText(options);
      }
    }
    return string.Empty;
  }

  /// <summary>
  /// Get the text content of the ruby element
  /// </summary>
  /// <param name="ruby"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.Ruby ruby, TextOptions options)
  {
    StringBuilder sb = new();
    if (options.UseHtmlFormatting && options.UseHtmlEntities)
    {
      //if (ruby.RubyProperties?.GetBold(false) == true)
      //  sb.Append(options.BoldStartTag);
      //if (ruby.RubyProperties?.GetItalic(false) == true)
      //  sb.Append(options.ItalicStartTag);
      //if (ruby.RubyProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Superscript)
      //  sb.Append(options.SuperscriptStartTag);
      //if (ruby.RubyProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Subscript)
      //  sb.Append(options.SubscriptStartTag);
    }
    var members = ruby.GetMembers();
    foreach (var member in members)
    {
      if (member is DXW.Text text)
        sb.Append(text.GetTextOf(options));
      else
        sb.Append(member.GetText(options));
    }
    if (options.UseHtmlFormatting && options.UseHtmlEntities)
    {
      //if (ruby.RubyProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Subscript)
      //  sb.Append(options.SubscriptEndTag);
      //if (ruby.RubyProperties?.GetVerticalPosition() == DXW.VerticalPositionValues.Superscript)
      //  sb.Append(options.SuperscriptEndTag);
      //if (ruby.RubyProperties?.GetItalic(false) == true)
      //  sb.Append(options.ItalicEndTag);
      //if (ruby.RubyProperties?.GetBold(false) == true)
      //  sb.Append(options.BoldEndTag);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Get the text for the embeddedObject element.
  /// </summary>
  /// <param name="embeddedObject"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.EmbeddedObject embeddedObject, TextOptions options)
  {
    if (options.IncludeEmbeddedObjects)
    {
      if (options.IgnoreEmbeddedObjectContent)
      {
        return options.EmbeddedObjectSubstituteTag;
      }
      else
      {
        return options.EmbeddedObjectStartTag + (embeddedObject.GetMembers()).GetText(options) + options.EmbeddedObjectEndTag;
      }
    }
    return string.Empty;
  }

  /// <summary>
  /// Get the text for the contentPart element.
  /// </summary>
  /// <param name="contentPart"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.ContentPart contentPart, TextOptions options)
  {
    if (options.IncludeOtherMembers)
    {
      if (options.IgnoreOtherMembersContent)
      {
        return options.OtherObjectSubstituteTag;
      }
      else
      {
        return contentPart.OuterXml;
      }
    }
    return string.Empty;
  }

  /// <summary>
  /// Get the text for the drawing element.
  /// </summary>
  /// <param name="drawing"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXW.Drawing drawing, TextOptions options)
  {
    if (options.IncludeDrawings)
    {
      if (options.IgnoreDrawingContents)
      {
        return options.DrawingSubstituteTag;
      }
      else
      {
        return options.DrawingStartTag + (drawing.GetMembers()).GetText(options) + options.DrawingEndTag;
      }
    }
    return string.Empty;
  }

  /// <summary>
  /// Get the text for the Blip element.
  /// </summary>
  /// <param name="blip"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static string GetTextOf(this DXD.Blip blip, TextOptions options)
  {
    var str = options.BlipTag;
    var embed = blip.Embed?.Value;
    if (embed != null)
      str = str.Insert(str.Length - 2, $" embed=\"{embed}\"");
    var link = blip.Link?.Value;
    if (link != null)
      str = str.Insert(str.Length - 2, $" link=\"{link}\"");
    return str;
  }
}