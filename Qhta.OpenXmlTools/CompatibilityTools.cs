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
      return PropNamesToElementNames.Count;
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
      return PropNamesToElementNames.Keys.ToArray();
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
      case "CompatibilityMode": properties.SetCompatibilityMode((int?)value);
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
        if (PropNamesToElements.TryGetValue(propertyName, out var elementType))
        {
          element2 = Activator.CreateInstance(elementType) as DXW.OnOffType;
          element2!.Val = value;
          properties.Append(element2);
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
    if (PropNamesToElementNames.TryGetValue(propName, out var elementName))
      return elementName;
    return propName.ToLowerFirst();
  }

  private static string GetPropName(string elementName)
  {
    if (ElementNamesToPropNames.TryGetValue(elementName, out var propName))
      return propName;
    return elementName.ToUpperFirst();
  }
  private static readonly Dictionary<string, string> PropNamesToElementNames = new()
  {
    { "CompatibilityMode", "compatibilityMode" },
    { "AdjustLineHeightInTable", "adjustLineHeightInTable" },
    { "AlignTablesRowByRow", "alignTablesRowByRow" },
    { "AllowHyphenationAtTrackBottom", "allowHyphenationAtTrackBottom" },
    { "AllowSpaceOfSameStyleInTable", "allowSpaceOfSameStyleInTable" },
    { "ApplyBreakingRules", "applyBreakingRules" },
    { "AutofitToFirstFixedWidthCell", "autofitToFirstFixedWidthCell" },
    { "AutoSpaceLikeWord95", "autoSpaceLikeWord95" },
    { "BalanceSingleByteDoubleByteWidth", "balanceSingleByteDoubleByteWidth" },
    { "CachedColumnBalance", "cachedColBalance" },
    { "ConvertMailMergeEscape", "convMailMergeEsc" },
    { "DifferentiateMultirowTableHeaders", "differentiateMultirowTableHeaders" },
    { "DisplayHangulFixedWidth", "displayHangulFixedWidth" },
    { "DoNotAutofitConstrainedTables", "doNotAutofitConstrainedTables" },
    { "DoNotBreakConstrainedForcedTable", "doNotBreakConstrainedForcedTable" },
    { "DoNotBreakWrappedTables", "doNotBreakWrappedTables" },
    { "DoNotExpandShiftReturn", "doNotExpandShiftReturn" },
    { "DoNotFlipMirrorIndents", "doNotFlipMirrorIndents" },
    { "DoNotLeaveBackslashAlone", "doNotLeaveBackslashAlone" },
    { "DoNotSnapToGridInCell", "doNotSnapToGridInCell" },
    { "DoNotSuppressIndentation", "doNotSuppressIndentation" },
    { "DoNotSuppressParagraphBorders", "doNotSuppressParagraphBorders" },
    { "DoNotUseEastAsianBreakRules", "doNotUseEastAsianBreakRules" },
    { "DoNotUseHTMLParagraphAutoSpacing", "doNotUseHTMLParagraphAutoSpacing" },
    { "DoNotUseIndentAsNumberingTabStop", "doNotUseIndentAsNumberingTabStop" },
    { "DoNotVerticallyAlignCellWithShape", "doNotVertAlignCellWithSp" },
    { "DoNotVerticallyAlignInTextBox", "doNotVertAlignInTxbx" },
    { "DoNotWrapTextWithPunctuation", "doNotWrapTextWithPunct" },
    { "EnableOpenTypeFeatures", "enableOpenTypeFeatures" },
    { "FootnoteLayoutLikeWord8", "footnoteLayoutLikeWW8" },
    { "ForgetLastTabAlignment", "forgetLastTabAlignment" },
    { "GrowAutofit", "growAutofit" },
    { "LayoutRawTableWidth", "layoutRawTableWidth" },
    { "LayoutTableRowsApart", "layoutTableRowsApart" },
    { "LineWrapLikeWord6", "lineWrapLikeWord6" },
    { "MacWordSmallCaps", "mwSmallCaps" },
    { "NoColumnBalance", "noColumnBalance" },
    { "NoExtraLineSpacing", "noExtraLineSpacing" },
    { "NoLeading", "noLeading" },
    { "NoSpaceRaiseLower", "noSpaceRaiseLower" },
    { "NoTabHangIndent", "noTabHangInd" },
    { "OverrideTableStyleFontSizeAndJustification", "overrideTableStyleFontSizeAndJustification" },
    { "PrintBodyTextBeforeHeader", "printBodyTextBeforeHeader" },
    { "PrintColorBlackWhite", "printColBlack" },
    { "SelectFieldWithFirstOrLastChar", "selectFldWithFirstOrLastChar" },
    { "ShapeLayoutLikeWord8", "shapeLayoutLikeWW8" },
    { "ShowBreaksInFrames", "showBreaksInFrames" },
    { "SpaceForUnderline", "spaceForUL" },
    { "SpacingInWholePoints", "spacingInWholePoints" },
    { "SplitPageBreakAndParagraphMark", "splitPgBreakAndParaMark" },
    { "SubFontBySize", "subFontBySize" },
    { "SuppressBottomSpacing", "suppressBottomSpacing" },
    { "SuppressSpacingAtTopOfPage", "suppressSpacingAtTopOfPage" },
    { "SuppressSpacingBeforeAfterPageBreak", "suppressSpBfAfterPgBrk" },
    { "SuppressTopSpacing", "suppressTopSpacing" },
    { "SuppressTopSpacingWordPerfect", "suppressTopSpacingWP" },
    { "SwapBordersFacingPages", "swapBordersFacingPages" },
    { "TruncateFontHeightsLikeWordPerfect", "truncateFontHeightsLikeWP6" },
    { "UnderlineTrailingSpaces", "ulTrailSpace" },
    { "UnderlineTabInNumberingList", "underlineTabInNumList" },
    { "UseAltKinsokuLineBreakRules", "useAltKinsokuLineBreakRules" },
    { "UseAnsiKerningPairs", "useAnsiKerningPairs" },
    { "UseFarEastLayout", "useFELayout" },
    { "UseNormalStyleForList", "useNormalStyleForList" },
    { "UsePrinterMetrics", "usePrinterMetrics" },
    { "UseSingleBorderForContiguousCells", "useSingleBorderforContiguousCells" },
    { "UseWord2002TableStyleRules", "useWord2002TableStyleRules" },
    { "UseWord2013TrackBottomHyphenation", "useWord2013TrackBottomHyphenation" },
    { "UseWord97LineBreakRules", "useWord97LineBreakRules" },
    { "WordPerfectJustification", "wpJustification" },
    { "WordPerfectSpaceWidth", "wpSpaceWidth" },
    { "WrapTrailSpaces", "wrapTrailSpaces" },
  };

  private static readonly Dictionary<string, string> ElementNamesToPropNames = new()
  {
    { "compatibilityMode", "CompatibilityMode" },
    { "adjustLineHeightInTable", "AdjustLineHeightInTable" },
    { "alignTablesRowByRow", "AlignTablesRowByRow" },
    { "allowHyphenationAtTrackBottom", "AllowHyphenationAtTrackBottom" },
    { "allowSpaceOfSameStyleInTable", "AllowSpaceOfSameStyleInTable" },
    { "applyBreakingRules", "ApplyBreakingRules" },
    { "autofitToFirstFixedWidthCell", "AutofitToFirstFixedWidthCell" },
    { "autoSpaceLikeWord95", "AutoSpaceLikeWord95" },
    { "balanceSingleByteDoubleByteWidth", "BalanceSingleByteDoubleByteWidth" },
    { "cachedColBalance", "CachedColumnBalance" },
    { "convMailMergeEsc", "ConvertMailMergeEscape" },
    { "differentiateMultirowTableHeaders", "DifferentiateMultirowTableHeaders" },
    { "displayHangulFixedWidth", "DisplayHangulFixedWidth" },
    { "doNotAutofitConstrainedTables", "DoNotAutofitConstrainedTables" },
    { "doNotBreakConstrainedForcedTable", "DoNotBreakConstrainedForcedTable" },
    { "doNotBreakWrappedTables", "DoNotBreakWrappedTables" },
    { "doNotExpandShiftReturn", "DoNotExpandShiftReturn" },
    { "doNotFlipMirrorIndents", "DoNotFlipMirrorIndents" },
    { "doNotLeaveBackslashAlone", "DoNotLeaveBackslashAlone" },
    { "doNotSnapToGridInCell", "DoNotSnapToGridInCell" },
    { "doNotSuppressIndentation", "DoNotSuppressIndentation" },
    { "doNotSuppressParagraphBorders", "DoNotSuppressParagraphBorders" },
    { "doNotUseEastAsianBreakRules", "DoNotUseEastAsianBreakRules" },
    { "doNotUseHTMLParagraphAutoSpacing", "DoNotUseHTMLParagraphAutoSpacing" },
    { "doNotUseIndentAsNumberingTabStop", "DoNotUseIndentAsNumberingTabStop" },
    { "doNotVertAlignCellWithSp", "DoNotVerticallyAlignCellWithShape" },
    { "doNotVertAlignInTxbx", "DoNotVerticallyAlignInTextBox" },
    { "doNotWrapTextWithPunct", "DoNotWrapTextWithPunctuation" },
    { "enableOpenTypeFeatures", "EnableOpenTypeFeatures" },
    { "footnoteLayoutLikeWW8", "FootnoteLayoutLikeWord8" },
    { "forgetLastTabAlignment", "ForgetLastTabAlignment" },
    { "growAutofit", "GrowAutofit" },
    { "layoutRawTableWidth", "LayoutRawTableWidth" },
    { "layoutTableRowsApart", "LayoutTableRowsApart" },
    { "lineWrapLikeWord6", "LineWrapLikeWord6" },
    { "mwSmallCaps", "MacWordSmallCaps" },
    { "noColumnBalance", "NoColumnBalance" },
    { "noExtraLineSpacing", "NoExtraLineSpacing" },
    { "noLeading", "NoLeading" },
    { "noSpaceRaiseLower", "NoSpaceRaiseLower" },
    { "noTabHangInd", "NoTabHangIndent" },
    { "overrideTableStyleFontSizeAndJustification", "OverrideTableStyleFontSizeAndJustification" },
    { "printBodyTextBeforeHeader", "PrintBodyTextBeforeHeader" },
    { "printColBlack", "PrintColorBlackWhite" },
    { "selectFldWithFirstOrLastChar", "SelectFieldWithFirstOrLastChar" },
    { "shapeLayoutLikeWW8", "ShapeLayoutLikeWord8" },
    { "showBreaksInFrames", "ShowBreaksInFrames" },
    { "spaceForUL", "SpaceForUnderline" },
    { "spacingInWholePoints", "SpacingInWholePoints" },
    { "splitPgBreakAndParaMark", "SplitPageBreakAndParagraphMark" },
    { "subFontBySize", "SubFontBySize" },
    { "suppressBottomSpacing", "SuppressBottomSpacing" },
    { "suppressSpacingAtTopOfPage", "SuppressSpacingAtTopOfPage" },
    { "suppressSpBfAfterPgBrk", "SuppressSpacingBeforeAfterPageBreak" },
    { "suppressTopSpacing", "SuppressTopSpacing" },
    { "suppressTopSpacingWP", "SuppressTopSpacingWordPerfect" },
    { "swapBordersFacingPages", "SwapBordersFacingPages" },
    { "truncateFontHeightsLikeWP6", "TruncateFontHeightsLikeWordPerfect" },
    { "ulTrailSpace", "UnderlineTrailingSpaces" },
    { "underlineTabInNumList", "UnderlineTabInNumberingList" },
    { "useAltKinsokuLineBreakRules", "UseAltKinsokuLineBreakRules" },
    { "useAnsiKerningPairs", "UseAnsiKerningPairs" },
    { "useFELayout", "UseFarEastLayout" },
    { "useNormalStyleForList", "UseNormalStyleForList" },
    { "usePrinterMetrics", "UsePrinterMetrics" },
    { "useSingleBorderforContiguousCells", "UseSingleBorderForContiguousCells" },
    { "useWord2002TableStyleRules", "UseWord2002TableStyleRules" },
    { "useWord2013TrackBottomHyphenation", "UseWord2013TrackBottomHyphenation" },
    { "useWord97LineBreakRules", "UseWord97LineBreakRules" },
    { "wpJustification", "WordPerfectJustification" },
    { "wpSpaceWidth", "WordPerfectSpaceWidth" },
    { "wrapTrailSpaces", "WrapTrailSpaces" },
  };

  private static readonly Dictionary<string, Type> PropNamesToElements = new()
  {
    { "AdjustLineHeightInTable", typeof(AdjustLineHeightInTable) },
    { "AlignTablesRowByRow", typeof(AlignTablesRowByRow) },
    { "AllowSpaceOfSameStyleInTable", typeof(AllowSpaceOfSameStyleInTable) },
    { "ApplyBreakingRules", typeof(ApplyBreakingRules) },
    { "AutofitToFirstFixedWidthCell", typeof(AutofitToFirstFixedWidthCell) },
    { "AutoSpaceLikeWord95", typeof(AutoSpaceLikeWord95) },
    { "BalanceSingleByteDoubleByteWidth", typeof(BalanceSingleByteDoubleByteWidth) },
    { "CachedColumnBalance", typeof(CachedColumnBalance) },
    { "ConvertMailMergeEscape", typeof(ConvertMailMergeEscape) },
    { "DisplayHangulFixedWidth", typeof(DisplayHangulFixedWidth) },
    { "DoNotAutofitConstrainedTables", typeof(DoNotAutofitConstrainedTables) },
    { "DoNotBreakConstrainedForcedTable", typeof(DoNotBreakConstrainedForcedTable) },
    { "DoNotBreakWrappedTables", typeof(DoNotBreakWrappedTables) },
    { "DoNotExpandShiftReturn", typeof(DoNotExpandShiftReturn) },
    { "DoNotLeaveBackslashAlone", typeof(DoNotLeaveBackslashAlone) },
    { "DoNotSnapToGridInCell", typeof(DoNotSnapToGridInCell) },
    { "DoNotSuppressIndentation", typeof(DoNotSuppressIndentation) },
    { "DoNotSuppressParagraphBorders", typeof(DoNotSuppressParagraphBorders) },
    { "DoNotUseEastAsianBreakRules", typeof(DoNotUseEastAsianBreakRules) },
    { "DoNotUseHTMLParagraphAutoSpacing", typeof(DoNotUseHTMLParagraphAutoSpacing) },
    { "DoNotUseIndentAsNumberingTabStop", typeof(DoNotUseIndentAsNumberingTabStop) },
    { "DoNotVerticallyAlignCellWithShape", typeof(DoNotVerticallyAlignCellWithShape) },
    { "DoNotVerticallyAlignInTextBox", typeof(DoNotVerticallyAlignInTextBox) },
    { "DoNotWrapTextWithPunctuation", typeof(DoNotWrapTextWithPunctuation) },
    { "FootnoteLayoutLikeWord8", typeof(FootnoteLayoutLikeWord8) },
    { "ForgetLastTabAlignment", typeof(ForgetLastTabAlignment) },
    { "GrowAutofit", typeof(GrowAutofit) },
    { "LayoutRawTableWidth", typeof(LayoutRawTableWidth) },
    { "LayoutTableRowsApart", typeof(LayoutTableRowsApart) },
    { "LineWrapLikeWord6", typeof(LineWrapLikeWord6) },
    { "MacWordSmallCaps", typeof(MacWordSmallCaps) },
    { "NoColumnBalance", typeof(NoColumnBalance) },
    { "NoExtraLineSpacing", typeof(NoExtraLineSpacing) },
    { "NoLeading", typeof(NoLeading) },
    { "NoSpaceRaiseLower", typeof(NoSpaceRaiseLower) },
    { "NoTabHangIndent", typeof(NoTabHangIndent) },
    { "PrintBodyTextBeforeHeader", typeof(PrintBodyTextBeforeHeader) },
    { "PrintColorBlackWhite", typeof(PrintColorBlackWhite) },
    { "SelectFieldWithFirstOrLastChar", typeof(SelectFieldWithFirstOrLastChar) },
    { "ShapeLayoutLikeWord8", typeof(ShapeLayoutLikeWord8) },
    { "ShowBreaksInFrames", typeof(ShowBreaksInFrames) },
    { "SpaceForUnderline", typeof(SpaceForUnderline) },
    { "SpacingInWholePoints", typeof(SpacingInWholePoints) },
    { "SplitPageBreakAndParagraphMark", typeof(SplitPageBreakAndParagraphMark) },
    { "SubFontBySize", typeof(SubFontBySize) },
    { "SuppressBottomSpacing", typeof(SuppressBottomSpacing) },
    { "SuppressSpacingAtTopOfPage", typeof(SuppressSpacingAtTopOfPage) },
    { "SuppressSpacingBeforeAfterPageBreak", typeof(SuppressSpacingBeforeAfterPageBreak) },
    { "SuppressTopSpacing", typeof(SuppressTopSpacing) },
    { "SuppressTopSpacingWordPerfect", typeof(SuppressTopSpacingWordPerfect) },
    { "SwapBordersFacingPages", typeof(SwapBordersFacingPages) },
    { "TruncateFontHeightsLikeWordPerfect", typeof(TruncateFontHeightsLikeWordPerfect) },
    { "UnderlineTrailingSpaces", typeof(UnderlineTrailingSpaces) },
    { "UnderlineTabInNumberingList", typeof(UnderlineTabInNumberingList) },
    { "UseAltKinsokuLineBreakRules", typeof(UseAltKinsokuLineBreakRules) },
    { "UseAnsiKerningPairs", typeof(UseAnsiKerningPairs) },
    { "UseFarEastLayout", typeof(UseFarEastLayout) },
    { "UseNormalStyleForList", typeof(UseNormalStyleForList) },
    { "UsePrinterMetrics", typeof(UsePrinterMetrics) },
    { "UseSingleBorderForContiguousCells", typeof(UseSingleBorderForContiguousCells) },
    { "UseWord2002TableStyleRules", typeof(UseWord2002TableStyleRules) },
    { "UseWord97LineBreakRules", typeof(UseWord97LineBreakRules) },
    { "WordPerfectJustification", typeof(WordPerfectJustification) },
    { "WordPerfectSpaceWidth", typeof(WordPerfectSpaceWidth) },
    { "WrapTrailSpaces", typeof(WrapTrailSpaces) },

  };

}
