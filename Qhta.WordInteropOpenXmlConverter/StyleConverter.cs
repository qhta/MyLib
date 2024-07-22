using System;
using System.Diagnostics;
using System.IO;
using DocumentFormat.OpenXml;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;
using static Qhta.WordInteropOpenXmlConverter.NumberConverter;
using static Qhta.WordInteropOpenXmlConverter.ColorConverter;
using static Qhta.WordInteropOpenXmlConverter.LanguageConverter;
using static Qhta.OpenXmlTools.RunTools;
using O = DocumentFormat.OpenXml;
using W = DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using Qhta.OpenXmlTools;
using Qhta.OpenXmlTools;

#nullable enable


namespace Qhta.WordInteropOpenXmlConverter;

public class StyleConverter
{
  public StyleConverter(Word.Document document)
  {
    styleTools = new StyleTools(document);
    buildInStyleNumbers = styleTools.LocalNameMyBuiltinStyles;
    defaultStyle = styleTools.GetStyle(Word.WdBuiltinStyle.wdStyleNormal);
    defaultFont = defaultStyle.Font;
    defaultParagraph = defaultStyle.ParagraphFormat;
    themeTools = new ThemeTools(document);
  }

  private readonly StyleTools styleTools;
  private readonly ThemeTools themeTools;
  private readonly Dictionary<string, MyBuiltinStyle> buildInStyleNumbers;
  private readonly Word.Style defaultStyle;
  private readonly Word.Font defaultFont;
  private readonly ParagraphFormat defaultParagraph;

  public W.Style ConvertStyle(Word.Style wordStyle)
  {
    // ReSharper disable once UseObjectOrCollectionInitializer
    var xStyle = new W.Style();

    #region style id and name
    var styleId = wordStyle.NameLocal;
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
      WdStyleType.wdStyleTypeParagraph => W.StyleValues.Paragraph,
      WdStyleType.wdStyleTypeCharacter => W.StyleValues.Character,
      WdStyleType.wdStyleTypeTable => W.StyleValues.Table,
      WdStyleType.wdStyleTypeList => W.StyleValues.Numbering,
      WdStyleType.wdStyleTypeParagraphOnly => W.StyleValues.Paragraph,
      WdStyleType.wdStyleTypeLinked => W.StyleValues.Paragraph,
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
    catch { }
    try
    {
      Word.Style baseStyle = (Word.Style)wordStyle.get_BaseStyle();
      if (baseStyle.NameLocal.Length > 0)
        xStyle.BasedOn = new W.BasedOn { Val = StyleTools.StyleNameToId(baseStyle.NameLocal) };
    }
    catch { }
    try
    {
      Word.Style linkStyle = (Word.Style)wordStyle.get_LinkStyle();
      xStyle.LinkedStyle = new W.LinkedStyle { Val = StyleTools.StyleNameToId(linkStyle.NameLocal) };
    }
    catch { }
    try
    {
      Word.Style nextParagraphStyle = (Word.Style)wordStyle.get_NextParagraphStyle();
      xStyle.NextParagraphStyle = new W.NextParagraphStyle { Val = StyleTools.StyleNameToId(nextParagraphStyle.NameLocal) };
    }
    catch { }

    #region style run properties
    try
    {
      var xRunProps = new RunPropertiesConverter(defaultStyle, themeTools).ConvertStyleFont(wordStyle);
      xStyle.StyleRunProperties = xRunProps;
    }
    catch { }
    #endregion style run properties

    #region paragraph properties
    try
    {
      var xParagraphProperties = new ParagraphPropertiesConverter(defaultStyle).ConvertStyleParagraphFormat(wordStyle);
      xStyle.StyleParagraphProperties = xParagraphProperties;
    }
    catch { }
    #endregion paragraph formating


    //try
    //{
    //  if (xNumbering != null)
    //  {
    //    var abstractNum = new NumberingPropertiesConverter(defaultStyle).CreateNumberingProperties(wordStyle);
    //    if (abstractNum != null)
    //    {
    //      xNumbering.Append(abstractNum);
    //    }
    //  }
    //}
    //catch { }
    return xStyle;
  }

}
