using System;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with styles in OpenXml documents.
/// </summary>
public static partial class StylesTools
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
    var result = styleName.StartsWith("Heading", StringComparison.OrdinalIgnoreCase);
    return result;
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
  /// Checks if the style name starts with "Heading".
  /// If true, returns the level of the heading (starting with 1).
  /// </summary>
  /// <param name="styleName">Name to check</param>
  /// <returns>level of the heading (starting with 1) or null</returns>
  public static int? HeadingLevel(string styleName)
  {
    if (styleName.StartsWith("Heading", StringComparison.OrdinalIgnoreCase))
    {
      var level = styleName.Substring(7);
      if (int.TryParse(level, out var result))
        return result;
    }
    return null;
  }

  /// <summary>
  /// Checks if the style name starts with "Heading".
  /// If true, returns the level of the heading (starting with 1).
  /// </summary>
  /// <param name="styles"></param>
  /// <param name="styleName">Name to check</param>
  /// <returns>level of the heading (starting with 1) or null</returns>
  public static int? HeadingLevel(this DXW.Styles styles, string styleName)
  {
    return HeadingLevel(styleName);
  }

  /// <summary>
  /// Checks if the style is a heading style.
  /// If true, returns the level of the heading (starting with 1).
  /// </summary>
  /// <param name="style">Style to check</param>
  /// <returns>level of the heading (starting with 1) or null</returns>
  public static int? HeadingLevel(this DXW.Style style)
  {
    return HeadingLevel(style.StyleName!.Val!.Value!);
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
  /// <param name="buildInStylesFileName">Path to a docx with buildIn styles defined and used</param>
  /// <returns>Instance of the styles element</returns>
  public static DXW.Styles GetStyles(this DXPack.WordprocessingDocument wordDoc, string? buildInStylesFileName = null)
  {
    var mainDocumentPart = wordDoc.GetMainDocumentPart();
    var styleDefinitionsPart = mainDocumentPart.StyleDefinitionsPart ??
                               mainDocumentPart.AddNewPart<DXPack.StyleDefinitionsPart>();

    styleDefinitionsPart.Styles ??= (styleDefinitionsPart.Styles = new DXW.Styles());
    if (buildInStylesFileName!=null)
      if (BuiltinStyles == null)
        LoadBuiltinStyles(buildInStylesFileName);
    return styleDefinitionsPart.Styles;
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
}

