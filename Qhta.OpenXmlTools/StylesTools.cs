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
  /// Get the count of the known styles definitions.
  /// </summary>
  /// <returns></returns>
  public static int GetBuildInStylesCount()
  => KnownStyles.Count;

  /// <summary>
  /// Get the name of the known style definitions.
  /// </summary>
  /// <param name="sc">Number of the style</param>
  /// <returns></returns>
  public static string GetBuiltInStyleName(int sc)
    => KnownStyles.Values.ElementAt(sc).Name;

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
  public static WStyleType GetType(this DXW.Styles styles, string styleName)
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
  private static WStyleType StyleValuesToStyleType(DXW.StyleValues value)
  {
    if (value == DXW.StyleValues.Paragraph)
      return WStyleType.Paragraph;
    if (value == DXW.StyleValues.Character)
      return WStyleType.Character;
    if (value == DXW.StyleValues.Table)
      return WStyleType.Table;
    if (value == DXW.StyleValues.Numbering)
      return WStyleType.Numbering;
    return 0;
  }

  /// <summary>
  /// Converts a style type to a style value.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  private static DXW.StyleValues? StyleTypeToStyleValues(WStyleType type)
  {
    if (type == WStyleType.Paragraph)
      return DXW.StyleValues.Paragraph;
    if (type == WStyleType.Character)
      return DXW.StyleValues.Character;
    if (type == WStyleType.Table)
      return DXW.StyleValues.Table;
    if (type == WStyleType.Numbering)
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
    BuiltinStyles = new Dictionary<WBuiltinStyle, Style>();
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
  private static Dictionary<WBuiltinStyle, Style>? BuiltinStyles = null;

  private static readonly Dictionary<WBuiltinStyle, (WStyleType Type, string Ident, string Name)> KnownStyles = new ()
    {
    { WBuiltinStyle.Normal, (WStyleType.Paragraph, "Normal", "Normal") },
    { WBuiltinStyle.Heading1, (WStyleType.Paragraph, "Heading1", "Heading 1") },
    { WBuiltinStyle.Heading1Char, (WStyleType.Linked, "Heading1Char", "Heading 1 Char") },
    { WBuiltinStyle.Heading2, (WStyleType.Paragraph, "Heading2", "Heading 2") },
    { WBuiltinStyle.Heading2Char, (WStyleType.Linked, "Heading2Char", "Heading 2 Char") },
    { WBuiltinStyle.Heading3, (WStyleType.Paragraph, "Heading3", "Heading 3") },
    { WBuiltinStyle.Heading3Char, (WStyleType.Linked, "Heading3Char", "Heading 3 Char") },
    { WBuiltinStyle.Heading4, (WStyleType.Paragraph, "Heading4", "Heading 4") },
    { WBuiltinStyle.Heading4Char, (WStyleType.Linked, "Heading4Char", "Heading 4 Char") },
    { WBuiltinStyle.Heading5, (WStyleType.Paragraph, "Heading5", "Heading 5") },
    { WBuiltinStyle.Heading5Char, (WStyleType.Linked, "Heading5Char", "Heading 5 Char") },
    { WBuiltinStyle.Heading6, (WStyleType.Paragraph, "Heading6", "Heading 6") },
    { WBuiltinStyle.Heading6Char, (WStyleType.Linked, "Heading6Char", "Heading 6 Char") },
    { WBuiltinStyle.Heading7, (WStyleType.Paragraph, "Heading7", "Heading 7") },
    { WBuiltinStyle.Heading7Char, (WStyleType.Linked, "Heading7Char", "Heading 7 Char") },
    { WBuiltinStyle.Heading8, (WStyleType.Paragraph, "Heading8", "Heading 8") },
    { WBuiltinStyle.Heading8Char, (WStyleType.Linked, "Heading8Char", "Heading 8 Char") },
    { WBuiltinStyle.Heading9, (WStyleType.Paragraph, "Heading9", "Heading 9") },
    { WBuiltinStyle.Heading9Char, (WStyleType.Linked, "Heading9Char", "Heading 9 Char") },
    { WBuiltinStyle.Index1, (WStyleType.Paragraph, "Index1", "Index 1") },
    { WBuiltinStyle.Index2, (WStyleType.Paragraph, "Index2", "Index 2") },
    { WBuiltinStyle.Index3, (WStyleType.Paragraph, "Index3", "Index 3") },
    { WBuiltinStyle.Index4, (WStyleType.Paragraph, "Index4", "Index 4") },
    { WBuiltinStyle.Index5, (WStyleType.Paragraph, "Index5", "Index 5") },
    { WBuiltinStyle.Index6, (WStyleType.Paragraph, "Index6", "Index 6") },
    { WBuiltinStyle.Index7, (WStyleType.Paragraph, "Index7", "Index 7") },
    { WBuiltinStyle.Index8, (WStyleType.Paragraph, "Index8", "Index 8") },
    { WBuiltinStyle.Index9, (WStyleType.Paragraph, "Index9", "Index 9") },
    { WBuiltinStyle.TOC1, (WStyleType.Paragraph, "TOC1", "TOC 1") },
    { WBuiltinStyle.TOC2, (WStyleType.Paragraph, "TOC2", "TOC 2") },
    { WBuiltinStyle.TOC3, (WStyleType.Paragraph, "TOC3", "TOC 3") },
    { WBuiltinStyle.TOC4, (WStyleType.Paragraph, "TOC4", "TOC 4") },
    { WBuiltinStyle.TOC5, (WStyleType.Paragraph, "TOC5", "TOC 5") },
    { WBuiltinStyle.TOC6, (WStyleType.Paragraph, "TOC6", "TOC 6") },
    { WBuiltinStyle.TOC7, (WStyleType.Paragraph, "TOC7", "TOC 7") },
    { WBuiltinStyle.TOC8, (WStyleType.Paragraph, "TOC8", "TOC 8") },
    { WBuiltinStyle.TOC9, (WStyleType.Paragraph, "TOC9", "TOC 9") },
    { WBuiltinStyle.NormalIndent, (WStyleType.Paragraph, "NormalIndent", "Normal Indent") },
    { WBuiltinStyle.FootnoteText, (WStyleType.Paragraph, "FootnoteText", "Footnote Text") },
    { WBuiltinStyle.FootnoteTextChar, (WStyleType.Linked, "FootnoteTextChar", "Footnote Text Char") },
    { WBuiltinStyle.CommentText, (WStyleType.Paragraph, "CommentText", "Comment Text") },
    { WBuiltinStyle.CommentTextChar, (WStyleType.Linked, "CommentTextChar", "Comment Text Char") },
    { WBuiltinStyle.Header, (WStyleType.Paragraph, "Header", "Header") },
    { WBuiltinStyle.HeaderChar, (WStyleType.Linked, "HeaderChar", "Header Char") },
    { WBuiltinStyle.Footer, (WStyleType.Paragraph, "Footer", "Footer") },
    { WBuiltinStyle.FooterChar, (WStyleType.Linked, "FooterChar", "Footer Char") },
    { WBuiltinStyle.IndexHeading, (WStyleType.Paragraph, "IndexHeading", "Index Heading") },
    { WBuiltinStyle.Caption, (WStyleType.Paragraph, "Caption", "Caption") },
    { WBuiltinStyle.TableOfFigures, (WStyleType.Paragraph, "TableOfFigures", "Table of Figures") },
    { WBuiltinStyle.EnvelopeAddress, (WStyleType.Paragraph, "EnvelopeAddress", "Envelope Address") },
    { WBuiltinStyle.EnvelopeReturn, (WStyleType.Paragraph, "EnvelopeReturn", "Envelope Return") },
    { WBuiltinStyle.FootnoteReference, (WStyleType.Character, "FootnoteReference", "Footnote Reference") },
    { WBuiltinStyle.CommentReference, (WStyleType.Character, "CommentReference", "Comment Reference") },
    { WBuiltinStyle.LineNumber, (WStyleType.Character, "LineNumber", "Line Number") },
    { WBuiltinStyle.PageNumber, (WStyleType.Character, "PageNumber", "Page Number") },
    { WBuiltinStyle.EndnoteReference, (WStyleType.Character, "EndnoteReference", "Endnote Reference") },
    { WBuiltinStyle.EndnoteText, (WStyleType.Paragraph, "EndnoteText", "Endnote Text") },
    { WBuiltinStyle.EndnoteTextChar, (WStyleType.Linked, "EndnoteTextChar", "Endnote Text Char") },
    { WBuiltinStyle.TableOfAuthorities, (WStyleType.Paragraph, "TableOfAuthorities", "Table of Authorities") },
    { WBuiltinStyle.Macro, (WStyleType.Paragraph, "Macro", "Macro") },
    { WBuiltinStyle.MacroChar, (WStyleType.Linked, "MacroChar", "Macro Text Char") },
    { WBuiltinStyle.TOAHeading, (WStyleType.Paragraph, "TOAHeading", "TOA Heading") },
    { WBuiltinStyle.List, (WStyleType.Paragraph, "List", "List") },
    { WBuiltinStyle.ListBullet, (WStyleType.Paragraph, "ListBullet", "List Bullet") },
    { WBuiltinStyle.ListNumber, (WStyleType.Paragraph, "ListNumber", "List Number") },
    { WBuiltinStyle.List2, (WStyleType.Paragraph, "List2", "List 2") },
    { WBuiltinStyle.List3, (WStyleType.Paragraph, "List3", "List 3") },
    { WBuiltinStyle.List4, (WStyleType.Paragraph, "List4", "List 4") },
    { WBuiltinStyle.List5, (WStyleType.Paragraph, "List5", "List 5") },
    { WBuiltinStyle.ListBullet2, (WStyleType.Paragraph, "ListBullet2", "List Bullet 2") },
    { WBuiltinStyle.ListBullet3, (WStyleType.Paragraph, "ListBullet3", "List Bullet 3") },
    { WBuiltinStyle.ListBullet4, (WStyleType.Paragraph, "ListBullet4", "List Bullet 4") },
    { WBuiltinStyle.ListBullet5, (WStyleType.Paragraph, "ListBullet5", "List Bullet 5") },
    { WBuiltinStyle.ListNumber2, (WStyleType.Paragraph, "ListNumber2", "List Number 2") },
    { WBuiltinStyle.ListNumber3, (WStyleType.Paragraph, "ListNumber3", "List Number 3") },
    { WBuiltinStyle.ListNumber4, (WStyleType.Paragraph, "ListNumber4", "List Number 4") },
    { WBuiltinStyle.ListNumber5, (WStyleType.Paragraph, "ListNumber5", "List Number 5") },
    { WBuiltinStyle.Title, (WStyleType.Paragraph, "Title", "Title") },
    { WBuiltinStyle.TitleChar, (WStyleType.Linked, "TitleChar", "Title Char") },
    { WBuiltinStyle.Closing, (WStyleType.Paragraph, "Closing", "Closing") },
    { WBuiltinStyle.ClosingChar, (WStyleType.Linked, "ClosingChar", "Closing Char") },
    { WBuiltinStyle.Signature, (WStyleType.Paragraph, "Signature", "Signature") },
    { WBuiltinStyle.SignatureChar, (WStyleType.Linked, "SignatureChar", "Signature Char") },
    { WBuiltinStyle.DefaultParagraphFont, (WStyleType.Character, "DefaultParagraphFont", "Default Paragraph Font") },
    { WBuiltinStyle.BodyText, (WStyleType.Paragraph, "BodyText", "Body Text") },
    { WBuiltinStyle.BodyTextChar, (WStyleType.Linked, "BodyTextChar", "Body Text Char") },
    { WBuiltinStyle.BodyTextIndent, (WStyleType.Paragraph, "BodyTextIndent", "Body Text Indent") },
    { WBuiltinStyle.BodyTextIndentChar, (WStyleType.Linked, "BodyTextIndentChar", "Body Text Indent Char") },
    { WBuiltinStyle.ListContinue, (WStyleType.Paragraph, "ListContinue", "List Continue") },
    { WBuiltinStyle.ListContinue2, (WStyleType.Paragraph, "ListContinue2", "List Continue 2") },
    { WBuiltinStyle.ListContinue3, (WStyleType.Paragraph, "ListContinue3", "List Continue 3") },
    { WBuiltinStyle.ListContinue4, (WStyleType.Paragraph, "ListContinue4", "List Continue 4") },
    { WBuiltinStyle.ListContinue5, (WStyleType.Paragraph, "ListContinue5", "List Continue 5") },
    { WBuiltinStyle.MessageHeader, (WStyleType.Paragraph, "MessageHeader", "Message Header") },
    { WBuiltinStyle.MessageHeaderChar, (WStyleType.Linked, "MessageHeaderChar", "Message Header Char") },
    { WBuiltinStyle.Subtitle, (WStyleType.Paragraph, "Subtitle", "Subtitle") },
    { WBuiltinStyle.SubtitleChar, (WStyleType.Linked, "SubtitleChar", "Subtitle Char") },
    { WBuiltinStyle.Salutation, (WStyleType.Paragraph, "Salutation", "Salutation") },
    { WBuiltinStyle.SalutationChar, (WStyleType.Linked, "SalutationChar", "Salutation Char") },
    { WBuiltinStyle.Date, (WStyleType.Paragraph, "Date", "Date") },
    { WBuiltinStyle.DateChar, (WStyleType.Linked, "DateChar", "Date Char") },
    { WBuiltinStyle.BodyTextFirstIndent, (WStyleType.Paragraph, "BodyTextFirstIndent", "Body Text First Indent") },
    { WBuiltinStyle.BodyTextFirstIndentChar, (WStyleType.Linked, "BodyTextFirstIndentChar", "Body Text First Indent Char") },
    { WBuiltinStyle.BodyTextFirstIndent2, (WStyleType.Paragraph, "BodyTextFirstIndent2", "Body Text First Indent 2") },
    { WBuiltinStyle.BodyTextFirstIndent2Char, (WStyleType.Linked, "BodyTextFirstIndent2Char", "Body Text First Indent 2 Char") },
    { WBuiltinStyle.NoteHeading, (WStyleType.Paragraph, "NoteHeading", "Note Heading") },
    { WBuiltinStyle.NoteHeadingChar, (WStyleType.Linked, "NoteHeadingChar", "Note Heading Char") },
    { WBuiltinStyle.BodyText2, (WStyleType.Paragraph, "BodyText2", "Body Text 2") },
    { WBuiltinStyle.BodyText2Char, (WStyleType.Linked, "BodyText2Char", "Body Text 2 Char") },
    { WBuiltinStyle.BodyText3, (WStyleType.Paragraph, "BodyText3", "Body Text 3") },
    { WBuiltinStyle.BodyText3Char, (WStyleType.Linked, "BodyText3Char", "Body Text 3 Char") },
    { WBuiltinStyle.BodyTextIndent2, (WStyleType.Paragraph, "BodyTextIndent2", "Body Text Indent 2") },
    { WBuiltinStyle.BodyTextIndent2Char, (WStyleType.Linked, "BodyTextIndent2Char", "Body Text Indent 2 Char") },
    { WBuiltinStyle.BodyTextIndent3, (WStyleType.Paragraph, "BodyTextIndent3", "Body Text Indent 3") },
    { WBuiltinStyle.BodyTextIndent3Char, (WStyleType.Linked, "BodyTextIndent3Char", "Body Text Indent 3 Char") },
    { WBuiltinStyle.BlockText, (WStyleType.Paragraph, "BlockText", "Block Text") },
    { WBuiltinStyle.Hyperlink, (WStyleType.Character, "Hyperlink", "Hyperlink") },
    { WBuiltinStyle.FollowedHyperlink, (WStyleType.Character, "FollowedHyperlink", "FollowedHyperlink") },
    { WBuiltinStyle.Strong, (WStyleType.Character, "Strong", "Strong") },
    { WBuiltinStyle.Emphasis, (WStyleType.Character, "Emphasis", "Emphasis") },
    { WBuiltinStyle.DocumentMap, (WStyleType.Paragraph, "DocumentMap", "Document Map") },
    { WBuiltinStyle.DocumentMapChar, (WStyleType.Linked, "DocumentMapChar", "Document Map Char") },
    { WBuiltinStyle.PlainText, (WStyleType.Paragraph, "PlainText", "Plain Text") },
    { WBuiltinStyle.PlainTextChar, (WStyleType.Linked, "PlainTextChar", "Plain Text Char") },
    { WBuiltinStyle.EmailSignature, (WStyleType.Paragraph, "EmailSignature", "E-mail Signature") },
    { WBuiltinStyle.EmailSignatureChar, (WStyleType.Linked, "EmailSignatureChar", "E-mail Signature Char") },
    { WBuiltinStyle.zTopOfForm, (WStyleType.Paragraph, "zTopOfForm", "z-Top of Form") },
    { WBuiltinStyle.zTopOfFormChar, (WStyleType.Linked, "zTopOfFormChar", "z-Top of Form Char") },
    { WBuiltinStyle.zBottomOfForm, (WStyleType.Paragraph, "zBottomOfForm", "z-Bottom of Form") },
    { WBuiltinStyle.zBottomOfFormChar, (WStyleType.Linked, "zBottomOfFormChar", "z-Bottom of Form Char") },
    { WBuiltinStyle.NormalWeb, (WStyleType.Paragraph, "NormalWeb", "Normal (Web)") },
    { WBuiltinStyle.HtmlAcronym, (WStyleType.Character, "HtmlAcronym", "HTML Acronym") },
    { WBuiltinStyle.HtmlAddress, (WStyleType.Paragraph, "HtmlAddress", "HTML Address") },
    { WBuiltinStyle.HtmlAddressChar, (WStyleType.Linked, "HtmlAddressChar", "HTML Address Char") },
    { WBuiltinStyle.HtmlCite, (WStyleType.Character, "HtmlCite", "HTML Cite") },
    { WBuiltinStyle.HtmlCode, (WStyleType.Character, "HtmlCode", "HTML Code") },
    { WBuiltinStyle.HtmlDefinition, (WStyleType.Character, "HtmlDefinition", "HTML Definition") },
    { WBuiltinStyle.HtmlKeyboard, (WStyleType.Character, "HtmlKeyboard", "HTML Keyboard") },
    { WBuiltinStyle.HtmlPreformatted, (WStyleType.Paragraph, "HtmlPreformatted", "HTML Preformatted") },
    { WBuiltinStyle.HtmlPreformattedChar, (WStyleType.Linked, "HtmlPreformattedChar", "HTML Preformatted Char") },
    { WBuiltinStyle.HtmlSample, (WStyleType.Character, "HtmlSample", "HTML Sample") },
    { WBuiltinStyle.HtmlTypewriter, (WStyleType.Character, "HtmlTypewriter", "HTML Typewriter") },
    { WBuiltinStyle.HtmlVariable, (WStyleType.Character, "HtmlVariable", "HTML Variable") },
    { WBuiltinStyle.NormalTable, (WStyleType.Table, "NormalTable", "Normal Table") },
    { WBuiltinStyle.CommentSubject, (WStyleType.Paragraph, "CommentSubject", "Comment Subject") },
    { WBuiltinStyle.CommentSubjectChar, (WStyleType.Linked, "CommentSubjectChar", "Comment Subject Char") },
    { WBuiltinStyle.NoList, (WStyleType.Numbering, "NoList", "No List") },
    { WBuiltinStyle.OutlineList1, (WStyleType.Numbering, "OutlineList1", "1 / a / i") },
    { WBuiltinStyle.OutlineList2, (WStyleType.Numbering, "OutlineList2", "1 / 1.1 / 1.1.1") },
    { WBuiltinStyle.OutlineList3, (WStyleType.Numbering, "OutlineList3", "Article / Section") },
    { WBuiltinStyle.TableSimple1, (WStyleType.Table, "TableSimple1", "Table Simple 1") },
    { WBuiltinStyle.TableSimple2, (WStyleType.Table, "TableSimple2", "Table Simple 2") },
    { WBuiltinStyle.TableSimple3, (WStyleType.Table, "TableSimple3", "Table Simple 3") },
    { WBuiltinStyle.TableClassic1, (WStyleType.Table, "TableClassic1", "Table Classic 1") },
    { WBuiltinStyle.TableClassic2, (WStyleType.Table, "TableClassic2", "Table Classic 2") },
    { WBuiltinStyle.TableClassic3, (WStyleType.Table, "TableClassic3", "Table Classic 3") },
    { WBuiltinStyle.TableClassic4, (WStyleType.Table, "TableClassic4", "Table Classic 4") },
    { WBuiltinStyle.TableColorful1, (WStyleType.Table, "TableColorful1", "Table Colorful 1") },
    { WBuiltinStyle.TableColorful2, (WStyleType.Table, "TableColorful2", "Table Colorful 2") },
    { WBuiltinStyle.TableColorful3, (WStyleType.Table, "TableColorful3", "Table Colorful 3") },
    { WBuiltinStyle.TableColumns1, (WStyleType.Table, "TableColumns1", "Table Columns 1") },
    { WBuiltinStyle.TableColumns2, (WStyleType.Table, "TableColumns2", "Table Columns 2") },
    { WBuiltinStyle.TableColumns3, (WStyleType.Table, "TableColumns3", "Table Columns 3") },
    { WBuiltinStyle.TableColumns4, (WStyleType.Table, "TableColumns4", "Table Columns 4") },
    { WBuiltinStyle.TableColumns5, (WStyleType.Table, "TableColumns5", "Table Columns 5") },
    { WBuiltinStyle.TableGrid1, (WStyleType.Table, "TableGrid1", "Table Grid 1") },
    { WBuiltinStyle.TableGrid2, (WStyleType.Table, "TableGrid2", "Table Grid 2") },
    { WBuiltinStyle.TableGrid3, (WStyleType.Table, "TableGrid3", "Table Grid 3") },
    { WBuiltinStyle.TableGrid4, (WStyleType.Table, "TableGrid4", "Table Grid 4") },
    { WBuiltinStyle.TableGrid5, (WStyleType.Table, "TableGrid5", "Table Grid 5") },
    { WBuiltinStyle.TableGrid6, (WStyleType.Table, "TableGrid6", "Table Grid 6") },
    { WBuiltinStyle.TableGrid7, (WStyleType.Table, "TableGrid7", "Table Grid 7") },
    { WBuiltinStyle.TableGrid8, (WStyleType.Table, "TableGrid8", "Table Grid 8") },
    { WBuiltinStyle.TableList1, (WStyleType.Table, "TableList1", "Table List 1") },
    { WBuiltinStyle.TableList2, (WStyleType.Table, "TableList2", "Table List 2") },
    { WBuiltinStyle.TableList3, (WStyleType.Table, "TableList3", "Table List 3") },
    { WBuiltinStyle.TableList4, (WStyleType.Table, "TableList4", "Table List 4") },
    { WBuiltinStyle.TableList5, (WStyleType.Table, "TableList5", "Table List 5") },
    { WBuiltinStyle.TableList6, (WStyleType.Table, "TableList6", "Table List 6") },
    { WBuiltinStyle.TableList7, (WStyleType.Table, "TableList7", "Table List 7") },
    { WBuiltinStyle.TableList8, (WStyleType.Table, "TableList8", "Table List 8") },
    { WBuiltinStyle.Table3dEffects1, (WStyleType.Table, "Table3dEffects1", "Table 3D effects 1") },
    { WBuiltinStyle.Table3dEffects2, (WStyleType.Table, "Table3dEffects2", "Table 3D effects 2") },
    { WBuiltinStyle.Table3dEffects3, (WStyleType.Table, "Table3dEffects3", "Table 3D effects 3") },
    { WBuiltinStyle.TableContemporary, (WStyleType.Table, "TableContemporary", "Table Contemporary") },
    { WBuiltinStyle.TableElegant, (WStyleType.Table, "TableElegant", "Table Elegant") },
    { WBuiltinStyle.TableProfessional, (WStyleType.Table, "TableProfessional", "Table Professional") },
    { WBuiltinStyle.TableSubtle1, (WStyleType.Table, "TableSubtle1", "Table Subtle 1") },
    { WBuiltinStyle.TableSubtle2, (WStyleType.Table, "TableSubtle2", "Table Subtle 2") },
    { WBuiltinStyle.TableWeb1, (WStyleType.Table, "TableWeb1", "Table Web 1") },
    { WBuiltinStyle.TableWeb2, (WStyleType.Table, "TableWeb2", "Table Web 2") },
    { WBuiltinStyle.TableWeb3, (WStyleType.Table, "TableWeb3", "Table Web 3") },
    { WBuiltinStyle.BalloonText, (WStyleType.Paragraph, "BalloonText", "Balloon Text") },
    { WBuiltinStyle.BalloonTextChar, (WStyleType.Linked, "BalloonTextChar", "Balloon Text Char") },
    { WBuiltinStyle.TableGrid, (WStyleType.Table, "TableGrid", "Table Grid") },
    { WBuiltinStyle.TableTheme, (WStyleType.Table, "TableTheme", "Table Theme") },
    { WBuiltinStyle.PlaceholderText, (WStyleType.Character, "PlaceholderText", "Placeholder Text") },
    { WBuiltinStyle.NoSpacing, (WStyleType.Paragraph, "NoSpacing", "No Spacing") },
    { WBuiltinStyle.LightShading, (WStyleType.Table, "LightShading", "Light Shading") },
    { WBuiltinStyle.LightList, (WStyleType.Table, "LightList", "Light List") },
    { WBuiltinStyle.LightGrid, (WStyleType.Table, "LightGrid", "Light Grid") },
    { WBuiltinStyle.MediumShading1, (WStyleType.Table, "MediumShading1", "Medium Shading 1") },
    { WBuiltinStyle.MediumShading2, (WStyleType.Table, "MediumShading2", "Medium Shading 2") },
    { WBuiltinStyle.MediumList1, (WStyleType.Table, "MediumList1", "Medium List 1") },
    { WBuiltinStyle.MediumList2, (WStyleType.Table, "MediumList2", "Medium List 2") },
    { WBuiltinStyle.MediumGrid1, (WStyleType.Table, "MediumGrid1", "Medium Grid 1") },
    { WBuiltinStyle.MediumGrid2, (WStyleType.Table, "MediumGrid2", "Medium Grid 2") },
    { WBuiltinStyle.MediumGrid3, (WStyleType.Table, "MediumGrid3", "Medium Grid 3") },
    { WBuiltinStyle.DarkList, (WStyleType.Table, "DarkList", "Dark List") },
    { WBuiltinStyle.ColorfulShading, (WStyleType.Table, "ColorfulShading", "Colorful Shading") },
    { WBuiltinStyle.ColorfulList, (WStyleType.Table, "ColorfulList", "Colorful List") },
    { WBuiltinStyle.ColorfulGrid, (WStyleType.Table, "ColorfulGrid", "Colorful Grid") },
    { WBuiltinStyle.LightShadingAccent1, (WStyleType.Table, "LightShadingAccent1", "Light Shading Accent 1") },
    { WBuiltinStyle.LightListAccent1, (WStyleType.Table, "LightListAccent1", "Light List Accent 1") },
    { WBuiltinStyle.LightGridAccent1, (WStyleType.Table, "LightGridAccent1", "Light Grid Accent 1") },
    { WBuiltinStyle.MediumShading1Accent1, (WStyleType.Table, "MediumShading1Accent1", "Medium Shading 1 Accent 1") },
    { WBuiltinStyle.MediumShading2Accent1, (WStyleType.Table, "MediumShading2Accent1", "Medium Shading 2 Accent 1") },
    { WBuiltinStyle.MediumList1Accent1, (WStyleType.Table, "MediumList1Accent1", "Medium List 1 Accent 1") },
    { WBuiltinStyle.Revision, (WStyleType.Paragraph, "Revision", "Revision") },
    { WBuiltinStyle.ListParagraph, (WStyleType.Paragraph, "ListParagraph", "List Paragraph") },
    { WBuiltinStyle.Quote, (WStyleType.Paragraph, "Quote", "Quote") },
    { WBuiltinStyle.QuoteChar, (WStyleType.Linked, "QuoteChar", "Quote Char") },
    { WBuiltinStyle.IntenseQuote, (WStyleType.Paragraph, "IntenseQuote", "Intense Quote") },
    { WBuiltinStyle.IntenseQuoteChar, (WStyleType.Linked, "IntenseQuoteChar", "Intense Quote Char") },
    { WBuiltinStyle.MediumList2Accent1, (WStyleType.Table, "MediumList2Accent1", "Medium List 2 Accent 1") },
    { WBuiltinStyle.MediumGrid1Accent1, (WStyleType.Table, "MediumGrid1Accent1", "Medium Grid 1 Accent 1") },
    { WBuiltinStyle.MediumGrid2Accent1, (WStyleType.Table, "MediumGrid2Accent1", "Medium Grid 2 Accent 1") },
    { WBuiltinStyle.MediumGrid3Accent1, (WStyleType.Table, "MediumGrid3Accent1", "Medium Grid 3 Accent 1") },
    { WBuiltinStyle.DarkListAccent1, (WStyleType.Table, "DarkListAccent1", "Dark List Accent 1") },
    { WBuiltinStyle.ColorfulShadingAccent1, (WStyleType.Table, "ColorfulShadingAccent1", "Colorful Shading Accent 1") },
    { WBuiltinStyle.ColorfulListAccent1, (WStyleType.Table, "ColorfulListAccent1", "Colorful List Accent 1") },
    { WBuiltinStyle.ColorfulGridAccent1, (WStyleType.Table, "ColorfulGridAccent1", "Colorful Grid Accent 1") },
    { WBuiltinStyle.LightShadingAccent2, (WStyleType.Table, "LightShadingAccent2", "Light Shading Accent 2") },
    { WBuiltinStyle.LightListAccent2, (WStyleType.Table, "LightListAccent2", "Light List Accent 2") },
    { WBuiltinStyle.LightGridAccent2, (WStyleType.Table, "LightGridAccent2", "Light Grid Accent 2") },
    { WBuiltinStyle.MediumShading1Accent2, (WStyleType.Table, "MediumShading1Accent2", "Medium Shading 1 Accent 2") },
    { WBuiltinStyle.MediumShading2Accent2, (WStyleType.Table, "MediumShading2Accent2", "Medium Shading 2 Accent 2") },
    { WBuiltinStyle.MediumList1Accent2, (WStyleType.Table, "MediumList1Accent2", "Medium List 1 Accent 2") },
    { WBuiltinStyle.MediumList2Accent2, (WStyleType.Table, "MediumList2Accent2", "Medium List 2 Accent 2") },
    { WBuiltinStyle.MediumGrid1Accent2, (WStyleType.Table, "MediumGrid1Accent2", "Medium Grid 1 Accent 2") },
    { WBuiltinStyle.MediumGrid2Accent2, (WStyleType.Table, "MediumGrid2Accent2", "Medium Grid 2 Accent 2") },
    { WBuiltinStyle.MediumGrid3Accent2, (WStyleType.Table, "MediumGrid3Accent2", "Medium Grid 3 Accent 2") },
    { WBuiltinStyle.DarkListAccent2, (WStyleType.Table, "DarkListAccent2", "Dark List Accent 2") },
    { WBuiltinStyle.ColorfulShadingAccent2, (WStyleType.Table, "ColorfulShadingAccent2", "Colorful Shading Accent 2") },
    { WBuiltinStyle.ColorfulListAccent2, (WStyleType.Table, "ColorfulListAccent2", "Colorful List Accent 2") },
    { WBuiltinStyle.ColorfulGridAccent2, (WStyleType.Table, "ColorfulGridAccent2", "Colorful Grid Accent 2") },
    { WBuiltinStyle.LightShadingAccent3, (WStyleType.Table, "LightShadingAccent3", "Light Shading Accent 3") },
    { WBuiltinStyle.LightListAccent3, (WStyleType.Table, "LightListAccent3", "Light List Accent 3") },
    { WBuiltinStyle.LightGridAccent3, (WStyleType.Table, "LightGridAccent3", "Light Grid Accent 3") },
    { WBuiltinStyle.MediumShading1Accent3, (WStyleType.Table, "MediumShading1Accent3", "Medium Shading 1 Accent 3") },
    { WBuiltinStyle.MediumShading2Accent3, (WStyleType.Table, "MediumShading2Accent3", "Medium Shading 2 Accent 3") },
    { WBuiltinStyle.MediumList1Accent3, (WStyleType.Table, "MediumList1Accent3", "Medium List 1 Accent 3") },
    { WBuiltinStyle.MediumList2Accent3, (WStyleType.Table, "MediumList2Accent3", "Medium List 2 Accent 3") },
    { WBuiltinStyle.MediumGrid1Accent3, (WStyleType.Table, "MediumGrid1Accent3", "Medium Grid 1 Accent 3") },
    { WBuiltinStyle.MediumGrid2Accent3, (WStyleType.Table, "MediumGrid2Accent3", "Medium Grid 2 Accent 3") },
    { WBuiltinStyle.MediumGrid3Accent3, (WStyleType.Table, "MediumGrid3Accent3", "Medium Grid 3 Accent 3") },
    { WBuiltinStyle.DarkListAccent3, (WStyleType.Table, "DarkListAccent3", "Dark List Accent 3") },
    { WBuiltinStyle.ColorfulShadingAccent3, (WStyleType.Table, "ColorfulShadingAccent3", "Colorful Shading Accent 3") },
    { WBuiltinStyle.ColorfulListAccent3, (WStyleType.Table, "ColorfulListAccent3", "Colorful List Accent 3") },
    { WBuiltinStyle.ColorfulGridAccent3, (WStyleType.Table, "ColorfulGridAccent3", "Colorful Grid Accent 3") },
    { WBuiltinStyle.LightShadingAccent4, (WStyleType.Table, "LightShadingAccent4", "Light Shading Accent 4") },
    { WBuiltinStyle.LightListAccent4, (WStyleType.Table, "LightListAccent4", "Light List Accent 4") },
    { WBuiltinStyle.LightGridAccent4, (WStyleType.Table, "LightGridAccent4", "Light Grid Accent 4") },
    { WBuiltinStyle.MediumShading1Accent4, (WStyleType.Table, "MediumShading1Accent4", "Medium Shading 1 Accent 4") },
    { WBuiltinStyle.MediumShading2Accent4, (WStyleType.Table, "MediumShading2Accent4", "Medium Shading 2 Accent 4") },
    { WBuiltinStyle.MediumList1Accent4, (WStyleType.Table, "MediumList1Accent4", "Medium List 1 Accent 4") },
    { WBuiltinStyle.MediumList2Accent4, (WStyleType.Table, "MediumList2Accent4", "Medium List 2 Accent 4") },
    { WBuiltinStyle.MediumGrid1Accent4, (WStyleType.Table, "MediumGrid1Accent4", "Medium Grid 1 Accent 4") },
    { WBuiltinStyle.MediumGrid2Accent4, (WStyleType.Table, "MediumGrid2Accent4", "Medium Grid 2 Accent 4") },
    { WBuiltinStyle.MediumGrid3Accent4, (WStyleType.Table, "MediumGrid3Accent4", "Medium Grid 3 Accent 4") },
    { WBuiltinStyle.DarkListAccent4, (WStyleType.Table, "DarkListAccent4", "Dark List Accent 4") },
    { WBuiltinStyle.ColorfulShadingAccent4, (WStyleType.Table, "ColorfulShadingAccent4", "Colorful Shading Accent 4") },
    { WBuiltinStyle.ColorfulListAccent4, (WStyleType.Table, "ColorfulListAccent4", "Colorful List Accent 4") },
    { WBuiltinStyle.ColorfulGridAccent4, (WStyleType.Table, "ColorfulGridAccent4", "Colorful Grid Accent 4") },
    { WBuiltinStyle.LightShadingAccent5, (WStyleType.Table, "LightShadingAccent5", "Light Shading Accent 5") },
    { WBuiltinStyle.LightListAccent5, (WStyleType.Table, "LightListAccent5", "Light List Accent 5") },
    { WBuiltinStyle.LightGridAccent5, (WStyleType.Table, "LightGridAccent5", "Light Grid Accent 5") },
    { WBuiltinStyle.MediumShading1Accent5, (WStyleType.Table, "MediumShading1Accent5", "Medium Shading 1 Accent 5") },
    { WBuiltinStyle.MediumShading2Accent5, (WStyleType.Table, "MediumShading2Accent5", "Medium Shading 2 Accent 5") },
    { WBuiltinStyle.MediumList1Accent5, (WStyleType.Table, "MediumList1Accent5", "Medium List 1 Accent 5") },
    { WBuiltinStyle.MediumList2Accent5, (WStyleType.Table, "MediumList2Accent5", "Medium List 2 Accent 5") },
    { WBuiltinStyle.MediumGrid1Accent5, (WStyleType.Table, "MediumGrid1Accent5", "Medium Grid 1 Accent 5") },
    { WBuiltinStyle.MediumGrid2Accent5, (WStyleType.Table, "MediumGrid2Accent5", "Medium Grid 2 Accent 5") },
    { WBuiltinStyle.MediumGrid3Accent5, (WStyleType.Table, "MediumGrid3Accent5", "Medium Grid 3 Accent 5") },
    { WBuiltinStyle.DarkListAccent5, (WStyleType.Table, "DarkListAccent5", "Dark List Accent 5") },
    { WBuiltinStyle.ColorfulShadingAccent5, (WStyleType.Table, "ColorfulShadingAccent5", "Colorful Shading Accent 5") },
    { WBuiltinStyle.ColorfulListAccent5, (WStyleType.Table, "ColorfulListAccent5", "Colorful List Accent 5") },
    { WBuiltinStyle.ColorfulGridAccent5, (WStyleType.Table, "ColorfulGridAccent5", "Colorful Grid Accent 5") },
    { WBuiltinStyle.LightShadingAccent6, (WStyleType.Table, "LightShadingAccent6", "Light Shading Accent 6") },
    { WBuiltinStyle.LightListAccent6, (WStyleType.Table, "LightListAccent6", "Light List Accent 6") },
    { WBuiltinStyle.LightGridAccent6, (WStyleType.Table, "LightGridAccent6", "Light Grid Accent 6") },
    { WBuiltinStyle.MediumShading1Accent6, (WStyleType.Table, "MediumShading1Accent6", "Medium Shading 1 Accent 6") },
    { WBuiltinStyle.MediumShading2Accent6, (WStyleType.Table, "MediumShading2Accent6", "Medium Shading 2 Accent 6") },
    { WBuiltinStyle.MediumList1Accent6, (WStyleType.Table, "MediumList1Accent6", "Medium List 1 Accent 6") },
    { WBuiltinStyle.MediumList2Accent6, (WStyleType.Table, "MediumList2Accent6", "Medium List 2 Accent 6") },
    { WBuiltinStyle.MediumGrid1Accent6, (WStyleType.Table, "MediumGrid1Accent6", "Medium Grid 1 Accent 6") },
    { WBuiltinStyle.MediumGrid2Accent6, (WStyleType.Table, "MediumGrid2Accent6", "Medium Grid 2 Accent 6") },
    { WBuiltinStyle.MediumGrid3Accent6, (WStyleType.Table, "MediumGrid3Accent6", "Medium Grid 3 Accent 6") },
    { WBuiltinStyle.DarkListAccent6, (WStyleType.Table, "DarkListAccent6", "Dark List Accent 6") },
    { WBuiltinStyle.ColorfulShadingAccent6, (WStyleType.Table, "ColorfulShadingAccent6", "Colorful Shading Accent 6") },
    { WBuiltinStyle.ColorfulListAccent6, (WStyleType.Table, "ColorfulListAccent6", "Colorful List Accent 6") },
    { WBuiltinStyle.ColorfulGridAccent6, (WStyleType.Table, "ColorfulGridAccent6", "Colorful Grid Accent 6") },
    { WBuiltinStyle.SubtleEmphasis, (WStyleType.Character, "SubtleEmphasis", "Subtle Emphasis") },
    { WBuiltinStyle.IntenseEmphasis, (WStyleType.Character, "IntenseEmphasis", "Intense Emphasis") },
    { WBuiltinStyle.SubtleReference, (WStyleType.Character, "SubtleReference", "Subtle Reference") },
    { WBuiltinStyle.IntenseReference, (WStyleType.Character, "IntenseReference", "Intense Reference") },
    { WBuiltinStyle.BookTitle, (WStyleType.Character, "BookTitle", "Book Title") },
    { WBuiltinStyle.Bibliography, (WStyleType.Paragraph, "Bibliography", "Bibliography") },
    { WBuiltinStyle.TocHeading, (WStyleType.Paragraph, "TocHeading", "TOC Heading") },
    { WBuiltinStyle.PlainTable1, (WStyleType.Table, "PlainTable1", "Plain Table 1") },
    { WBuiltinStyle.PlainTable2, (WStyleType.Table, "PlainTable2", "Plain Table 2") },
    { WBuiltinStyle.PlainTable3, (WStyleType.Table, "PlainTable3", "Plain Table 3") },
    { WBuiltinStyle.PlainTable4, (WStyleType.Table, "PlainTable4", "Plain Table 4") },
    { WBuiltinStyle.PlainTable5, (WStyleType.Table, "PlainTable5", "Plain Table 5") },
    { WBuiltinStyle.GridTableLight, (WStyleType.Table, "GridTableLight", "Grid Table Light") },
    { WBuiltinStyle.GridTable1Light, (WStyleType.Table, "GridTable1Light", "Grid Table 1 Light") },
    { WBuiltinStyle.GridTable2, (WStyleType.Table, "GridTable2", "Grid Table 2") },
    { WBuiltinStyle.GridTable3, (WStyleType.Table, "GridTable3", "Grid Table 3") },
    { WBuiltinStyle.GridTable4, (WStyleType.Table, "GridTable4", "Grid Table 4") },
    { WBuiltinStyle.GridTable5Dark, (WStyleType.Table, "GridTable5Dark", "Grid Table 5 Dark") },
    { WBuiltinStyle.GridTable6Colorful, (WStyleType.Table, "GridTable6Colorful", "Grid Table 6 Colorful") },
    { WBuiltinStyle.GridTable7Colorful, (WStyleType.Table, "GridTable7Colorful", "Grid Table 7 Colorful") },
    { WBuiltinStyle.GridTable1LightAccent1, (WStyleType.Table, "GridTable1LightAccent1", "Grid Table 1 Light Accent 1") },
    { WBuiltinStyle.GridTable2Accent1, (WStyleType.Table, "GridTable2Accent1", "Grid Table 2 Accent 1") },
    { WBuiltinStyle.GridTable3Accent1, (WStyleType.Table, "GridTable3Accent1", "Grid Table 3 Accent 1") },
    { WBuiltinStyle.GridTable4Accent1, (WStyleType.Table, "GridTable4Accent1", "Grid Table 4 Accent 1") },
    { WBuiltinStyle.GridTable5DarkAccent1, (WStyleType.Table, "GridTable5DarkAccent1", "Grid Table 5 Dark Accent 1") },
    { WBuiltinStyle.GridTable6ColorfulAccent1, (WStyleType.Table, "GridTable6ColorfulAccent1", "Grid Table 6 Colorful Accent 1") },
    { WBuiltinStyle.GridTable7ColorfulAccent1, (WStyleType.Table, "GridTable7ColorfulAccent1", "Grid Table 7 Colorful Accent 1") },
    { WBuiltinStyle.GridTable1LightAccent2, (WStyleType.Table, "GridTable1LightAccent2", "Grid Table 1 Light Accent 2") },
    { WBuiltinStyle.GridTable2Accent2, (WStyleType.Table, "GridTable2Accent2", "Grid Table 2 Accent 2") },
    { WBuiltinStyle.GridTable3Accent2, (WStyleType.Table, "GridTable3Accent2", "Grid Table 3 Accent 2") },
    { WBuiltinStyle.GridTable4Accent2, (WStyleType.Table, "GridTable4Accent2", "Grid Table 4 Accent 2") },
    { WBuiltinStyle.GridTable5DarkAccent2, (WStyleType.Table, "GridTable5DarkAccent2", "Grid Table 5 Dark Accent 2") },
    { WBuiltinStyle.GridTable6ColorfulAccent2, (WStyleType.Table, "GridTable6ColorfulAccent2", "Grid Table 6 Colorful Accent 2") },
    { WBuiltinStyle.GridTable7ColorfulAccent2, (WStyleType.Table, "GridTable7ColorfulAccent2", "Grid Table 7 Colorful Accent 2") },
    { WBuiltinStyle.GridTable1LightAccent3, (WStyleType.Table, "GridTable1LightAccent3", "Grid Table 1 Light Accent 3") },
    { WBuiltinStyle.GridTable2Accent3, (WStyleType.Table, "GridTable2Accent3", "Grid Table 2 Accent 3") },
    { WBuiltinStyle.GridTable3Accent3, (WStyleType.Table, "GridTable3Accent3", "Grid Table 3 Accent 3") },
    { WBuiltinStyle.GridTable4Accent3, (WStyleType.Table, "GridTable4Accent3", "Grid Table 4 Accent 3") },
    { WBuiltinStyle.GridTable5DarkAccent3, (WStyleType.Table, "GridTable5DarkAccent3", "Grid Table 5 Dark Accent 3") },
    { WBuiltinStyle.GridTable6ColorfulAccent3, (WStyleType.Table, "GridTable6ColorfulAccent3", "Grid Table 6 Colorful Accent 3") },
    { WBuiltinStyle.GridTable7ColorfulAccent3, (WStyleType.Table, "GridTable7ColorfulAccent3", "Grid Table 7 Colorful Accent 3") },
    { WBuiltinStyle.GridTable1LightAccent4, (WStyleType.Table, "GridTable1LightAccent4", "Grid Table 1 Light Accent 4") },
    { WBuiltinStyle.GridTable2Accent4, (WStyleType.Table, "GridTable2Accent4", "Grid Table 2 Accent 4") },
    { WBuiltinStyle.GridTable3Accent4, (WStyleType.Table, "GridTable3Accent4", "Grid Table 3 Accent 4") },
    { WBuiltinStyle.GridTable4Accent4, (WStyleType.Table, "GridTable4Accent4", "Grid Table 4 Accent 4") },
    { WBuiltinStyle.GridTable5DarkAccent4, (WStyleType.Table, "GridTable5DarkAccent4", "Grid Table 5 Dark Accent 4") },
    { WBuiltinStyle.GridTable6ColorfulAccent4, (WStyleType.Table, "GridTable6ColorfulAccent4", "Grid Table 6 Colorful Accent 4") },
    { WBuiltinStyle.GridTable7ColorfulAccent4, (WStyleType.Table, "GridTable7ColorfulAccent4", "Grid Table 7 Colorful Accent 4") },
    { WBuiltinStyle.GridTable1LightAccent5, (WStyleType.Table, "GridTable1LightAccent5", "Grid Table 1 Light Accent 5") },
    { WBuiltinStyle.GridTable2Accent5, (WStyleType.Table, "GridTable2Accent5", "Grid Table 2 Accent 5") },
    { WBuiltinStyle.GridTable3Accent5, (WStyleType.Table, "GridTable3Accent5", "Grid Table 3 Accent 5") },
    { WBuiltinStyle.GridTable4Accent5, (WStyleType.Table, "GridTable4Accent5", "Grid Table 4 Accent 5") },
    { WBuiltinStyle.GridTable5DarkAccent5, (WStyleType.Table, "GridTable5DarkAccent5", "Grid Table 5 Dark Accent 5") },
    { WBuiltinStyle.GridTable6ColorfulAccent5, (WStyleType.Table, "GridTable6ColorfulAccent5", "Grid Table 6 Colorful Accent 5") },
    { WBuiltinStyle.GridTable7ColorfulAccent5, (WStyleType.Table, "GridTable7ColorfulAccent5", "Grid Table 7 Colorful Accent 5") },
    { WBuiltinStyle.GridTable1LightAccent6, (WStyleType.Table, "GridTable1LightAccent6", "Grid Table 1 Light Accent 6") },
    { WBuiltinStyle.GridTable2Accent6, (WStyleType.Table, "GridTable2Accent6", "Grid Table 2 Accent 6") },
    { WBuiltinStyle.GridTable3Accent6, (WStyleType.Table, "GridTable3Accent6", "Grid Table 3 Accent 6") },
    { WBuiltinStyle.GridTable4Accent6, (WStyleType.Table, "GridTable4Accent6", "Grid Table 4 Accent 6") },
    { WBuiltinStyle.GridTable5DarkAccent6, (WStyleType.Table, "GridTable5DarkAccent6", "Grid Table 5 Dark Accent 6") },
    { WBuiltinStyle.GridTable6ColorfulAccent6, (WStyleType.Table, "GridTable6ColorfulAccent6", "Grid Table 6 Colorful Accent 6") },
    { WBuiltinStyle.GridTable7ColorfulAccent6, (WStyleType.Table, "GridTable7ColorfulAccent6", "Grid Table 7 Colorful Accent 6") },
    { WBuiltinStyle.ListTable1Light, (WStyleType.Table, "ListTable1Light", "List Table 1 Light") },
    { WBuiltinStyle.ListTable2, (WStyleType.Table, "ListTable2", "List Table 2") },
    { WBuiltinStyle.ListTable3, (WStyleType.Table, "ListTable3", "List Table 3") },
    { WBuiltinStyle.ListTable4, (WStyleType.Table, "ListTable4", "List Table 4") },
    { WBuiltinStyle.ListTable5Dark, (WStyleType.Table, "ListTable5Dark", "List Table 5 Dark") },
    { WBuiltinStyle.ListTable6Colorful, (WStyleType.Table, "ListTable6Colorful", "List Table 6 Colorful") },
    { WBuiltinStyle.ListTable7Colorful, (WStyleType.Table, "ListTable7Colorful", "List Table 7 Colorful") },
    { WBuiltinStyle.ListTable1LightAccent1, (WStyleType.Table, "ListTable1LightAccent1", "List Table 1 Light Accent 1") },
    { WBuiltinStyle.ListTable2Accent1, (WStyleType.Table, "ListTable2Accent1", "List Table 2 Accent 1") },
    { WBuiltinStyle.ListTable3Accent1, (WStyleType.Table, "ListTable3Accent1", "List Table 3 Accent 1") },
    { WBuiltinStyle.ListTable4Accent1, (WStyleType.Table, "ListTable4Accent1", "List Table 4 Accent 1") },
    { WBuiltinStyle.ListTable5DarkAccent1, (WStyleType.Table, "ListTable5DarkAccent1", "List Table 5 Dark Accent 1") },
    { WBuiltinStyle.ListTable6ColorfulAccent1, (WStyleType.Table, "ListTable6ColorfulAccent1", "List Table 6 Colorful Accent 1") },
    { WBuiltinStyle.ListTable7ColorfulAccent1, (WStyleType.Table, "ListTable7ColorfulAccent1", "List Table 7 Colorful Accent 1") },
    { WBuiltinStyle.ListTable1LightAccent2, (WStyleType.Table, "ListTable1LightAccent2", "List Table 1 Light Accent 2") },
    { WBuiltinStyle.ListTable2Accent2, (WStyleType.Table, "ListTable2Accent2", "List Table 2 Accent 2") },
    { WBuiltinStyle.ListTable3Accent2, (WStyleType.Table, "ListTable3Accent2", "List Table 3 Accent 2") },
    { WBuiltinStyle.ListTable4Accent2, (WStyleType.Table, "ListTable4Accent2", "List Table 4 Accent 2") },
    { WBuiltinStyle.ListTable5DarkAccent2, (WStyleType.Table, "ListTable5DarkAccent2", "List Table 5 Dark Accent 2") },
    { WBuiltinStyle.ListTable6ColorfulAccent2, (WStyleType.Table, "ListTable6ColorfulAccent2", "List Table 6 Colorful Accent 2") },
    { WBuiltinStyle.ListTable7ColorfulAccent2, (WStyleType.Table, "ListTable7ColorfulAccent2", "List Table 7 Colorful Accent 2") },
    { WBuiltinStyle.ListTable1LightAccent3, (WStyleType.Table, "ListTable1LightAccent3", "List Table 1 Light Accent 3") },
    { WBuiltinStyle.ListTable2Accent3, (WStyleType.Table, "ListTable2Accent3", "List Table 2 Accent 3") },
    { WBuiltinStyle.ListTable3Accent3, (WStyleType.Table, "ListTable3Accent3", "List Table 3 Accent 3") },
    { WBuiltinStyle.ListTable4Accent3, (WStyleType.Table, "ListTable4Accent3", "List Table 4 Accent 3") },
    { WBuiltinStyle.ListTable5DarkAccent3, (WStyleType.Table, "ListTable5DarkAccent3", "List Table 5 Dark Accent 3") },
    { WBuiltinStyle.ListTable6ColorfulAccent3, (WStyleType.Table, "ListTable6ColorfulAccent3", "List Table 6 Colorful Accent 3") },
    { WBuiltinStyle.ListTable7ColorfulAccent3, (WStyleType.Table, "ListTable7ColorfulAccent3", "List Table 7 Colorful Accent 3") },
    { WBuiltinStyle.ListTable1LightAccent4, (WStyleType.Table, "ListTable1LightAccent4", "List Table 1 Light Accent 4") },
    { WBuiltinStyle.ListTable2Accent4, (WStyleType.Table, "ListTable2Accent4", "List Table 2 Accent 4") },
    { WBuiltinStyle.ListTable3Accent4, (WStyleType.Table, "ListTable3Accent4", "List Table 3 Accent 4") },
    { WBuiltinStyle.ListTable4Accent4, (WStyleType.Table, "ListTable4Accent4", "List Table 4 Accent 4") },
    { WBuiltinStyle.ListTable5DarkAccent4, (WStyleType.Table, "ListTable5DarkAccent4", "List Table 5 Dark Accent 4") },
    { WBuiltinStyle.ListTable6ColorfulAccent4, (WStyleType.Table, "ListTable6ColorfulAccent4", "List Table 6 Colorful Accent 4") },
    { WBuiltinStyle.ListTable7ColorfulAccent4, (WStyleType.Table, "ListTable7ColorfulAccent4", "List Table 7 Colorful Accent 4") },
    { WBuiltinStyle.ListTable1LightAccent5, (WStyleType.Table, "ListTable1LightAccent5", "List Table 1 Light Accent 5") },
    { WBuiltinStyle.ListTable2Accent5, (WStyleType.Table, "ListTable2Accent5", "List Table 2 Accent 5") },
    { WBuiltinStyle.ListTable3Accent5, (WStyleType.Table, "ListTable3Accent5", "List Table 3 Accent 5") },
    { WBuiltinStyle.ListTable4Accent5, (WStyleType.Table, "ListTable4Accent5", "List Table 4 Accent 5") },
    { WBuiltinStyle.ListTable5DarkAccent5, (WStyleType.Table, "ListTable5DarkAccent5", "List Table 5 Dark Accent 5") },
    { WBuiltinStyle.ListTable6ColorfulAccent5, (WStyleType.Table, "ListTable6ColorfulAccent5", "List Table 6 Colorful Accent 5") },
    { WBuiltinStyle.ListTable7ColorfulAccent5, (WStyleType.Table, "ListTable7ColorfulAccent5", "List Table 7 Colorful Accent 5") },
    { WBuiltinStyle.ListTable1LightAccent6, (WStyleType.Table, "ListTable1LightAccent6", "List Table 1 Light Accent 6") },
    { WBuiltinStyle.ListTable2Accent6, (WStyleType.Table, "ListTable2Accent6", "List Table 2 Accent 6") },
    { WBuiltinStyle.ListTable3Accent6, (WStyleType.Table, "ListTable3Accent6", "List Table 3 Accent 6") },
    { WBuiltinStyle.ListTable4Accent6, (WStyleType.Table, "ListTable4Accent6", "List Table 4 Accent 6") },
    { WBuiltinStyle.ListTable5DarkAccent6, (WStyleType.Table, "ListTable5DarkAccent6", "List Table 5 Dark Accent 6") },
    { WBuiltinStyle.ListTable6ColorfulAccent6, (WStyleType.Table, "ListTable6ColorfulAccent6", "List Table 6 Colorful Accent 6") },
    { WBuiltinStyle.ListTable7ColorfulAccent6, (WStyleType.Table, "ListTable7ColorfulAccent6", "List Table 7 Colorful Accent 6") },
    { WBuiltinStyle.Mention, (WStyleType.Character, "Mention", "Mention") },
    { WBuiltinStyle.SmartHyperlink, (WStyleType.Character, "SmartHyperlink", "Smart Hyperlink") },
    { WBuiltinStyle.Hashtag, (WStyleType.Character, "Hashtag", "Hashtag") },
    { WBuiltinStyle.UnresolvedMention, (WStyleType.Character, "UnresolvedMention", "Unresolved Mention") },
    { WBuiltinStyle.SmartLink, (WStyleType.Character, "SmartLink", "Smart Link") },
 };


  private static Dictionary<string, WBuiltinStyle> StyleNamesDictionary
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

  private static readonly Dictionary<string, WBuiltinStyle> _styleNamesDictionary
    = new (StringComparer.InvariantCultureIgnoreCase)
    {
      { "Outline List 1", WBuiltinStyle.OutlineList1 },
      { "Outline List 2", WBuiltinStyle.OutlineList2 },
      { "Outline List 3", WBuiltinStyle.OutlineList3 },
      { "Annotation Text", WBuiltinStyle.CommentText},
      { "Annotation Subject", WBuiltinStyle.CommentSubject},
      { "Annotation Reference", WBuiltinStyle.CommentReference},
      { "HTML Top of Form", WBuiltinStyle.zTopOfForm },
      { "HTML Bottom of Form", WBuiltinStyle.zBottomOfForm },
    };
}

