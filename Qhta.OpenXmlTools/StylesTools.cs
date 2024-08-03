using System;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with styles in OpenXml documents.
/// </summary>
public static class StylesTools
{

  /// <summary>
  /// Converts a style name to a valid style ID.
  /// </summary>
  /// <param name="styleName">Name to convert</param>
  /// <returns>Style normalized ID</returns>
  public static string StyleNameToId(string styleName)
  {
    if (StyleNamesDictionary.TryGetValue(styleName, out var builtinStyle))
      return KnownStyles[builtinStyle].Ident;
    styleName = styleName.CamelCase();
    var chars = styleName.ToCharArray().ToList();
    chars.RemoveAll(c => !(c <= '\x7f' && Char.IsLetterOrDigit(c) || c == '-'));
    return new string(chars.ToArray());
  }

  /// <summary>
  /// Checks if the style name starts with "Heading".
  /// </summary>
  /// <param name="styleName">Name to check</param>
  /// <returns>true or false</returns>
  public static bool IsHeading(string styleName)
  {
    return styleName.StartsWith("Heading", StringComparison.OrdinalIgnoreCase);
  }

  /// <summary>
  /// Checks if the style name starts with "Heading".
  /// </summary>
  /// <param name="styles"></param>
  /// <param name="styleName">Name to check</param>
  /// <returns>true or false</returns>
  public static bool IsHeading(this DXW.Styles styles, string styleName)
  {
    return IsHeading(styleName);
  }
  
  /// <summary>
  /// Checks if the style is a heading style.
  /// </summary>
  /// <param name="style">Style to check</param>
  /// <returns>true or false</returns>
  public static bool IsHeading(this DXW.Style style)
  {
    return IsHeading(style.StyleName!.Val!.Value!);
  }

  /// <summary>
  /// Check if the document has styles defined
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument</param>
  /// <returns>True if the document has styles defined</returns>
  public static bool HasStyles(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.MainDocumentPart?.StyleDefinitionsPart?.Styles != null;
  }

  /// <summary>
  /// Gets the styles from the document. If the document does not have styles element, it is created
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument</param>
  /// <returns>Instance of the styles element</returns>
  public static DXW.Styles GetStyles(this DXPack.WordprocessingDocument wordDoc)
  {
    var mainDocumentPart = wordDoc.GetMainDocumentPart();
    var styleDefinitionsPart = mainDocumentPart.StyleDefinitionsPart ??
                               mainDocumentPart.AddNewPart<DXPack.StyleDefinitionsPart>();
    return styleDefinitionsPart.Styles ?? (styleDefinitionsPart.Styles = new DXW.Styles());
  }


  /// <summary>
  /// Get the count of the style definitions.
  /// </summary>
  /// <param name="styles"></param>
  /// <param name="filter">specifies if all style names should be counted or only the defined ones</param>
  /// <returns></returns>
  public static int Count(this DXW.Styles styles, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.BuiltIn)
      return KnownStyles.Count;
    if (filter == ItemFilter.All)
      return styles.GetNames(filter).Count();
    return styles.Elements<Style>().Count();
  }

  /// <summary>
  /// Get the names of the style definitions.
  /// </summary>
  /// <param name="styles"></param>
  /// <param name="filter">specifies if all style names should be counted or only the defined ones</param>
  /// <returns></returns>
  public static string[] GetNames(this DXW.Styles styles, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.BuiltIn)
      return KnownStyles.Values.Select(item => item.Name).ToArray();
    if (filter == ItemFilter.All)
    {
      var styleNames = KnownStyles.Values.Select(item => item.Name).ToArray(); ;
        //styles.LatentStyles?.Elements<LatentStyleExceptionInfo>().Select(s => s.Name?.Value!).ToArray() ??
        //new string[] { };
      foreach (var style in styles.Elements<Style>())
      {
        var styleName = style.StyleName?.Val?.Value;
        if (styleName != null && !styleNames.Contains(styleName, StringComparer.InvariantCultureIgnoreCase))
          styleNames = styleNames.Append(styleName).ToArray();
      }
      return styleNames.ToArray();
    }
    return styles.Elements<Style>().Select(s => s.StyleName?.Val?.Value!).ToArray();
  }

  /// <summary>
  /// Get the type of the style with its name.
  /// </summary>
  /// <param name="styles"></param>
  /// <param name="styleName"></param>
  /// <returns></returns>
  public static WdStyleType GetType(this DXW.Styles styles, string styleName)
  {
    if (StyleNamesDictionary.TryGetValue(styleName, out var builtinStyle))
      return KnownStyles[builtinStyle].Type;
    var styleId = StyleNameToId(styleName);
    var style = styles.Elements<Style>()
      .FirstOrDefault(s => String.Equals(StyleNameToId(s.StyleName?.Val?.Value!), styleId,
        StringComparison.InvariantCultureIgnoreCase));
    if (style != null)
      if (style.Type != null && style.Type.HasValue)
        return StyleValuesToStyleType(style.Type.Value);
    throw new ArgumentException($"Style {styleName} not found");
  }

  /// <summary>
  /// Get the style definition with its name.
  /// </summary>
  /// <param name="styles"></param>
  /// <param name="styleName"></param>
  /// <returns></returns>
  public static Style? GetStyle(this DXW.Styles styles, string styleName)
  {
    var element = styles.Elements<Style>().FirstOrDefault(s => s.StyleName?.Val?.Value == styleName);
    if (element != null)
      return element;
    if (BuiltinStyles == null)
      LoadBuiltinStyles();
    if (StyleNamesDictionary.TryGetValue(styleName, out var builtinStyle))
    {
      if (BuiltinStyles!.TryGetValue(builtinStyle, out var style))
        return style;
    }
    return null;
  }

  /// <summary>
  /// Set the style definition. If the style with the same name exists, it is replaced.
  /// Otherwise, the new style is added.
  /// </summary>
  /// <param name="styles"></param>
  /// <param name="aStyle">Style to set</param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  public static void SetStyle(this DXW.Styles styles, Style aStyle)
  {
    var styleName = aStyle.StyleName?.Val?.Value;
    var element = styles.Elements<Style>().FirstOrDefault(s => s.StyleName?.Val?.Value == styleName);
    if (element != null)
      element.Remove();
    styles.AppendChild(aStyle);
  }

  /// <summary>
  /// Converts a style value to a style type.
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  private static WdStyleType StyleValuesToStyleType(DXW.StyleValues value)
  {
    if (value == DXW.StyleValues.Paragraph)
      return WdStyleType.Paragraph;
    if (value == DXW.StyleValues.Character)
      return WdStyleType.Character;
    if (value == DXW.StyleValues.Table)
      return WdStyleType.Table;
    if (value == DXW.StyleValues.Numbering)
      return WdStyleType.Numbering;
    return 0;
  }

  /// <summary>
  /// Converts a style type to a style value.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  private static DXW.StyleValues? StyleTypeToStyleValues(WdStyleType type)
  {
    if (type == WdStyleType.Paragraph)
      return DXW.StyleValues.Paragraph;
    if (type == WdStyleType.Character)
      return DXW.StyleValues.Character;
    if (type == WdStyleType.Table)
      return DXW.StyleValues.Table;
    if (type == WdStyleType.Numbering)
      return DXW.StyleValues.Numbering;
    return null;
  }

  private static void LoadBuiltinStyles()
  {
    using (var wordDoc = DXPack.WordprocessingDocument.Open("BuiltinStyleProbes.docx", false))
    {
      LoadBuiltinStyles(wordDoc);
    }
  }

  private static void LoadBuiltinStyles(this DXPack.WordprocessingDocument wordDoc)
  {
    BuiltinStyles = new Dictionary<WdBuiltinStyle, Style>();
    foreach (var style in wordDoc.GetStyles().Elements<Style>())
    {
      var styleName = style.StyleName?.Val?.Value!;
      if (StyleNamesDictionary.TryGetValue(styleName, out var builtinStyle))
      {
        BuiltinStyles.Add(builtinStyle, (Style)style.CloneNode(true));
      }
      else
      {
        Console.WriteLine($"Unknown style: {styleName}");
      }
    }
  }
  private static Dictionary<WdBuiltinStyle, Style>? BuiltinStyles = null;

  private static readonly Dictionary<WdBuiltinStyle, (WdStyleType Type, string Ident, string Name)> KnownStyles = new ()
    {
    { WdBuiltinStyle.Normal, (WdStyleType.Paragraph, "Normal", "Normal") },
    { WdBuiltinStyle.Heading1, (WdStyleType.Paragraph, "Heading1", "Heading 1") },
    { WdBuiltinStyle.Heading1Char, (WdStyleType.Linked, "Heading1Char", "Heading 1 Char") },
    { WdBuiltinStyle.Heading2, (WdStyleType.Paragraph, "Heading2", "Heading 2") },
    { WdBuiltinStyle.Heading2Char, (WdStyleType.Linked, "Heading2Char", "Heading 2 Char") },
    { WdBuiltinStyle.Heading3, (WdStyleType.Paragraph, "Heading3", "Heading 3") },
    { WdBuiltinStyle.Heading3Char, (WdStyleType.Linked, "Heading3Char", "Heading 3 Char") },
    { WdBuiltinStyle.Heading4, (WdStyleType.Paragraph, "Heading4", "Heading 4") },
    { WdBuiltinStyle.Heading4Char, (WdStyleType.Linked, "Heading4Char", "Heading 4 Char") },
    { WdBuiltinStyle.Heading5, (WdStyleType.Paragraph, "Heading5", "Heading 5") },
    { WdBuiltinStyle.Heading5Char, (WdStyleType.Linked, "Heading5Char", "Heading 5 Char") },
    { WdBuiltinStyle.Heading6, (WdStyleType.Paragraph, "Heading6", "Heading 6") },
    { WdBuiltinStyle.Heading6Char, (WdStyleType.Linked, "Heading6Char", "Heading 6 Char") },
    { WdBuiltinStyle.Heading7, (WdStyleType.Paragraph, "Heading7", "Heading 7") },
    { WdBuiltinStyle.Heading7Char, (WdStyleType.Linked, "Heading7Char", "Heading 7 Char") },
    { WdBuiltinStyle.Heading8, (WdStyleType.Paragraph, "Heading8", "Heading 8") },
    { WdBuiltinStyle.Heading8Char, (WdStyleType.Linked, "Heading8Char", "Heading 8 Char") },
    { WdBuiltinStyle.Heading9, (WdStyleType.Paragraph, "Heading9", "Heading 9") },
    { WdBuiltinStyle.Heading9Char, (WdStyleType.Linked, "Heading9Char", "Heading 9 Char") },
    { WdBuiltinStyle.Index1, (WdStyleType.Paragraph, "Index1", "Index 1") },
    { WdBuiltinStyle.Index2, (WdStyleType.Paragraph, "Index2", "Index 2") },
    { WdBuiltinStyle.Index3, (WdStyleType.Paragraph, "Index3", "Index 3") },
    { WdBuiltinStyle.Index4, (WdStyleType.Paragraph, "Index4", "Index 4") },
    { WdBuiltinStyle.Index5, (WdStyleType.Paragraph, "Index5", "Index 5") },
    { WdBuiltinStyle.Index6, (WdStyleType.Paragraph, "Index6", "Index 6") },
    { WdBuiltinStyle.Index7, (WdStyleType.Paragraph, "Index7", "Index 7") },
    { WdBuiltinStyle.Index8, (WdStyleType.Paragraph, "Index8", "Index 8") },
    { WdBuiltinStyle.Index9, (WdStyleType.Paragraph, "Index9", "Index 9") },
    { WdBuiltinStyle.TOC1, (WdStyleType.Paragraph, "TOC1", "TOC 1") },
    { WdBuiltinStyle.TOC2, (WdStyleType.Paragraph, "TOC2", "TOC 2") },
    { WdBuiltinStyle.TOC3, (WdStyleType.Paragraph, "TOC3", "TOC 3") },
    { WdBuiltinStyle.TOC4, (WdStyleType.Paragraph, "TOC4", "TOC 4") },
    { WdBuiltinStyle.TOC5, (WdStyleType.Paragraph, "TOC5", "TOC 5") },
    { WdBuiltinStyle.TOC6, (WdStyleType.Paragraph, "TOC6", "TOC 6") },
    { WdBuiltinStyle.TOC7, (WdStyleType.Paragraph, "TOC7", "TOC 7") },
    { WdBuiltinStyle.TOC8, (WdStyleType.Paragraph, "TOC8", "TOC 8") },
    { WdBuiltinStyle.TOC9, (WdStyleType.Paragraph, "TOC9", "TOC 9") },
    { WdBuiltinStyle.NormalIndent, (WdStyleType.Paragraph, "NormalIndent", "Normal Indent") },
    { WdBuiltinStyle.FootnoteText, (WdStyleType.Paragraph, "FootnoteText", "Footnote Text") },
    { WdBuiltinStyle.FootnoteTextChar, (WdStyleType.Linked, "FootnoteTextChar", "Footnote Text Char") },
    { WdBuiltinStyle.CommentText, (WdStyleType.Paragraph, "CommentText", "Comment Text") },
    { WdBuiltinStyle.CommentTextChar, (WdStyleType.Linked, "CommentTextChar", "Comment Text Char") },
    { WdBuiltinStyle.Header, (WdStyleType.Paragraph, "Header", "Header") },
    { WdBuiltinStyle.HeaderChar, (WdStyleType.Linked, "HeaderChar", "Header Char") },
    { WdBuiltinStyle.Footer, (WdStyleType.Paragraph, "Footer", "Footer") },
    { WdBuiltinStyle.FooterChar, (WdStyleType.Linked, "FooterChar", "Footer Char") },
    { WdBuiltinStyle.IndexHeading, (WdStyleType.Paragraph, "IndexHeading", "Index Heading") },
    { WdBuiltinStyle.Caption, (WdStyleType.Paragraph, "Caption", "Caption") },
    { WdBuiltinStyle.TableOfFigures, (WdStyleType.Paragraph, "TableOfFigures", "Table of Figures") },
    { WdBuiltinStyle.EnvelopeAddress, (WdStyleType.Paragraph, "EnvelopeAddress", "Envelope Address") },
    { WdBuiltinStyle.EnvelopeReturn, (WdStyleType.Paragraph, "EnvelopeReturn", "Envelope Return") },
    { WdBuiltinStyle.FootnoteReference, (WdStyleType.Character, "FootnoteReference", "Footnote Reference") },
    { WdBuiltinStyle.CommentReference, (WdStyleType.Character, "CommentReference", "Comment Reference") },
    { WdBuiltinStyle.LineNumber, (WdStyleType.Character, "LineNumber", "Line Number") },
    { WdBuiltinStyle.PageNumber, (WdStyleType.Character, "PageNumber", "Page Number") },
    { WdBuiltinStyle.EndnoteReference, (WdStyleType.Character, "EndnoteReference", "Endnote Reference") },
    { WdBuiltinStyle.EndnoteText, (WdStyleType.Paragraph, "EndnoteText", "Endnote Text") },
    { WdBuiltinStyle.EndnoteTextChar, (WdStyleType.Linked, "EndnoteTextChar", "Endnote Text Char") },
    { WdBuiltinStyle.TableOfAuthorities, (WdStyleType.Paragraph, "TableOfAuthorities", "Table of Authorities") },
    { WdBuiltinStyle.Macro, (WdStyleType.Paragraph, "Macro", "Macro") },
    { WdBuiltinStyle.MacroChar, (WdStyleType.Linked, "MacroChar", "Macro Text Char") },
    { WdBuiltinStyle.TOAHeading, (WdStyleType.Paragraph, "TOAHeading", "TOA Heading") },
    { WdBuiltinStyle.List, (WdStyleType.Paragraph, "List", "List") },
    { WdBuiltinStyle.ListBullet, (WdStyleType.Paragraph, "ListBullet", "List Bullet") },
    { WdBuiltinStyle.ListNumber, (WdStyleType.Paragraph, "ListNumber", "List Number") },
    { WdBuiltinStyle.List2, (WdStyleType.Paragraph, "List2", "List 2") },
    { WdBuiltinStyle.List3, (WdStyleType.Paragraph, "List3", "List 3") },
    { WdBuiltinStyle.List4, (WdStyleType.Paragraph, "List4", "List 4") },
    { WdBuiltinStyle.List5, (WdStyleType.Paragraph, "List5", "List 5") },
    { WdBuiltinStyle.ListBullet2, (WdStyleType.Paragraph, "ListBullet2", "List Bullet 2") },
    { WdBuiltinStyle.ListBullet3, (WdStyleType.Paragraph, "ListBullet3", "List Bullet 3") },
    { WdBuiltinStyle.ListBullet4, (WdStyleType.Paragraph, "ListBullet4", "List Bullet 4") },
    { WdBuiltinStyle.ListBullet5, (WdStyleType.Paragraph, "ListBullet5", "List Bullet 5") },
    { WdBuiltinStyle.ListNumber2, (WdStyleType.Paragraph, "ListNumber2", "List Number 2") },
    { WdBuiltinStyle.ListNumber3, (WdStyleType.Paragraph, "ListNumber3", "List Number 3") },
    { WdBuiltinStyle.ListNumber4, (WdStyleType.Paragraph, "ListNumber4", "List Number 4") },
    { WdBuiltinStyle.ListNumber5, (WdStyleType.Paragraph, "ListNumber5", "List Number 5") },
    { WdBuiltinStyle.Title, (WdStyleType.Paragraph, "Title", "Title") },
    { WdBuiltinStyle.TitleChar, (WdStyleType.Linked, "TitleChar", "Title Char") },
    { WdBuiltinStyle.Closing, (WdStyleType.Paragraph, "Closing", "Closing") },
    { WdBuiltinStyle.ClosingChar, (WdStyleType.Linked, "ClosingChar", "Closing Char") },
    { WdBuiltinStyle.Signature, (WdStyleType.Paragraph, "Signature", "Signature") },
    { WdBuiltinStyle.SignatureChar, (WdStyleType.Linked, "SignatureChar", "Signature Char") },
    { WdBuiltinStyle.DefaultParagraphFont, (WdStyleType.Character, "DefaultParagraphFont", "Default Paragraph Font") },
    { WdBuiltinStyle.BodyText, (WdStyleType.Paragraph, "BodyText", "Body Text") },
    { WdBuiltinStyle.BodyTextChar, (WdStyleType.Linked, "BodyTextChar", "Body Text Char") },
    { WdBuiltinStyle.BodyTextIndent, (WdStyleType.Paragraph, "BodyTextIndent", "Body Text Indent") },
    { WdBuiltinStyle.BodyTextIndentChar, (WdStyleType.Linked, "BodyTextIndentChar", "Body Text Indent Char") },
    { WdBuiltinStyle.ListContinue, (WdStyleType.Paragraph, "ListContinue", "List Continue") },
    { WdBuiltinStyle.ListContinue2, (WdStyleType.Paragraph, "ListContinue2", "List Continue 2") },
    { WdBuiltinStyle.ListContinue3, (WdStyleType.Paragraph, "ListContinue3", "List Continue 3") },
    { WdBuiltinStyle.ListContinue4, (WdStyleType.Paragraph, "ListContinue4", "List Continue 4") },
    { WdBuiltinStyle.ListContinue5, (WdStyleType.Paragraph, "ListContinue5", "List Continue 5") },
    { WdBuiltinStyle.MessageHeader, (WdStyleType.Paragraph, "MessageHeader", "Message Header") },
    { WdBuiltinStyle.MessageHeaderChar, (WdStyleType.Linked, "MessageHeaderChar", "Message Header Char") },
    { WdBuiltinStyle.Subtitle, (WdStyleType.Paragraph, "Subtitle", "Subtitle") },
    { WdBuiltinStyle.SubtitleChar, (WdStyleType.Linked, "SubtitleChar", "Subtitle Char") },
    { WdBuiltinStyle.Salutation, (WdStyleType.Paragraph, "Salutation", "Salutation") },
    { WdBuiltinStyle.SalutationChar, (WdStyleType.Linked, "SalutationChar", "Salutation Char") },
    { WdBuiltinStyle.Date, (WdStyleType.Paragraph, "Date", "Date") },
    { WdBuiltinStyle.DateChar, (WdStyleType.Linked, "DateChar", "Date Char") },
    { WdBuiltinStyle.BodyTextFirstIndent, (WdStyleType.Paragraph, "BodyTextFirstIndent", "Body Text First Indent") },
    { WdBuiltinStyle.BodyTextFirstIndentChar, (WdStyleType.Linked, "BodyTextFirstIndentChar", "Body Text First Indent Char") },
    { WdBuiltinStyle.BodyTextFirstIndent2, (WdStyleType.Paragraph, "BodyTextFirstIndent2", "Body Text First Indent 2") },
    { WdBuiltinStyle.BodyTextFirstIndent2Char, (WdStyleType.Linked, "BodyTextFirstIndent2Char", "Body Text First Indent 2 Char") },
    { WdBuiltinStyle.NoteHeading, (WdStyleType.Paragraph, "NoteHeading", "Note Heading") },
    { WdBuiltinStyle.NoteHeadingChar, (WdStyleType.Linked, "NoteHeadingChar", "Note Heading Char") },
    { WdBuiltinStyle.BodyText2, (WdStyleType.Paragraph, "BodyText2", "Body Text 2") },
    { WdBuiltinStyle.BodyText2Char, (WdStyleType.Linked, "BodyText2Char", "Body Text 2 Char") },
    { WdBuiltinStyle.BodyText3, (WdStyleType.Paragraph, "BodyText3", "Body Text 3") },
    { WdBuiltinStyle.BodyText3Char, (WdStyleType.Linked, "BodyText3Char", "Body Text 3 Char") },
    { WdBuiltinStyle.BodyTextIndent2, (WdStyleType.Paragraph, "BodyTextIndent2", "Body Text Indent 2") },
    { WdBuiltinStyle.BodyTextIndent2Char, (WdStyleType.Linked, "BodyTextIndent2Char", "Body Text Indent 2 Char") },
    { WdBuiltinStyle.BodyTextIndent3, (WdStyleType.Paragraph, "BodyTextIndent3", "Body Text Indent 3") },
    { WdBuiltinStyle.BodyTextIndent3Char, (WdStyleType.Linked, "BodyTextIndent3Char", "Body Text Indent 3 Char") },
    { WdBuiltinStyle.BlockText, (WdStyleType.Paragraph, "BlockText", "Block Text") },
    { WdBuiltinStyle.Hyperlink, (WdStyleType.Character, "Hyperlink", "Hyperlink") },
    { WdBuiltinStyle.FollowedHyperlink, (WdStyleType.Character, "FollowedHyperlink", "FollowedHyperlink") },
    { WdBuiltinStyle.Strong, (WdStyleType.Character, "Strong", "Strong") },
    { WdBuiltinStyle.Emphasis, (WdStyleType.Character, "Emphasis", "Emphasis") },
    { WdBuiltinStyle.DocumentMap, (WdStyleType.Paragraph, "DocumentMap", "Document Map") },
    { WdBuiltinStyle.DocumentMapChar, (WdStyleType.Linked, "DocumentMapChar", "Document Map Char") },
    { WdBuiltinStyle.PlainText, (WdStyleType.Paragraph, "PlainText", "Plain Text") },
    { WdBuiltinStyle.PlainTextChar, (WdStyleType.Linked, "PlainTextChar", "Plain Text Char") },
    { WdBuiltinStyle.EmailSignature, (WdStyleType.Paragraph, "EmailSignature", "E-mail Signature") },
    { WdBuiltinStyle.EmailSignatureChar, (WdStyleType.Linked, "EmailSignatureChar", "E-mail Signature Char") },
    { WdBuiltinStyle.zTopOfForm, (WdStyleType.Paragraph, "zTopOfForm", "z-Top of Form") },
    { WdBuiltinStyle.zTopOfFormChar, (WdStyleType.Linked, "zTopOfFormChar", "z-Top of Form Char") },
    { WdBuiltinStyle.zBottomOfForm, (WdStyleType.Paragraph, "zBottomOfForm", "z-Bottom of Form") },
    { WdBuiltinStyle.zBottomOfFormChar, (WdStyleType.Linked, "zBottomOfFormChar", "z-Bottom of Form Char") },
    { WdBuiltinStyle.NormalWeb, (WdStyleType.Paragraph, "NormalWeb", "Normal (Web)") },
    { WdBuiltinStyle.HtmlAcronym, (WdStyleType.Character, "HtmlAcronym", "HTML Acronym") },
    { WdBuiltinStyle.HtmlAddress, (WdStyleType.Paragraph, "HtmlAddress", "HTML Address") },
    { WdBuiltinStyle.HtmlAddressChar, (WdStyleType.Linked, "HtmlAddressChar", "HTML Address Char") },
    { WdBuiltinStyle.HtmlCite, (WdStyleType.Character, "HtmlCite", "HTML Cite") },
    { WdBuiltinStyle.HtmlCode, (WdStyleType.Character, "HtmlCode", "HTML Code") },
    { WdBuiltinStyle.HtmlDefinition, (WdStyleType.Character, "HtmlDefinition", "HTML Definition") },
    { WdBuiltinStyle.HtmlKeyboard, (WdStyleType.Character, "HtmlKeyboard", "HTML Keyboard") },
    { WdBuiltinStyle.HtmlPreformatted, (WdStyleType.Paragraph, "HtmlPreformatted", "HTML Preformatted") },
    { WdBuiltinStyle.HtmlPreformattedChar, (WdStyleType.Linked, "HtmlPreformattedChar", "HTML Preformatted Char") },
    { WdBuiltinStyle.HtmlSample, (WdStyleType.Character, "HtmlSample", "HTML Sample") },
    { WdBuiltinStyle.HtmlTypewriter, (WdStyleType.Character, "HtmlTypewriter", "HTML Typewriter") },
    { WdBuiltinStyle.HtmlVariable, (WdStyleType.Character, "HtmlVariable", "HTML Variable") },
    { WdBuiltinStyle.NormalTable, (WdStyleType.Table, "NormalTable", "Normal Table") },
    { WdBuiltinStyle.CommentSubject, (WdStyleType.Paragraph, "CommentSubject", "Comment Subject") },
    { WdBuiltinStyle.CommentSubjectChar, (WdStyleType.Linked, "CommentSubjectChar", "Comment Subject Char") },
    { WdBuiltinStyle.NoList, (WdStyleType.Numbering, "NoList", "No List") },
    { WdBuiltinStyle.OutlineList1, (WdStyleType.Numbering, "OutlineList1", "1 / a / i") },
    { WdBuiltinStyle.OutlineList2, (WdStyleType.Numbering, "OutlineList2", "1 / 1.1 / 1.1.1") },
    { WdBuiltinStyle.OutlineList3, (WdStyleType.Numbering, "OutlineList3", "Article / Section") },
    { WdBuiltinStyle.TableSimple1, (WdStyleType.Table, "TableSimple1", "Table Simple 1") },
    { WdBuiltinStyle.TableSimple2, (WdStyleType.Table, "TableSimple2", "Table Simple 2") },
    { WdBuiltinStyle.TableSimple3, (WdStyleType.Table, "TableSimple3", "Table Simple 3") },
    { WdBuiltinStyle.TableClassic1, (WdStyleType.Table, "TableClassic1", "Table Classic 1") },
    { WdBuiltinStyle.TableClassic2, (WdStyleType.Table, "TableClassic2", "Table Classic 2") },
    { WdBuiltinStyle.TableClassic3, (WdStyleType.Table, "TableClassic3", "Table Classic 3") },
    { WdBuiltinStyle.TableClassic4, (WdStyleType.Table, "TableClassic4", "Table Classic 4") },
    { WdBuiltinStyle.TableColorful1, (WdStyleType.Table, "TableColorful1", "Table Colorful 1") },
    { WdBuiltinStyle.TableColorful2, (WdStyleType.Table, "TableColorful2", "Table Colorful 2") },
    { WdBuiltinStyle.TableColorful3, (WdStyleType.Table, "TableColorful3", "Table Colorful 3") },
    { WdBuiltinStyle.TableColumns1, (WdStyleType.Table, "TableColumns1", "Table Columns 1") },
    { WdBuiltinStyle.TableColumns2, (WdStyleType.Table, "TableColumns2", "Table Columns 2") },
    { WdBuiltinStyle.TableColumns3, (WdStyleType.Table, "TableColumns3", "Table Columns 3") },
    { WdBuiltinStyle.TableColumns4, (WdStyleType.Table, "TableColumns4", "Table Columns 4") },
    { WdBuiltinStyle.TableColumns5, (WdStyleType.Table, "TableColumns5", "Table Columns 5") },
    { WdBuiltinStyle.TableGrid1, (WdStyleType.Table, "TableGrid1", "Table Grid 1") },
    { WdBuiltinStyle.TableGrid2, (WdStyleType.Table, "TableGrid2", "Table Grid 2") },
    { WdBuiltinStyle.TableGrid3, (WdStyleType.Table, "TableGrid3", "Table Grid 3") },
    { WdBuiltinStyle.TableGrid4, (WdStyleType.Table, "TableGrid4", "Table Grid 4") },
    { WdBuiltinStyle.TableGrid5, (WdStyleType.Table, "TableGrid5", "Table Grid 5") },
    { WdBuiltinStyle.TableGrid6, (WdStyleType.Table, "TableGrid6", "Table Grid 6") },
    { WdBuiltinStyle.TableGrid7, (WdStyleType.Table, "TableGrid7", "Table Grid 7") },
    { WdBuiltinStyle.TableGrid8, (WdStyleType.Table, "TableGrid8", "Table Grid 8") },
    { WdBuiltinStyle.TableList1, (WdStyleType.Table, "TableList1", "Table List 1") },
    { WdBuiltinStyle.TableList2, (WdStyleType.Table, "TableList2", "Table List 2") },
    { WdBuiltinStyle.TableList3, (WdStyleType.Table, "TableList3", "Table List 3") },
    { WdBuiltinStyle.TableList4, (WdStyleType.Table, "TableList4", "Table List 4") },
    { WdBuiltinStyle.TableList5, (WdStyleType.Table, "TableList5", "Table List 5") },
    { WdBuiltinStyle.TableList6, (WdStyleType.Table, "TableList6", "Table List 6") },
    { WdBuiltinStyle.TableList7, (WdStyleType.Table, "TableList7", "Table List 7") },
    { WdBuiltinStyle.TableList8, (WdStyleType.Table, "TableList8", "Table List 8") },
    { WdBuiltinStyle.Table3dEffects1, (WdStyleType.Table, "Table3dEffects1", "Table 3D effects 1") },
    { WdBuiltinStyle.Table3dEffects2, (WdStyleType.Table, "Table3dEffects2", "Table 3D effects 2") },
    { WdBuiltinStyle.Table3dEffects3, (WdStyleType.Table, "Table3dEffects3", "Table 3D effects 3") },
    { WdBuiltinStyle.TableContemporary, (WdStyleType.Table, "TableContemporary", "Table Contemporary") },
    { WdBuiltinStyle.TableElegant, (WdStyleType.Table, "TableElegant", "Table Elegant") },
    { WdBuiltinStyle.TableProfessional, (WdStyleType.Table, "TableProfessional", "Table Professional") },
    { WdBuiltinStyle.TableSubtle1, (WdStyleType.Table, "TableSubtle1", "Table Subtle 1") },
    { WdBuiltinStyle.TableSubtle2, (WdStyleType.Table, "TableSubtle2", "Table Subtle 2") },
    { WdBuiltinStyle.TableWeb1, (WdStyleType.Table, "TableWeb1", "Table Web 1") },
    { WdBuiltinStyle.TableWeb2, (WdStyleType.Table, "TableWeb2", "Table Web 2") },
    { WdBuiltinStyle.TableWeb3, (WdStyleType.Table, "TableWeb3", "Table Web 3") },
    { WdBuiltinStyle.BalloonText, (WdStyleType.Paragraph, "BalloonText", "Balloon Text") },
    { WdBuiltinStyle.BalloonTextChar, (WdStyleType.Linked, "BalloonTextChar", "Balloon Text Char") },
    { WdBuiltinStyle.TableGrid, (WdStyleType.Table, "TableGrid", "Table Grid") },
    { WdBuiltinStyle.TableTheme, (WdStyleType.Table, "TableTheme", "Table Theme") },
    { WdBuiltinStyle.PlaceholderText, (WdStyleType.Character, "PlaceholderText", "Placeholder Text") },
    { WdBuiltinStyle.NoSpacing, (WdStyleType.Paragraph, "NoSpacing", "No Spacing") },
    { WdBuiltinStyle.LightShading, (WdStyleType.Table, "LightShading", "Light Shading") },
    { WdBuiltinStyle.LightList, (WdStyleType.Table, "LightList", "Light List") },
    { WdBuiltinStyle.LightGrid, (WdStyleType.Table, "LightGrid", "Light Grid") },
    { WdBuiltinStyle.MediumShading1, (WdStyleType.Table, "MediumShading1", "Medium Shading 1") },
    { WdBuiltinStyle.MediumShading2, (WdStyleType.Table, "MediumShading2", "Medium Shading 2") },
    { WdBuiltinStyle.MediumList1, (WdStyleType.Table, "MediumList1", "Medium List 1") },
    { WdBuiltinStyle.MediumList2, (WdStyleType.Table, "MediumList2", "Medium List 2") },
    { WdBuiltinStyle.MediumGrid1, (WdStyleType.Table, "MediumGrid1", "Medium Grid 1") },
    { WdBuiltinStyle.MediumGrid2, (WdStyleType.Table, "MediumGrid2", "Medium Grid 2") },
    { WdBuiltinStyle.MediumGrid3, (WdStyleType.Table, "MediumGrid3", "Medium Grid 3") },
    { WdBuiltinStyle.DarkList, (WdStyleType.Table, "DarkList", "Dark List") },
    { WdBuiltinStyle.ColorfulShading, (WdStyleType.Table, "ColorfulShading", "Colorful Shading") },
    { WdBuiltinStyle.ColorfulList, (WdStyleType.Table, "ColorfulList", "Colorful List") },
    { WdBuiltinStyle.ColorfulGrid, (WdStyleType.Table, "ColorfulGrid", "Colorful Grid") },
    { WdBuiltinStyle.LightShadingAccent1, (WdStyleType.Table, "LightShadingAccent1", "Light Shading Accent 1") },
    { WdBuiltinStyle.LightListAccent1, (WdStyleType.Table, "LightListAccent1", "Light List Accent 1") },
    { WdBuiltinStyle.LightGridAccent1, (WdStyleType.Table, "LightGridAccent1", "Light Grid Accent 1") },
    { WdBuiltinStyle.MediumShading1Accent1, (WdStyleType.Table, "MediumShading1Accent1", "Medium Shading 1 Accent 1") },
    { WdBuiltinStyle.MediumShading2Accent1, (WdStyleType.Table, "MediumShading2Accent1", "Medium Shading 2 Accent 1") },
    { WdBuiltinStyle.MediumList1Accent1, (WdStyleType.Table, "MediumList1Accent1", "Medium List 1 Accent 1") },
    { WdBuiltinStyle.Revision, (WdStyleType.Paragraph, "Revision", "Revision") },
    { WdBuiltinStyle.ListParagraph, (WdStyleType.Paragraph, "ListParagraph", "List Paragraph") },
    { WdBuiltinStyle.Quote, (WdStyleType.Paragraph, "Quote", "Quote") },
    { WdBuiltinStyle.QuoteChar, (WdStyleType.Linked, "QuoteChar", "Quote Char") },
    { WdBuiltinStyle.IntenseQuote, (WdStyleType.Paragraph, "IntenseQuote", "Intense Quote") },
    { WdBuiltinStyle.IntenseQuoteChar, (WdStyleType.Linked, "IntenseQuoteChar", "Intense Quote Char") },
    { WdBuiltinStyle.MediumList2Accent1, (WdStyleType.Table, "MediumList2Accent1", "Medium List 2 Accent 1") },
    { WdBuiltinStyle.MediumGrid1Accent1, (WdStyleType.Table, "MediumGrid1Accent1", "Medium Grid 1 Accent 1") },
    { WdBuiltinStyle.MediumGrid2Accent1, (WdStyleType.Table, "MediumGrid2Accent1", "Medium Grid 2 Accent 1") },
    { WdBuiltinStyle.MediumGrid3Accent1, (WdStyleType.Table, "MediumGrid3Accent1", "Medium Grid 3 Accent 1") },
    { WdBuiltinStyle.DarkListAccent1, (WdStyleType.Table, "DarkListAccent1", "Dark List Accent 1") },
    { WdBuiltinStyle.ColorfulShadingAccent1, (WdStyleType.Table, "ColorfulShadingAccent1", "Colorful Shading Accent 1") },
    { WdBuiltinStyle.ColorfulListAccent1, (WdStyleType.Table, "ColorfulListAccent1", "Colorful List Accent 1") },
    { WdBuiltinStyle.ColorfulGridAccent1, (WdStyleType.Table, "ColorfulGridAccent1", "Colorful Grid Accent 1") },
    { WdBuiltinStyle.LightShadingAccent2, (WdStyleType.Table, "LightShadingAccent2", "Light Shading Accent 2") },
    { WdBuiltinStyle.LightListAccent2, (WdStyleType.Table, "LightListAccent2", "Light List Accent 2") },
    { WdBuiltinStyle.LightGridAccent2, (WdStyleType.Table, "LightGridAccent2", "Light Grid Accent 2") },
    { WdBuiltinStyle.MediumShading1Accent2, (WdStyleType.Table, "MediumShading1Accent2", "Medium Shading 1 Accent 2") },
    { WdBuiltinStyle.MediumShading2Accent2, (WdStyleType.Table, "MediumShading2Accent2", "Medium Shading 2 Accent 2") },
    { WdBuiltinStyle.MediumList1Accent2, (WdStyleType.Table, "MediumList1Accent2", "Medium List 1 Accent 2") },
    { WdBuiltinStyle.MediumList2Accent2, (WdStyleType.Table, "MediumList2Accent2", "Medium List 2 Accent 2") },
    { WdBuiltinStyle.MediumGrid1Accent2, (WdStyleType.Table, "MediumGrid1Accent2", "Medium Grid 1 Accent 2") },
    { WdBuiltinStyle.MediumGrid2Accent2, (WdStyleType.Table, "MediumGrid2Accent2", "Medium Grid 2 Accent 2") },
    { WdBuiltinStyle.MediumGrid3Accent2, (WdStyleType.Table, "MediumGrid3Accent2", "Medium Grid 3 Accent 2") },
    { WdBuiltinStyle.DarkListAccent2, (WdStyleType.Table, "DarkListAccent2", "Dark List Accent 2") },
    { WdBuiltinStyle.ColorfulShadingAccent2, (WdStyleType.Table, "ColorfulShadingAccent2", "Colorful Shading Accent 2") },
    { WdBuiltinStyle.ColorfulListAccent2, (WdStyleType.Table, "ColorfulListAccent2", "Colorful List Accent 2") },
    { WdBuiltinStyle.ColorfulGridAccent2, (WdStyleType.Table, "ColorfulGridAccent2", "Colorful Grid Accent 2") },
    { WdBuiltinStyle.LightShadingAccent3, (WdStyleType.Table, "LightShadingAccent3", "Light Shading Accent 3") },
    { WdBuiltinStyle.LightListAccent3, (WdStyleType.Table, "LightListAccent3", "Light List Accent 3") },
    { WdBuiltinStyle.LightGridAccent3, (WdStyleType.Table, "LightGridAccent3", "Light Grid Accent 3") },
    { WdBuiltinStyle.MediumShading1Accent3, (WdStyleType.Table, "MediumShading1Accent3", "Medium Shading 1 Accent 3") },
    { WdBuiltinStyle.MediumShading2Accent3, (WdStyleType.Table, "MediumShading2Accent3", "Medium Shading 2 Accent 3") },
    { WdBuiltinStyle.MediumList1Accent3, (WdStyleType.Table, "MediumList1Accent3", "Medium List 1 Accent 3") },
    { WdBuiltinStyle.MediumList2Accent3, (WdStyleType.Table, "MediumList2Accent3", "Medium List 2 Accent 3") },
    { WdBuiltinStyle.MediumGrid1Accent3, (WdStyleType.Table, "MediumGrid1Accent3", "Medium Grid 1 Accent 3") },
    { WdBuiltinStyle.MediumGrid2Accent3, (WdStyleType.Table, "MediumGrid2Accent3", "Medium Grid 2 Accent 3") },
    { WdBuiltinStyle.MediumGrid3Accent3, (WdStyleType.Table, "MediumGrid3Accent3", "Medium Grid 3 Accent 3") },
    { WdBuiltinStyle.DarkListAccent3, (WdStyleType.Table, "DarkListAccent3", "Dark List Accent 3") },
    { WdBuiltinStyle.ColorfulShadingAccent3, (WdStyleType.Table, "ColorfulShadingAccent3", "Colorful Shading Accent 3") },
    { WdBuiltinStyle.ColorfulListAccent3, (WdStyleType.Table, "ColorfulListAccent3", "Colorful List Accent 3") },
    { WdBuiltinStyle.ColorfulGridAccent3, (WdStyleType.Table, "ColorfulGridAccent3", "Colorful Grid Accent 3") },
    { WdBuiltinStyle.LightShadingAccent4, (WdStyleType.Table, "LightShadingAccent4", "Light Shading Accent 4") },
    { WdBuiltinStyle.LightListAccent4, (WdStyleType.Table, "LightListAccent4", "Light List Accent 4") },
    { WdBuiltinStyle.LightGridAccent4, (WdStyleType.Table, "LightGridAccent4", "Light Grid Accent 4") },
    { WdBuiltinStyle.MediumShading1Accent4, (WdStyleType.Table, "MediumShading1Accent4", "Medium Shading 1 Accent 4") },
    { WdBuiltinStyle.MediumShading2Accent4, (WdStyleType.Table, "MediumShading2Accent4", "Medium Shading 2 Accent 4") },
    { WdBuiltinStyle.MediumList1Accent4, (WdStyleType.Table, "MediumList1Accent4", "Medium List 1 Accent 4") },
    { WdBuiltinStyle.MediumList2Accent4, (WdStyleType.Table, "MediumList2Accent4", "Medium List 2 Accent 4") },
    { WdBuiltinStyle.MediumGrid1Accent4, (WdStyleType.Table, "MediumGrid1Accent4", "Medium Grid 1 Accent 4") },
    { WdBuiltinStyle.MediumGrid2Accent4, (WdStyleType.Table, "MediumGrid2Accent4", "Medium Grid 2 Accent 4") },
    { WdBuiltinStyle.MediumGrid3Accent4, (WdStyleType.Table, "MediumGrid3Accent4", "Medium Grid 3 Accent 4") },
    { WdBuiltinStyle.DarkListAccent4, (WdStyleType.Table, "DarkListAccent4", "Dark List Accent 4") },
    { WdBuiltinStyle.ColorfulShadingAccent4, (WdStyleType.Table, "ColorfulShadingAccent4", "Colorful Shading Accent 4") },
    { WdBuiltinStyle.ColorfulListAccent4, (WdStyleType.Table, "ColorfulListAccent4", "Colorful List Accent 4") },
    { WdBuiltinStyle.ColorfulGridAccent4, (WdStyleType.Table, "ColorfulGridAccent4", "Colorful Grid Accent 4") },
    { WdBuiltinStyle.LightShadingAccent5, (WdStyleType.Table, "LightShadingAccent5", "Light Shading Accent 5") },
    { WdBuiltinStyle.LightListAccent5, (WdStyleType.Table, "LightListAccent5", "Light List Accent 5") },
    { WdBuiltinStyle.LightGridAccent5, (WdStyleType.Table, "LightGridAccent5", "Light Grid Accent 5") },
    { WdBuiltinStyle.MediumShading1Accent5, (WdStyleType.Table, "MediumShading1Accent5", "Medium Shading 1 Accent 5") },
    { WdBuiltinStyle.MediumShading2Accent5, (WdStyleType.Table, "MediumShading2Accent5", "Medium Shading 2 Accent 5") },
    { WdBuiltinStyle.MediumList1Accent5, (WdStyleType.Table, "MediumList1Accent5", "Medium List 1 Accent 5") },
    { WdBuiltinStyle.MediumList2Accent5, (WdStyleType.Table, "MediumList2Accent5", "Medium List 2 Accent 5") },
    { WdBuiltinStyle.MediumGrid1Accent5, (WdStyleType.Table, "MediumGrid1Accent5", "Medium Grid 1 Accent 5") },
    { WdBuiltinStyle.MediumGrid2Accent5, (WdStyleType.Table, "MediumGrid2Accent5", "Medium Grid 2 Accent 5") },
    { WdBuiltinStyle.MediumGrid3Accent5, (WdStyleType.Table, "MediumGrid3Accent5", "Medium Grid 3 Accent 5") },
    { WdBuiltinStyle.DarkListAccent5, (WdStyleType.Table, "DarkListAccent5", "Dark List Accent 5") },
    { WdBuiltinStyle.ColorfulShadingAccent5, (WdStyleType.Table, "ColorfulShadingAccent5", "Colorful Shading Accent 5") },
    { WdBuiltinStyle.ColorfulListAccent5, (WdStyleType.Table, "ColorfulListAccent5", "Colorful List Accent 5") },
    { WdBuiltinStyle.ColorfulGridAccent5, (WdStyleType.Table, "ColorfulGridAccent5", "Colorful Grid Accent 5") },
    { WdBuiltinStyle.LightShadingAccent6, (WdStyleType.Table, "LightShadingAccent6", "Light Shading Accent 6") },
    { WdBuiltinStyle.LightListAccent6, (WdStyleType.Table, "LightListAccent6", "Light List Accent 6") },
    { WdBuiltinStyle.LightGridAccent6, (WdStyleType.Table, "LightGridAccent6", "Light Grid Accent 6") },
    { WdBuiltinStyle.MediumShading1Accent6, (WdStyleType.Table, "MediumShading1Accent6", "Medium Shading 1 Accent 6") },
    { WdBuiltinStyle.MediumShading2Accent6, (WdStyleType.Table, "MediumShading2Accent6", "Medium Shading 2 Accent 6") },
    { WdBuiltinStyle.MediumList1Accent6, (WdStyleType.Table, "MediumList1Accent6", "Medium List 1 Accent 6") },
    { WdBuiltinStyle.MediumList2Accent6, (WdStyleType.Table, "MediumList2Accent6", "Medium List 2 Accent 6") },
    { WdBuiltinStyle.MediumGrid1Accent6, (WdStyleType.Table, "MediumGrid1Accent6", "Medium Grid 1 Accent 6") },
    { WdBuiltinStyle.MediumGrid2Accent6, (WdStyleType.Table, "MediumGrid2Accent6", "Medium Grid 2 Accent 6") },
    { WdBuiltinStyle.MediumGrid3Accent6, (WdStyleType.Table, "MediumGrid3Accent6", "Medium Grid 3 Accent 6") },
    { WdBuiltinStyle.DarkListAccent6, (WdStyleType.Table, "DarkListAccent6", "Dark List Accent 6") },
    { WdBuiltinStyle.ColorfulShadingAccent6, (WdStyleType.Table, "ColorfulShadingAccent6", "Colorful Shading Accent 6") },
    { WdBuiltinStyle.ColorfulListAccent6, (WdStyleType.Table, "ColorfulListAccent6", "Colorful List Accent 6") },
    { WdBuiltinStyle.ColorfulGridAccent6, (WdStyleType.Table, "ColorfulGridAccent6", "Colorful Grid Accent 6") },
    { WdBuiltinStyle.SubtleEmphasis, (WdStyleType.Character, "SubtleEmphasis", "Subtle Emphasis") },
    { WdBuiltinStyle.IntenseEmphasis, (WdStyleType.Character, "IntenseEmphasis", "Intense Emphasis") },
    { WdBuiltinStyle.SubtleReference, (WdStyleType.Character, "SubtleReference", "Subtle Reference") },
    { WdBuiltinStyle.IntenseReference, (WdStyleType.Character, "IntenseReference", "Intense Reference") },
    { WdBuiltinStyle.BookTitle, (WdStyleType.Character, "BookTitle", "Book Title") },
    { WdBuiltinStyle.Bibliography, (WdStyleType.Paragraph, "Bibliography", "Bibliography") },
    { WdBuiltinStyle.TocHeading, (WdStyleType.Paragraph, "TocHeading", "TOC Heading") },
    { WdBuiltinStyle.PlainTable1, (WdStyleType.Table, "PlainTable1", "Plain Table 1") },
    { WdBuiltinStyle.PlainTable2, (WdStyleType.Table, "PlainTable2", "Plain Table 2") },
    { WdBuiltinStyle.PlainTable3, (WdStyleType.Table, "PlainTable3", "Plain Table 3") },
    { WdBuiltinStyle.PlainTable4, (WdStyleType.Table, "PlainTable4", "Plain Table 4") },
    { WdBuiltinStyle.PlainTable5, (WdStyleType.Table, "PlainTable5", "Plain Table 5") },
    { WdBuiltinStyle.GridTableLight, (WdStyleType.Table, "GridTableLight", "Grid Table Light") },
    { WdBuiltinStyle.GridTable1Light, (WdStyleType.Table, "GridTable1Light", "Grid Table 1 Light") },
    { WdBuiltinStyle.GridTable2, (WdStyleType.Table, "GridTable2", "Grid Table 2") },
    { WdBuiltinStyle.GridTable3, (WdStyleType.Table, "GridTable3", "Grid Table 3") },
    { WdBuiltinStyle.GridTable4, (WdStyleType.Table, "GridTable4", "Grid Table 4") },
    { WdBuiltinStyle.GridTable5Dark, (WdStyleType.Table, "GridTable5Dark", "Grid Table 5 Dark") },
    { WdBuiltinStyle.GridTable6Colorful, (WdStyleType.Table, "GridTable6Colorful", "Grid Table 6 Colorful") },
    { WdBuiltinStyle.GridTable7Colorful, (WdStyleType.Table, "GridTable7Colorful", "Grid Table 7 Colorful") },
    { WdBuiltinStyle.GridTable1LightAccent1, (WdStyleType.Table, "GridTable1LightAccent1", "Grid Table 1 Light Accent 1") },
    { WdBuiltinStyle.GridTable2Accent1, (WdStyleType.Table, "GridTable2Accent1", "Grid Table 2 Accent 1") },
    { WdBuiltinStyle.GridTable3Accent1, (WdStyleType.Table, "GridTable3Accent1", "Grid Table 3 Accent 1") },
    { WdBuiltinStyle.GridTable4Accent1, (WdStyleType.Table, "GridTable4Accent1", "Grid Table 4 Accent 1") },
    { WdBuiltinStyle.GridTable5DarkAccent1, (WdStyleType.Table, "GridTable5DarkAccent1", "Grid Table 5 Dark Accent 1") },
    { WdBuiltinStyle.GridTable6ColorfulAccent1, (WdStyleType.Table, "GridTable6ColorfulAccent1", "Grid Table 6 Colorful Accent 1") },
    { WdBuiltinStyle.GridTable7ColorfulAccent1, (WdStyleType.Table, "GridTable7ColorfulAccent1", "Grid Table 7 Colorful Accent 1") },
    { WdBuiltinStyle.GridTable1LightAccent2, (WdStyleType.Table, "GridTable1LightAccent2", "Grid Table 1 Light Accent 2") },
    { WdBuiltinStyle.GridTable2Accent2, (WdStyleType.Table, "GridTable2Accent2", "Grid Table 2 Accent 2") },
    { WdBuiltinStyle.GridTable3Accent2, (WdStyleType.Table, "GridTable3Accent2", "Grid Table 3 Accent 2") },
    { WdBuiltinStyle.GridTable4Accent2, (WdStyleType.Table, "GridTable4Accent2", "Grid Table 4 Accent 2") },
    { WdBuiltinStyle.GridTable5DarkAccent2, (WdStyleType.Table, "GridTable5DarkAccent2", "Grid Table 5 Dark Accent 2") },
    { WdBuiltinStyle.GridTable6ColorfulAccent2, (WdStyleType.Table, "GridTable6ColorfulAccent2", "Grid Table 6 Colorful Accent 2") },
    { WdBuiltinStyle.GridTable7ColorfulAccent2, (WdStyleType.Table, "GridTable7ColorfulAccent2", "Grid Table 7 Colorful Accent 2") },
    { WdBuiltinStyle.GridTable1LightAccent3, (WdStyleType.Table, "GridTable1LightAccent3", "Grid Table 1 Light Accent 3") },
    { WdBuiltinStyle.GridTable2Accent3, (WdStyleType.Table, "GridTable2Accent3", "Grid Table 2 Accent 3") },
    { WdBuiltinStyle.GridTable3Accent3, (WdStyleType.Table, "GridTable3Accent3", "Grid Table 3 Accent 3") },
    { WdBuiltinStyle.GridTable4Accent3, (WdStyleType.Table, "GridTable4Accent3", "Grid Table 4 Accent 3") },
    { WdBuiltinStyle.GridTable5DarkAccent3, (WdStyleType.Table, "GridTable5DarkAccent3", "Grid Table 5 Dark Accent 3") },
    { WdBuiltinStyle.GridTable6ColorfulAccent3, (WdStyleType.Table, "GridTable6ColorfulAccent3", "Grid Table 6 Colorful Accent 3") },
    { WdBuiltinStyle.GridTable7ColorfulAccent3, (WdStyleType.Table, "GridTable7ColorfulAccent3", "Grid Table 7 Colorful Accent 3") },
    { WdBuiltinStyle.GridTable1LightAccent4, (WdStyleType.Table, "GridTable1LightAccent4", "Grid Table 1 Light Accent 4") },
    { WdBuiltinStyle.GridTable2Accent4, (WdStyleType.Table, "GridTable2Accent4", "Grid Table 2 Accent 4") },
    { WdBuiltinStyle.GridTable3Accent4, (WdStyleType.Table, "GridTable3Accent4", "Grid Table 3 Accent 4") },
    { WdBuiltinStyle.GridTable4Accent4, (WdStyleType.Table, "GridTable4Accent4", "Grid Table 4 Accent 4") },
    { WdBuiltinStyle.GridTable5DarkAccent4, (WdStyleType.Table, "GridTable5DarkAccent4", "Grid Table 5 Dark Accent 4") },
    { WdBuiltinStyle.GridTable6ColorfulAccent4, (WdStyleType.Table, "GridTable6ColorfulAccent4", "Grid Table 6 Colorful Accent 4") },
    { WdBuiltinStyle.GridTable7ColorfulAccent4, (WdStyleType.Table, "GridTable7ColorfulAccent4", "Grid Table 7 Colorful Accent 4") },
    { WdBuiltinStyle.GridTable1LightAccent5, (WdStyleType.Table, "GridTable1LightAccent5", "Grid Table 1 Light Accent 5") },
    { WdBuiltinStyle.GridTable2Accent5, (WdStyleType.Table, "GridTable2Accent5", "Grid Table 2 Accent 5") },
    { WdBuiltinStyle.GridTable3Accent5, (WdStyleType.Table, "GridTable3Accent5", "Grid Table 3 Accent 5") },
    { WdBuiltinStyle.GridTable4Accent5, (WdStyleType.Table, "GridTable4Accent5", "Grid Table 4 Accent 5") },
    { WdBuiltinStyle.GridTable5DarkAccent5, (WdStyleType.Table, "GridTable5DarkAccent5", "Grid Table 5 Dark Accent 5") },
    { WdBuiltinStyle.GridTable6ColorfulAccent5, (WdStyleType.Table, "GridTable6ColorfulAccent5", "Grid Table 6 Colorful Accent 5") },
    { WdBuiltinStyle.GridTable7ColorfulAccent5, (WdStyleType.Table, "GridTable7ColorfulAccent5", "Grid Table 7 Colorful Accent 5") },
    { WdBuiltinStyle.GridTable1LightAccent6, (WdStyleType.Table, "GridTable1LightAccent6", "Grid Table 1 Light Accent 6") },
    { WdBuiltinStyle.GridTable2Accent6, (WdStyleType.Table, "GridTable2Accent6", "Grid Table 2 Accent 6") },
    { WdBuiltinStyle.GridTable3Accent6, (WdStyleType.Table, "GridTable3Accent6", "Grid Table 3 Accent 6") },
    { WdBuiltinStyle.GridTable4Accent6, (WdStyleType.Table, "GridTable4Accent6", "Grid Table 4 Accent 6") },
    { WdBuiltinStyle.GridTable5DarkAccent6, (WdStyleType.Table, "GridTable5DarkAccent6", "Grid Table 5 Dark Accent 6") },
    { WdBuiltinStyle.GridTable6ColorfulAccent6, (WdStyleType.Table, "GridTable6ColorfulAccent6", "Grid Table 6 Colorful Accent 6") },
    { WdBuiltinStyle.GridTable7ColorfulAccent6, (WdStyleType.Table, "GridTable7ColorfulAccent6", "Grid Table 7 Colorful Accent 6") },
    { WdBuiltinStyle.ListTable1Light, (WdStyleType.Table, "ListTable1Light", "List Table 1 Light") },
    { WdBuiltinStyle.ListTable2, (WdStyleType.Table, "ListTable2", "List Table 2") },
    { WdBuiltinStyle.ListTable3, (WdStyleType.Table, "ListTable3", "List Table 3") },
    { WdBuiltinStyle.ListTable4, (WdStyleType.Table, "ListTable4", "List Table 4") },
    { WdBuiltinStyle.ListTable5Dark, (WdStyleType.Table, "ListTable5Dark", "List Table 5 Dark") },
    { WdBuiltinStyle.ListTable6Colorful, (WdStyleType.Table, "ListTable6Colorful", "List Table 6 Colorful") },
    { WdBuiltinStyle.ListTable7Colorful, (WdStyleType.Table, "ListTable7Colorful", "List Table 7 Colorful") },
    { WdBuiltinStyle.ListTable1LightAccent1, (WdStyleType.Table, "ListTable1LightAccent1", "List Table 1 Light Accent 1") },
    { WdBuiltinStyle.ListTable2Accent1, (WdStyleType.Table, "ListTable2Accent1", "List Table 2 Accent 1") },
    { WdBuiltinStyle.ListTable3Accent1, (WdStyleType.Table, "ListTable3Accent1", "List Table 3 Accent 1") },
    { WdBuiltinStyle.ListTable4Accent1, (WdStyleType.Table, "ListTable4Accent1", "List Table 4 Accent 1") },
    { WdBuiltinStyle.ListTable5DarkAccent1, (WdStyleType.Table, "ListTable5DarkAccent1", "List Table 5 Dark Accent 1") },
    { WdBuiltinStyle.ListTable6ColorfulAccent1, (WdStyleType.Table, "ListTable6ColorfulAccent1", "List Table 6 Colorful Accent 1") },
    { WdBuiltinStyle.ListTable7ColorfulAccent1, (WdStyleType.Table, "ListTable7ColorfulAccent1", "List Table 7 Colorful Accent 1") },
    { WdBuiltinStyle.ListTable1LightAccent2, (WdStyleType.Table, "ListTable1LightAccent2", "List Table 1 Light Accent 2") },
    { WdBuiltinStyle.ListTable2Accent2, (WdStyleType.Table, "ListTable2Accent2", "List Table 2 Accent 2") },
    { WdBuiltinStyle.ListTable3Accent2, (WdStyleType.Table, "ListTable3Accent2", "List Table 3 Accent 2") },
    { WdBuiltinStyle.ListTable4Accent2, (WdStyleType.Table, "ListTable4Accent2", "List Table 4 Accent 2") },
    { WdBuiltinStyle.ListTable5DarkAccent2, (WdStyleType.Table, "ListTable5DarkAccent2", "List Table 5 Dark Accent 2") },
    { WdBuiltinStyle.ListTable6ColorfulAccent2, (WdStyleType.Table, "ListTable6ColorfulAccent2", "List Table 6 Colorful Accent 2") },
    { WdBuiltinStyle.ListTable7ColorfulAccent2, (WdStyleType.Table, "ListTable7ColorfulAccent2", "List Table 7 Colorful Accent 2") },
    { WdBuiltinStyle.ListTable1LightAccent3, (WdStyleType.Table, "ListTable1LightAccent3", "List Table 1 Light Accent 3") },
    { WdBuiltinStyle.ListTable2Accent3, (WdStyleType.Table, "ListTable2Accent3", "List Table 2 Accent 3") },
    { WdBuiltinStyle.ListTable3Accent3, (WdStyleType.Table, "ListTable3Accent3", "List Table 3 Accent 3") },
    { WdBuiltinStyle.ListTable4Accent3, (WdStyleType.Table, "ListTable4Accent3", "List Table 4 Accent 3") },
    { WdBuiltinStyle.ListTable5DarkAccent3, (WdStyleType.Table, "ListTable5DarkAccent3", "List Table 5 Dark Accent 3") },
    { WdBuiltinStyle.ListTable6ColorfulAccent3, (WdStyleType.Table, "ListTable6ColorfulAccent3", "List Table 6 Colorful Accent 3") },
    { WdBuiltinStyle.ListTable7ColorfulAccent3, (WdStyleType.Table, "ListTable7ColorfulAccent3", "List Table 7 Colorful Accent 3") },
    { WdBuiltinStyle.ListTable1LightAccent4, (WdStyleType.Table, "ListTable1LightAccent4", "List Table 1 Light Accent 4") },
    { WdBuiltinStyle.ListTable2Accent4, (WdStyleType.Table, "ListTable2Accent4", "List Table 2 Accent 4") },
    { WdBuiltinStyle.ListTable3Accent4, (WdStyleType.Table, "ListTable3Accent4", "List Table 3 Accent 4") },
    { WdBuiltinStyle.ListTable4Accent4, (WdStyleType.Table, "ListTable4Accent4", "List Table 4 Accent 4") },
    { WdBuiltinStyle.ListTable5DarkAccent4, (WdStyleType.Table, "ListTable5DarkAccent4", "List Table 5 Dark Accent 4") },
    { WdBuiltinStyle.ListTable6ColorfulAccent4, (WdStyleType.Table, "ListTable6ColorfulAccent4", "List Table 6 Colorful Accent 4") },
    { WdBuiltinStyle.ListTable7ColorfulAccent4, (WdStyleType.Table, "ListTable7ColorfulAccent4", "List Table 7 Colorful Accent 4") },
    { WdBuiltinStyle.ListTable1LightAccent5, (WdStyleType.Table, "ListTable1LightAccent5", "List Table 1 Light Accent 5") },
    { WdBuiltinStyle.ListTable2Accent5, (WdStyleType.Table, "ListTable2Accent5", "List Table 2 Accent 5") },
    { WdBuiltinStyle.ListTable3Accent5, (WdStyleType.Table, "ListTable3Accent5", "List Table 3 Accent 5") },
    { WdBuiltinStyle.ListTable4Accent5, (WdStyleType.Table, "ListTable4Accent5", "List Table 4 Accent 5") },
    { WdBuiltinStyle.ListTable5DarkAccent5, (WdStyleType.Table, "ListTable5DarkAccent5", "List Table 5 Dark Accent 5") },
    { WdBuiltinStyle.ListTable6ColorfulAccent5, (WdStyleType.Table, "ListTable6ColorfulAccent5", "List Table 6 Colorful Accent 5") },
    { WdBuiltinStyle.ListTable7ColorfulAccent5, (WdStyleType.Table, "ListTable7ColorfulAccent5", "List Table 7 Colorful Accent 5") },
    { WdBuiltinStyle.ListTable1LightAccent6, (WdStyleType.Table, "ListTable1LightAccent6", "List Table 1 Light Accent 6") },
    { WdBuiltinStyle.ListTable2Accent6, (WdStyleType.Table, "ListTable2Accent6", "List Table 2 Accent 6") },
    { WdBuiltinStyle.ListTable3Accent6, (WdStyleType.Table, "ListTable3Accent6", "List Table 3 Accent 6") },
    { WdBuiltinStyle.ListTable4Accent6, (WdStyleType.Table, "ListTable4Accent6", "List Table 4 Accent 6") },
    { WdBuiltinStyle.ListTable5DarkAccent6, (WdStyleType.Table, "ListTable5DarkAccent6", "List Table 5 Dark Accent 6") },
    { WdBuiltinStyle.ListTable6ColorfulAccent6, (WdStyleType.Table, "ListTable6ColorfulAccent6", "List Table 6 Colorful Accent 6") },
    { WdBuiltinStyle.ListTable7ColorfulAccent6, (WdStyleType.Table, "ListTable7ColorfulAccent6", "List Table 7 Colorful Accent 6") },
    { WdBuiltinStyle.Mention, (WdStyleType.Character, "Mention", "Mention") },
    { WdBuiltinStyle.SmartHyperlink, (WdStyleType.Character, "SmartHyperlink", "Smart Hyperlink") },
    { WdBuiltinStyle.Hashtag, (WdStyleType.Character, "Hashtag", "Hashtag") },
    { WdBuiltinStyle.UnresolvedMention, (WdStyleType.Character, "UnresolvedMention", "Unresolved Mention") },
    { WdBuiltinStyle.SmartLink, (WdStyleType.Character, "SmartLink", "Smart Link") },
 };


  private static Dictionary<string, WdBuiltinStyle> StyleNamesDictionary
  {
    get
    {
      if (_styleNamesDictionary.Count < KnownStyles.Count)
      {
        foreach (var item in KnownStyles)
        {
          _styleNamesDictionary.Add(item.Value.Name, item.Key);
        }
      }
      return _styleNamesDictionary;
    }
  }

  private static readonly Dictionary<string, WdBuiltinStyle> _styleNamesDictionary
    = new (StringComparer.InvariantCultureIgnoreCase)
    {
      { "Outline List 1", WdBuiltinStyle.OutlineList1 },
      { "Outline List 2", WdBuiltinStyle.OutlineList2 },
      { "Outline List 3", WdBuiltinStyle.OutlineList3 },
      { "Annotation Text", WdBuiltinStyle.CommentText},
      { "Annotation Subject", WdBuiltinStyle.CommentSubject},
      { "Annotation Reference", WdBuiltinStyle.CommentReference},
      { "HTML Top of Form", WdBuiltinStyle.zTopOfForm },
      { "HTML Bottom of Form", WdBuiltinStyle.zBottomOfForm },
    };
}

