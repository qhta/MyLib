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
  public static StyleTypes GetType(this DXW.Styles styles, string styleName)
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
  private static StyleTypes StyleValuesToStyleType(DXW.StyleValues value)
  {
    if (value == DXW.StyleValues.Paragraph)
      return StyleTypes.Paragraph;
    if (value == DXW.StyleValues.Character)
      return StyleTypes.Character;
    if (value == DXW.StyleValues.Table)
      return StyleTypes.Table;
    if (value == DXW.StyleValues.Numbering)
      return StyleTypes.Numbering;
    return 0;
  }

  /// <summary>
  /// Converts a style type to a style value.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  private static DXW.StyleValues? StyleTypeToStyleValues(StyleTypes type)
  {
    if (type == StyleTypes.Paragraph)
      return DXW.StyleValues.Paragraph;
    if (type == StyleTypes.Character)
      return DXW.StyleValues.Character;
    if (type == StyleTypes.Table)
      return DXW.StyleValues.Table;
    if (type == StyleTypes.Numbering)
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
    BuiltinStyles = new Dictionary<BuiltinStyles, Style>();
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
  private static Dictionary<BuiltinStyles, Style>? BuiltinStyles = null;

  private static readonly Dictionary<BuiltinStyles, (StyleTypes Type, string Ident, string Name)> KnownStyles = new ()
    {
    { OpenXmlTools.BuiltinStyles.Normal, (StyleTypes.Paragraph, "Normal", "Normal") },
    { OpenXmlTools.BuiltinStyles.Heading1, (StyleTypes.Paragraph, "Heading1", "Heading 1") },
    { OpenXmlTools.BuiltinStyles.Heading1Char, (StyleTypes.Linked, "Heading1Char", "Heading 1 Char") },
    { OpenXmlTools.BuiltinStyles.Heading2, (StyleTypes.Paragraph, "Heading2", "Heading 2") },
    { OpenXmlTools.BuiltinStyles.Heading2Char, (StyleTypes.Linked, "Heading2Char", "Heading 2 Char") },
    { OpenXmlTools.BuiltinStyles.Heading3, (StyleTypes.Paragraph, "Heading3", "Heading 3") },
    { OpenXmlTools.BuiltinStyles.Heading3Char, (StyleTypes.Linked, "Heading3Char", "Heading 3 Char") },
    { OpenXmlTools.BuiltinStyles.Heading4, (StyleTypes.Paragraph, "Heading4", "Heading 4") },
    { OpenXmlTools.BuiltinStyles.Heading4Char, (StyleTypes.Linked, "Heading4Char", "Heading 4 Char") },
    { OpenXmlTools.BuiltinStyles.Heading5, (StyleTypes.Paragraph, "Heading5", "Heading 5") },
    { OpenXmlTools.BuiltinStyles.Heading5Char, (StyleTypes.Linked, "Heading5Char", "Heading 5 Char") },
    { OpenXmlTools.BuiltinStyles.Heading6, (StyleTypes.Paragraph, "Heading6", "Heading 6") },
    { OpenXmlTools.BuiltinStyles.Heading6Char, (StyleTypes.Linked, "Heading6Char", "Heading 6 Char") },
    { OpenXmlTools.BuiltinStyles.Heading7, (StyleTypes.Paragraph, "Heading7", "Heading 7") },
    { OpenXmlTools.BuiltinStyles.Heading7Char, (StyleTypes.Linked, "Heading7Char", "Heading 7 Char") },
    { OpenXmlTools.BuiltinStyles.Heading8, (StyleTypes.Paragraph, "Heading8", "Heading 8") },
    { OpenXmlTools.BuiltinStyles.Heading8Char, (StyleTypes.Linked, "Heading8Char", "Heading 8 Char") },
    { OpenXmlTools.BuiltinStyles.Heading9, (StyleTypes.Paragraph, "Heading9", "Heading 9") },
    { OpenXmlTools.BuiltinStyles.Heading9Char, (StyleTypes.Linked, "Heading9Char", "Heading 9 Char") },
    { OpenXmlTools.BuiltinStyles.Index1, (StyleTypes.Paragraph, "Index1", "Index 1") },
    { OpenXmlTools.BuiltinStyles.Index2, (StyleTypes.Paragraph, "Index2", "Index 2") },
    { OpenXmlTools.BuiltinStyles.Index3, (StyleTypes.Paragraph, "Index3", "Index 3") },
    { OpenXmlTools.BuiltinStyles.Index4, (StyleTypes.Paragraph, "Index4", "Index 4") },
    { OpenXmlTools.BuiltinStyles.Index5, (StyleTypes.Paragraph, "Index5", "Index 5") },
    { OpenXmlTools.BuiltinStyles.Index6, (StyleTypes.Paragraph, "Index6", "Index 6") },
    { OpenXmlTools.BuiltinStyles.Index7, (StyleTypes.Paragraph, "Index7", "Index 7") },
    { OpenXmlTools.BuiltinStyles.Index8, (StyleTypes.Paragraph, "Index8", "Index 8") },
    { OpenXmlTools.BuiltinStyles.Index9, (StyleTypes.Paragraph, "Index9", "Index 9") },
    { OpenXmlTools.BuiltinStyles.TOC1, (StyleTypes.Paragraph, "TOC1", "TOC 1") },
    { OpenXmlTools.BuiltinStyles.TOC2, (StyleTypes.Paragraph, "TOC2", "TOC 2") },
    { OpenXmlTools.BuiltinStyles.TOC3, (StyleTypes.Paragraph, "TOC3", "TOC 3") },
    { OpenXmlTools.BuiltinStyles.TOC4, (StyleTypes.Paragraph, "TOC4", "TOC 4") },
    { OpenXmlTools.BuiltinStyles.TOC5, (StyleTypes.Paragraph, "TOC5", "TOC 5") },
    { OpenXmlTools.BuiltinStyles.TOC6, (StyleTypes.Paragraph, "TOC6", "TOC 6") },
    { OpenXmlTools.BuiltinStyles.TOC7, (StyleTypes.Paragraph, "TOC7", "TOC 7") },
    { OpenXmlTools.BuiltinStyles.TOC8, (StyleTypes.Paragraph, "TOC8", "TOC 8") },
    { OpenXmlTools.BuiltinStyles.TOC9, (StyleTypes.Paragraph, "TOC9", "TOC 9") },
    { OpenXmlTools.BuiltinStyles.NormalIndent, (StyleTypes.Paragraph, "NormalIndent", "Normal Indent") },
    { OpenXmlTools.BuiltinStyles.FootnoteText, (StyleTypes.Paragraph, "FootnoteText", "Footnote Text") },
    { OpenXmlTools.BuiltinStyles.FootnoteTextChar, (StyleTypes.Linked, "FootnoteTextChar", "Footnote Text Char") },
    { OpenXmlTools.BuiltinStyles.CommentText, (StyleTypes.Paragraph, "CommentText", "Comment Text") },
    { OpenXmlTools.BuiltinStyles.CommentTextChar, (StyleTypes.Linked, "CommentTextChar", "Comment Text Char") },
    { OpenXmlTools.BuiltinStyles.Header, (StyleTypes.Paragraph, "Header", "Header") },
    { OpenXmlTools.BuiltinStyles.HeaderChar, (StyleTypes.Linked, "HeaderChar", "Header Char") },
    { OpenXmlTools.BuiltinStyles.Footer, (StyleTypes.Paragraph, "Footer", "Footer") },
    { OpenXmlTools.BuiltinStyles.FooterChar, (StyleTypes.Linked, "FooterChar", "Footer Char") },
    { OpenXmlTools.BuiltinStyles.IndexHeading, (StyleTypes.Paragraph, "IndexHeading", "Index Heading") },
    { OpenXmlTools.BuiltinStyles.Caption, (StyleTypes.Paragraph, "Caption", "Caption") },
    { OpenXmlTools.BuiltinStyles.TableOfFigures, (StyleTypes.Paragraph, "TableOfFigures", "Table of Figures") },
    { OpenXmlTools.BuiltinStyles.EnvelopeAddress, (StyleTypes.Paragraph, "EnvelopeAddress", "Envelope Address") },
    { OpenXmlTools.BuiltinStyles.EnvelopeReturn, (StyleTypes.Paragraph, "EnvelopeReturn", "Envelope Return") },
    { OpenXmlTools.BuiltinStyles.FootnoteReference, (StyleTypes.Character, "FootnoteReference", "Footnote Reference") },
    { OpenXmlTools.BuiltinStyles.CommentReference, (StyleTypes.Character, "CommentReference", "Comment Reference") },
    { OpenXmlTools.BuiltinStyles.LineNumber, (StyleTypes.Character, "LineNumber", "Line Number") },
    { OpenXmlTools.BuiltinStyles.PageNumber, (StyleTypes.Character, "PageNumber", "Page Number") },
    { OpenXmlTools.BuiltinStyles.EndnoteReference, (StyleTypes.Character, "EndnoteReference", "Endnote Reference") },
    { OpenXmlTools.BuiltinStyles.EndnoteText, (StyleTypes.Paragraph, "EndnoteText", "Endnote Text") },
    { OpenXmlTools.BuiltinStyles.EndnoteTextChar, (StyleTypes.Linked, "EndnoteTextChar", "Endnote Text Char") },
    { OpenXmlTools.BuiltinStyles.TableOfAuthorities, (StyleTypes.Paragraph, "TableOfAuthorities", "Table of Authorities") },
    { OpenXmlTools.BuiltinStyles.Macro, (StyleTypes.Paragraph, "Macro", "Macro") },
    { OpenXmlTools.BuiltinStyles.MacroChar, (StyleTypes.Linked, "MacroChar", "Macro Text Char") },
    { OpenXmlTools.BuiltinStyles.TOAHeading, (StyleTypes.Paragraph, "TOAHeading", "TOA Heading") },
    { OpenXmlTools.BuiltinStyles.List, (StyleTypes.Paragraph, "List", "List") },
    { OpenXmlTools.BuiltinStyles.ListBullet, (StyleTypes.Paragraph, "ListBullet", "List Bullet") },
    { OpenXmlTools.BuiltinStyles.ListNumber, (StyleTypes.Paragraph, "ListNumber", "List Number") },
    { OpenXmlTools.BuiltinStyles.List2, (StyleTypes.Paragraph, "List2", "List 2") },
    { OpenXmlTools.BuiltinStyles.List3, (StyleTypes.Paragraph, "List3", "List 3") },
    { OpenXmlTools.BuiltinStyles.List4, (StyleTypes.Paragraph, "List4", "List 4") },
    { OpenXmlTools.BuiltinStyles.List5, (StyleTypes.Paragraph, "List5", "List 5") },
    { OpenXmlTools.BuiltinStyles.ListBullet2, (StyleTypes.Paragraph, "ListBullet2", "List Bullet 2") },
    { OpenXmlTools.BuiltinStyles.ListBullet3, (StyleTypes.Paragraph, "ListBullet3", "List Bullet 3") },
    { OpenXmlTools.BuiltinStyles.ListBullet4, (StyleTypes.Paragraph, "ListBullet4", "List Bullet 4") },
    { OpenXmlTools.BuiltinStyles.ListBullet5, (StyleTypes.Paragraph, "ListBullet5", "List Bullet 5") },
    { OpenXmlTools.BuiltinStyles.ListNumber2, (StyleTypes.Paragraph, "ListNumber2", "List Number 2") },
    { OpenXmlTools.BuiltinStyles.ListNumber3, (StyleTypes.Paragraph, "ListNumber3", "List Number 3") },
    { OpenXmlTools.BuiltinStyles.ListNumber4, (StyleTypes.Paragraph, "ListNumber4", "List Number 4") },
    { OpenXmlTools.BuiltinStyles.ListNumber5, (StyleTypes.Paragraph, "ListNumber5", "List Number 5") },
    { OpenXmlTools.BuiltinStyles.Title, (StyleTypes.Paragraph, "Title", "Title") },
    { OpenXmlTools.BuiltinStyles.TitleChar, (StyleTypes.Linked, "TitleChar", "Title Char") },
    { OpenXmlTools.BuiltinStyles.Closing, (StyleTypes.Paragraph, "Closing", "Closing") },
    { OpenXmlTools.BuiltinStyles.ClosingChar, (StyleTypes.Linked, "ClosingChar", "Closing Char") },
    { OpenXmlTools.BuiltinStyles.Signature, (StyleTypes.Paragraph, "Signature", "Signature") },
    { OpenXmlTools.BuiltinStyles.SignatureChar, (StyleTypes.Linked, "SignatureChar", "Signature Char") },
    { OpenXmlTools.BuiltinStyles.DefaultParagraphFont, (StyleTypes.Character, "DefaultParagraphFont", "Default Paragraph Font") },
    { OpenXmlTools.BuiltinStyles.BodyText, (StyleTypes.Paragraph, "BodyText", "Body Text") },
    { OpenXmlTools.BuiltinStyles.BodyTextChar, (StyleTypes.Linked, "BodyTextChar", "Body Text Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent, (StyleTypes.Paragraph, "BodyTextIndent", "Body Text Indent") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndentChar, (StyleTypes.Linked, "BodyTextIndentChar", "Body Text Indent Char") },
    { OpenXmlTools.BuiltinStyles.ListContinue, (StyleTypes.Paragraph, "ListContinue", "List Continue") },
    { OpenXmlTools.BuiltinStyles.ListContinue2, (StyleTypes.Paragraph, "ListContinue2", "List Continue 2") },
    { OpenXmlTools.BuiltinStyles.ListContinue3, (StyleTypes.Paragraph, "ListContinue3", "List Continue 3") },
    { OpenXmlTools.BuiltinStyles.ListContinue4, (StyleTypes.Paragraph, "ListContinue4", "List Continue 4") },
    { OpenXmlTools.BuiltinStyles.ListContinue5, (StyleTypes.Paragraph, "ListContinue5", "List Continue 5") },
    { OpenXmlTools.BuiltinStyles.MessageHeader, (StyleTypes.Paragraph, "MessageHeader", "Message Header") },
    { OpenXmlTools.BuiltinStyles.MessageHeaderChar, (StyleTypes.Linked, "MessageHeaderChar", "Message Header Char") },
    { OpenXmlTools.BuiltinStyles.Subtitle, (StyleTypes.Paragraph, "Subtitle", "Subtitle") },
    { OpenXmlTools.BuiltinStyles.SubtitleChar, (StyleTypes.Linked, "SubtitleChar", "Subtitle Char") },
    { OpenXmlTools.BuiltinStyles.Salutation, (StyleTypes.Paragraph, "Salutation", "Salutation") },
    { OpenXmlTools.BuiltinStyles.SalutationChar, (StyleTypes.Linked, "SalutationChar", "Salutation Char") },
    { OpenXmlTools.BuiltinStyles.Date, (StyleTypes.Paragraph, "Date", "Date") },
    { OpenXmlTools.BuiltinStyles.DateChar, (StyleTypes.Linked, "DateChar", "Date Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextFirstIndent, (StyleTypes.Paragraph, "BodyTextFirstIndent", "Body Text First Indent") },
    { OpenXmlTools.BuiltinStyles.BodyTextFirstIndentChar, (StyleTypes.Linked, "BodyTextFirstIndentChar", "Body Text First Indent Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextFirstIndent2, (StyleTypes.Paragraph, "BodyTextFirstIndent2", "Body Text First Indent 2") },
    { OpenXmlTools.BuiltinStyles.BodyTextFirstIndent2Char, (StyleTypes.Linked, "BodyTextFirstIndent2Char", "Body Text First Indent 2 Char") },
    { OpenXmlTools.BuiltinStyles.NoteHeading, (StyleTypes.Paragraph, "NoteHeading", "Note Heading") },
    { OpenXmlTools.BuiltinStyles.NoteHeadingChar, (StyleTypes.Linked, "NoteHeadingChar", "Note Heading Char") },
    { OpenXmlTools.BuiltinStyles.BodyText2, (StyleTypes.Paragraph, "BodyText2", "Body Text 2") },
    { OpenXmlTools.BuiltinStyles.BodyText2Char, (StyleTypes.Linked, "BodyText2Char", "Body Text 2 Char") },
    { OpenXmlTools.BuiltinStyles.BodyText3, (StyleTypes.Paragraph, "BodyText3", "Body Text 3") },
    { OpenXmlTools.BuiltinStyles.BodyText3Char, (StyleTypes.Linked, "BodyText3Char", "Body Text 3 Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent2, (StyleTypes.Paragraph, "BodyTextIndent2", "Body Text Indent 2") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent2Char, (StyleTypes.Linked, "BodyTextIndent2Char", "Body Text Indent 2 Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent3, (StyleTypes.Paragraph, "BodyTextIndent3", "Body Text Indent 3") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent3Char, (StyleTypes.Linked, "BodyTextIndent3Char", "Body Text Indent 3 Char") },
    { OpenXmlTools.BuiltinStyles.BlockText, (StyleTypes.Paragraph, "BlockText", "Block Text") },
    { OpenXmlTools.BuiltinStyles.Hyperlink, (StyleTypes.Character, "Hyperlink", "Hyperlink") },
    { OpenXmlTools.BuiltinStyles.FollowedHyperlink, (StyleTypes.Character, "FollowedHyperlink", "FollowedHyperlink") },
    { OpenXmlTools.BuiltinStyles.Strong, (StyleTypes.Character, "Strong", "Strong") },
    { OpenXmlTools.BuiltinStyles.Emphasis, (StyleTypes.Character, "Emphasis", "Emphasis") },
    { OpenXmlTools.BuiltinStyles.DocumentMap, (StyleTypes.Paragraph, "DocumentMap", "Document Map") },
    { OpenXmlTools.BuiltinStyles.DocumentMapChar, (StyleTypes.Linked, "DocumentMapChar", "Document Map Char") },
    { OpenXmlTools.BuiltinStyles.PlainText, (StyleTypes.Paragraph, "PlainText", "Plain Text") },
    { OpenXmlTools.BuiltinStyles.PlainTextChar, (StyleTypes.Linked, "PlainTextChar", "Plain Text Char") },
    { OpenXmlTools.BuiltinStyles.EmailSignature, (StyleTypes.Paragraph, "EmailSignature", "E-mail Signature") },
    { OpenXmlTools.BuiltinStyles.EmailSignatureChar, (StyleTypes.Linked, "EmailSignatureChar", "E-mail Signature Char") },
    { OpenXmlTools.BuiltinStyles.zTopOfForm, (StyleTypes.Paragraph, "zTopOfForm", "z-Top of Form") },
    { OpenXmlTools.BuiltinStyles.zTopOfFormChar, (StyleTypes.Linked, "zTopOfFormChar", "z-Top of Form Char") },
    { OpenXmlTools.BuiltinStyles.zBottomOfForm, (StyleTypes.Paragraph, "zBottomOfForm", "z-Bottom of Form") },
    { OpenXmlTools.BuiltinStyles.zBottomOfFormChar, (StyleTypes.Linked, "zBottomOfFormChar", "z-Bottom of Form Char") },
    { OpenXmlTools.BuiltinStyles.NormalWeb, (StyleTypes.Paragraph, "NormalWeb", "Normal (Web)") },
    { OpenXmlTools.BuiltinStyles.HtmlAcronym, (StyleTypes.Character, "HtmlAcronym", "HTML Acronym") },
    { OpenXmlTools.BuiltinStyles.HtmlAddress, (StyleTypes.Paragraph, "HtmlAddress", "HTML Address") },
    { OpenXmlTools.BuiltinStyles.HtmlAddressChar, (StyleTypes.Linked, "HtmlAddressChar", "HTML Address Char") },
    { OpenXmlTools.BuiltinStyles.HtmlCite, (StyleTypes.Character, "HtmlCite", "HTML Cite") },
    { OpenXmlTools.BuiltinStyles.HtmlCode, (StyleTypes.Character, "HtmlCode", "HTML Code") },
    { OpenXmlTools.BuiltinStyles.HtmlDefinition, (StyleTypes.Character, "HtmlDefinition", "HTML Definition") },
    { OpenXmlTools.BuiltinStyles.HtmlKeyboard, (StyleTypes.Character, "HtmlKeyboard", "HTML Keyboard") },
    { OpenXmlTools.BuiltinStyles.HtmlPreformatted, (StyleTypes.Paragraph, "HtmlPreformatted", "HTML Preformatted") },
    { OpenXmlTools.BuiltinStyles.HtmlPreformattedChar, (StyleTypes.Linked, "HtmlPreformattedChar", "HTML Preformatted Char") },
    { OpenXmlTools.BuiltinStyles.HtmlSample, (StyleTypes.Character, "HtmlSample", "HTML Sample") },
    { OpenXmlTools.BuiltinStyles.HtmlTypewriter, (StyleTypes.Character, "HtmlTypewriter", "HTML Typewriter") },
    { OpenXmlTools.BuiltinStyles.HtmlVariable, (StyleTypes.Character, "HtmlVariable", "HTML Variable") },
    { OpenXmlTools.BuiltinStyles.NormalTable, (StyleTypes.Table, "NormalTable", "Normal Table") },
    { OpenXmlTools.BuiltinStyles.CommentSubject, (StyleTypes.Paragraph, "CommentSubject", "Comment Subject") },
    { OpenXmlTools.BuiltinStyles.CommentSubjectChar, (StyleTypes.Linked, "CommentSubjectChar", "Comment Subject Char") },
    { OpenXmlTools.BuiltinStyles.NoList, (StyleTypes.Numbering, "NoList", "No List") },
    { OpenXmlTools.BuiltinStyles.OutlineList1, (StyleTypes.Numbering, "OutlineList1", "1 / a / i") },
    { OpenXmlTools.BuiltinStyles.OutlineList2, (StyleTypes.Numbering, "OutlineList2", "1 / 1.1 / 1.1.1") },
    { OpenXmlTools.BuiltinStyles.OutlineList3, (StyleTypes.Numbering, "OutlineList3", "Article / Section") },
    { OpenXmlTools.BuiltinStyles.TableSimple1, (StyleTypes.Table, "TableSimple1", "Table Simple 1") },
    { OpenXmlTools.BuiltinStyles.TableSimple2, (StyleTypes.Table, "TableSimple2", "Table Simple 2") },
    { OpenXmlTools.BuiltinStyles.TableSimple3, (StyleTypes.Table, "TableSimple3", "Table Simple 3") },
    { OpenXmlTools.BuiltinStyles.TableClassic1, (StyleTypes.Table, "TableClassic1", "Table Classic 1") },
    { OpenXmlTools.BuiltinStyles.TableClassic2, (StyleTypes.Table, "TableClassic2", "Table Classic 2") },
    { OpenXmlTools.BuiltinStyles.TableClassic3, (StyleTypes.Table, "TableClassic3", "Table Classic 3") },
    { OpenXmlTools.BuiltinStyles.TableClassic4, (StyleTypes.Table, "TableClassic4", "Table Classic 4") },
    { OpenXmlTools.BuiltinStyles.TableColorful1, (StyleTypes.Table, "TableColorful1", "Table Colorful 1") },
    { OpenXmlTools.BuiltinStyles.TableColorful2, (StyleTypes.Table, "TableColorful2", "Table Colorful 2") },
    { OpenXmlTools.BuiltinStyles.TableColorful3, (StyleTypes.Table, "TableColorful3", "Table Colorful 3") },
    { OpenXmlTools.BuiltinStyles.TableColumns1, (StyleTypes.Table, "TableColumns1", "Table Columns 1") },
    { OpenXmlTools.BuiltinStyles.TableColumns2, (StyleTypes.Table, "TableColumns2", "Table Columns 2") },
    { OpenXmlTools.BuiltinStyles.TableColumns3, (StyleTypes.Table, "TableColumns3", "Table Columns 3") },
    { OpenXmlTools.BuiltinStyles.TableColumns4, (StyleTypes.Table, "TableColumns4", "Table Columns 4") },
    { OpenXmlTools.BuiltinStyles.TableColumns5, (StyleTypes.Table, "TableColumns5", "Table Columns 5") },
    { OpenXmlTools.BuiltinStyles.TableGrid1, (StyleTypes.Table, "TableGrid1", "Table Grid 1") },
    { OpenXmlTools.BuiltinStyles.TableGrid2, (StyleTypes.Table, "TableGrid2", "Table Grid 2") },
    { OpenXmlTools.BuiltinStyles.TableGrid3, (StyleTypes.Table, "TableGrid3", "Table Grid 3") },
    { OpenXmlTools.BuiltinStyles.TableGrid4, (StyleTypes.Table, "TableGrid4", "Table Grid 4") },
    { OpenXmlTools.BuiltinStyles.TableGrid5, (StyleTypes.Table, "TableGrid5", "Table Grid 5") },
    { OpenXmlTools.BuiltinStyles.TableGrid6, (StyleTypes.Table, "TableGrid6", "Table Grid 6") },
    { OpenXmlTools.BuiltinStyles.TableGrid7, (StyleTypes.Table, "TableGrid7", "Table Grid 7") },
    { OpenXmlTools.BuiltinStyles.TableGrid8, (StyleTypes.Table, "TableGrid8", "Table Grid 8") },
    { OpenXmlTools.BuiltinStyles.TableList1, (StyleTypes.Table, "TableList1", "Table List 1") },
    { OpenXmlTools.BuiltinStyles.TableList2, (StyleTypes.Table, "TableList2", "Table List 2") },
    { OpenXmlTools.BuiltinStyles.TableList3, (StyleTypes.Table, "TableList3", "Table List 3") },
    { OpenXmlTools.BuiltinStyles.TableList4, (StyleTypes.Table, "TableList4", "Table List 4") },
    { OpenXmlTools.BuiltinStyles.TableList5, (StyleTypes.Table, "TableList5", "Table List 5") },
    { OpenXmlTools.BuiltinStyles.TableList6, (StyleTypes.Table, "TableList6", "Table List 6") },
    { OpenXmlTools.BuiltinStyles.TableList7, (StyleTypes.Table, "TableList7", "Table List 7") },
    { OpenXmlTools.BuiltinStyles.TableList8, (StyleTypes.Table, "TableList8", "Table List 8") },
    { OpenXmlTools.BuiltinStyles.Table3dEffects1, (StyleTypes.Table, "Table3dEffects1", "Table 3D effects 1") },
    { OpenXmlTools.BuiltinStyles.Table3dEffects2, (StyleTypes.Table, "Table3dEffects2", "Table 3D effects 2") },
    { OpenXmlTools.BuiltinStyles.Table3dEffects3, (StyleTypes.Table, "Table3dEffects3", "Table 3D effects 3") },
    { OpenXmlTools.BuiltinStyles.TableContemporary, (StyleTypes.Table, "TableContemporary", "Table Contemporary") },
    { OpenXmlTools.BuiltinStyles.TableElegant, (StyleTypes.Table, "TableElegant", "Table Elegant") },
    { OpenXmlTools.BuiltinStyles.TableProfessional, (StyleTypes.Table, "TableProfessional", "Table Professional") },
    { OpenXmlTools.BuiltinStyles.TableSubtle1, (StyleTypes.Table, "TableSubtle1", "Table Subtle 1") },
    { OpenXmlTools.BuiltinStyles.TableSubtle2, (StyleTypes.Table, "TableSubtle2", "Table Subtle 2") },
    { OpenXmlTools.BuiltinStyles.TableWeb1, (StyleTypes.Table, "TableWeb1", "Table Web 1") },
    { OpenXmlTools.BuiltinStyles.TableWeb2, (StyleTypes.Table, "TableWeb2", "Table Web 2") },
    { OpenXmlTools.BuiltinStyles.TableWeb3, (StyleTypes.Table, "TableWeb3", "Table Web 3") },
    { OpenXmlTools.BuiltinStyles.BalloonText, (StyleTypes.Paragraph, "BalloonText", "Balloon Text") },
    { OpenXmlTools.BuiltinStyles.BalloonTextChar, (StyleTypes.Linked, "BalloonTextChar", "Balloon Text Char") },
    { OpenXmlTools.BuiltinStyles.TableGrid, (StyleTypes.Table, "TableGrid", "Table Grid") },
    { OpenXmlTools.BuiltinStyles.TableTheme, (StyleTypes.Table, "TableTheme", "Table Theme") },
    { OpenXmlTools.BuiltinStyles.PlaceholderText, (StyleTypes.Character, "PlaceholderText", "Placeholder Text") },
    { OpenXmlTools.BuiltinStyles.NoSpacing, (StyleTypes.Paragraph, "NoSpacing", "No Spacing") },
    { OpenXmlTools.BuiltinStyles.LightShading, (StyleTypes.Table, "LightShading", "Light Shading") },
    { OpenXmlTools.BuiltinStyles.LightList, (StyleTypes.Table, "LightList", "Light List") },
    { OpenXmlTools.BuiltinStyles.LightGrid, (StyleTypes.Table, "LightGrid", "Light Grid") },
    { OpenXmlTools.BuiltinStyles.MediumShading1, (StyleTypes.Table, "MediumShading1", "Medium Shading 1") },
    { OpenXmlTools.BuiltinStyles.MediumShading2, (StyleTypes.Table, "MediumShading2", "Medium Shading 2") },
    { OpenXmlTools.BuiltinStyles.MediumList1, (StyleTypes.Table, "MediumList1", "Medium List 1") },
    { OpenXmlTools.BuiltinStyles.MediumList2, (StyleTypes.Table, "MediumList2", "Medium List 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1, (StyleTypes.Table, "MediumGrid1", "Medium Grid 1") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2, (StyleTypes.Table, "MediumGrid2", "Medium Grid 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3, (StyleTypes.Table, "MediumGrid3", "Medium Grid 3") },
    { OpenXmlTools.BuiltinStyles.DarkList, (StyleTypes.Table, "DarkList", "Dark List") },
    { OpenXmlTools.BuiltinStyles.ColorfulShading, (StyleTypes.Table, "ColorfulShading", "Colorful Shading") },
    { OpenXmlTools.BuiltinStyles.ColorfulList, (StyleTypes.Table, "ColorfulList", "Colorful List") },
    { OpenXmlTools.BuiltinStyles.ColorfulGrid, (StyleTypes.Table, "ColorfulGrid", "Colorful Grid") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent1, (StyleTypes.Table, "LightShadingAccent1", "Light Shading Accent 1") },
    { OpenXmlTools.BuiltinStyles.LightListAccent1, (StyleTypes.Table, "LightListAccent1", "Light List Accent 1") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent1, (StyleTypes.Table, "LightGridAccent1", "Light Grid Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent1, (StyleTypes.Table, "MediumShading1Accent1", "Medium Shading 1 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent1, (StyleTypes.Table, "MediumShading2Accent1", "Medium Shading 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent1, (StyleTypes.Table, "MediumList1Accent1", "Medium List 1 Accent 1") },
    { OpenXmlTools.BuiltinStyles.Revision, (StyleTypes.Paragraph, "Revision", "Revision") },
    { OpenXmlTools.BuiltinStyles.ListParagraph, (StyleTypes.Paragraph, "ListParagraph", "List Paragraph") },
    { OpenXmlTools.BuiltinStyles.Quote, (StyleTypes.Paragraph, "Quote", "Quote") },
    { OpenXmlTools.BuiltinStyles.QuoteChar, (StyleTypes.Linked, "QuoteChar", "Quote Char") },
    { OpenXmlTools.BuiltinStyles.IntenseQuote, (StyleTypes.Paragraph, "IntenseQuote", "Intense Quote") },
    { OpenXmlTools.BuiltinStyles.IntenseQuoteChar, (StyleTypes.Linked, "IntenseQuoteChar", "Intense Quote Char") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent1, (StyleTypes.Table, "MediumList2Accent1", "Medium List 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent1, (StyleTypes.Table, "MediumGrid1Accent1", "Medium Grid 1 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent1, (StyleTypes.Table, "MediumGrid2Accent1", "Medium Grid 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent1, (StyleTypes.Table, "MediumGrid3Accent1", "Medium Grid 3 Accent 1") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent1, (StyleTypes.Table, "DarkListAccent1", "Dark List Accent 1") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent1, (StyleTypes.Table, "ColorfulShadingAccent1", "Colorful Shading Accent 1") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent1, (StyleTypes.Table, "ColorfulListAccent1", "Colorful List Accent 1") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent1, (StyleTypes.Table, "ColorfulGridAccent1", "Colorful Grid Accent 1") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent2, (StyleTypes.Table, "LightShadingAccent2", "Light Shading Accent 2") },
    { OpenXmlTools.BuiltinStyles.LightListAccent2, (StyleTypes.Table, "LightListAccent2", "Light List Accent 2") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent2, (StyleTypes.Table, "LightGridAccent2", "Light Grid Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent2, (StyleTypes.Table, "MediumShading1Accent2", "Medium Shading 1 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent2, (StyleTypes.Table, "MediumShading2Accent2", "Medium Shading 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent2, (StyleTypes.Table, "MediumList1Accent2", "Medium List 1 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent2, (StyleTypes.Table, "MediumList2Accent2", "Medium List 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent2, (StyleTypes.Table, "MediumGrid1Accent2", "Medium Grid 1 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent2, (StyleTypes.Table, "MediumGrid2Accent2", "Medium Grid 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent2, (StyleTypes.Table, "MediumGrid3Accent2", "Medium Grid 3 Accent 2") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent2, (StyleTypes.Table, "DarkListAccent2", "Dark List Accent 2") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent2, (StyleTypes.Table, "ColorfulShadingAccent2", "Colorful Shading Accent 2") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent2, (StyleTypes.Table, "ColorfulListAccent2", "Colorful List Accent 2") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent2, (StyleTypes.Table, "ColorfulGridAccent2", "Colorful Grid Accent 2") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent3, (StyleTypes.Table, "LightShadingAccent3", "Light Shading Accent 3") },
    { OpenXmlTools.BuiltinStyles.LightListAccent3, (StyleTypes.Table, "LightListAccent3", "Light List Accent 3") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent3, (StyleTypes.Table, "LightGridAccent3", "Light Grid Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent3, (StyleTypes.Table, "MediumShading1Accent3", "Medium Shading 1 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent3, (StyleTypes.Table, "MediumShading2Accent3", "Medium Shading 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent3, (StyleTypes.Table, "MediumList1Accent3", "Medium List 1 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent3, (StyleTypes.Table, "MediumList2Accent3", "Medium List 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent3, (StyleTypes.Table, "MediumGrid1Accent3", "Medium Grid 1 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent3, (StyleTypes.Table, "MediumGrid2Accent3", "Medium Grid 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent3, (StyleTypes.Table, "MediumGrid3Accent3", "Medium Grid 3 Accent 3") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent3, (StyleTypes.Table, "DarkListAccent3", "Dark List Accent 3") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent3, (StyleTypes.Table, "ColorfulShadingAccent3", "Colorful Shading Accent 3") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent3, (StyleTypes.Table, "ColorfulListAccent3", "Colorful List Accent 3") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent3, (StyleTypes.Table, "ColorfulGridAccent3", "Colorful Grid Accent 3") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent4, (StyleTypes.Table, "LightShadingAccent4", "Light Shading Accent 4") },
    { OpenXmlTools.BuiltinStyles.LightListAccent4, (StyleTypes.Table, "LightListAccent4", "Light List Accent 4") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent4, (StyleTypes.Table, "LightGridAccent4", "Light Grid Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent4, (StyleTypes.Table, "MediumShading1Accent4", "Medium Shading 1 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent4, (StyleTypes.Table, "MediumShading2Accent4", "Medium Shading 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent4, (StyleTypes.Table, "MediumList1Accent4", "Medium List 1 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent4, (StyleTypes.Table, "MediumList2Accent4", "Medium List 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent4, (StyleTypes.Table, "MediumGrid1Accent4", "Medium Grid 1 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent4, (StyleTypes.Table, "MediumGrid2Accent4", "Medium Grid 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent4, (StyleTypes.Table, "MediumGrid3Accent4", "Medium Grid 3 Accent 4") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent4, (StyleTypes.Table, "DarkListAccent4", "Dark List Accent 4") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent4, (StyleTypes.Table, "ColorfulShadingAccent4", "Colorful Shading Accent 4") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent4, (StyleTypes.Table, "ColorfulListAccent4", "Colorful List Accent 4") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent4, (StyleTypes.Table, "ColorfulGridAccent4", "Colorful Grid Accent 4") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent5, (StyleTypes.Table, "LightShadingAccent5", "Light Shading Accent 5") },
    { OpenXmlTools.BuiltinStyles.LightListAccent5, (StyleTypes.Table, "LightListAccent5", "Light List Accent 5") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent5, (StyleTypes.Table, "LightGridAccent5", "Light Grid Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent5, (StyleTypes.Table, "MediumShading1Accent5", "Medium Shading 1 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent5, (StyleTypes.Table, "MediumShading2Accent5", "Medium Shading 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent5, (StyleTypes.Table, "MediumList1Accent5", "Medium List 1 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent5, (StyleTypes.Table, "MediumList2Accent5", "Medium List 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent5, (StyleTypes.Table, "MediumGrid1Accent5", "Medium Grid 1 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent5, (StyleTypes.Table, "MediumGrid2Accent5", "Medium Grid 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent5, (StyleTypes.Table, "MediumGrid3Accent5", "Medium Grid 3 Accent 5") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent5, (StyleTypes.Table, "DarkListAccent5", "Dark List Accent 5") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent5, (StyleTypes.Table, "ColorfulShadingAccent5", "Colorful Shading Accent 5") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent5, (StyleTypes.Table, "ColorfulListAccent5", "Colorful List Accent 5") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent5, (StyleTypes.Table, "ColorfulGridAccent5", "Colorful Grid Accent 5") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent6, (StyleTypes.Table, "LightShadingAccent6", "Light Shading Accent 6") },
    { OpenXmlTools.BuiltinStyles.LightListAccent6, (StyleTypes.Table, "LightListAccent6", "Light List Accent 6") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent6, (StyleTypes.Table, "LightGridAccent6", "Light Grid Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent6, (StyleTypes.Table, "MediumShading1Accent6", "Medium Shading 1 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent6, (StyleTypes.Table, "MediumShading2Accent6", "Medium Shading 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent6, (StyleTypes.Table, "MediumList1Accent6", "Medium List 1 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent6, (StyleTypes.Table, "MediumList2Accent6", "Medium List 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent6, (StyleTypes.Table, "MediumGrid1Accent6", "Medium Grid 1 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent6, (StyleTypes.Table, "MediumGrid2Accent6", "Medium Grid 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent6, (StyleTypes.Table, "MediumGrid3Accent6", "Medium Grid 3 Accent 6") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent6, (StyleTypes.Table, "DarkListAccent6", "Dark List Accent 6") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent6, (StyleTypes.Table, "ColorfulShadingAccent6", "Colorful Shading Accent 6") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent6, (StyleTypes.Table, "ColorfulListAccent6", "Colorful List Accent 6") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent6, (StyleTypes.Table, "ColorfulGridAccent6", "Colorful Grid Accent 6") },
    { OpenXmlTools.BuiltinStyles.SubtleEmphasis, (StyleTypes.Character, "SubtleEmphasis", "Subtle Emphasis") },
    { OpenXmlTools.BuiltinStyles.IntenseEmphasis, (StyleTypes.Character, "IntenseEmphasis", "Intense Emphasis") },
    { OpenXmlTools.BuiltinStyles.SubtleReference, (StyleTypes.Character, "SubtleReference", "Subtle Reference") },
    { OpenXmlTools.BuiltinStyles.IntenseReference, (StyleTypes.Character, "IntenseReference", "Intense Reference") },
    { OpenXmlTools.BuiltinStyles.BookTitle, (StyleTypes.Character, "BookTitle", "Book Title") },
    { OpenXmlTools.BuiltinStyles.Bibliography, (StyleTypes.Paragraph, "Bibliography", "Bibliography") },
    { OpenXmlTools.BuiltinStyles.TocHeading, (StyleTypes.Paragraph, "TocHeading", "TOC Heading") },
    { OpenXmlTools.BuiltinStyles.PlainTable1, (StyleTypes.Table, "PlainTable1", "Plain Table 1") },
    { OpenXmlTools.BuiltinStyles.PlainTable2, (StyleTypes.Table, "PlainTable2", "Plain Table 2") },
    { OpenXmlTools.BuiltinStyles.PlainTable3, (StyleTypes.Table, "PlainTable3", "Plain Table 3") },
    { OpenXmlTools.BuiltinStyles.PlainTable4, (StyleTypes.Table, "PlainTable4", "Plain Table 4") },
    { OpenXmlTools.BuiltinStyles.PlainTable5, (StyleTypes.Table, "PlainTable5", "Plain Table 5") },
    { OpenXmlTools.BuiltinStyles.GridTableLight, (StyleTypes.Table, "GridTableLight", "Grid Table Light") },
    { OpenXmlTools.BuiltinStyles.GridTable1Light, (StyleTypes.Table, "GridTable1Light", "Grid Table 1 Light") },
    { OpenXmlTools.BuiltinStyles.GridTable2, (StyleTypes.Table, "GridTable2", "Grid Table 2") },
    { OpenXmlTools.BuiltinStyles.GridTable3, (StyleTypes.Table, "GridTable3", "Grid Table 3") },
    { OpenXmlTools.BuiltinStyles.GridTable4, (StyleTypes.Table, "GridTable4", "Grid Table 4") },
    { OpenXmlTools.BuiltinStyles.GridTable5Dark, (StyleTypes.Table, "GridTable5Dark", "Grid Table 5 Dark") },
    { OpenXmlTools.BuiltinStyles.GridTable6Colorful, (StyleTypes.Table, "GridTable6Colorful", "Grid Table 6 Colorful") },
    { OpenXmlTools.BuiltinStyles.GridTable7Colorful, (StyleTypes.Table, "GridTable7Colorful", "Grid Table 7 Colorful") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent1, (StyleTypes.Table, "GridTable1LightAccent1", "Grid Table 1 Light Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent1, (StyleTypes.Table, "GridTable2Accent1", "Grid Table 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent1, (StyleTypes.Table, "GridTable3Accent1", "Grid Table 3 Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent1, (StyleTypes.Table, "GridTable4Accent1", "Grid Table 4 Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent1, (StyleTypes.Table, "GridTable5DarkAccent1", "Grid Table 5 Dark Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent1, (StyleTypes.Table, "GridTable6ColorfulAccent1", "Grid Table 6 Colorful Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent1, (StyleTypes.Table, "GridTable7ColorfulAccent1", "Grid Table 7 Colorful Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent2, (StyleTypes.Table, "GridTable1LightAccent2", "Grid Table 1 Light Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent2, (StyleTypes.Table, "GridTable2Accent2", "Grid Table 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent2, (StyleTypes.Table, "GridTable3Accent2", "Grid Table 3 Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent2, (StyleTypes.Table, "GridTable4Accent2", "Grid Table 4 Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent2, (StyleTypes.Table, "GridTable5DarkAccent2", "Grid Table 5 Dark Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent2, (StyleTypes.Table, "GridTable6ColorfulAccent2", "Grid Table 6 Colorful Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent2, (StyleTypes.Table, "GridTable7ColorfulAccent2", "Grid Table 7 Colorful Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent3, (StyleTypes.Table, "GridTable1LightAccent3", "Grid Table 1 Light Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent3, (StyleTypes.Table, "GridTable2Accent3", "Grid Table 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent3, (StyleTypes.Table, "GridTable3Accent3", "Grid Table 3 Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent3, (StyleTypes.Table, "GridTable4Accent3", "Grid Table 4 Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent3, (StyleTypes.Table, "GridTable5DarkAccent3", "Grid Table 5 Dark Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent3, (StyleTypes.Table, "GridTable6ColorfulAccent3", "Grid Table 6 Colorful Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent3, (StyleTypes.Table, "GridTable7ColorfulAccent3", "Grid Table 7 Colorful Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent4, (StyleTypes.Table, "GridTable1LightAccent4", "Grid Table 1 Light Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent4, (StyleTypes.Table, "GridTable2Accent4", "Grid Table 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent4, (StyleTypes.Table, "GridTable3Accent4", "Grid Table 3 Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent4, (StyleTypes.Table, "GridTable4Accent4", "Grid Table 4 Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent4, (StyleTypes.Table, "GridTable5DarkAccent4", "Grid Table 5 Dark Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent4, (StyleTypes.Table, "GridTable6ColorfulAccent4", "Grid Table 6 Colorful Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent4, (StyleTypes.Table, "GridTable7ColorfulAccent4", "Grid Table 7 Colorful Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent5, (StyleTypes.Table, "GridTable1LightAccent5", "Grid Table 1 Light Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent5, (StyleTypes.Table, "GridTable2Accent5", "Grid Table 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent5, (StyleTypes.Table, "GridTable3Accent5", "Grid Table 3 Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent5, (StyleTypes.Table, "GridTable4Accent5", "Grid Table 4 Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent5, (StyleTypes.Table, "GridTable5DarkAccent5", "Grid Table 5 Dark Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent5, (StyleTypes.Table, "GridTable6ColorfulAccent5", "Grid Table 6 Colorful Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent5, (StyleTypes.Table, "GridTable7ColorfulAccent5", "Grid Table 7 Colorful Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent6, (StyleTypes.Table, "GridTable1LightAccent6", "Grid Table 1 Light Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent6, (StyleTypes.Table, "GridTable2Accent6", "Grid Table 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent6, (StyleTypes.Table, "GridTable3Accent6", "Grid Table 3 Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent6, (StyleTypes.Table, "GridTable4Accent6", "Grid Table 4 Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent6, (StyleTypes.Table, "GridTable5DarkAccent6", "Grid Table 5 Dark Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent6, (StyleTypes.Table, "GridTable6ColorfulAccent6", "Grid Table 6 Colorful Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent6, (StyleTypes.Table, "GridTable7ColorfulAccent6", "Grid Table 7 Colorful Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable1Light, (StyleTypes.Table, "ListTable1Light", "List Table 1 Light") },
    { OpenXmlTools.BuiltinStyles.ListTable2, (StyleTypes.Table, "ListTable2", "List Table 2") },
    { OpenXmlTools.BuiltinStyles.ListTable3, (StyleTypes.Table, "ListTable3", "List Table 3") },
    { OpenXmlTools.BuiltinStyles.ListTable4, (StyleTypes.Table, "ListTable4", "List Table 4") },
    { OpenXmlTools.BuiltinStyles.ListTable5Dark, (StyleTypes.Table, "ListTable5Dark", "List Table 5 Dark") },
    { OpenXmlTools.BuiltinStyles.ListTable6Colorful, (StyleTypes.Table, "ListTable6Colorful", "List Table 6 Colorful") },
    { OpenXmlTools.BuiltinStyles.ListTable7Colorful, (StyleTypes.Table, "ListTable7Colorful", "List Table 7 Colorful") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent1, (StyleTypes.Table, "ListTable1LightAccent1", "List Table 1 Light Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent1, (StyleTypes.Table, "ListTable2Accent1", "List Table 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent1, (StyleTypes.Table, "ListTable3Accent1", "List Table 3 Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent1, (StyleTypes.Table, "ListTable4Accent1", "List Table 4 Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent1, (StyleTypes.Table, "ListTable5DarkAccent1", "List Table 5 Dark Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent1, (StyleTypes.Table, "ListTable6ColorfulAccent1", "List Table 6 Colorful Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent1, (StyleTypes.Table, "ListTable7ColorfulAccent1", "List Table 7 Colorful Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent2, (StyleTypes.Table, "ListTable1LightAccent2", "List Table 1 Light Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent2, (StyleTypes.Table, "ListTable2Accent2", "List Table 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent2, (StyleTypes.Table, "ListTable3Accent2", "List Table 3 Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent2, (StyleTypes.Table, "ListTable4Accent2", "List Table 4 Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent2, (StyleTypes.Table, "ListTable5DarkAccent2", "List Table 5 Dark Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent2, (StyleTypes.Table, "ListTable6ColorfulAccent2", "List Table 6 Colorful Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent2, (StyleTypes.Table, "ListTable7ColorfulAccent2", "List Table 7 Colorful Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent3, (StyleTypes.Table, "ListTable1LightAccent3", "List Table 1 Light Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent3, (StyleTypes.Table, "ListTable2Accent3", "List Table 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent3, (StyleTypes.Table, "ListTable3Accent3", "List Table 3 Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent3, (StyleTypes.Table, "ListTable4Accent3", "List Table 4 Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent3, (StyleTypes.Table, "ListTable5DarkAccent3", "List Table 5 Dark Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent3, (StyleTypes.Table, "ListTable6ColorfulAccent3", "List Table 6 Colorful Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent3, (StyleTypes.Table, "ListTable7ColorfulAccent3", "List Table 7 Colorful Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent4, (StyleTypes.Table, "ListTable1LightAccent4", "List Table 1 Light Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent4, (StyleTypes.Table, "ListTable2Accent4", "List Table 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent4, (StyleTypes.Table, "ListTable3Accent4", "List Table 3 Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent4, (StyleTypes.Table, "ListTable4Accent4", "List Table 4 Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent4, (StyleTypes.Table, "ListTable5DarkAccent4", "List Table 5 Dark Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent4, (StyleTypes.Table, "ListTable6ColorfulAccent4", "List Table 6 Colorful Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent4, (StyleTypes.Table, "ListTable7ColorfulAccent4", "List Table 7 Colorful Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent5, (StyleTypes.Table, "ListTable1LightAccent5", "List Table 1 Light Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent5, (StyleTypes.Table, "ListTable2Accent5", "List Table 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent5, (StyleTypes.Table, "ListTable3Accent5", "List Table 3 Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent5, (StyleTypes.Table, "ListTable4Accent5", "List Table 4 Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent5, (StyleTypes.Table, "ListTable5DarkAccent5", "List Table 5 Dark Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent5, (StyleTypes.Table, "ListTable6ColorfulAccent5", "List Table 6 Colorful Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent5, (StyleTypes.Table, "ListTable7ColorfulAccent5", "List Table 7 Colorful Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent6, (StyleTypes.Table, "ListTable1LightAccent6", "List Table 1 Light Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent6, (StyleTypes.Table, "ListTable2Accent6", "List Table 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent6, (StyleTypes.Table, "ListTable3Accent6", "List Table 3 Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent6, (StyleTypes.Table, "ListTable4Accent6", "List Table 4 Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent6, (StyleTypes.Table, "ListTable5DarkAccent6", "List Table 5 Dark Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent6, (StyleTypes.Table, "ListTable6ColorfulAccent6", "List Table 6 Colorful Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent6, (StyleTypes.Table, "ListTable7ColorfulAccent6", "List Table 7 Colorful Accent 6") },
    { OpenXmlTools.BuiltinStyles.Mention, (StyleTypes.Character, "Mention", "Mention") },
    { OpenXmlTools.BuiltinStyles.SmartHyperlink, (StyleTypes.Character, "SmartHyperlink", "Smart Hyperlink") },
    { OpenXmlTools.BuiltinStyles.Hashtag, (StyleTypes.Character, "Hashtag", "Hashtag") },
    { OpenXmlTools.BuiltinStyles.UnresolvedMention, (StyleTypes.Character, "UnresolvedMention", "Unresolved Mention") },
    { OpenXmlTools.BuiltinStyles.SmartLink, (StyleTypes.Character, "SmartLink", "Smart Link") },
 };


  private static Dictionary<string, BuiltinStyles> StyleNamesDictionary
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

  private static readonly Dictionary<string, BuiltinStyles> _styleNamesDictionary
    = new (StringComparer.InvariantCultureIgnoreCase)
    {
      { "Outline List 1", OpenXmlTools.BuiltinStyles.OutlineList1 },
      { "Outline List 2", OpenXmlTools.BuiltinStyles.OutlineList2 },
      { "Outline List 3", OpenXmlTools.BuiltinStyles.OutlineList3 },
      { "Annotation Text", OpenXmlTools.BuiltinStyles.CommentText},
      { "Annotation Subject", OpenXmlTools.BuiltinStyles.CommentSubject},
      { "Annotation Reference", OpenXmlTools.BuiltinStyles.CommentReference},
      { "HTML Top of Form", OpenXmlTools.BuiltinStyles.zTopOfForm },
      { "HTML Bottom of Form", OpenXmlTools.BuiltinStyles.zBottomOfForm },
    };
}

