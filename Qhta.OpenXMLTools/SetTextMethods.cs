using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Xml;

using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;
using Qhta.TypeUtils;

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
    if (options.Mode == FormattedTextMode.XmlTagged)
      text = text.HtmlDecode();
    var members = run.GetMembers().ToArray();
    foreach (var member in members)
      member.Remove();
    var sb = new StringBuilder();
    for (int i = 0; i < text.Length; i++)
    {
      if (text.ContainsAt(options.TabTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.TabChar());
        i += options.TabTag.Length - 1;
      }
      else if (text.ContainsAt(options.CarriageReturnTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.CarriageReturn());
        i += options.CarriageReturnTag.Length - 1;
      }
      else if (text.ContainsAt(options.SoftHyphenTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.SoftHyphen());
        i += options.SoftHyphenTag.Length - 1;
      }
      else if (text.ContainsAt(options.NoBreakHyphenTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.NoBreakHyphen());
        i += options.NoBreakHyphenTag.Length - 1;
      }
      else if (text.ContainsAt(options.LastRenderedPageBreakTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.LastRenderedPageBreak());
        i += options.LastRenderedPageBreakTag.Length - 1;
      }
      else if (text.ContainsAt(options.PageNumberTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.PageNumber());
        i += options.PageNumberTag.Length - 1;
      }
      else if (text.ContainsAt(options.AnnotationReferenceMarkTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.AnnotationReferenceMark());
        i += options.AnnotationReferenceMarkTag.Length - 1;
      }
      else if (text.ContainsAt(options.FootnoteReferenceMarkTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.FootnoteReferenceMark());
        i += options.FootnoteReferenceMarkTag.Length - 1;
      }
      else if (text.ContainsAt(options.EndnoteReferenceMarkTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.EndnoteReferenceMark());
        i += options.EndnoteReferenceMarkTag.Length - 1;
      }
      else if (text.ContainsAt(options.SeparatorMarkTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.SeparatorMark());
        i += options.SeparatorMarkTag.Length - 1;
      }
      else if (text.ContainsAt(options.ContinuationSeparatorMarkTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.ContinuationSeparatorMark());
        i += options.ContinuationSeparatorMarkTag.Length - 1;
      }
      else if (text.ContainsAt(options.PageNumberTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.PageNumber());
        i += options.PageNumberTag.Length - 1;
      }
      else if (text.ContainsAt(options.DayLongTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.DayLong());
        i += options.DayLongTag.Length - 1;
      }
      else if (text.ContainsAt(options.DayShortTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.DayShort());
        i += options.DayShortTag.Length - 1;
      }
      else if (text.ContainsAt(options.MonthLongTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.MonthLong());
        i += options.MonthLongTag.Length - 1;
      }
      else if (text.ContainsAt(options.MonthShortTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.MonthShort());
        i += options.MonthShortTag.Length - 1;
      }
      else if (text.ContainsAt(options.YearLongTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.YearLong());
        i += options.YearLongTag.Length - 1;
      }
      else if (text.ContainsAt(options.YearShortTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.YearShort());
        i += options.YearShortTag.Length - 1;
      }
      else if (text.ContainsAt(options.FieldCharBeginTag, i))
      {
        run.TryAddCollectedText(sb);
        var fieldChar = new DXW.FieldChar { FieldCharType = new DX.EnumValue<DXW.FieldCharValues>(DXW.FieldCharValues.Begin) };
        if (i < text.Length)
        {
          i += options.FieldCharBeginTag.Length - 1;
          var ch = text[i + 1];
          if (ch == '{')
          {
            i++;
            var j = text.IndexOf('}', i);
            if (j > 0)
            {
              var s = text.Substring(i + 1, j - i - 1);
              fieldChar.ReadAttributes(s);
              i = j;
            }
          }
        }
        run.AppendChild(fieldChar);
      }
      else if (text.ContainsAt(options.FieldCharSeparateTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.FieldChar
        {
          FieldCharType = new DX.EnumValue<DXW.FieldCharValues>(DXW.FieldCharValues.Separate)
        });
        i += options.FieldCharSeparateTag.Length - 1;
      }
      else if (text.ContainsAt(options.FieldCharEndTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.FieldChar { FieldCharType = new DX.EnumValue<DXW.FieldCharValues>(DXW.FieldCharValues.End) });
        i += options.FieldCharEndTag.Length - 1;
      }
      else if (text.ContainsAt(options.BreakLineTag, i))
      {
        run.TryAddCollectedText(sb);
        var breakElement = new DXW.Break { Type = new DX.EnumValue<DXW.BreakValues>(DXW.BreakValues.TextWrapping) };
        if (i < text.Length)
        {
          i += options.BreakLineTag.Length - 1;
          var ch = text[i + 1];
          if (ch == '{')
          {
            i++;
            var j = text.IndexOf('}', i);
            if (j > 0)
            {
              var s = text.Substring(i + 1, j - i - 1);
              breakElement.ReadAttributes(s);
              i = j;
            }
          }
        }
        run.AppendChild(breakElement);
      }
      else if (text.ContainsAt(options.BreakColumnTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.Break { Type = new DX.EnumValue<DXW.BreakValues>(DXW.BreakValues.Column) });
        i += options.BreakColumnTag.Length - 1;
      }
      else if (text.ContainsAt(options.BreakPageTag, i))
      {
        run.TryAddCollectedText(sb);
        run.AppendChild(new DXW.Break { Type = new DX.EnumValue<DXW.BreakValues>(DXW.BreakValues.Page) });
        i += options.BreakPageTag.Length - 1;
      }
      else
      {
        sb.Append(text[i]);
      }
    }
    run.TryAddCollectedText(sb);
    return true;
  }

  private static void ReadAttributes(this DX.OpenXmlElement element, string text)
  {
    var ss = text.Split(';');
    foreach (var s in ss)
    {
      var kv = s.Split('=');
      if (kv.Length == 2)
      {
        var name = kv[0].Trim();
        var value = kv[1].Trim();
        element.SetAttribute(name, value);
      }
    }
  }

  private static void SetAttribute(this DX.OpenXmlElement element, string name, string value)
  {
    var property = element.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
    if (property == null)
      throw new InvalidOperationException($"Property {name} not found in {element.GetType()}");
    var type = property.PropertyType;
    if (type == typeof(string))
    {
      property.SetValue(element, value);
    }
    else if (type == typeof(int))
    {
      if (int.TryParse(value, out var n))
        property.SetValue(element, n);
    }
    else if (type == typeof(bool))
    {
      if (bool.TryParse(value, out var b))
        property.SetValue(element, b);
    }
    else if (type == typeof(DX.OnOffValue))
    {
      if (bool.TryParse(value, out var b))
        property.SetValue(element, new DX.OnOffValue(b));
    }
    else if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(DX.EnumValue<>))
    {

      var enumType = type.GetGenericArguments()[0];
      var enumValues = enumType.GetConstructor([typeof(string)])!.Invoke(new object[] { value });
      var enumValue = Activator.CreateInstance(type, new object[] { enumValues });
      property.SetValue(element, enumValue);
    }
    else
      throw new InvalidOperationException($"Property type {type} not supported");
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
    if (options.Mode == FormattedTextMode.XmlTagged)
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
    if (pos < 0 || pos > aText.Length)
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
        //case var type when type == typeof(FootnoteReferenceMarkChar):
        //  return (element as FootnoteReferenceMarkChar)!.SetTextOf(text, options);
        //case var type when type == typeof(EndnoteReferenceMarkChar):
        //  return (element as EndnoteReferenceMarkChar)!.SetTextOf(text, options);
        //case var type when type == typeof(AnnotationReferenceMark):
        //  return (element as AnnotationReferenceMark)!.SetTextOf(text, options);
        //case var type when type == typeof(SeparatorMarkChar):
        //  return (element as SeparatorMarkChar)!.SetTextOf(text, options);
        //case var type when type == typeof(ContinuationSeparatorMarkChar):
        //  return (element as ContinuationSeparatorMarkChar)!.SetTextOf(text, options);
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
    if (options.Mode == FormattedTextMode.XmlTagged)
      runText.Text = text.HtmlDecode();
    else
      runText.Text = text;
    return true;
  }
}