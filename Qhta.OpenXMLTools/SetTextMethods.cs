using System.Xml;
using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;
/// <summary>
/// Methods to set text to OpenXml elements.
/// </summary>
public static class SetTextMethods
{
  /// <summary>
  /// Set text to an OpenXml element using text parser.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="text"></param>
  /// <param name="options"></param>
  public static bool SetText(this DX.OpenXmlElement element, string text, TextOptions? options = null)
  {
    if (options == null)
      options = TextOptions.PlainText;
    if (element is DXW.Run run)
    {
      run.SetTextTo(text, options);
      return true;
    }
    using (var parser = new GotTextParser(element, options))
    {
      return parser.ParseText(text);
    }
  }

  /// <summary>
  /// Set text to a run element.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="text"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static bool SetTextTo(this DXW.Run run, string text, TextOptions options)
  {
    if (options.UseHtmlEntities)
      text = text.HtmlDecode();
    var members = run.GetMembers().ToArray();
    foreach ( var member in members )
      member.Remove();
    run.AppendText(text);
    return true;
  }

  /// <summary>
  /// Set text to a run SearchText element to/before the specified member.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="member"></param>
  /// <param name="text"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static bool SetTextTo(this DXW.Run run, DX.OpenXmlElement? member, string text, TextOptions? options = null)
  {
    if (options == null)
      options = TextOptions.PlainText;
    if (options.UseHtmlEntities)
      text = text.HtmlDecode();
    if (member is DXW.Text runText)
    {
      runText.Text = text;
    }
    else if (member != null)
    {
      var newText = new DXW.Text(text);
      member.InsertBeforeSelf(newText);
    }
    else
    {
      run.AppendText(text);
    }
    return true;
  }

  /// <summary>
  /// Insert text to a run SearchText element at the specified position.
  /// </summary>
  /// <param name="runText"></param>
  /// <param name="text"></param>
  /// <param name="pos"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static bool InsertTextAt(this DXW.Text runText, int pos, string text, TextOptions? options = null)
  {
    if (options == null)
      options = TextOptions.PlainText;
    var aText = runText.GetText(options);
    if (pos < 0 || pos>aText.Length)
      return false;
    aText = aText.Insert(pos, text);
    runText.SetTextOf(aText, options);
    return true;
  }

  /// <summary>
  /// Set text to a run SearchText element at the specified position.
  /// Existing text before the position is kept intact.
  /// SearchText after the position is replaced with the new text.
  /// </summary>
  /// <param name="runText"></param>
  /// <param name="text"></param>
  /// <param name="pos"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public static bool SetTextAt(this DXW.Text runText, int pos, string text, TextOptions? options = null)
  {
    if (options == null)
      options = TextOptions.PlainText;
    var aText = runText.GetText(options);
    if (pos < 0 || pos > aText.Length)
      return false;
    aText = aText.Substring(0, pos) + text;
    runText.SetTextOf(aText, options);
    return true;
  }

  ///// <summary>
  ///// Sets the text of the element.
  ///// </summary>
  ///// <param name="element"></param>
  ///// <param name="text">SearchText to set </param>
  ///// <param name="options"></param>
  ///// <returns></returns>
  //public static void SetText(this DX.OpenXmlElement element, string text, TextOptions options)
  //{
  //  if (DispatchSetText(element, text, options))
  //    return;
  //  throw new InvalidOperationException($"Cant set text to {element.GetType()}");
  //}


  private static bool DispatchSetText(DX.OpenXmlElement element, string text, TextOptions options)
  {
    switch (element.GetType())
    {
      case var type when type == typeof(DXW.Text):
        return (element as DXW.Text)!.SetTextOf(text, options);
      //case var type when type == typeof(DXW.TabChar):
      //  return text == options.TabChar;
        //case var type when type == typeof(DXW.Run):
        //  return (element as DXW.Run)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.Paragraph):
        //  return (element as DXW.Paragraph)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.Hyperlink):
        //  return (element as DXW.Hyperlink)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.Table):
        //  return (element as DXW.Table)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.TableRow):
        //  return (element as DXW.TableRow)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.TableCell):
        //  return (element as DXW.TableCell)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.Break):
        //  return (element as DXW.Break)!.SetTextOf(text, options);
        //case var type when type == typeof(CarriageReturn):
        //  return text == options.CarriageReturnTag;
        //case var type when type == typeof(SoftHyphen):
        //  return text == options.SoftHyphenTag;
        //case var type when type == typeof(NoBreakHyphen):
        //  return text == options.NoBreakHyphenTag;
        //case var type when type == typeof(LastRenderedPageBreak):
        //  return text == options.LastRenderedPageBreakTag;
        //case var type when type == typeof(PageNumber):
        //  return text == options.PageNumberTag;
        //case var type when type == typeof(FieldCode):
        //  return (element as FieldCode)!.SetTextOf(text, options);
        //case var type when type == typeof(SymbolChar):
        //  return (element as SymbolChar)!.SetTextOf(text, options);
        //case var type when type == typeof(PositionalTab):
        //  return options.TabChar;
        //case var type when type == typeof(FieldChar):
        //  return (element as FieldChar)!.SetTextOf(text, options);
        //case var type when type == typeof(Ruby):
        //  return (element as Ruby)!.SetTextOf(text, options);
        //case var type when type == typeof(FootnoteReference):
        //  return (element as FootnoteReference)!.SetTextOf(text, options);
        //case var type when type == typeof(EndnoteReference):
        //  return (element as EndnoteReference)!.SetTextOf(text, options);
        //case var type when type == typeof(CommentReference):
        //  return (element as CommentReference)!.SetTextOf(text, options);
        //case var type when type == typeof(FootnoteReferenceMark):
        //  return (element as FootnoteReferenceMark)!.SetTextOf(text, options);
        //case var type when type == typeof(EndnoteReferenceMark):
        //  return (element as EndnoteReferenceMark)!.SetTextOf(text, options);
        //case var type when type == typeof(AnnotationReferenceMark):
        //  return (element as AnnotationReferenceMark)!.SetTextOf(text, options);
        //case var type when type == typeof(SeparatorMark):
        //  return (element as SeparatorMark)!.SetTextOf(text, options);
        //case var type when type == typeof(ContinuationSeparatorMark):
        //  return (element as ContinuationSeparatorMark)!.SetTextOf(text, options);
        //case var type when type == typeof(DayLong):
        //  return (element as DayLong)!.SetTextOf(text, options);
        //case var type when type == typeof(DayShort):
        //  return (element as DayShort)!.SetTextOf(text, options);
        //case var type when type == typeof(MonthLong):
        //  return (element as MonthLong)!.SetTextOf(text, options);
        //case var type when type == typeof(MonthShort):
        //  return (element as MonthShort)!.SetTextOf(text, options);
        //case var type when type == typeof(YearLong):
        //  return (element as YearLong)!.SetTextOf(text, options);
        //case var type when type == typeof(YearShort):
        //  return (element as YearShort)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.Drawing):
        //  return (element as DXW.Drawing)!.SetTextOf(text, options);
        //case var type when type == typeof(DXD.Blip):
        //  return (element as DXD.Blip)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.EmbeddedObject):
        //  return (element as DXW.EmbeddedObject)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.ContentPart):
        //  return (element as DXW.ContentPart)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.DeletedText):
        //  return (element as DXW.DeletedText)!.SetTextOf(text, options);
        //case var type when type == typeof(DXW.DeletedFieldCode):
        //  return (element as DXW.DeletedFieldCode)!.SetTextOf(text, options);
    }

    return false;
  }

  /// <summary>
  /// Set the text of the run SearchText element.
  /// </summary>
  /// <param name="runText"></param>
  /// <param name="text"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private static bool SetTextOf(this DXW.Text runText, string text, TextOptions options)
  {
    if (options.UseHtmlEntities)
      runText.Text = text.HtmlDecode();
    else
      runText.Text = text;
    return true;
  }
}