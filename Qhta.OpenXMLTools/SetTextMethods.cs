using System.Text;
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
    //using (var parser = new GotTextParser(element, options))
    //{
    //  return parser.ParseText(text);
    //}
    return false;
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
    foreach (var member in members )
      member.Remove();
    var sb = new StringBuilder();
    for (int i = 0; i < text.Length; i++)
    {
      var c = text[i];
      if (c == TextOptions.PlainText.TabChar[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.TabChar());
      }
      else if (c == TextOptions.PlainText.CarriageReturnTag[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.CarriageReturn());
      }
      else if (c == TextOptions.PlainText.SoftHyphen[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.SoftHyphen());
      }
      else if (c == TextOptions.PlainText.NoBreakHyphenTag[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.NoBreakHyphen());
      }
      else if(c == TextOptions.PlainText.BreakLineTag[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.Break { Type = new DX.EnumValue<DXW.BreakValues>(DXW.BreakValues.TextWrapping) });
      }
      else if (c == TextOptions.PlainText.BreakColumnTag[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.Break { Type = new DX.EnumValue<DXW.BreakValues>(DXW.BreakValues.Column) });
      }
      else if (c == TextOptions.PlainText.BreakPageTag[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.Break { Type = new DX.EnumValue<DXW.BreakValues>(DXW.BreakValues.Page) });
      }
      else if (c == TextOptions.PlainText.AnnotationReferenceMark[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.AnnotationReferenceMark());
      }
      else if (c == TextOptions.PlainText.LastRenderedPageBreak[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.LastRenderedPageBreak());
      }
      else if (c == TextOptions.PlainText.ContinuationSeparatorMark[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.ContinuationSeparatorMark());
      }
      else if (c == TextOptions.PlainText.SeparatorMark[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.SeparatorMark());
      }
      else if (c == TextOptions.PlainText.EndnoteReferenceMark[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.EndnoteReferenceMark());
      }
      else if (c == TextOptions.PlainText.FootnoteReferenceMark[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.FootnoteReferenceMark());
      }
      else if (c == TextOptions.PlainText.PageNumber[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.PageNumber());
      }
      else if (c == TextOptions.PlainText.DayLong[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.DayLong());
      }
      else if (c == TextOptions.PlainText.DayShort[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.DayShort());
      }
      else if (c == TextOptions.PlainText.MonthLong[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.MonthLong());
      }
      else if (c == TextOptions.PlainText.MonthShort[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.MonthShort());
      }
      else if (c == TextOptions.PlainText.YearLong[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.YearLong());
      }
      else if (c == TextOptions.PlainText.YearShort[0])
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.YearShort());
      }
      else
      {
        sb.Append(c);
      }
    }
    run.TryAddCollectedText(sb);
    return true;
  }

  private static bool TryAddCollectedText(this DXW.Run run, StringBuilder sb)
  {
    if (sb.Length > 0)
    {
      run.AppendText(sb.ToString());
      sb.Clear();
      return true;
    }
    return false;
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
        //  return text == options.SoftHyphen;
        //case var type when type == typeof(NoBreakHyphen):
        //  return text == options.NoBreakHyphenTag;
        //case var type when type == typeof(LastRenderedPageBreak):
        //  return text == options.LastRenderedPageBreak;
        //case var type when type == typeof(PageNumber):
        //  return text == options.PageNumber;
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