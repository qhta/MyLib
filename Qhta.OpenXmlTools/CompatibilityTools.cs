using System;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;
/// <summary>
/// Tools for working with compatibility settings
/// </summary>
public static class CompatibilityTools
{
  /// <summary>
  /// Check if the document has compatibility settings
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument</param>
  /// <returns>True if the document has compatibility settings</returns>
  public static bool HasCompatibilitySettings(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.MainDocumentPart?.DocumentSettingsPart?.Settings?
      .Elements<DXW.Compatibility>().FirstOrDefault() != null;
  }

  /// <summary>
  /// Gets the compatibility settings from the document. If the document does not have compatibility settings, they are created.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument</param>
  /// <returns>The instance of the compatibility settings</returns>
  public static DXW.Compatibility GetCompatibilitySettings(this DXPack.WordprocessingDocument wordDoc)
  {
    var settings = wordDoc.GetSettings();
    var Compatibility = settings.GetFirstElement<DXW.Compatibility>();
    if (Compatibility == null)
    {
      Compatibility = new DXW.Compatibility();
      settings.Append(Compatibility);
    }
    return Compatibility;
  }


  /// <summary>
  /// Get the count of the compatibility settings.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="filter">specifies if all property names should be counted or non-empty ones</param>
  /// <returns></returns>
  public static int Count(this DXW.Compatibility properties, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.All)
      return PropDefs.Count;
    return properties.Elements().Count();
  }

  /// <summary>
  /// Get the names of the compatibility settings.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="filter">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public static string[] GetNames(this DXW.Compatibility properties, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.All)
      return PropDefs.Keys.ToArray();
    return properties.Elements<OpenXmlElement>().Select
      (x => (x is CompatibilitySetting compatibilitySetting) ? GetPropName(compatibilitySetting.Name!.ToString()!) : GetPropName(x.LocalName)).ToArray();
  }

  /// <summary>
  /// Get the type of property with its name.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static Type GetType(this DXW.Compatibility properties, string propertyName)
  {
    if (propertyName == "CompatibilityMode")
      return typeof(int);
    return typeof(bool?);
  }

  /// <summary>
  /// Gets the <c>WordprocessingDocument</c> from the <c>Compatibility</c> object.
  /// </summary>
  /// <param name="properties"></param>
  /// <result>wordprocessing document</result>
  public static DXPack.WordprocessingDocument? GetDocument(this DXW.Compatibility properties)
  {
    return (properties.Parent as DXW.Settings)?.DocumentSettingsPart?.OpenXmlPackage as DXPack.WordprocessingDocument;
  }

  /// <summary>
  /// Gets the value of a <c>Compatibility</c> property.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static object? GetValue(this DXW.Compatibility properties, string propertyName)
  {
    switch (propertyName)
    {
      case "CompatibilityMode": return properties.GetCompatibilityMode();
      default:
        return properties.GetCompatibilitySettingBoolVal(propertyName);
    }
  }

  /// <summary>
  /// Sets the value of a <c>Compatibility</c> property.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="propertyName"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetValue(this DXW.Compatibility properties, string propertyName, object? value)
  {
    switch (propertyName)
    {
      case "CompatibilityMode":
        properties.SetCompatibilityMode((int?)value);
        break;
      default:
        properties.SetCompatibilitySettingBoolVal(propertyName, (bool?)value);
        break;
    }
  }

  #region get settings

  /// <summary>
  /// Gets the <c>CompatibilitySetting</c> <c>Val</c> property bool value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static bool? GetCompatibilitySettingBoolVal(this DXW.Compatibility properties, string propertyName)
  {
    var elementName = GetElementName(propertyName);
    var element = properties.Elements<DXW.CompatibilitySetting>().FirstOrDefault(item => item.Name?.ToString() == elementName);
    if (element != null)
      return int.Parse(element.Val?.Value!) == 1;
    var element2 = properties.Elements<DXW.OnOffType>().FirstOrDefault(item => item.LocalName == elementName);
    if (element2 != null)
      switch (element2.Val?.Value)
      {
        case true: return true;
        case false: return false;
        default:
          return true;
      }
    return null;
  }

  /// <summary>
  /// Gets the <c>CompatibilityMode</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <returns></returns>
  public static int? GetCompatibilityMode(this Compatibility properties)
  {
    var element = properties.Elements<DXW.CompatibilitySetting>().FirstOrDefault(item => item.Name! == CompatSettingNameValues.CompatibilityMode);
    if (element != null)
      return int.Parse(element.Val?.Value!);
    return null;
  }

  #endregion get settings

  #region set settings

  /// <summary>
  /// sets the <c>CompatibilitySetting</c> <c>Val</c> property bool value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="propertyName"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetCompatibilitySettingBoolVal(this DXW.Compatibility properties, string propertyName, bool? value)
  {
    var elementName = GetElementName(propertyName);
    var compatName = (CompatSettingNameValues?)typeof(CompatSettingNameValues).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
    if (compatName != null)
    {
      var element = properties.Elements<CompatibilitySetting>().FirstOrDefault(item => item.Name! == compatName);
      if (element != null)
      {
        if (value != null)
          element.Val = new StringValue(value.ToString()!);
        else
          element.Remove();
      }
      else if (value != null)
      {
        element = new CompatibilitySetting
        {
          Name = compatName,
          Uri = "http://schemas.microsoft.com/office/word",
          Val = new StringValue((bool)value ? "1" : "0")
        };
        properties.Append(element);
      }
    }
    else
    {
      var element2 = properties.Elements<DXW.OnOffType>().FirstOrDefault(item => item.LocalName == elementName);
      if (element2 != null)
      {
        if (value != null)
        {
          if (value == false)
            element2.Val = new OnOffValue((bool)value);
        }
        else
          element2.Remove();
      }
      else if (value != null)
      {
        if (PropDefs.TryGetValue(propertyName, out var propDef))
        {
          element2 = Activator.CreateInstance(propDef.type) as DXW.OnOffType;
          if (element2 != null)
          {
            element2.Val = value;
            properties.Append(element2);
          }
        }
        else
          throw new ArgumentException($"Property {propertyName} not found in CompatibilityTools");
      }
    }
  }

  /// <summary>
  /// Sets the <c>CompatibilityMode</c> property value.
  /// </summary>
  /// <param name="properties"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetCompatibilityMode(this Compatibility properties, int? value)
  {
    var element = properties.Elements<DXW.CompatibilitySetting>().FirstOrDefault(item => item.Name! == CompatSettingNameValues.CompatibilityMode);
    if (element != null)
    {
      if (value != null)
        element.Val = new DX.StringValue(value.ToString()!);
      else
        element.Remove();
    }
    else if (value != null)
    {
      element = new DXW.CompatibilitySetting
      {
        Name = CompatSettingNameValues.CompatibilityMode,
        Uri = "http://schemas.microsoft.com/office/word",
        Val = new DX.StringValue(value.ToString()!)
      };
      properties.Append(element);
    }
  }

  #endregion set settings

  private static string GetElementName(string propName)
  {
    if (PropDefs.TryGetValue(propName, out var propDef))
      return propDef.tag;
    return propName.ToLowerFirst();
  }

  private static string GetPropName(string elementName)
  {
    foreach (var kvp in PropDefs)
      if (kvp.Value.tag == elementName)
        return kvp.Key;
    return elementName;
  }

  private static readonly Dictionary<string, (Type type, string tag, int version)> PropDefs = new()
  {
    { "CompatibilityMode", (typeof(int), "compatibilityMode", 1)},
    { "AdjustLineHeightInTable", (typeof(AdjustLineHeightInTable), "adjustLineHeightInTable", 0)},
    { "AlignTablesRowByRow", (typeof(AlignTablesRowByRow), "alignTablesRowByRow", 0)},
    { "AllowHyphenationAtTrackBottom", (typeof(DXW.CompatibilitySetting), "allowHyphenationAtTrackBottom", 1)},
    { "AllowSpaceOfSameStyleInTable", (typeof(AllowSpaceOfSameStyleInTable), "allowSpaceOfSameStyleInTable", 0)},
    { "AllowTextAfterFloatingTableBreak", (typeof(DXW.CompatibilitySetting), "allowTextAfterFloatingTableBreak",1)},
    { "ApplyBreakingRules", (typeof(ApplyBreakingRules), "applyBreakingRules", 0)},
    { "AutofitToFirstFixedWidthCell", (typeof(AutofitToFirstFixedWidthCell), "autofitToFirstFixedWidthCell", 0)},
    { "AutoSpaceLikeWord95", (typeof(AutoSpaceLikeWord95), "autoSpaceLikeWord95", 0)},
    { "BalanceSingleByteDoubleByteWidth", (typeof(BalanceSingleByteDoubleByteWidth), "balanceSingleByteDoubleByteWidth", 0)},
    { "CachedColumnBalance", (typeof(CachedColumnBalance), "cachedColBalance", 0)},
    { "ConvertMailMergeEscape", (typeof(ConvertMailMergeEscape), "convMailMergeEsc", 0)},
    { "DifferentiateMultirowTableHeaders", (typeof(DXW.CompatibilitySetting), "differentiateMultirowTableHeaders", 1)},
    { "DisplayHangulFixedWidth", (typeof(DisplayHangulFixedWidth), "displayHangulFixedWidth", 0)},
    { "DoNotAutofitConstrainedTables", (typeof(DoNotAutofitConstrainedTables), "doNotAutofitConstrainedTables", 0)},
    { "DoNotBreakConstrainedForcedTable", (typeof(DoNotBreakConstrainedForcedTable), "doNotBreakConstrainedForcedTable", 0)},
    { "DoNotBreakWrappedTables", (typeof(DoNotBreakWrappedTables), "doNotBreakWrappedTables", 0)},
    { "DoNotExpandShiftReturn", (typeof(DoNotExpandShiftReturn), "doNotExpandShiftReturn", 0)},
    { "DoNotFlipMirrorIndents", (typeof(DXW.CompatibilitySetting), "doNotFlipMirrorIndents", 1)},
    { "DoNotLeaveBackslashAlone", (typeof(DoNotLeaveBackslashAlone), "doNotLeaveBackslashAlone", 0)},
    { "DoNotSnapToGridInCell", (typeof(DoNotSnapToGridInCell), "doNotSnapToGridInCell", 0)},
    { "DoNotSuppressIndentation", (typeof(DoNotSuppressIndentation), "doNotSuppressIndentation", 0)},
    { "DoNotSuppressParagraphBorders", (typeof(DoNotSuppressParagraphBorders), "doNotSuppressParagraphBorders", 0)},
    { "DoNotUseEastAsianBreakRules", (typeof(DoNotUseEastAsianBreakRules), "doNotUseEastAsianBreakRules", 0)},
    { "DoNotUseHTMLParagraphAutoSpacing", (typeof(DoNotUseHTMLParagraphAutoSpacing), "doNotUseHTMLParagraphAutoSpacing", 0)},
    { "DoNotUseIndentAsNumberingTabStop", (typeof(DoNotUseIndentAsNumberingTabStop), "doNotUseIndentAsNumberingTabStop", 0)},
    { "DoNotVerticallyAlignCellWithShape", (typeof(DoNotVerticallyAlignCellWithShape), "doNotVertAlignCellWithSp", 0)},
    { "DoNotVerticallyAlignInTextBox", (typeof(DoNotVerticallyAlignInTextBox), "doNotVertAlignInTxbx", 0)},
    { "DoNotWrapTextWithPunctuation", (typeof(DoNotWrapTextWithPunctuation), "doNotWrapTextWithPunct", 0)},
    { "EnableOpenTypeFeatures", (typeof(DXW.CompatibilitySetting), "enableOpenTypeFeatures", 1)},
    { "FootnoteLayoutLikeWord8", (typeof(FootnoteLayoutLikeWord8), "footnoteLayoutLikeWW8", 0)},
    { "ForgetLastTabAlignment", (typeof(ForgetLastTabAlignment), "forgetLastTabAlignment", 0)},
    { "GrowAutofit", (typeof(GrowAutofit), "growAutofit", 0)},
    { "LayoutRawTableWidth", (typeof(LayoutRawTableWidth), "layoutRawTableWidth", 0)},
    { "LayoutTableRowsApart", (typeof(LayoutTableRowsApart), "layoutTableRowsApart", 0)},
    { "LineWrapLikeWord6", (typeof(LineWrapLikeWord6), "lineWrapLikeWord6", 0)},
    { "MacWordSmallCaps", (typeof(MacWordSmallCaps), "mwSmallCaps", 0)},
    { "NoColumnBalance", (typeof(NoColumnBalance), "noColumnBalance", 0)},
    { "NoExtraLineSpacing", (typeof(NoExtraLineSpacing), "noExtraLineSpacing", 0)},
    { "NoLeading", (typeof(NoLeading), "noLeading", 0)},
    { "NoSpaceRaiseLower", (typeof(NoSpaceRaiseLower), "noSpaceRaiseLower", 0)},
    { "NoTabHangIndent", (typeof(NoTabHangIndent), "noTabHangInd", 0)},
    { "OverrideTableStyleFontSizeAndJustification", (typeof(DXW.CompatibilitySetting), "overrideTableStyleFontSizeAndJustification", 1)},
    { "PrintBodyTextBeforeHeader", (typeof(PrintBodyTextBeforeHeader), "printBodyTextBeforeHeader", 0)},
    { "PrintColorBlackWhite", (typeof(PrintColorBlackWhite), "printColBlack", 0)},
    { "SelectFieldWithFirstOrLastChar", (typeof(SelectFieldWithFirstOrLastChar), "selectFldWithFirstOrLastChar", 0)},
    { "ShapeLayoutLikeWord8", (typeof(ShapeLayoutLikeWord8), "shapeLayoutLikeWW8", 0)},
    { "ShowBreaksInFrames", (typeof(ShowBreaksInFrames), "showBreaksInFrames", 0)},
    { "SpaceForUnderline", (typeof(SpaceForUnderline), "spaceForUL", 0)},
    { "SpacingInWholePoints", (typeof(SpacingInWholePoints), "spacingInWholePoints", 0)},
    { "SplitPageBreakAndParagraphMark", (typeof(SplitPageBreakAndParagraphMark), "splitPgBreakAndParaMark", 0)},
    { "SubFontBySize", (typeof(SubFontBySize), "subFontBySize", 0)},
    { "SuppressBottomSpacing", (typeof(SuppressBottomSpacing), "suppressBottomSpacing", 0)},
    { "SuppressSpacingAtTopOfPage", (typeof(SuppressSpacingAtTopOfPage), "suppressSpacingAtTopOfPage", 0)},
    { "SuppressSpacingBeforeAfterPageBreak", (typeof(SuppressSpacingBeforeAfterPageBreak), "suppressSpBfAfterPgBrk", 0)},
    { "SuppressTopSpacing", (typeof(SuppressTopSpacing), "suppressTopSpacing", 0)},
    { "SuppressTopSpacingWordPerfect", (typeof(SuppressTopSpacingWordPerfect), "suppressTopSpacingWP", 0)},
    { "SwapBordersFacingPages", (typeof(SwapBordersFacingPages), "swapBordersFacingPages", 0)},
    { "TruncateFontHeightsLikeWordPerfect", (typeof(TruncateFontHeightsLikeWordPerfect), "truncateFontHeightsLikeWP6", 0)},
    { "UnderlineTrailingSpaces", (typeof(UnderlineTrailingSpaces), "ulTrailSpace", 0)},
    { "UnderlineTabInNumberingList", (typeof(UnderlineTabInNumberingList), "underlineTabInNumList", 0)},
    { "UseAltKinsokuLineBreakRules", (typeof(UseAltKinsokuLineBreakRules), "useAltKinsokuLineBreakRules", 0)},
    { "UseAnsiKerningPairs", (typeof(UseAnsiKerningPairs), "useAnsiKerningPairs", 0)},
    { "UseFarEastLayout", (typeof(UseFarEastLayout), "useFELayout", 0)},
    { "UseNormalStyleForList", (typeof(UseNormalStyleForList), "useNormalStyleForList", 0)},
    { "UsePrinterMetrics", (typeof(UsePrinterMetrics), "usePrinterMetrics", 0)},
    { "UseSingleBorderForContiguousCells", (typeof(UseSingleBorderForContiguousCells), "useSingleBorderForContiguousCells", 0)},
    { "UseWord2002TableStyleRules", (typeof(UseWord2002TableStyleRules), "useWord2002TableStyleRules", 0)},
    { "UseWord2013TrackBottomHyphenation", (typeof(DXW.CompatibilitySetting), "useWord2013TrackBottomHyphenation", 1)},
    { "UseWord97LineBreakRules", (typeof(UseWord97LineBreakRules), "useWord97LineBreakRules", 0)},
    { "WordPerfectJustification", (typeof(WordPerfectJustification), "wpJustification", 0)},
    { "WordPerfectSpaceWidth", (typeof(WordPerfectSpaceWidth), "wpSpaceWidth", 0)},
    { "WrapTrailSpaces", (typeof(WrapTrailSpaces), "wrapTrailSpaces", 0)},
  };

}
