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
  public static StyleType GetType(this DXW.Styles styles, string styleName)
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
  private static StyleType StyleValuesToStyleType(DXW.StyleValues value)
  {
    if (value == DXW.StyleValues.Paragraph)
      return StyleType.Paragraph;
    if (value == DXW.StyleValues.Character)
      return StyleType.Character;
    if (value == DXW.StyleValues.Table)
      return StyleType.Table;
    if (value == DXW.StyleValues.Numbering)
      return StyleType.Numbering;
    return 0;
  }

  /// <summary>
  /// Converts a style type to a style value.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  private static DXW.StyleValues? StyleTypeToStyleValues(StyleType type)
  {
    if (type == StyleType.Paragraph)
      return DXW.StyleValues.Paragraph;
    if (type == StyleType.Character)
      return DXW.StyleValues.Character;
    if (type == StyleType.Table)
      return DXW.StyleValues.Table;
    if (type == StyleType.Numbering)
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

  private static readonly Dictionary<BuiltinStyles, (StyleType Type, string Ident, string Name)> KnownStyles = new ()
    {
    { OpenXmlTools.BuiltinStyles.Normal, (StyleType.Paragraph, "Normal", "Normal") },
    { OpenXmlTools.BuiltinStyles.Heading1, (StyleType.Paragraph, "Heading1", "Heading 1") },
    { OpenXmlTools.BuiltinStyles.Heading1Char, (StyleType.Linked, "Heading1Char", "Heading 1 Char") },
    { OpenXmlTools.BuiltinStyles.Heading2, (StyleType.Paragraph, "Heading2", "Heading 2") },
    { OpenXmlTools.BuiltinStyles.Heading2Char, (StyleType.Linked, "Heading2Char", "Heading 2 Char") },
    { OpenXmlTools.BuiltinStyles.Heading3, (StyleType.Paragraph, "Heading3", "Heading 3") },
    { OpenXmlTools.BuiltinStyles.Heading3Char, (StyleType.Linked, "Heading3Char", "Heading 3 Char") },
    { OpenXmlTools.BuiltinStyles.Heading4, (StyleType.Paragraph, "Heading4", "Heading 4") },
    { OpenXmlTools.BuiltinStyles.Heading4Char, (StyleType.Linked, "Heading4Char", "Heading 4 Char") },
    { OpenXmlTools.BuiltinStyles.Heading5, (StyleType.Paragraph, "Heading5", "Heading 5") },
    { OpenXmlTools.BuiltinStyles.Heading5Char, (StyleType.Linked, "Heading5Char", "Heading 5 Char") },
    { OpenXmlTools.BuiltinStyles.Heading6, (StyleType.Paragraph, "Heading6", "Heading 6") },
    { OpenXmlTools.BuiltinStyles.Heading6Char, (StyleType.Linked, "Heading6Char", "Heading 6 Char") },
    { OpenXmlTools.BuiltinStyles.Heading7, (StyleType.Paragraph, "Heading7", "Heading 7") },
    { OpenXmlTools.BuiltinStyles.Heading7Char, (StyleType.Linked, "Heading7Char", "Heading 7 Char") },
    { OpenXmlTools.BuiltinStyles.Heading8, (StyleType.Paragraph, "Heading8", "Heading 8") },
    { OpenXmlTools.BuiltinStyles.Heading8Char, (StyleType.Linked, "Heading8Char", "Heading 8 Char") },
    { OpenXmlTools.BuiltinStyles.Heading9, (StyleType.Paragraph, "Heading9", "Heading 9") },
    { OpenXmlTools.BuiltinStyles.Heading9Char, (StyleType.Linked, "Heading9Char", "Heading 9 Char") },
    { OpenXmlTools.BuiltinStyles.Index1, (StyleType.Paragraph, "Index1", "Index 1") },
    { OpenXmlTools.BuiltinStyles.Index2, (StyleType.Paragraph, "Index2", "Index 2") },
    { OpenXmlTools.BuiltinStyles.Index3, (StyleType.Paragraph, "Index3", "Index 3") },
    { OpenXmlTools.BuiltinStyles.Index4, (StyleType.Paragraph, "Index4", "Index 4") },
    { OpenXmlTools.BuiltinStyles.Index5, (StyleType.Paragraph, "Index5", "Index 5") },
    { OpenXmlTools.BuiltinStyles.Index6, (StyleType.Paragraph, "Index6", "Index 6") },
    { OpenXmlTools.BuiltinStyles.Index7, (StyleType.Paragraph, "Index7", "Index 7") },
    { OpenXmlTools.BuiltinStyles.Index8, (StyleType.Paragraph, "Index8", "Index 8") },
    { OpenXmlTools.BuiltinStyles.Index9, (StyleType.Paragraph, "Index9", "Index 9") },
    { OpenXmlTools.BuiltinStyles.TOC1, (StyleType.Paragraph, "TOC1", "TOC 1") },
    { OpenXmlTools.BuiltinStyles.TOC2, (StyleType.Paragraph, "TOC2", "TOC 2") },
    { OpenXmlTools.BuiltinStyles.TOC3, (StyleType.Paragraph, "TOC3", "TOC 3") },
    { OpenXmlTools.BuiltinStyles.TOC4, (StyleType.Paragraph, "TOC4", "TOC 4") },
    { OpenXmlTools.BuiltinStyles.TOC5, (StyleType.Paragraph, "TOC5", "TOC 5") },
    { OpenXmlTools.BuiltinStyles.TOC6, (StyleType.Paragraph, "TOC6", "TOC 6") },
    { OpenXmlTools.BuiltinStyles.TOC7, (StyleType.Paragraph, "TOC7", "TOC 7") },
    { OpenXmlTools.BuiltinStyles.TOC8, (StyleType.Paragraph, "TOC8", "TOC 8") },
    { OpenXmlTools.BuiltinStyles.TOC9, (StyleType.Paragraph, "TOC9", "TOC 9") },
    { OpenXmlTools.BuiltinStyles.NormalIndent, (StyleType.Paragraph, "NormalIndent", "Normal Indent") },
    { OpenXmlTools.BuiltinStyles.FootnoteText, (StyleType.Paragraph, "FootnoteText", "Footnote Text") },
    { OpenXmlTools.BuiltinStyles.FootnoteTextChar, (StyleType.Linked, "FootnoteTextChar", "Footnote Text Char") },
    { OpenXmlTools.BuiltinStyles.CommentText, (StyleType.Paragraph, "CommentText", "Comment Text") },
    { OpenXmlTools.BuiltinStyles.CommentTextChar, (StyleType.Linked, "CommentTextChar", "Comment Text Char") },
    { OpenXmlTools.BuiltinStyles.Header, (StyleType.Paragraph, "Header", "Header") },
    { OpenXmlTools.BuiltinStyles.HeaderChar, (StyleType.Linked, "HeaderChar", "Header Char") },
    { OpenXmlTools.BuiltinStyles.Footer, (StyleType.Paragraph, "Footer", "Footer") },
    { OpenXmlTools.BuiltinStyles.FooterChar, (StyleType.Linked, "FooterChar", "Footer Char") },
    { OpenXmlTools.BuiltinStyles.IndexHeading, (StyleType.Paragraph, "IndexHeading", "Index Heading") },
    { OpenXmlTools.BuiltinStyles.Caption, (StyleType.Paragraph, "Caption", "Caption") },
    { OpenXmlTools.BuiltinStyles.TableOfFigures, (StyleType.Paragraph, "TableOfFigures", "Table of Figures") },
    { OpenXmlTools.BuiltinStyles.EnvelopeAddress, (StyleType.Paragraph, "EnvelopeAddress", "Envelope Address") },
    { OpenXmlTools.BuiltinStyles.EnvelopeReturn, (StyleType.Paragraph, "EnvelopeReturn", "Envelope Return") },
    { OpenXmlTools.BuiltinStyles.FootnoteReference, (StyleType.Character, "FootnoteReference", "Footnote Reference") },
    { OpenXmlTools.BuiltinStyles.CommentReference, (StyleType.Character, "CommentReference", "Comment Reference") },
    { OpenXmlTools.BuiltinStyles.LineNumber, (StyleType.Character, "LineNumber", "Line Number") },
    { OpenXmlTools.BuiltinStyles.PageNumber, (StyleType.Character, "PageNumber", "Page Number") },
    { OpenXmlTools.BuiltinStyles.EndnoteReference, (StyleType.Character, "EndnoteReference", "Endnote Reference") },
    { OpenXmlTools.BuiltinStyles.EndnoteText, (StyleType.Paragraph, "EndnoteText", "Endnote Text") },
    { OpenXmlTools.BuiltinStyles.EndnoteTextChar, (StyleType.Linked, "EndnoteTextChar", "Endnote Text Char") },
    { OpenXmlTools.BuiltinStyles.TableOfAuthorities, (StyleType.Paragraph, "TableOfAuthorities", "Table of Authorities") },
    { OpenXmlTools.BuiltinStyles.Macro, (StyleType.Paragraph, "Macro", "Macro") },
    { OpenXmlTools.BuiltinStyles.MacroChar, (StyleType.Linked, "MacroChar", "Macro Text Char") },
    { OpenXmlTools.BuiltinStyles.TOAHeading, (StyleType.Paragraph, "TOAHeading", "TOA Heading") },
    { OpenXmlTools.BuiltinStyles.List, (StyleType.Paragraph, "List", "List") },
    { OpenXmlTools.BuiltinStyles.ListBullet, (StyleType.Paragraph, "ListBullet", "List Bullet") },
    { OpenXmlTools.BuiltinStyles.ListNumber, (StyleType.Paragraph, "ListNumber", "List Number") },
    { OpenXmlTools.BuiltinStyles.List2, (StyleType.Paragraph, "List2", "List 2") },
    { OpenXmlTools.BuiltinStyles.List3, (StyleType.Paragraph, "List3", "List 3") },
    { OpenXmlTools.BuiltinStyles.List4, (StyleType.Paragraph, "List4", "List 4") },
    { OpenXmlTools.BuiltinStyles.List5, (StyleType.Paragraph, "List5", "List 5") },
    { OpenXmlTools.BuiltinStyles.ListBullet2, (StyleType.Paragraph, "ListBullet2", "List Bullet 2") },
    { OpenXmlTools.BuiltinStyles.ListBullet3, (StyleType.Paragraph, "ListBullet3", "List Bullet 3") },
    { OpenXmlTools.BuiltinStyles.ListBullet4, (StyleType.Paragraph, "ListBullet4", "List Bullet 4") },
    { OpenXmlTools.BuiltinStyles.ListBullet5, (StyleType.Paragraph, "ListBullet5", "List Bullet 5") },
    { OpenXmlTools.BuiltinStyles.ListNumber2, (StyleType.Paragraph, "ListNumber2", "List Number 2") },
    { OpenXmlTools.BuiltinStyles.ListNumber3, (StyleType.Paragraph, "ListNumber3", "List Number 3") },
    { OpenXmlTools.BuiltinStyles.ListNumber4, (StyleType.Paragraph, "ListNumber4", "List Number 4") },
    { OpenXmlTools.BuiltinStyles.ListNumber5, (StyleType.Paragraph, "ListNumber5", "List Number 5") },
    { OpenXmlTools.BuiltinStyles.Title, (StyleType.Paragraph, "Title", "Title") },
    { OpenXmlTools.BuiltinStyles.TitleChar, (StyleType.Linked, "TitleChar", "Title Char") },
    { OpenXmlTools.BuiltinStyles.Closing, (StyleType.Paragraph, "Closing", "Closing") },
    { OpenXmlTools.BuiltinStyles.ClosingChar, (StyleType.Linked, "ClosingChar", "Closing Char") },
    { OpenXmlTools.BuiltinStyles.Signature, (StyleType.Paragraph, "Signature", "Signature") },
    { OpenXmlTools.BuiltinStyles.SignatureChar, (StyleType.Linked, "SignatureChar", "Signature Char") },
    { OpenXmlTools.BuiltinStyles.DefaultParagraphFont, (StyleType.Character, "DefaultParagraphFont", "Default Paragraph Font") },
    { OpenXmlTools.BuiltinStyles.BodyText, (StyleType.Paragraph, "BodyText", "Body Text") },
    { OpenXmlTools.BuiltinStyles.BodyTextChar, (StyleType.Linked, "BodyTextChar", "Body Text Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent, (StyleType.Paragraph, "BodyTextIndent", "Body Text Indent") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndentChar, (StyleType.Linked, "BodyTextIndentChar", "Body Text Indent Char") },
    { OpenXmlTools.BuiltinStyles.ListContinue, (StyleType.Paragraph, "ListContinue", "List Continue") },
    { OpenXmlTools.BuiltinStyles.ListContinue2, (StyleType.Paragraph, "ListContinue2", "List Continue 2") },
    { OpenXmlTools.BuiltinStyles.ListContinue3, (StyleType.Paragraph, "ListContinue3", "List Continue 3") },
    { OpenXmlTools.BuiltinStyles.ListContinue4, (StyleType.Paragraph, "ListContinue4", "List Continue 4") },
    { OpenXmlTools.BuiltinStyles.ListContinue5, (StyleType.Paragraph, "ListContinue5", "List Continue 5") },
    { OpenXmlTools.BuiltinStyles.MessageHeader, (StyleType.Paragraph, "MessageHeader", "Message Header") },
    { OpenXmlTools.BuiltinStyles.MessageHeaderChar, (StyleType.Linked, "MessageHeaderChar", "Message Header Char") },
    { OpenXmlTools.BuiltinStyles.Subtitle, (StyleType.Paragraph, "Subtitle", "Subtitle") },
    { OpenXmlTools.BuiltinStyles.SubtitleChar, (StyleType.Linked, "SubtitleChar", "Subtitle Char") },
    { OpenXmlTools.BuiltinStyles.Salutation, (StyleType.Paragraph, "Salutation", "Salutation") },
    { OpenXmlTools.BuiltinStyles.SalutationChar, (StyleType.Linked, "SalutationChar", "Salutation Char") },
    { OpenXmlTools.BuiltinStyles.Date, (StyleType.Paragraph, "Date", "Date") },
    { OpenXmlTools.BuiltinStyles.DateChar, (StyleType.Linked, "DateChar", "Date Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextFirstIndent, (StyleType.Paragraph, "BodyTextFirstIndent", "Body Text First Indent") },
    { OpenXmlTools.BuiltinStyles.BodyTextFirstIndentChar, (StyleType.Linked, "BodyTextFirstIndentChar", "Body Text First Indent Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextFirstIndent2, (StyleType.Paragraph, "BodyTextFirstIndent2", "Body Text First Indent 2") },
    { OpenXmlTools.BuiltinStyles.BodyTextFirstIndent2Char, (StyleType.Linked, "BodyTextFirstIndent2Char", "Body Text First Indent 2 Char") },
    { OpenXmlTools.BuiltinStyles.NoteHeading, (StyleType.Paragraph, "NoteHeading", "Note Heading") },
    { OpenXmlTools.BuiltinStyles.NoteHeadingChar, (StyleType.Linked, "NoteHeadingChar", "Note Heading Char") },
    { OpenXmlTools.BuiltinStyles.BodyText2, (StyleType.Paragraph, "BodyText2", "Body Text 2") },
    { OpenXmlTools.BuiltinStyles.BodyText2Char, (StyleType.Linked, "BodyText2Char", "Body Text 2 Char") },
    { OpenXmlTools.BuiltinStyles.BodyText3, (StyleType.Paragraph, "BodyText3", "Body Text 3") },
    { OpenXmlTools.BuiltinStyles.BodyText3Char, (StyleType.Linked, "BodyText3Char", "Body Text 3 Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent2, (StyleType.Paragraph, "BodyTextIndent2", "Body Text Indent 2") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent2Char, (StyleType.Linked, "BodyTextIndent2Char", "Body Text Indent 2 Char") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent3, (StyleType.Paragraph, "BodyTextIndent3", "Body Text Indent 3") },
    { OpenXmlTools.BuiltinStyles.BodyTextIndent3Char, (StyleType.Linked, "BodyTextIndent3Char", "Body Text Indent 3 Char") },
    { OpenXmlTools.BuiltinStyles.BlockText, (StyleType.Paragraph, "BlockText", "Block Text") },
    { OpenXmlTools.BuiltinStyles.Hyperlink, (StyleType.Character, "Hyperlink", "Hyperlink") },
    { OpenXmlTools.BuiltinStyles.FollowedHyperlink, (StyleType.Character, "FollowedHyperlink", "FollowedHyperlink") },
    { OpenXmlTools.BuiltinStyles.Strong, (StyleType.Character, "Strong", "Strong") },
    { OpenXmlTools.BuiltinStyles.Emphasis, (StyleType.Character, "Emphasis", "Emphasis") },
    { OpenXmlTools.BuiltinStyles.DocumentMap, (StyleType.Paragraph, "DocumentMap", "Document Map") },
    { OpenXmlTools.BuiltinStyles.DocumentMapChar, (StyleType.Linked, "DocumentMapChar", "Document Map Char") },
    { OpenXmlTools.BuiltinStyles.PlainText, (StyleType.Paragraph, "PlainText", "Plain Text") },
    { OpenXmlTools.BuiltinStyles.PlainTextChar, (StyleType.Linked, "PlainTextChar", "Plain Text Char") },
    { OpenXmlTools.BuiltinStyles.EmailSignature, (StyleType.Paragraph, "EmailSignature", "E-mail Signature") },
    { OpenXmlTools.BuiltinStyles.EmailSignatureChar, (StyleType.Linked, "EmailSignatureChar", "E-mail Signature Char") },
    { OpenXmlTools.BuiltinStyles.zTopOfForm, (StyleType.Paragraph, "zTopOfForm", "z-Top of Form") },
    { OpenXmlTools.BuiltinStyles.zTopOfFormChar, (StyleType.Linked, "zTopOfFormChar", "z-Top of Form Char") },
    { OpenXmlTools.BuiltinStyles.zBottomOfForm, (StyleType.Paragraph, "zBottomOfForm", "z-Bottom of Form") },
    { OpenXmlTools.BuiltinStyles.zBottomOfFormChar, (StyleType.Linked, "zBottomOfFormChar", "z-Bottom of Form Char") },
    { OpenXmlTools.BuiltinStyles.NormalWeb, (StyleType.Paragraph, "NormalWeb", "Normal (Web)") },
    { OpenXmlTools.BuiltinStyles.HtmlAcronym, (StyleType.Character, "HtmlAcronym", "HTML Acronym") },
    { OpenXmlTools.BuiltinStyles.HtmlAddress, (StyleType.Paragraph, "HtmlAddress", "HTML Address") },
    { OpenXmlTools.BuiltinStyles.HtmlAddressChar, (StyleType.Linked, "HtmlAddressChar", "HTML Address Char") },
    { OpenXmlTools.BuiltinStyles.HtmlCite, (StyleType.Character, "HtmlCite", "HTML Cite") },
    { OpenXmlTools.BuiltinStyles.HtmlCode, (StyleType.Character, "HtmlCode", "HTML Code") },
    { OpenXmlTools.BuiltinStyles.HtmlDefinition, (StyleType.Character, "HtmlDefinition", "HTML Definition") },
    { OpenXmlTools.BuiltinStyles.HtmlKeyboard, (StyleType.Character, "HtmlKeyboard", "HTML Keyboard") },
    { OpenXmlTools.BuiltinStyles.HtmlPreformatted, (StyleType.Paragraph, "HtmlPreformatted", "HTML Preformatted") },
    { OpenXmlTools.BuiltinStyles.HtmlPreformattedChar, (StyleType.Linked, "HtmlPreformattedChar", "HTML Preformatted Char") },
    { OpenXmlTools.BuiltinStyles.HtmlSample, (StyleType.Character, "HtmlSample", "HTML Sample") },
    { OpenXmlTools.BuiltinStyles.HtmlTypewriter, (StyleType.Character, "HtmlTypewriter", "HTML Typewriter") },
    { OpenXmlTools.BuiltinStyles.HtmlVariable, (StyleType.Character, "HtmlVariable", "HTML Variable") },
    { OpenXmlTools.BuiltinStyles.NormalTable, (StyleType.Table, "NormalTable", "Normal Table") },
    { OpenXmlTools.BuiltinStyles.CommentSubject, (StyleType.Paragraph, "CommentSubject", "Comment Subject") },
    { OpenXmlTools.BuiltinStyles.CommentSubjectChar, (StyleType.Linked, "CommentSubjectChar", "Comment Subject Char") },
    { OpenXmlTools.BuiltinStyles.NoList, (StyleType.Numbering, "NoList", "No List") },
    { OpenXmlTools.BuiltinStyles.OutlineList1, (StyleType.Numbering, "OutlineList1", "1 / a / i") },
    { OpenXmlTools.BuiltinStyles.OutlineList2, (StyleType.Numbering, "OutlineList2", "1 / 1.1 / 1.1.1") },
    { OpenXmlTools.BuiltinStyles.OutlineList3, (StyleType.Numbering, "OutlineList3", "Article / Section") },
    { OpenXmlTools.BuiltinStyles.TableSimple1, (StyleType.Table, "TableSimple1", "Table Simple 1") },
    { OpenXmlTools.BuiltinStyles.TableSimple2, (StyleType.Table, "TableSimple2", "Table Simple 2") },
    { OpenXmlTools.BuiltinStyles.TableSimple3, (StyleType.Table, "TableSimple3", "Table Simple 3") },
    { OpenXmlTools.BuiltinStyles.TableClassic1, (StyleType.Table, "TableClassic1", "Table Classic 1") },
    { OpenXmlTools.BuiltinStyles.TableClassic2, (StyleType.Table, "TableClassic2", "Table Classic 2") },
    { OpenXmlTools.BuiltinStyles.TableClassic3, (StyleType.Table, "TableClassic3", "Table Classic 3") },
    { OpenXmlTools.BuiltinStyles.TableClassic4, (StyleType.Table, "TableClassic4", "Table Classic 4") },
    { OpenXmlTools.BuiltinStyles.TableColorful1, (StyleType.Table, "TableColorful1", "Table Colorful 1") },
    { OpenXmlTools.BuiltinStyles.TableColorful2, (StyleType.Table, "TableColorful2", "Table Colorful 2") },
    { OpenXmlTools.BuiltinStyles.TableColorful3, (StyleType.Table, "TableColorful3", "Table Colorful 3") },
    { OpenXmlTools.BuiltinStyles.TableColumns1, (StyleType.Table, "TableColumns1", "Table Columns 1") },
    { OpenXmlTools.BuiltinStyles.TableColumns2, (StyleType.Table, "TableColumns2", "Table Columns 2") },
    { OpenXmlTools.BuiltinStyles.TableColumns3, (StyleType.Table, "TableColumns3", "Table Columns 3") },
    { OpenXmlTools.BuiltinStyles.TableColumns4, (StyleType.Table, "TableColumns4", "Table Columns 4") },
    { OpenXmlTools.BuiltinStyles.TableColumns5, (StyleType.Table, "TableColumns5", "Table Columns 5") },
    { OpenXmlTools.BuiltinStyles.TableGrid1, (StyleType.Table, "TableGrid1", "Table Grid 1") },
    { OpenXmlTools.BuiltinStyles.TableGrid2, (StyleType.Table, "TableGrid2", "Table Grid 2") },
    { OpenXmlTools.BuiltinStyles.TableGrid3, (StyleType.Table, "TableGrid3", "Table Grid 3") },
    { OpenXmlTools.BuiltinStyles.TableGrid4, (StyleType.Table, "TableGrid4", "Table Grid 4") },
    { OpenXmlTools.BuiltinStyles.TableGrid5, (StyleType.Table, "TableGrid5", "Table Grid 5") },
    { OpenXmlTools.BuiltinStyles.TableGrid6, (StyleType.Table, "TableGrid6", "Table Grid 6") },
    { OpenXmlTools.BuiltinStyles.TableGrid7, (StyleType.Table, "TableGrid7", "Table Grid 7") },
    { OpenXmlTools.BuiltinStyles.TableGrid8, (StyleType.Table, "TableGrid8", "Table Grid 8") },
    { OpenXmlTools.BuiltinStyles.TableList1, (StyleType.Table, "TableList1", "Table List 1") },
    { OpenXmlTools.BuiltinStyles.TableList2, (StyleType.Table, "TableList2", "Table List 2") },
    { OpenXmlTools.BuiltinStyles.TableList3, (StyleType.Table, "TableList3", "Table List 3") },
    { OpenXmlTools.BuiltinStyles.TableList4, (StyleType.Table, "TableList4", "Table List 4") },
    { OpenXmlTools.BuiltinStyles.TableList5, (StyleType.Table, "TableList5", "Table List 5") },
    { OpenXmlTools.BuiltinStyles.TableList6, (StyleType.Table, "TableList6", "Table List 6") },
    { OpenXmlTools.BuiltinStyles.TableList7, (StyleType.Table, "TableList7", "Table List 7") },
    { OpenXmlTools.BuiltinStyles.TableList8, (StyleType.Table, "TableList8", "Table List 8") },
    { OpenXmlTools.BuiltinStyles.Table3dEffects1, (StyleType.Table, "Table3dEffects1", "Table 3D effects 1") },
    { OpenXmlTools.BuiltinStyles.Table3dEffects2, (StyleType.Table, "Table3dEffects2", "Table 3D effects 2") },
    { OpenXmlTools.BuiltinStyles.Table3dEffects3, (StyleType.Table, "Table3dEffects3", "Table 3D effects 3") },
    { OpenXmlTools.BuiltinStyles.TableContemporary, (StyleType.Table, "TableContemporary", "Table Contemporary") },
    { OpenXmlTools.BuiltinStyles.TableElegant, (StyleType.Table, "TableElegant", "Table Elegant") },
    { OpenXmlTools.BuiltinStyles.TableProfessional, (StyleType.Table, "TableProfessional", "Table Professional") },
    { OpenXmlTools.BuiltinStyles.TableSubtle1, (StyleType.Table, "TableSubtle1", "Table Subtle 1") },
    { OpenXmlTools.BuiltinStyles.TableSubtle2, (StyleType.Table, "TableSubtle2", "Table Subtle 2") },
    { OpenXmlTools.BuiltinStyles.TableWeb1, (StyleType.Table, "TableWeb1", "Table Web 1") },
    { OpenXmlTools.BuiltinStyles.TableWeb2, (StyleType.Table, "TableWeb2", "Table Web 2") },
    { OpenXmlTools.BuiltinStyles.TableWeb3, (StyleType.Table, "TableWeb3", "Table Web 3") },
    { OpenXmlTools.BuiltinStyles.BalloonText, (StyleType.Paragraph, "BalloonText", "Balloon Text") },
    { OpenXmlTools.BuiltinStyles.BalloonTextChar, (StyleType.Linked, "BalloonTextChar", "Balloon Text Char") },
    { OpenXmlTools.BuiltinStyles.TableGrid, (StyleType.Table, "TableGrid", "Table Grid") },
    { OpenXmlTools.BuiltinStyles.TableTheme, (StyleType.Table, "TableTheme", "Table Theme") },
    { OpenXmlTools.BuiltinStyles.PlaceholderText, (StyleType.Character, "PlaceholderText", "Placeholder Text") },
    { OpenXmlTools.BuiltinStyles.NoSpacing, (StyleType.Paragraph, "NoSpacing", "No Spacing") },
    { OpenXmlTools.BuiltinStyles.LightShading, (StyleType.Table, "LightShading", "Light Shading") },
    { OpenXmlTools.BuiltinStyles.LightList, (StyleType.Table, "LightList", "Light List") },
    { OpenXmlTools.BuiltinStyles.LightGrid, (StyleType.Table, "LightGrid", "Light Grid") },
    { OpenXmlTools.BuiltinStyles.MediumShading1, (StyleType.Table, "MediumShading1", "Medium Shading 1") },
    { OpenXmlTools.BuiltinStyles.MediumShading2, (StyleType.Table, "MediumShading2", "Medium Shading 2") },
    { OpenXmlTools.BuiltinStyles.MediumList1, (StyleType.Table, "MediumList1", "Medium List 1") },
    { OpenXmlTools.BuiltinStyles.MediumList2, (StyleType.Table, "MediumList2", "Medium List 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1, (StyleType.Table, "MediumGrid1", "Medium Grid 1") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2, (StyleType.Table, "MediumGrid2", "Medium Grid 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3, (StyleType.Table, "MediumGrid3", "Medium Grid 3") },
    { OpenXmlTools.BuiltinStyles.DarkList, (StyleType.Table, "DarkList", "Dark List") },
    { OpenXmlTools.BuiltinStyles.ColorfulShading, (StyleType.Table, "ColorfulShading", "Colorful Shading") },
    { OpenXmlTools.BuiltinStyles.ColorfulList, (StyleType.Table, "ColorfulList", "Colorful List") },
    { OpenXmlTools.BuiltinStyles.ColorfulGrid, (StyleType.Table, "ColorfulGrid", "Colorful Grid") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent1, (StyleType.Table, "LightShadingAccent1", "Light Shading Accent 1") },
    { OpenXmlTools.BuiltinStyles.LightListAccent1, (StyleType.Table, "LightListAccent1", "Light List Accent 1") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent1, (StyleType.Table, "LightGridAccent1", "Light Grid Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent1, (StyleType.Table, "MediumShading1Accent1", "Medium Shading 1 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent1, (StyleType.Table, "MediumShading2Accent1", "Medium Shading 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent1, (StyleType.Table, "MediumList1Accent1", "Medium List 1 Accent 1") },
    { OpenXmlTools.BuiltinStyles.Revision, (StyleType.Paragraph, "Revision", "Revision") },
    { OpenXmlTools.BuiltinStyles.ListParagraph, (StyleType.Paragraph, "ListParagraph", "List Paragraph") },
    { OpenXmlTools.BuiltinStyles.Quote, (StyleType.Paragraph, "Quote", "Quote") },
    { OpenXmlTools.BuiltinStyles.QuoteChar, (StyleType.Linked, "QuoteChar", "Quote Char") },
    { OpenXmlTools.BuiltinStyles.IntenseQuote, (StyleType.Paragraph, "IntenseQuote", "Intense Quote") },
    { OpenXmlTools.BuiltinStyles.IntenseQuoteChar, (StyleType.Linked, "IntenseQuoteChar", "Intense Quote Char") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent1, (StyleType.Table, "MediumList2Accent1", "Medium List 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent1, (StyleType.Table, "MediumGrid1Accent1", "Medium Grid 1 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent1, (StyleType.Table, "MediumGrid2Accent1", "Medium Grid 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent1, (StyleType.Table, "MediumGrid3Accent1", "Medium Grid 3 Accent 1") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent1, (StyleType.Table, "DarkListAccent1", "Dark List Accent 1") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent1, (StyleType.Table, "ColorfulShadingAccent1", "Colorful Shading Accent 1") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent1, (StyleType.Table, "ColorfulListAccent1", "Colorful List Accent 1") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent1, (StyleType.Table, "ColorfulGridAccent1", "Colorful Grid Accent 1") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent2, (StyleType.Table, "LightShadingAccent2", "Light Shading Accent 2") },
    { OpenXmlTools.BuiltinStyles.LightListAccent2, (StyleType.Table, "LightListAccent2", "Light List Accent 2") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent2, (StyleType.Table, "LightGridAccent2", "Light Grid Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent2, (StyleType.Table, "MediumShading1Accent2", "Medium Shading 1 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent2, (StyleType.Table, "MediumShading2Accent2", "Medium Shading 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent2, (StyleType.Table, "MediumList1Accent2", "Medium List 1 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent2, (StyleType.Table, "MediumList2Accent2", "Medium List 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent2, (StyleType.Table, "MediumGrid1Accent2", "Medium Grid 1 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent2, (StyleType.Table, "MediumGrid2Accent2", "Medium Grid 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent2, (StyleType.Table, "MediumGrid3Accent2", "Medium Grid 3 Accent 2") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent2, (StyleType.Table, "DarkListAccent2", "Dark List Accent 2") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent2, (StyleType.Table, "ColorfulShadingAccent2", "Colorful Shading Accent 2") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent2, (StyleType.Table, "ColorfulListAccent2", "Colorful List Accent 2") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent2, (StyleType.Table, "ColorfulGridAccent2", "Colorful Grid Accent 2") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent3, (StyleType.Table, "LightShadingAccent3", "Light Shading Accent 3") },
    { OpenXmlTools.BuiltinStyles.LightListAccent3, (StyleType.Table, "LightListAccent3", "Light List Accent 3") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent3, (StyleType.Table, "LightGridAccent3", "Light Grid Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent3, (StyleType.Table, "MediumShading1Accent3", "Medium Shading 1 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent3, (StyleType.Table, "MediumShading2Accent3", "Medium Shading 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent3, (StyleType.Table, "MediumList1Accent3", "Medium List 1 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent3, (StyleType.Table, "MediumList2Accent3", "Medium List 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent3, (StyleType.Table, "MediumGrid1Accent3", "Medium Grid 1 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent3, (StyleType.Table, "MediumGrid2Accent3", "Medium Grid 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent3, (StyleType.Table, "MediumGrid3Accent3", "Medium Grid 3 Accent 3") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent3, (StyleType.Table, "DarkListAccent3", "Dark List Accent 3") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent3, (StyleType.Table, "ColorfulShadingAccent3", "Colorful Shading Accent 3") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent3, (StyleType.Table, "ColorfulListAccent3", "Colorful List Accent 3") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent3, (StyleType.Table, "ColorfulGridAccent3", "Colorful Grid Accent 3") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent4, (StyleType.Table, "LightShadingAccent4", "Light Shading Accent 4") },
    { OpenXmlTools.BuiltinStyles.LightListAccent4, (StyleType.Table, "LightListAccent4", "Light List Accent 4") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent4, (StyleType.Table, "LightGridAccent4", "Light Grid Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent4, (StyleType.Table, "MediumShading1Accent4", "Medium Shading 1 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent4, (StyleType.Table, "MediumShading2Accent4", "Medium Shading 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent4, (StyleType.Table, "MediumList1Accent4", "Medium List 1 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent4, (StyleType.Table, "MediumList2Accent4", "Medium List 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent4, (StyleType.Table, "MediumGrid1Accent4", "Medium Grid 1 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent4, (StyleType.Table, "MediumGrid2Accent4", "Medium Grid 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent4, (StyleType.Table, "MediumGrid3Accent4", "Medium Grid 3 Accent 4") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent4, (StyleType.Table, "DarkListAccent4", "Dark List Accent 4") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent4, (StyleType.Table, "ColorfulShadingAccent4", "Colorful Shading Accent 4") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent4, (StyleType.Table, "ColorfulListAccent4", "Colorful List Accent 4") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent4, (StyleType.Table, "ColorfulGridAccent4", "Colorful Grid Accent 4") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent5, (StyleType.Table, "LightShadingAccent5", "Light Shading Accent 5") },
    { OpenXmlTools.BuiltinStyles.LightListAccent5, (StyleType.Table, "LightListAccent5", "Light List Accent 5") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent5, (StyleType.Table, "LightGridAccent5", "Light Grid Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent5, (StyleType.Table, "MediumShading1Accent5", "Medium Shading 1 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent5, (StyleType.Table, "MediumShading2Accent5", "Medium Shading 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent5, (StyleType.Table, "MediumList1Accent5", "Medium List 1 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent5, (StyleType.Table, "MediumList2Accent5", "Medium List 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent5, (StyleType.Table, "MediumGrid1Accent5", "Medium Grid 1 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent5, (StyleType.Table, "MediumGrid2Accent5", "Medium Grid 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent5, (StyleType.Table, "MediumGrid3Accent5", "Medium Grid 3 Accent 5") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent5, (StyleType.Table, "DarkListAccent5", "Dark List Accent 5") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent5, (StyleType.Table, "ColorfulShadingAccent5", "Colorful Shading Accent 5") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent5, (StyleType.Table, "ColorfulListAccent5", "Colorful List Accent 5") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent5, (StyleType.Table, "ColorfulGridAccent5", "Colorful Grid Accent 5") },
    { OpenXmlTools.BuiltinStyles.LightShadingAccent6, (StyleType.Table, "LightShadingAccent6", "Light Shading Accent 6") },
    { OpenXmlTools.BuiltinStyles.LightListAccent6, (StyleType.Table, "LightListAccent6", "Light List Accent 6") },
    { OpenXmlTools.BuiltinStyles.LightGridAccent6, (StyleType.Table, "LightGridAccent6", "Light Grid Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumShading1Accent6, (StyleType.Table, "MediumShading1Accent6", "Medium Shading 1 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumShading2Accent6, (StyleType.Table, "MediumShading2Accent6", "Medium Shading 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumList1Accent6, (StyleType.Table, "MediumList1Accent6", "Medium List 1 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumList2Accent6, (StyleType.Table, "MediumList2Accent6", "Medium List 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumGrid1Accent6, (StyleType.Table, "MediumGrid1Accent6", "Medium Grid 1 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumGrid2Accent6, (StyleType.Table, "MediumGrid2Accent6", "Medium Grid 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.MediumGrid3Accent6, (StyleType.Table, "MediumGrid3Accent6", "Medium Grid 3 Accent 6") },
    { OpenXmlTools.BuiltinStyles.DarkListAccent6, (StyleType.Table, "DarkListAccent6", "Dark List Accent 6") },
    { OpenXmlTools.BuiltinStyles.ColorfulShadingAccent6, (StyleType.Table, "ColorfulShadingAccent6", "Colorful Shading Accent 6") },
    { OpenXmlTools.BuiltinStyles.ColorfulListAccent6, (StyleType.Table, "ColorfulListAccent6", "Colorful List Accent 6") },
    { OpenXmlTools.BuiltinStyles.ColorfulGridAccent6, (StyleType.Table, "ColorfulGridAccent6", "Colorful Grid Accent 6") },
    { OpenXmlTools.BuiltinStyles.SubtleEmphasis, (StyleType.Character, "SubtleEmphasis", "Subtle Emphasis") },
    { OpenXmlTools.BuiltinStyles.IntenseEmphasis, (StyleType.Character, "IntenseEmphasis", "Intense Emphasis") },
    { OpenXmlTools.BuiltinStyles.SubtleReference, (StyleType.Character, "SubtleReference", "Subtle Reference") },
    { OpenXmlTools.BuiltinStyles.IntenseReference, (StyleType.Character, "IntenseReference", "Intense Reference") },
    { OpenXmlTools.BuiltinStyles.BookTitle, (StyleType.Character, "BookTitle", "Book Title") },
    { OpenXmlTools.BuiltinStyles.Bibliography, (StyleType.Paragraph, "Bibliography", "Bibliography") },
    { OpenXmlTools.BuiltinStyles.TocHeading, (StyleType.Paragraph, "TocHeading", "TOC Heading") },
    { OpenXmlTools.BuiltinStyles.PlainTable1, (StyleType.Table, "PlainTable1", "Plain Table 1") },
    { OpenXmlTools.BuiltinStyles.PlainTable2, (StyleType.Table, "PlainTable2", "Plain Table 2") },
    { OpenXmlTools.BuiltinStyles.PlainTable3, (StyleType.Table, "PlainTable3", "Plain Table 3") },
    { OpenXmlTools.BuiltinStyles.PlainTable4, (StyleType.Table, "PlainTable4", "Plain Table 4") },
    { OpenXmlTools.BuiltinStyles.PlainTable5, (StyleType.Table, "PlainTable5", "Plain Table 5") },
    { OpenXmlTools.BuiltinStyles.GridTableLight, (StyleType.Table, "GridTableLight", "Grid Table Light") },
    { OpenXmlTools.BuiltinStyles.GridTable1Light, (StyleType.Table, "GridTable1Light", "Grid Table 1 Light") },
    { OpenXmlTools.BuiltinStyles.GridTable2, (StyleType.Table, "GridTable2", "Grid Table 2") },
    { OpenXmlTools.BuiltinStyles.GridTable3, (StyleType.Table, "GridTable3", "Grid Table 3") },
    { OpenXmlTools.BuiltinStyles.GridTable4, (StyleType.Table, "GridTable4", "Grid Table 4") },
    { OpenXmlTools.BuiltinStyles.GridTable5Dark, (StyleType.Table, "GridTable5Dark", "Grid Table 5 Dark") },
    { OpenXmlTools.BuiltinStyles.GridTable6Colorful, (StyleType.Table, "GridTable6Colorful", "Grid Table 6 Colorful") },
    { OpenXmlTools.BuiltinStyles.GridTable7Colorful, (StyleType.Table, "GridTable7Colorful", "Grid Table 7 Colorful") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent1, (StyleType.Table, "GridTable1LightAccent1", "Grid Table 1 Light Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent1, (StyleType.Table, "GridTable2Accent1", "Grid Table 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent1, (StyleType.Table, "GridTable3Accent1", "Grid Table 3 Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent1, (StyleType.Table, "GridTable4Accent1", "Grid Table 4 Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent1, (StyleType.Table, "GridTable5DarkAccent1", "Grid Table 5 Dark Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent1, (StyleType.Table, "GridTable6ColorfulAccent1", "Grid Table 6 Colorful Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent1, (StyleType.Table, "GridTable7ColorfulAccent1", "Grid Table 7 Colorful Accent 1") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent2, (StyleType.Table, "GridTable1LightAccent2", "Grid Table 1 Light Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent2, (StyleType.Table, "GridTable2Accent2", "Grid Table 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent2, (StyleType.Table, "GridTable3Accent2", "Grid Table 3 Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent2, (StyleType.Table, "GridTable4Accent2", "Grid Table 4 Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent2, (StyleType.Table, "GridTable5DarkAccent2", "Grid Table 5 Dark Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent2, (StyleType.Table, "GridTable6ColorfulAccent2", "Grid Table 6 Colorful Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent2, (StyleType.Table, "GridTable7ColorfulAccent2", "Grid Table 7 Colorful Accent 2") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent3, (StyleType.Table, "GridTable1LightAccent3", "Grid Table 1 Light Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent3, (StyleType.Table, "GridTable2Accent3", "Grid Table 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent3, (StyleType.Table, "GridTable3Accent3", "Grid Table 3 Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent3, (StyleType.Table, "GridTable4Accent3", "Grid Table 4 Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent3, (StyleType.Table, "GridTable5DarkAccent3", "Grid Table 5 Dark Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent3, (StyleType.Table, "GridTable6ColorfulAccent3", "Grid Table 6 Colorful Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent3, (StyleType.Table, "GridTable7ColorfulAccent3", "Grid Table 7 Colorful Accent 3") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent4, (StyleType.Table, "GridTable1LightAccent4", "Grid Table 1 Light Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent4, (StyleType.Table, "GridTable2Accent4", "Grid Table 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent4, (StyleType.Table, "GridTable3Accent4", "Grid Table 3 Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent4, (StyleType.Table, "GridTable4Accent4", "Grid Table 4 Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent4, (StyleType.Table, "GridTable5DarkAccent4", "Grid Table 5 Dark Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent4, (StyleType.Table, "GridTable6ColorfulAccent4", "Grid Table 6 Colorful Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent4, (StyleType.Table, "GridTable7ColorfulAccent4", "Grid Table 7 Colorful Accent 4") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent5, (StyleType.Table, "GridTable1LightAccent5", "Grid Table 1 Light Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent5, (StyleType.Table, "GridTable2Accent5", "Grid Table 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent5, (StyleType.Table, "GridTable3Accent5", "Grid Table 3 Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent5, (StyleType.Table, "GridTable4Accent5", "Grid Table 4 Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent5, (StyleType.Table, "GridTable5DarkAccent5", "Grid Table 5 Dark Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent5, (StyleType.Table, "GridTable6ColorfulAccent5", "Grid Table 6 Colorful Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent5, (StyleType.Table, "GridTable7ColorfulAccent5", "Grid Table 7 Colorful Accent 5") },
    { OpenXmlTools.BuiltinStyles.GridTable1LightAccent6, (StyleType.Table, "GridTable1LightAccent6", "Grid Table 1 Light Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable2Accent6, (StyleType.Table, "GridTable2Accent6", "Grid Table 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable3Accent6, (StyleType.Table, "GridTable3Accent6", "Grid Table 3 Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable4Accent6, (StyleType.Table, "GridTable4Accent6", "Grid Table 4 Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable5DarkAccent6, (StyleType.Table, "GridTable5DarkAccent6", "Grid Table 5 Dark Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable6ColorfulAccent6, (StyleType.Table, "GridTable6ColorfulAccent6", "Grid Table 6 Colorful Accent 6") },
    { OpenXmlTools.BuiltinStyles.GridTable7ColorfulAccent6, (StyleType.Table, "GridTable7ColorfulAccent6", "Grid Table 7 Colorful Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable1Light, (StyleType.Table, "ListTable1Light", "List Table 1 Light") },
    { OpenXmlTools.BuiltinStyles.ListTable2, (StyleType.Table, "ListTable2", "List Table 2") },
    { OpenXmlTools.BuiltinStyles.ListTable3, (StyleType.Table, "ListTable3", "List Table 3") },
    { OpenXmlTools.BuiltinStyles.ListTable4, (StyleType.Table, "ListTable4", "List Table 4") },
    { OpenXmlTools.BuiltinStyles.ListTable5Dark, (StyleType.Table, "ListTable5Dark", "List Table 5 Dark") },
    { OpenXmlTools.BuiltinStyles.ListTable6Colorful, (StyleType.Table, "ListTable6Colorful", "List Table 6 Colorful") },
    { OpenXmlTools.BuiltinStyles.ListTable7Colorful, (StyleType.Table, "ListTable7Colorful", "List Table 7 Colorful") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent1, (StyleType.Table, "ListTable1LightAccent1", "List Table 1 Light Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent1, (StyleType.Table, "ListTable2Accent1", "List Table 2 Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent1, (StyleType.Table, "ListTable3Accent1", "List Table 3 Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent1, (StyleType.Table, "ListTable4Accent1", "List Table 4 Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent1, (StyleType.Table, "ListTable5DarkAccent1", "List Table 5 Dark Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent1, (StyleType.Table, "ListTable6ColorfulAccent1", "List Table 6 Colorful Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent1, (StyleType.Table, "ListTable7ColorfulAccent1", "List Table 7 Colorful Accent 1") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent2, (StyleType.Table, "ListTable1LightAccent2", "List Table 1 Light Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent2, (StyleType.Table, "ListTable2Accent2", "List Table 2 Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent2, (StyleType.Table, "ListTable3Accent2", "List Table 3 Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent2, (StyleType.Table, "ListTable4Accent2", "List Table 4 Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent2, (StyleType.Table, "ListTable5DarkAccent2", "List Table 5 Dark Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent2, (StyleType.Table, "ListTable6ColorfulAccent2", "List Table 6 Colorful Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent2, (StyleType.Table, "ListTable7ColorfulAccent2", "List Table 7 Colorful Accent 2") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent3, (StyleType.Table, "ListTable1LightAccent3", "List Table 1 Light Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent3, (StyleType.Table, "ListTable2Accent3", "List Table 2 Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent3, (StyleType.Table, "ListTable3Accent3", "List Table 3 Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent3, (StyleType.Table, "ListTable4Accent3", "List Table 4 Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent3, (StyleType.Table, "ListTable5DarkAccent3", "List Table 5 Dark Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent3, (StyleType.Table, "ListTable6ColorfulAccent3", "List Table 6 Colorful Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent3, (StyleType.Table, "ListTable7ColorfulAccent3", "List Table 7 Colorful Accent 3") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent4, (StyleType.Table, "ListTable1LightAccent4", "List Table 1 Light Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent4, (StyleType.Table, "ListTable2Accent4", "List Table 2 Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent4, (StyleType.Table, "ListTable3Accent4", "List Table 3 Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent4, (StyleType.Table, "ListTable4Accent4", "List Table 4 Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent4, (StyleType.Table, "ListTable5DarkAccent4", "List Table 5 Dark Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent4, (StyleType.Table, "ListTable6ColorfulAccent4", "List Table 6 Colorful Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent4, (StyleType.Table, "ListTable7ColorfulAccent4", "List Table 7 Colorful Accent 4") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent5, (StyleType.Table, "ListTable1LightAccent5", "List Table 1 Light Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent5, (StyleType.Table, "ListTable2Accent5", "List Table 2 Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent5, (StyleType.Table, "ListTable3Accent5", "List Table 3 Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent5, (StyleType.Table, "ListTable4Accent5", "List Table 4 Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent5, (StyleType.Table, "ListTable5DarkAccent5", "List Table 5 Dark Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent5, (StyleType.Table, "ListTable6ColorfulAccent5", "List Table 6 Colorful Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent5, (StyleType.Table, "ListTable7ColorfulAccent5", "List Table 7 Colorful Accent 5") },
    { OpenXmlTools.BuiltinStyles.ListTable1LightAccent6, (StyleType.Table, "ListTable1LightAccent6", "List Table 1 Light Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable2Accent6, (StyleType.Table, "ListTable2Accent6", "List Table 2 Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable3Accent6, (StyleType.Table, "ListTable3Accent6", "List Table 3 Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable4Accent6, (StyleType.Table, "ListTable4Accent6", "List Table 4 Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable5DarkAccent6, (StyleType.Table, "ListTable5DarkAccent6", "List Table 5 Dark Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable6ColorfulAccent6, (StyleType.Table, "ListTable6ColorfulAccent6", "List Table 6 Colorful Accent 6") },
    { OpenXmlTools.BuiltinStyles.ListTable7ColorfulAccent6, (StyleType.Table, "ListTable7ColorfulAccent6", "List Table 7 Colorful Accent 6") },
    { OpenXmlTools.BuiltinStyles.Mention, (StyleType.Character, "Mention", "Mention") },
    { OpenXmlTools.BuiltinStyles.SmartHyperlink, (StyleType.Character, "SmartHyperlink", "Smart Hyperlink") },
    { OpenXmlTools.BuiltinStyles.Hashtag, (StyleType.Character, "Hashtag", "Hashtag") },
    { OpenXmlTools.BuiltinStyles.UnresolvedMention, (StyleType.Character, "UnresolvedMention", "Unresolved Mention") },
    { OpenXmlTools.BuiltinStyles.SmartLink, (StyleType.Character, "SmartLink", "Smart Link") },
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

