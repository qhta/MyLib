using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using DocumentFormat.OpenXml;

using static Microsoft.Office.Interop.Word.WdStyleType;

using W = DocumentFormat.OpenXml.Wordprocessing;
using Word = Microsoft.Office.Interop.Word;


#nullable enable


namespace Qhta.WordInteropOpenXmlConverter;

public class StyleConverter
{
  public StyleConverter(Word.Document document)
  {
    styleTools = new StyleTools(document);
    defaultStyle = styleTools.GetStyle(Word.WdBuiltinStyle.wdStyleNormal);
    themeTools = new ThemeTools(document);
  }

  private readonly StyleTools styleTools;
  private readonly ThemeTools themeTools;
  private readonly Word.Style defaultStyle;

  public W.Style ConvertStyle(Word.Style wordStyle)
  {
    // ReSharper disable once UseObjectOrCollectionInitializer
    var xStyle = new W.Style();

    #region style id and name
    var styleId = wordStyle.NameLocal;
    Debug.WriteLine($"Converting style \"{styleId}\"");
    styleId = StyleTools.StyleNameToId(styleId!);
    xStyle.StyleId = styleId;

    if (!styleTools.TryLocalNameToBuiltinName(wordStyle.NameLocal, out var styleName))
      styleName = wordStyle.NameLocal;
    if (styleName != null)
      xStyle.StyleName = new W.StyleName { Val = styleName };
    #endregion style id and name

    #region style type
    W.StyleValues styleType = wordStyle.Type switch
    {
      wdStyleTypeParagraph => W.StyleValues.Paragraph,
      wdStyleTypeCharacter => W.StyleValues.Character,
      wdStyleTypeTable => W.StyleValues.Table,
      wdStyleTypeList => W.StyleValues.Numbering,
      wdStyleTypeParagraphOnly => W.StyleValues.Paragraph,
      wdStyleTypeLinked => W.StyleValues.Paragraph,
      _ => throw new ArgumentOutOfRangeException(nameof(wordStyle.Type),
        // ReSharper disable once LocalizableElement
        $"Unsupported style type: {wordStyle.Type}")
    };
    xStyle.Type = new EnumValue<W.StyleValues>(styleType);
    #endregion style type

    if (wordStyle.Hidden)
      xStyle.SemiHidden = new W.SemiHidden();
    if (wordStyle.UnhideWhenUsed)
      xStyle.UnhideWhenUsed = new W.UnhideWhenUsed();
    if (wordStyle.Priority > 0)
      xStyle.UIPriority = new W.UIPriority { Val = wordStyle.Priority - 1 };
    try
    {
      if (wordStyle.AutomaticallyUpdate)
        xStyle.AutoRedefine = new W.AutoRedefine();
    }
    catch (COMException) { }
    try
    {
      Word.Style baseStyle = (Word.Style)wordStyle.get_BaseStyle();
      if (baseStyle.NameLocal.Length > 0)
        xStyle.BasedOn = new W.BasedOn { Val = StyleTools.StyleNameToId(baseStyle.NameLocal) };
    }
    catch (COMException) { }
    try
    {
      Word.Style linkStyle = (Word.Style)wordStyle.get_LinkStyle();
      xStyle.LinkedStyle = new W.LinkedStyle { Val = StyleTools.StyleNameToId(linkStyle.NameLocal) };
    }
    catch (COMException) { }
    try
    {
      Word.Style nextParagraphStyle = (Word.Style)wordStyle.get_NextParagraphStyle();
      xStyle.NextParagraphStyle = new W.NextParagraphStyle { Val = StyleTools.StyleNameToId(nextParagraphStyle.NameLocal) };
    }
    catch (COMException) { }

    #region style run properties
    try
    {
      var xRunProps = new RunPropertiesConverter(defaultStyle, themeTools).ConvertStyleFont(wordStyle);
      xStyle.StyleRunProperties = xRunProps;
    }
    catch (COMException) { }
    #endregion style run properties

    #region paragraph properties
    try
    {
      var xParagraphProperties = new ParagraphPropertiesConverter(defaultStyle).ConvertStyleParagraphFormat(wordStyle);
      xStyle.StyleParagraphProperties = xParagraphProperties;
    }
    catch (COMException) { }
    #endregion paragraph properties

    #region table properties
    try
    {
      var xTableProperties = new TablePropertiesConverter(defaultStyle).ConvertTableProperties(wordStyle);
      xStyle.StyleTableProperties = xTableProperties;
    }
    catch (COMException) { }
    #endregion table properties
    return xStyle;
  }

}
