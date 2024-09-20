using System;

using DocumentFormat.OpenXml.CustomXmlSchemaReferences;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with the Settings element.
/// </summary>
public static class SettingsTools
{

  private const string AttachedRelationshipType =
    "http://schemas.openxmlformats.org/officeDocument/2006/relationships/attachedTemplate";

  /// <summary>
  /// Checks if the document has settings.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static bool HasSettings(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.MainDocumentPart?.DocumentSettingsPart?.Settings != null;
  }

  /// <summary>
  /// Returns the settings of the document. If the settings are not found, they are created.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static Settings GetSettings(this DXPack.WordprocessingDocument wordDoc)
  {
    var mainDocumentPart = wordDoc.GetMainDocumentPart();
    var documentSettingsPart = mainDocumentPart.DocumentSettingsPart;
    if (documentSettingsPart == null)
    {
      documentSettingsPart = mainDocumentPart.AddNewPart<DXPack.DocumentSettingsPart>();
      documentSettingsPart.Settings = new Settings();
    }
    return documentSettingsPart.Settings;
  }

  /// <summary>
  /// Get the count of the settings properties.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="filter">specifies if all property names should be counted or non-empty ones</param>
  /// <returns></returns>
  public static int Count(this Settings settings, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.All)
      return PropDefs.Count;
    return PropDefs.Count(item => settings.GetValue(item.Key) != null);
  }

  /// <summary>
  /// Get the names of the settings properties.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="filter">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public static string[] GetNames(this Settings settings, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.All)
      return PropDefs.Keys.ToArray();
    return PropDefs.Where(item => settings.GetValue(item.Key) != null).Select(item => item.Key).ToArray();
  }

  /// <summary>
  /// Get the type of property with its name.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static Type GetType(this Settings settings, string propertyName)
  {
    if (PropDefs.TryGetValue(propertyName, out var info))
      return info.type;
    throw new ArgumentException($"Property {propertyName} not found");
  }

  /// <summary>
  /// Get the category of property with its name.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static SettingCategory GetCategory(this Settings settings, string propertyName)
  {
    if (PropDefs.TryGetValue(propertyName, out var info))
      return info.category;
    throw new ArgumentException($"Property {propertyName} not found");
  }

  /// <summary>
  /// Get the info of property with its name.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static (Type, SettingCategory) GetInfo(this Settings settings, string propertyName)
  {
    if (PropDefs.TryGetValue(propertyName, out var info))
      return info;
    throw new ArgumentException($"Property {propertyName} not found");
  }

  /// <summary>
  /// Gets the value of a settings property.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static object? GetValue(this Settings settings, string propertyName)
  {
    switch (propertyName)
    {
      case "BordersDoNotSurroundHeader": return settings.GetBordersDoNotSurroundHeader();
      case "DrawingGridHorizontalSpacing": return settings.GetDrawingGridHorizontalSpacing();
      case "PersistentDocumentId": return settings.GetPersistentDocumentId();
      case "View": return settings.GetView();
      case "Zoom": return settings.GetZoom();
      case "RemovePersonalInformation": return settings.GetRemovePersonalInformation();
      case "RemoveDateAndTime": return settings.GetRemoveDateAndTime();
      case "DoNotDisplayPageBoundaries": return settings.GetDoNotDisplayPageBoundaries();
      case "DisplayBackgroundShape": return settings.GetDisplayBackgroundShape();
      case "PrintPostScriptOverText": return settings.GetPrintPostScriptOverText();
      case "PrintFractionalCharacterWidth": return settings.GetPrintFractionalCharacterWidth();
      case "PrintFormsData": return settings.GetPrintFormsData();
      case "EmbedTrueTypeFonts": return settings.GetEmbedTrueTypeFonts();
      case "EmbedSystemFonts": return settings.GetEmbedSystemFonts();
      case "SaveSubsetFonts": return settings.GetSaveSubsetFonts();
      case "SaveFormsData": return settings.GetSaveFormsData();
      case "MirrorMargins": return settings.GetMirrorMargins();
      case "AlignBorderAndEdges": return settings.GetAlignBorderAndEdges();
      case "BordersDoNotSurroundFooter": return settings.GetBordersDoNotSurroundFooter();
      case "GutterAtTop": return settings.GetGutterAtTop();
      case "HideSpellingErrors": return settings.GetHideSpellingErrors();
      case "HideGrammaticalErrors": return settings.GetHideGrammaticalErrors();
      case "ActiveWritingStyle": return settings.GetActiveWritingStyle();
      case "ProofState": return settings.GetProofState();
      case "FormsDesign": return settings.GetFormsDesign();
      case "AttachedTemplate": return settings.GetAttachedTemplate();
      case "LinkStyles": return settings.GetLinkStyles();
      case "StylePaneFormatFilter": return settings.GetStylePaneFormatFilter();
      case "StylePaneSortMethods": return settings.GetStylePaneSortMethods();
      case "DocumentType": return settings.GetDocumentType();
      case "MailMerge": return settings.GetMailMerge();
      case "RevisionView": return settings.GetRevisionView();
      case "TrackRevisions": return settings.GetTrackRevisions();
      case "DoNotTrackMoves": return settings.GetDoNotTrackMoves();
      case "DoNotTrackFormatting": return settings.GetDoNotTrackFormatting();
      case "DocumentProtection": return settings.GetDocumentProtection();
      case "AutoFormatOverride": return settings.GetAutoFormatOverride();
      case "StyleLockThemesPart": return settings.GetStyleLockThemesPart();
      case "StyleLockStylesPart": return settings.GetStyleLockStylesPart();
      case "DefaultTabStop": return settings.GetDefaultTabStop();
      case "AutoHyphenation": return settings.GetAutoHyphenation();
      case "ConsecutiveHyphenLimit": return settings.GetConsecutiveHyphenLimit();
      case "HyphenationZone": return settings.GetHyphenationZone();
      case "DoNotHyphenateCaps": return settings.GetDoNotHyphenateCaps();
      case "ShowEnvelope": return settings.GetShowEnvelope();
      case "SummaryLength": return settings.GetSummaryLength();
      case "ClickAndTypeStyle": return settings.GetClickAndTypeStyle();
      case "DefaultTableStyle": return settings.GetDefaultTableStyle();
      case "EvenAndOddHeaders": return settings.GetEvenAndOddHeaders();
      case "BookFoldReversePrinting": return settings.GetBookFoldReversePrinting();
      case "BookFoldPrinting": return settings.GetBookFoldPrinting();
      case "BookFoldPrintingSheets": return settings.GetBookFoldPrintingSheets();
      case "WriteProtection": return settings.GetWriteProtection();
      case "DrawingGridVerticalSpacing": return settings.GetDrawingGridVerticalSpacing();
      case "DisplayHorizontalDrawingGrid": return settings.GetDisplayHorizontalDrawingGrid();
      case "DisplayVerticalDrawingGrid": return settings.GetDisplayVerticalDrawingGrid();
      case "DoNotUseMarginsForDrawingGridOrigin": return settings.GetDoNotUseMarginsForDrawingGridOrigin();
      case "DrawingGridHorizontalOrigin": return settings.GetDrawingGridHorizontalOrigin();
      case "DrawingGridVerticalOrigin": return settings.GetDrawingGridVerticalOrigin();
      case "DoNotShadeFormData": return settings.GetDoNotShadeFormData();
      case "NoPunctuationKerning": return settings.GetNoPunctuationKerning();
      case "CharacterSpacingControl": return settings.GetCharacterSpacingControl();
      case "PrintTwoOnOne": return settings.GetPrintTwoOnOne();
      case "StrictFirstAndLastChars": return settings.GetStrictFirstAndLastChars();
      case "NoLineBreaksAfterKinsoku": return settings.GetNoLineBreaksAfterKinsoku();
      case "NoLineBreaksBeforeKinsoku": return settings.GetNoLineBreaksBeforeKinsoku();
      case "SavePreviewPicture": return settings.GetSavePreviewPicture();
      case "DoNotValidateAgainstSchema": return settings.GetDoNotValidateAgainstSchema();
      case "SaveInvalidXml": return settings.GetSaveInvalidXml();
      case "IgnoreMixedContent": return settings.GetIgnoreMixedContent();
      case "AlwaysShowPlaceholderText": return settings.GetAlwaysShowPlaceholderText();
      case "DoNotDemarcateInvalidXml": return settings.GetDoNotDemarcateInvalidXml();
      case "SaveXmlDataOnly": return settings.GetSaveXmlDataOnly();
      case "UseXsltWhenSaving": return settings.GetUseXsltWhenSaving();
      case "SaveThroughXslt": return settings.GetSaveThroughXslt();
      case "ShowXmlTags": return settings.GetShowXmlTags();
      case "AlwaysMergeEmptyNamespace": return settings.GetAlwaysMergeEmptyNamespace();
      case "UpdateFieldsOnOpen": return settings.GetUpdateFieldsOnOpen();
      case "HeaderShapeDefaults": return settings.GetHeaderShapeDefaults();
      case "FootnoteDocumentWideProperties": return settings.GetFootnoteDocumentWideProperties();
      case "EndnoteDocumentWideProperties": return settings.GetEndnoteDocumentWideProperties();
      case "Compatibility": return settings.GetCompatibility();
      case "DocumentVariables": return settings.GetDocumentVariables();
      case "Rsids": return settings.GetRsids();
      case "MathProperties": return settings.GetMathProperties();
      case "UICompatibleWith97To2003": return settings.GetUICompatibleWith97To2003();
      case "AttachedSchema": return settings.GetAttachedSchema();
      case "ThemeFontLanguages": return settings.GetThemeFontLanguages();
      case "ColorSchemeMapping": return settings.GetColorSchemeMapping();
      case "DoNotIncludeSubdocsInStats": return settings.GetDoNotIncludeSubdocsInStats();
      case "DoNotAutoCompressPictures": return settings.GetDoNotAutoCompressPictures();
      case "ForceUpgrade": return settings.GetForceUpgrade();
      case "Captions": return settings.GetCaptions();
      case "ReadModeInkLockDown": return settings.GetReadModeInkLockDown();
      case "SchemaLibrary": return settings.GetSchemaLibrary();
      case "ShapeDefaults": return settings.GetShapeDefaults();
      case "DecimalSymbol": return settings.GetDecimalSymbol();
      case "ListSeparator": return settings.GetListSeparator();
      case "DocumentId": return settings.GetDocumentId();
      case "DiscardImageEditingData": return settings.GetDiscardImageEditingData();
      case "DefaultImageDpi": return settings.GetDefaultImageDpi();
      case "ConflictMode": return settings.GetConflictMode();
      case "ChartTrackingRefBased": return settings.GetChartTrackingRefBased();
    }
    throw new ArgumentException($"Property {propertyName} not found");
  }

  /// <summary>
  /// Sets the value of a settings property.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="propertyName"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetValue(this Settings settings, string propertyName, object? value)
  {
    switch (propertyName)
    {
      case "BordersDoNotSurroundHeader": settings.SetBordersDoNotSurroundHeader((bool?)value); break;
      case "DrawingGridHorizontalSpacing": settings.SetDrawingGridHorizontalSpacing((Twips?)value); break;
      case "PersistentDocumentId": settings.SetPersistentDocumentId((Guid?)value); break;
      case "View": settings.SetView((ViewValues?)value); break;
      case "Zoom": settings.SetZoom((Zoom?)value); break;
      case "RemovePersonalInformation": settings.SetRemovePersonalInformation((bool?)value); break;
      case "RemoveDateAndTime": settings.SetRemoveDateAndTime((bool?)value); break;
      case "DoNotDisplayPageBoundaries": settings.SetDoNotDisplayPageBoundaries((bool?)value); break;
      case "DisplayBackgroundShape": settings.SetDisplayBackgroundShape((bool?)value); break;
      case "PrintPostScriptOverText": settings.SetPrintPostScriptOverText((bool?)value); break;
      case "PrintFractionalCharacterWidth": settings.SetPrintFractionalCharacterWidth((bool?)value); break;
      case "PrintFormsData": settings.SetPrintFormsData((bool?)value); break;
      case "EmbedTrueTypeFonts": settings.SetEmbedTrueTypeFonts((bool?)value); break;
      case "EmbedSystemFonts": settings.SetEmbedSystemFonts((bool?)value); break;
      case "SaveSubsetFonts": settings.SetSaveSubsetFonts((bool?)value); break;
      case "SaveFormsData": settings.SetSaveFormsData((bool?)value); break;
      case "MirrorMargins": settings.SetMirrorMargins((bool?)value); break;
      case "AlignBorderAndEdges": settings.SetAlignBorderAndEdges((bool?)value); break;
      case "BordersDoNotSurroundFooter": settings.SetBordersDoNotSurroundFooter((bool?)value); break;
      case "GutterAtTop": settings.SetGutterAtTop((bool?)value); break;
      case "HideSpellingErrors": settings.SetHideSpellingErrors((bool?)value); break;
      case "HideGrammaticalErrors": settings.SetHideGrammaticalErrors((bool?)value); break;
      case "ActiveWritingStyle": settings.SetActiveWritingStyle((ActiveWritingStyle?)value); break;
      case "ProofState": settings.SetProofState((ProofState?)value); break;
      case "FormsDesign": settings.SetFormsDesign((bool?)value); break;
      case "AttachedTemplate": settings.SetAttachedTemplate((string?)value); break;
      case "LinkStyles": settings.SetLinkStyles((bool?)value); break;
      case "StylePaneFormatFilter": settings.SetStylePaneFormatFilter((StylePaneFormatFilter?)value); break;
      case "StylePaneSortMethods": settings.SetStylePaneSortMethods((string?)value); break;
      case "DocumentType": settings.SetDocumentType((DocumentTypeValues?)value); break;
      case "MailMerge": settings.SetMailMerge((MailMerge?)value); break;
      case "RevisionView": settings.SetRevisionView((RevisionView?)value); break;
      case "TrackRevisions": settings.SetTrackRevisions((bool?)value); break;
      case "DoNotTrackMoves": settings.SetDoNotTrackMoves((bool?)value); break;
      case "DoNotTrackFormatting": settings.SetDoNotTrackFormatting((bool?)value); break;
      case "DocumentProtection": settings.SetDocumentProtection((DocumentProtection?)value); break;
      case "AutoFormatOverride": settings.SetAutoFormatOverride((bool?)value); break;
      case "StyleLockThemesPart": settings.SetStyleLockThemesPart((bool?)value); break;
      case "StyleLockStylesPart": settings.SetStyleLockStylesPart((bool?)value); break;
      case "DefaultTabStop": settings.SetDefaultTabStop((short?)value); break;
      case "AutoHyphenation": settings.SetAutoHyphenation((bool?)value); break;
      case "ConsecutiveHyphenLimit": settings.SetConsecutiveHyphenLimit((ushort?)value); break;
      case "HyphenationZone": settings.SetHyphenationZone((Twips?)value); break;
      case "DoNotHyphenateCaps": settings.SetDoNotHyphenateCaps((bool?)value); break;
      case "ShowEnvelope": settings.SetShowEnvelope((bool?)value); break;
      case "SummaryLength": settings.SetSummaryLength((int?)value); break;
      case "ClickAndTypeStyle": settings.SetClickAndTypeStyle((string?)value); break;
      case "DefaultTableStyle": settings.SetDefaultTableStyle((string?)value); break;
      case "EvenAndOddHeaders": settings.SetEvenAndOddHeaders((bool?)value); break;
      case "BookFoldReversePrinting": settings.SetBookFoldReversePrinting((bool?)value); break;
      case "BookFoldPrinting": settings.SetBookFoldPrinting((bool?)value); break;
      case "BookFoldPrintingSheets": settings.SetBookFoldPrintingSheets((short?)value); break;
      case "WriteProtection": settings.SetWriteProtection((WriteProtection?)value); break;
      case "DrawingGridVerticalSpacing": settings.SetDrawingGridVerticalSpacing((Twips?)value); break;
      case "DisplayHorizontalDrawingGrid": settings.SetDisplayHorizontalDrawingGrid((byte?)value); break;
      case "DisplayVerticalDrawingGrid": settings.SetDisplayVerticalDrawingGrid((byte?)value); break;
      case "DoNotUseMarginsForDrawingGridOrigin": settings.SetDoNotUseMarginsForDrawingGridOrigin((bool?)value); break;
      case "DrawingGridHorizontalOrigin": settings.SetDrawingGridHorizontalOrigin((Twips?)value); break;
      case "DrawingGridVerticalOrigin": settings.SetDrawingGridVerticalOrigin((Twips?)value); break;
      case "DoNotShadeFormData": settings.SetDoNotShadeFormData((bool?)value); break;
      case "NoPunctuationKerning": settings.SetNoPunctuationKerning((bool?)value); break;
      case "CharacterSpacingControl": settings.SetCharacterSpacingControl((CharacterSpacingValues?)value); break;
      case "PrintTwoOnOne": settings.SetPrintTwoOnOne((bool?)value); break;
      case "StrictFirstAndLastChars": settings.SetStrictFirstAndLastChars((bool?)value); break;
      case "NoLineBreaksAfterKinsoku": settings.SetNoLineBreaksAfterKinsoku((NoLineBreaksAfterKinsoku?)value); break;
      case "NoLineBreaksBeforeKinsoku": settings.SetNoLineBreaksBeforeKinsoku((NoLineBreaksBeforeKinsoku?)value); break;
      case "SavePreviewPicture": settings.SetSavePreviewPicture((bool?)value); break;
      case "DoNotValidateAgainstSchema": settings.SetDoNotValidateAgainstSchema((bool?)value); break;
      case "SaveInvalidXml": settings.SetSaveInvalidXml((bool?)value); break;
      case "IgnoreMixedContent": settings.SetIgnoreMixedContent((bool?)value); break;
      case "AlwaysShowPlaceholderText": settings.SetAlwaysShowPlaceholderText((bool?)value); break;
      case "DoNotDemarcateInvalidXml": settings.SetDoNotDemarcateInvalidXml((bool?)value); break;
      case "SaveXmlDataOnly": settings.SetSaveXmlDataOnly((bool?)value); break;
      case "UseXsltWhenSaving": settings.SetUseXsltWhenSaving((bool?)value); break;
      case "SaveThroughXslt": settings.SetSaveThroughXslt((SaveThroughXslt?)value); break;
      case "ShowXmlTags": settings.SetShowXmlTags((bool?)value); break;
      case "AlwaysMergeEmptyNamespace": settings.SetAlwaysMergeEmptyNamespace((bool?)value); break;
      case "UpdateFieldsOnOpen": settings.SetUpdateFieldsOnOpen((bool?)value); break;
      case "HeaderShapeDefaults": settings.SetHeaderShapeDefaults((HeaderShapeDefaults?)value); break;
      case "FootnoteDocumentWideProperties": settings.SetFootnoteDocumentWideProperties((FootnoteDocumentWideProperties?)value); break;
      case "EndnoteDocumentWideProperties": settings.SetEndnoteDocumentWideProperties((EndnoteDocumentWideProperties?)value); break;
      case "Compatibility": settings.SetCompatibility((Compatibility?)value); break;
      case "DocumentVariables": settings.SetDocumentVariables((DocumentVariables?)value); break;
      case "Rsids": settings.SetRsids((Rsids?)value); break;
      case "MathProperties": settings.SetMathProperties((MathProperties?)value); break;
      case "UICompatibleWith97To2003": settings.SetUICompatibleWith97To2003((UICompatibleWith97To2003?)value); break;
      case "AttachedSchema": settings.SetAttachedSchema((string?)value); break;
      case "ThemeFontLanguages": settings.SetThemeFontLanguages((ThemeFontLanguages?)value); break;
      case "ColorSchemeMapping": settings.SetColorSchemeMapping((ColorSchemeMapping?)value); break;
      case "DoNotIncludeSubdocsInStats": settings.SetDoNotIncludeSubdocsInStats((bool?)value); break;
      case "DoNotAutoCompressPictures": settings.SetDoNotAutoCompressPictures((bool?)value); break;
      case "ForceUpgrade": settings.SetForceUpgrade((bool?)value); break;
      case "Captions": settings.SetCaptions((Captions?)value); break;
      case "ReadModeInkLockDown": settings.SetReadModeInkLockDown((ReadModeInkLockDown?)value); break;
      case "SchemaLibrary": settings.SetSchemaLibrary((SchemaLibrary?)value); break;
      case "ShapeDefaults": settings.SetShapeDefaults((ShapeDefaults?)value); break;
      case "DecimalSymbol": settings.SetDecimalSymbol((string?)value); break;
      case "ListSeparator": settings.SetListSeparator((string?)value); break;
      case "DocumentId": settings.SetDocumentId((int?)value); break;
      case "DiscardImageEditingData": settings.SetDiscardImageEditingData((DXO10W.OnOffValues?)value); break;
      case "DefaultImageDpi": settings.SetDefaultImageDpi((int?)value); break;
      case "ConflictMode": settings.SetConflictMode((DXO10W.OnOffValues?)value); break;
      case "ChartTrackingRefBased": settings.SetChartTrackingRefBased((DX.OnOffValue?)value); break;

    }
  }

  #region get settings
  /// <summary>
  /// Get the <c>BordersDoNotSurroundHeader</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetBordersDoNotSurroundHeader(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<BordersDoNotSurroundHeader>();
  }

  /// <summary>
  /// Get the <c>DrawingGridHorizontalSpacing</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Twips? GetDrawingGridHorizontalSpacing(this Settings settings)
  {

    return settings.GetFirstTwipsMeasureTypeElementVal<DrawingGridHorizontalSpacing>();
  }

  /// <summary>
  /// Get the <c>DocumentProtection</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Guid? GetPersistentDocumentId(this Settings settings)
  {

    return settings.GetFirstElementGuidVal<PersistentDocumentId>();
  }

  /// <summary>
  /// Get the <c>View</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static ViewValues? GetView(this Settings settings)
  {

    return settings.GetFirstEnumTypeElementVal<View, ViewValues>();
  }

  /// <summary>
  /// Get the <c>Zoom</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Zoom? GetZoom(this Settings settings)
  {

    return settings.GetFirstElement<Zoom>();
  }

  /// <summary>
  /// Get the <c>RemovePersonalInformation</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetRemovePersonalInformation(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<RemovePersonalInformation>();
  }

  /// <summary>
  /// Get the <c>RemoveDateAndTime</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetRemoveDateAndTime(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<RemoveDateAndTime>();
  }

  /// <summary>
  /// Get the <c>DoNotDisplayPageBoundaries</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotDisplayPageBoundaries(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotDisplayPageBoundaries>();
  }

  /// <summary>
  /// Get the <c>DisplayBackgroundShape</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDisplayBackgroundShape(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DisplayBackgroundShape>();
  }

  /// <summary>
  /// Get the <c>PrintPostScriptOverText</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetPrintPostScriptOverText(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<PrintPostScriptOverText>();
  }

  /// <summary>
  /// Get the <c>PrintFractionalCharacterWidth</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetPrintFractionalCharacterWidth(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<PrintFractionalCharacterWidth>();
  }

  /// <summary>
  /// Get the <c>PrintFormsData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetPrintFormsData(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<PrintFormsData>();
  }

  /// <summary>
  /// Get the <c>EmbedTrueTypeFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetEmbedTrueTypeFonts(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<EmbedTrueTypeFonts>();
  }

  /// <summary>
  /// Get the <c>EmbedSystemFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetEmbedSystemFonts(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<EmbedSystemFonts>();
  }

  /// <summary>
  /// Get the <c>SaveSubsetFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetSaveSubsetFonts(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<SaveSubsetFonts>();
  }

  /// <summary>
  /// Get the <c>SaveFormsData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetSaveFormsData(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<SaveFormsData>();
  }

  /// <summary>
  /// Get the <c>MirrorMargins</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetMirrorMargins(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<MirrorMargins>();
  }

  /// <summary>
  /// Get the <c>AlignBordersAndEdges</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetAlignBorderAndEdges(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<AlignBorderAndEdges>();
  }

  /// <summary>
  /// Get the <c>BordersDoNotSurroundFooter</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetBordersDoNotSurroundFooter(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<BordersDoNotSurroundFooter>();
  }

  /// <summary>
  /// Get the <c>DoNotSuppressParagraphBorders</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetGutterAtTop(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<GutterAtTop>();
  }

  /// <summary>
  /// Get the <c>HideSpellingErrors</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetHideSpellingErrors(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<HideSpellingErrors>();
  }

  /// <summary>
  /// Get the <c>HideGrammaticalErrors</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetHideGrammaticalErrors(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<HideGrammaticalErrors>();
  }

  /// <summary>
  /// Get the <c>ActiveWritingStyle</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static ActiveWritingStyle? GetActiveWritingStyle(this Settings settings)
  {

    return settings.GetFirstElement<ActiveWritingStyle>();
  }

  /// <summary>
  /// Get the <c>ProofState</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static ProofState? GetProofState(this Settings settings)
  {

    return settings.GetFirstElement<ProofState>();
  }

  /// <summary>
  /// Get the <c>FormsDesign</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetFormsDesign(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<FormsDesign>();
  }

  /// <summary>
  /// Get the <c>AttachedTemplate</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetAttachedTemplate(this Settings settings)
  {

    var rId = settings.GetFirstRelationshipElementId<AttachedTemplate>();
    if (rId != null)
      return settings.GetRelationshipValue(AttachedRelationshipType, rId);
    return null;
  }

  /// <summary>
  /// Get the <c>LinkStyles</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetLinkStyles(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<LinkStyles>();
  }

  /// <summary>
  /// Get the <c>StylePaneFormatFilter</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static StylePaneFormatFilter? GetStylePaneFormatFilter(this Settings settings)
  {

    return settings.GetFirstElement<StylePaneFormatFilter>();
  }

  /// <summary>
  /// Get the <c>StylePaneSortMethod</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetStylePaneSortMethods(this Settings settings)
  {

    return settings.GetFirstElementStringVal<StylePaneSortMethods>();
  }

  /// <summary>
  /// Get the <c>DocumentType</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static DocumentTypeValues? GetDocumentType(this Settings settings)
  {

    return settings.GetFirstEnumTypeElementVal<DocumentType, DocumentTypeValues>();
  }

  /// <summary>
  /// Get the <c>MailMerge</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static MailMerge? GetMailMerge(this Settings settings)
  {

    return settings.GetFirstElement<MailMerge>();
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static RevisionView? GetRevisionView(this Settings settings)
  {

    return settings.GetFirstElement<RevisionView>();
  }

  /// <summary>
  /// Get the <c>TrackRevisions</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetTrackRevisions(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<TrackRevisions>();
  }

  /// <summary>
  /// Get the <c>DoNotTrackMoves</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotTrackMoves(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotTrackMoves>();
  }

  /// <summary>
  /// Get the <c>DoNotTrackFormatting</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotTrackFormatting(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotTrackFormatting>();
  }

  /// <summary>
  /// Get the <c>DocumentProtection</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static DocumentProtection? GetDocumentProtection(this Settings settings)
  {

    return settings.GetFirstElement<DocumentProtection>();
  }

  /// <summary>
  /// Get the <c>AutoFormatOverride</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetAutoFormatOverride(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<AutoFormatOverride>();
  }

  /// <summary>
  /// Get the <c>StyleLockTheme</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetStyleLockThemesPart(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<StyleLockThemesPart>();
  }

  /// <summary>
  /// Get the <c>StyleLockQFSet</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetStyleLockStylesPart(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<StyleLockStylesPart>();
  }

  /// <summary>
  /// Get the <c>DefaultTabStop</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static short? GetDefaultTabStop(this Settings settings)
  {

    return settings.GetFirstNonNegativeShortTypeElementVal<DefaultTabStop>();
  }

  /// <summary>
  /// Get the <c>AutoHyphenation</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetAutoHyphenation(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<AutoHyphenation>();
  }

  /// <summary>
  /// Get the <c>ConsecutiveHyphenLimit</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static ushort? GetConsecutiveHyphenLimit(this Settings settings)
  {

    return settings.GetFirstElementUShortVal<ConsecutiveHyphenLimit>();
  }

  /// <summary>
  /// Get the <c>HyphenationZone</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Twips? GetHyphenationZone(this Settings settings)
  {

    return settings.GetFirstTwipsMeasureTypeElementVal<HyphenationZone>();
  }

  /// <summary>
  /// Get the <c>DoNotHyphenateCaps</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotHyphenateCaps(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotHyphenateCaps>();
  }

  /// <summary>
  /// Get the <c>DoNotLeaveBackslashAlone</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetShowEnvelope(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<ShowEnvelope>();
  }

  /// <summary>
  /// Get the <c>SummaryLength</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static int? GetSummaryLength(this Settings settings)
  {

    return settings.GetFirstElementIntVal<SummaryLength>();
  }

  /// <summary>
  /// Get the <c>ClickAndTypeStyle</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetClickAndTypeStyle(this Settings settings)
  {

    return settings.GetFirstString253TypeElementVal<ClickAndTypeStyle>();
  }

  /// <summary>
  /// Get the <c>DefaultTableStyle</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetDefaultTableStyle(this Settings settings)
  {

    return settings.GetFirstString253TypeElementVal<DefaultTableStyle>();
  }

  /// <summary>
  /// Get the <c>EvenAndOddHeaders</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetEvenAndOddHeaders(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<EvenAndOddHeaders>();
  }

  /// <summary>
  /// Get the <c>BookFoldPrintingSheets</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetBookFoldReversePrinting(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<BookFoldReversePrinting>();
  }

  /// <summary>
  /// Get the <c>BookFoldPrinting</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetBookFoldPrinting(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<BookFoldPrinting>();
  }

  /// <summary>
  /// Get the <c>BookFoldPrintingSheets</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static short? GetBookFoldPrintingSheets(this Settings settings)
  {

    return settings.GetFirstNonNegativeShortTypeElementVal<BookFoldPrintingSheets>();
  }

  /// <summary>
  /// Get the <c>WriteProtection</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static WriteProtection? GetWriteProtection(this Settings settings)
  {

    return settings.GetFirstElement<WriteProtection>();
  }

  /// <summary>
  /// Get the <c>DrawingGridVerticalSpacing</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Twips? GetDrawingGridVerticalSpacing(this Settings settings)
  {

    return settings.GetFirstTwipsMeasureTypeElementVal<DrawingGridVerticalSpacing>();
  }

  /// <summary>
  /// Get the <c>DisplayHorizontalDrawingGrid</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static byte? GetDisplayHorizontalDrawingGrid(this Settings settings)
  {

    return settings.GetFirstUnsignedInt7TypeElementVal<DisplayHorizontalDrawingGrid>();
  }

  /// <summary>
  /// Get the <c>DisplayVerticalDrawingGrid</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static byte? GetDisplayVerticalDrawingGrid(this Settings settings)
  {

    return settings.GetFirstUnsignedInt7TypeElementVal<DisplayVerticalDrawingGrid>();
  }

  /// <summary>
  /// Get the <c>DoNotUseMarginsForDrawingGridOrigin</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotUseMarginsForDrawingGridOrigin(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotUseMarginsForDrawingGridOrigin>();
  }

  /// <summary>
  /// Get the <c>DoNotShadeFormData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Twips? GetDrawingGridHorizontalOrigin(this Settings settings)
  {

    return settings.GetFirstTwipsMeasureTypeElementVal<DrawingGridHorizontalOrigin>();
  }

  /// <summary>
  /// Get the <c>DrawingGridVerticalOrigin</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Twips? GetDrawingGridVerticalOrigin(this Settings settings)
  {

    return settings.GetFirstTwipsMeasureTypeElementVal<DrawingGridVerticalOrigin>();
  }

  /// <summary>
  /// Get the <c>DoNotShadeFormData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotShadeFormData(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotShadeFormData>();
  }

  /// <summary>
  /// Get the <c>NoPunctuationKerning</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetNoPunctuationKerning(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<NoPunctuationKerning>();
  }

  /// <summary>
  /// Get the <c>CharacterSpacingControl</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static CharacterSpacingValues? GetCharacterSpacingControl(this Settings settings)
  {

    return settings.GetFirstEnumTypeElementVal<CharacterSpacingControl, CharacterSpacingValues>();
  }

  /// <summary>
  /// Get the <c>PrintTwoOnOne</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetPrintTwoOnOne(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<PrintTwoOnOne>();
  }

  /// <summary>
  /// Get the <c>StrictFirstAndLastChars</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetStrictFirstAndLastChars(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<StrictFirstAndLastChars>();
  }

  /// <summary>
  /// Get the <c>NoLineBreaksAfterKinsoku</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static NoLineBreaksAfterKinsoku? GetNoLineBreaksAfterKinsoku(this Settings settings)
  {

    return settings.GetFirstElement<NoLineBreaksAfterKinsoku>();
  }

  /// <summary>
  /// Get the <c>NoLineBreaksBeforeKinsoku</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static NoLineBreaksBeforeKinsoku? GetNoLineBreaksBeforeKinsoku(this Settings settings)
  {

    return settings.GetFirstElement<NoLineBreaksBeforeKinsoku>();
  }

  /// <summary>
  /// Get the <c>SavePreviewPicture</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetSavePreviewPicture(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<SavePreviewPicture>();
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotValidateAgainstSchema(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotValidateAgainstSchema>();
  }

  /// <summary>
  /// Get the <c>SaveInvalidXml</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetSaveInvalidXml(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<SaveInvalidXml>();
  }

  /// <summary>
  /// Get the <c>IgnoreMixedContent</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetIgnoreMixedContent(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<IgnoreMixedContent>();
  }

  /// <summary>
  ///   Get the <c>AlwaysShowPlaceholderText</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetAlwaysShowPlaceholderText(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<AlwaysShowPlaceholderText>();
  }

  /// <summary>
  /// Get the <c>DoNotDemarcateInvalidXml</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotDemarcateInvalidXml(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotDemarcateInvalidXml>();
  }

  /// <summary>
  /// Get the <c>SaveXmlDataOnly</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetSaveXmlDataOnly(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<SaveXmlDataOnly>();
  }

  /// <summary>
  /// Get the <c>UseXsltWhenSaving</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetUseXsltWhenSaving(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<UseXsltWhenSaving>();
  }

  /// <summary>
  /// Get the <c>SaveThroughXslt</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static SaveThroughXslt? GetSaveThroughXslt(this Settings settings)
  {

    return settings.GetFirstElement<SaveThroughXslt>();
  }

  /// <summary>
  /// Get the <c>ShowXmlTags</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetShowXmlTags(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<ShowXmlTags>();
  }

  /// <summary>
  /// Get the <c>AlwaysMergeEmptyNamespace</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetAlwaysMergeEmptyNamespace(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<AlwaysMergeEmptyNamespace>();
  }

  /// <summary>
  /// Get the <c>UpdateFieldsOnOpen</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetUpdateFieldsOnOpen(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<UpdateFieldsOnOpen>();
  }

  /// <summary>
  /// Get the <c>HeaderShapeDefaults</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static HeaderShapeDefaults? GetHeaderShapeDefaults(this Settings settings)
  {

    return settings.GetFirstElement<HeaderShapeDefaults>();
  }

  /// <summary>
  /// Get the <c>FootnoteDocumentWideProperties</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static FootnoteDocumentWideProperties? GetFootnoteDocumentWideProperties(this Settings settings)
  {

    return settings.GetFirstElement<FootnoteDocumentWideProperties>();
  }

  /// <summary>
  /// Get the <c>EndnoteDocumentWideProperties</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static EndnoteDocumentWideProperties? GetEndnoteDocumentWideProperties(this Settings settings)
  {

    return settings.GetFirstElement<EndnoteDocumentWideProperties>();
  }

  /// <summary>
  /// Get the <c>Compatibility</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Compatibility? GetCompatibility(this Settings settings)
  {

    return settings.GetFirstElement<Compatibility>();
  }

  /// <summary>
  /// Get the <c>DocumentVariables</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static DocumentVariables? GetDocumentVariables(this Settings settings)
  {

    return settings.GetFirstElement<DocumentVariables>();
  }

  /// <summary>
  /// Get the <c>Rsids</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Rsids? GetRsids(this Settings settings)
  {

    return settings.GetFirstElement<Rsids>();
  }

  /// <summary>
  /// Get the <c>MathProperties</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static MathProperties? GetMathProperties(this Settings settings)
  {
    return settings.GetFirstElement<MathProperties>();
  }

  /// <summary>
  /// Get the <c>UICompatibleWith97To2003</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static UICompatibleWith97To2003? GetUICompatibleWith97To2003(this Settings settings)
  {

    return settings.GetFirstElement<UICompatibleWith97To2003>();
  }

  /// <summary>
  /// Get the <c>AttachedSchema</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetAttachedSchema(this Settings settings)
  {

    return settings.GetFirstElementStringVal<AttachedSchema>();
  }

  /// <summary>
  /// Get the <c>ThemeFontLang</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static ThemeFontLanguages? GetThemeFontLanguages(this Settings settings)
  {

    return settings.GetFirstElement<ThemeFontLanguages>();
  }

  /// <summary>
  ///  Get the <c>ColorSchemeMapping</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static ColorSchemeMapping? GetColorSchemeMapping(this Settings settings)
  {

    return settings.GetFirstElement<ColorSchemeMapping>();
  }

  /// <summary>
  ///  Get the <c>DoNotIncludeSubdocsInStats</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotIncludeSubdocsInStats(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotIncludeSubdocsInStats>();
  }

  /// <summary>
  /// Get the <c>DoNotAutoCompressPictures</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotAutoCompressPictures(this Settings settings)
  {

    return settings.GetFirstOnOffTypeElementVal<DoNotAutoCompressPictures>();
  }

  /// <summary>
  /// Get the <c>ForceUpgrade</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetForceUpgrade(this Settings settings)
  {

    return settings.GetFirstEmptyTypeElementAsBoolean<ForceUpgrade>();
  }

  /// <summary>
  /// Get the <c>Captions</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static Captions? GetCaptions(this Settings settings)
  {

    return settings.GetFirstElement<Captions>();
  }

  /// <summary>
  /// Get the <c>ReadModeInkLockDown</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static ReadModeInkLockDown? GetReadModeInkLockDown(this Settings settings)
  {

    return settings.GetFirstElement<ReadModeInkLockDown>();
  }

  /// <summary>
  /// Get the <c>SchemaLibrary</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static SchemaLibrary? GetSchemaLibrary(this Settings settings)
  {

    return settings.GetFirstElement<SchemaLibrary>();
  }

  /// <summary>
  /// Get the <c>ShapeDefaults</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static ShapeDefaults? GetShapeDefaults(this Settings settings)
  {

    return settings.GetFirstElement<ShapeDefaults>();
  }

  /// <summary>
  /// Get the <c>DecimalSymbol</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetDecimalSymbol(this Settings settings)
  {

    return settings.GetFirstStringTypeElementVal<DecimalSymbol>();
  }

  /// <summary>
  /// Get the <c>ListSeparator</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetListSeparator(this Settings settings)
  {

    return settings.GetFirstStringTypeElementVal<ListSeparator>();
  }

  /// <summary>
  /// Get the <c>DocumentId</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static HexInt? GetDocumentId(this Settings settings)
  {

    return settings.GetFirstElementHexIntVal<DocumentId>();
  }

  /// <summary>
  /// Get the <c>DiscardImageEditingData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static DXO10W.OnOffValues? GetDiscardImageEditingData(this Settings settings)
  {

    return settings.GetFirstOnOffValuesElementVal<DiscardImageEditingData>();
  }

  /// <summary>
  /// Get the <c>DefaultImageDpi</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static int? GetDefaultImageDpi(this Settings settings)
  {

    return settings.GetFirstElementIntVal<DefaultImageDpi>();
  }

  /// <summary>
  /// Get the <c>ConflictMode</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static DXO10W.OnOffValues? GetConflictMode(this Settings settings)
  {

    return settings.GetFirstOnOffValuesElementVal<ConflictMode>();
  }

  /// <summary>
  /// Get the <c>ChartTrackingRefBased</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static DX.OnOffValue? GetChartTrackingRefBased(this Settings settings)
  {

    return settings.GetFirstOnOffValueElementVal<ChartTrackingRefBased>();
  }
  #endregion get settings

  #region set settings
  /// <summary>
  /// Set the <c>BordersDoNotSurroundHeader</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetBordersDoNotSurroundHeader(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<BordersDoNotSurroundHeader>(value);
  }

  /// <summary>
  /// Set the <c>DrawingGridHorizontalSpacing</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDrawingGridHorizontalSpacing(this Settings settings, Twips? value)
  {
    settings.SetFirstTwipsMeasureTypeElementVal<DrawingGridHorizontalSpacing>(value);
  }

  /// <summary>
  /// Set the <c>DocumentProtection</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetPersistentDocumentId(this Settings settings, Guid? value)
  {
    settings.SetFirstElementGuidVal<PersistentDocumentId>(value);
  }

  /// <summary>
  /// Set the <c>View</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetView(this Settings settings, ViewValues? value)
  {
    settings.SetFirstElementVal<View, ViewValues?>(value);
  }

  /// <summary>
  /// Set the <c>Zoom</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetZoom(this Settings settings, Zoom? value)
  {
    settings.SetFirstElement<Zoom>(value);
  }

  /// <summary>
  /// Set the <c>RemovePersonalInformation</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetRemovePersonalInformation(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<RemovePersonalInformation>(value);
  }

  /// <summary>
  /// Set the <c>RemoveDateAndTime</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetRemoveDateAndTime(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<RemoveDateAndTime>(value);
  }

  /// <summary>
  /// Set the <c>DoNotDisplayPageBoundaries</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotDisplayPageBoundaries(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotDisplayPageBoundaries>(value);
  }

  /// <summary>
  /// Set the <c>DisplayBackgroundShape</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDisplayBackgroundShape(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DisplayBackgroundShape>(value);
  }

  /// <summary>
  /// Set the <c>PrintPostScriptOverText</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetPrintPostScriptOverText(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<PrintPostScriptOverText>(value);
  }

  /// <summary>
  /// Set the <c>PrintFractionalCharacterWidth</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetPrintFractionalCharacterWidth(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<PrintFractionalCharacterWidth>(value);
  }

  /// <summary>
  /// Set the <c>PrintFormsData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetPrintFormsData(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<PrintFormsData>(value);
  }

  /// <summary>
  /// Set the <c>EmbedTrueTypeFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetEmbedTrueTypeFonts(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<EmbedTrueTypeFonts>(value);
  }

  /// <summary>
  /// Set the <c>EmbedSystemFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetEmbedSystemFonts(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<EmbedSystemFonts>(value);
  }

  /// <summary>
  /// Set the <c>SaveSubsetFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSaveSubsetFonts(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<SaveSubsetFonts>(value);
  }

  /// <summary>
  /// Set the <c>SaveFormsData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSaveFormsData(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<SaveFormsData>(value);
  }

  /// <summary>
  /// Set the <c>MirrorMargins</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetMirrorMargins(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<MirrorMargins>(value);
  }

  /// <summary>
  /// Set the <c>AlignBordersAndEdges</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAlignBorderAndEdges(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<AlignBorderAndEdges>(value);
  }

  /// <summary>
  /// Set the <c>BordersDoNotSurroundFooter</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetBordersDoNotSurroundFooter(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<BordersDoNotSurroundFooter>(value);
  }

  /// <summary>
  /// Set the <c>DoNotSuppressParagraphBorders</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetGutterAtTop(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<GutterAtTop>(value);
  }

  /// <summary>
  /// Set the <c>HideSpellingErrors</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetHideSpellingErrors(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<HideSpellingErrors>(value);
  }

  /// <summary>
  /// Set the <c>HideGrammaticalErrors</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetHideGrammaticalErrors(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<HideGrammaticalErrors>(value);
  }

  /// <summary>
  /// Set the <c>ActiveWritingStyle</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetActiveWritingStyle(this Settings settings, ActiveWritingStyle? value)
  {
    settings.SetFirstElement<ActiveWritingStyle>(value);
  }

  /// <summary>
  /// Set the <c>ProofState</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetProofState(this Settings settings, ProofState? value)
  {
    settings.SetFirstElement<ProofState>(value);
  }

  /// <summary>
  /// Set the <c>FormsDesign</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetFormsDesign(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<FormsDesign>(value);
  }

  /// <summary>
  /// Set the <c>AttachedTemplate</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAttachedTemplate(this Settings settings, string? value)
  {
    settings.SetRelationshipValue(AttachedRelationshipType, value);
  }

  /// <summary>
  /// Set the <c>LinkStyles</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetLinkStyles(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<LinkStyles>(value);
  }

  /// <summary>
  /// Set the <c>StylePaneFormatFilter</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetStylePaneFormatFilter(this Settings settings, StylePaneFormatFilter? value)
  {
    settings.SetFirstElement<StylePaneFormatFilter>(value);
  }

  /// <summary>
  /// Set the <c>StylePaneSortMethod</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetStylePaneSortMethods(this Settings settings, string? value)
  {
    settings.SetFirstElementStringVal<StylePaneSortMethods>(value);
  }

  /// <summary>
  /// Set the <c>DocumentType</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDocumentType(this Settings settings, DocumentTypeValues? value)
  {
    settings.SetFirstEnumTypeElementVal<DocumentType, DocumentTypeValues>(value);
  }

  /// <summary>
  /// Set the <c>MailMerge</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetMailMerge(this Settings settings, MailMerge? value)
  {
    settings.SetFirstElement<MailMerge>(value);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetRevisionView(this Settings settings, RevisionView? value)
  {
    settings.SetFirstElement<RevisionView>(value);
  }

  /// <summary>
  /// Set the <c>TrackRevisions</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetTrackRevisions(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<TrackRevisions>(value);
  }

  /// <summary>
  /// Set the <c>DoNotTrackMoves</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotTrackMoves(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotTrackMoves>(value);
  }

  /// <summary>
  /// Set the <c>DoNotTrackFormatting</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotTrackFormatting(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotTrackFormatting>(value);
  }

  /// <summary>
  /// Set the <c>DocumentProtection</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDocumentProtection(this Settings settings, DocumentProtection? value)
  {
    settings.SetFirstElement<DocumentProtection>(value);
  }

  /// <summary>
  /// Set the <c>AutoFormatOverride</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAutoFormatOverride(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<AutoFormatOverride>(value);
  }

  /// <summary>
  /// Set the <c>StyleLockTheme</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetStyleLockThemesPart(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<StyleLockThemesPart>(value);
  }

  /// <summary>
  /// Set the <c>StyleLockQFSet</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetStyleLockStylesPart(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<StyleLockStylesPart>(value);
  }

  /// <summary>
  /// Set the <c>DefaultTabStop</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDefaultTabStop(this Settings settings, short? value)
  {
    settings.SetFirstNonNegativeShortTypeElementVal<DefaultTabStop>(value);
  }

  /// <summary>
  /// Set the <c>AutoHyphenation</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAutoHyphenation(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<AutoHyphenation>(value);
  }

  /// <summary>
  /// Set the <c>ConsecutiveHyphenLimit</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetConsecutiveHyphenLimit(this Settings settings, ushort? value)
  {
    settings.SetFirstElementUShortVal<ConsecutiveHyphenLimit>(value);
  }

  /// <summary>
  /// Set the <c>HyphenationZone</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetHyphenationZone(this Settings settings, Twips? value)
  {
    settings.SetFirstTwipsMeasureTypeElementVal<HyphenationZone>(value);
  }

  /// <summary>
  /// Set the <c>DoNotHyphenateCaps</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotHyphenateCaps(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotHyphenateCaps>(value);
  }

  /// <summary>
  /// Set the <c>DoNotLeaveBackslashAlone</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetShowEnvelope(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<ShowEnvelope>(value);
  }

  /// <summary>
  /// Set the <c>SummaryLength</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSummaryLength(this Settings settings, int? value)
  {
    settings.SetFirstElementIntVal<SummaryLength>(value);
  }

  /// <summary>
  /// Set the <c>ClickAndTypeStyle</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetClickAndTypeStyle(this Settings settings, string? value)
  {
    settings.SetFirstString253TypeElementVal<ClickAndTypeStyle>(value);
  }

  /// <summary>
  /// Set the <c>DefaultTableStyle</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDefaultTableStyle(this Settings settings, string? value)
  {
    settings.SetFirstString253TypeElementVal<DefaultTableStyle>(value);
  }

  /// <summary>
  /// Set the <c>EvenAndOddHeaders</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetEvenAndOddHeaders(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<EvenAndOddHeaders>(value);
  }

  /// <summary>
  /// Set the <c>BookFoldPrintingSheets</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetBookFoldReversePrinting(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<BookFoldReversePrinting>(value);
  }

  /// <summary>
  /// Set the <c>BookFoldPrinting</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetBookFoldPrinting(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<BookFoldPrinting>(value);
  }

  /// <summary>
  /// Set the <c>BookFoldPrintingSheets</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetBookFoldPrintingSheets(this Settings settings, short? value)
  {
    settings.SetFirstNonNegativeShortTypeElementVal<BookFoldPrintingSheets>(value);
  }

  /// <summary>
  /// Set the <c>WriteProtection</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetWriteProtection(this Settings settings, WriteProtection? value)
  {
    settings.SetFirstElement<WriteProtection>(value);
  }

  /// <summary>
  /// Set the <c>DrawingGridVerticalSpacing</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDrawingGridVerticalSpacing(this Settings settings, Twips? value)
  {
    settings.SetFirstTwipsMeasureTypeElementVal<DrawingGridVerticalSpacing>(value);
  }

  /// <summary>
  /// Set the <c>DisplayHorizontalDrawingGrid</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDisplayHorizontalDrawingGrid(this Settings settings, byte? value)
  {
    settings.SetFirstUnsignedInt7TypeElementVal<DisplayHorizontalDrawingGrid>(value);
  }

  /// <summary>
  /// Set the <c>DisplayVerticalDrawingGrid</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDisplayVerticalDrawingGrid(this Settings settings, byte? value)
  {
    settings.SetFirstUnsignedInt7TypeElementVal<DisplayVerticalDrawingGrid>(value);
  }

  /// <summary>
  /// Set the <c>DoNotUseMarginsForDrawingGridOrigin</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotUseMarginsForDrawingGridOrigin(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotUseMarginsForDrawingGridOrigin>(value);
  }

  /// <summary>
  /// Set the <c>DoNotShadeFormData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDrawingGridHorizontalOrigin(this Settings settings, Twips? value)
  {
    settings.SetFirstTwipsMeasureTypeElementVal<DrawingGridHorizontalOrigin>(value);
  }

  /// <summary>
  /// Set the <c>DrawingGridVerticalOrigin</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDrawingGridVerticalOrigin(this Settings settings, Twips? value)
  {
    settings.SetFirstTwipsMeasureTypeElementVal<DrawingGridVerticalOrigin>(value);
  }

  /// <summary>
  /// Set the <c>DoNotShadeFormData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotShadeFormData(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotShadeFormData>(value);
  }

  /// <summary>
  /// Set the <c>NoPunctuationKerning</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetNoPunctuationKerning(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<NoPunctuationKerning>(value);
  }

  /// <summary>
  /// Set the <c>CharacterSpacingControl</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetCharacterSpacingControl(this Settings settings, CharacterSpacingValues? value)
  {
    settings.SetFirstElementVal<CharacterSpacingControl, CharacterSpacingValues?>(value);
  }

  /// <summary>
  /// Set the <c>PrintTwoOnOne</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetPrintTwoOnOne(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<PrintTwoOnOne>(value);
  }

  /// <summary>
  /// Set the <c>StrictFirstAndLastChars</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetStrictFirstAndLastChars(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<StrictFirstAndLastChars>(value);
  }

  /// <summary>
  /// Set the <c>NoLineBreaksAfterKinsoku</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetNoLineBreaksAfterKinsoku(this Settings settings, NoLineBreaksAfterKinsoku? value)
  {
    settings.SetFirstElement<NoLineBreaksAfterKinsoku>(value);
  }

  /// <summary>
  /// Set the <c>NoLineBreaksBeforeKinsoku</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetNoLineBreaksBeforeKinsoku(this Settings settings, NoLineBreaksBeforeKinsoku? value)
  {
    settings.SetFirstElement<NoLineBreaksBeforeKinsoku>(value);
  }

  /// <summary>
  /// Set the <c>SavePreviewPicture</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSavePreviewPicture(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<SavePreviewPicture>(value);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotValidateAgainstSchema(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotValidateAgainstSchema>(value);
  }

  /// <summary>
  /// Set the <c>SaveInvalidXml</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSaveInvalidXml(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<SaveInvalidXml>(value);
  }

  /// <summary>
  /// Set the <c>IgnoreMixedContent</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetIgnoreMixedContent(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<IgnoreMixedContent>(value);
  }

  /// <summary>
  ///   Set the <c>AlwaysShowPlaceholderText</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAlwaysShowPlaceholderText(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<AlwaysShowPlaceholderText>(value);
  }

  /// <summary>
  /// Set the <c>DoNotDemarcateInvalidXml</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotDemarcateInvalidXml(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotDemarcateInvalidXml>(value);
  }

  /// <summary>
  /// Set the <c>SaveXmlDataOnly</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSaveXmlDataOnly(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<SaveXmlDataOnly>(value);
  }

  /// <summary>
  /// Set the <c>UseXsltWhenSaving</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetUseXsltWhenSaving(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<UseXsltWhenSaving>(value);
  }

  /// <summary>
  /// Set the <c>SaveThroughXslt</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSaveThroughXslt(this Settings settings, SaveThroughXslt? value)
  {
    settings.SetFirstElement<SaveThroughXslt>(value);
  }

  /// <summary>
  /// Set the <c>ShowXmlTags</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetShowXmlTags(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<ShowXmlTags>(value);
  }

  /// <summary>
  /// Set the <c>AlwaysMergeEmptyNamespace</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAlwaysMergeEmptyNamespace(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<AlwaysMergeEmptyNamespace>(value);
  }

  /// <summary>
  /// Set the <c>UpdateFieldsOnOpen</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetUpdateFieldsOnOpen(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<UpdateFieldsOnOpen>(value);
  }

  /// <summary>
  /// Set the <c>HeaderShapeDefaults</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetHeaderShapeDefaults(this Settings settings, HeaderShapeDefaults? value)
  {
    settings.SetFirstElement<HeaderShapeDefaults>(value);
  }

  /// <summary>
  /// Set the <c>FootnoteDocumentWideProperties</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetFootnoteDocumentWideProperties(this Settings settings, FootnoteDocumentWideProperties? value)
  {
    settings.SetFirstElement<FootnoteDocumentWideProperties>(value);
  }

  /// <summary>
  /// Set the <c>EndnoteDocumentWideProperties</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetEndnoteDocumentWideProperties(this Settings settings, EndnoteDocumentWideProperties? value)
  {
    settings.SetFirstElement<EndnoteDocumentWideProperties>(value);
  }

  /// <summary>
  /// Set the <c>Compatibility</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetCompatibility(this Settings settings, Compatibility? value)
  {
    settings.SetFirstElement<Compatibility>(value);
  }

  /// <summary>
  /// Set the <c>DocumentVariables</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDocumentVariables(this Settings settings, DocumentVariables? value)
  {
    settings.SetFirstElement<DocumentVariables>(value);
  }

  /// <summary>
  /// Set the <c>Rsids</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetRsids(this Settings settings, Rsids? value)
  {
    settings.SetFirstElement<Rsids>(value);
  }

  /// <summary>
  /// Set the <c>MathProperties</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetMathProperties(this Settings settings, MathProperties? value)
  {
    settings.SetFirstElement<MathProperties>(value);
  }

  /// <summary>
  /// Set the <c>UICompatibleWith97To2003</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetUICompatibleWith97To2003(this Settings settings, UICompatibleWith97To2003? value)
  {
    settings.SetFirstElement<UICompatibleWith97To2003>(value);
  }

  /// <summary>
  /// Set the <c>AttachedSchema</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAttachedSchema(this Settings settings, string? value)
  {
    settings.SetFirstElementStringVal<AttachedSchema>(value);
  }

  /// <summary>
  /// Set the <c>ThemeFontLang</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetThemeFontLanguages(this Settings settings, ThemeFontLanguages? value)
  {
    settings.SetFirstElement<ThemeFontLanguages>(value);
  }

  /// <summary>
  ///  Set the <c>ColorSchemeMapping</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetColorSchemeMapping(this Settings settings, ColorSchemeMapping? value)
  {
    settings.SetFirstElement<ColorSchemeMapping>(value);
  }

  /// <summary>
  ///  Set the <c>DoNotIncludeSubdocsInStats</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotIncludeSubdocsInStats(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotIncludeSubdocsInStats>(value);
  }

  /// <summary>
  /// Set the <c>DoNotAutoCompressPictures</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotAutoCompressPictures(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffTypeElementVal<DoNotAutoCompressPictures>(value);
  }

  /// <summary>
  /// Set the <c>ForceUpgrade</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetForceUpgrade(this Settings settings, bool? value)
  {
    settings.SetFirstEmptyTypeElementAsBoolean<ForceUpgrade>(value);
  }

  /// <summary>
  /// Set the <c>Captions</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetCaptions(this Settings settings, Captions? value)
  {
    settings.SetFirstElement<Captions>(value);
  }

  /// <summary>
  /// Set the <c>ReadModeInkLockDown</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetReadModeInkLockDown(this Settings settings, ReadModeInkLockDown? value)
  {
    settings.SetFirstElement<ReadModeInkLockDown>(value);
  }

  /// <summary>
  /// Set the <c>SchemaLibrary</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSchemaLibrary(this Settings settings, SchemaLibrary? value)
  {
    settings.SetFirstElement<SchemaLibrary>(value);
  }

  /// <summary>
  /// Set the <c>ShapeDefaults</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetShapeDefaults(this Settings settings, ShapeDefaults? value)
  {
    settings.SetFirstElement<ShapeDefaults>(value);
  }

  /// <summary>
  /// Set the <c>DecimalSymbol</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDecimalSymbol(this Settings settings, string? value)
  {
    settings.SetFirstStringTypeElementVal<DecimalSymbol>(value);
  }

  /// <summary>
  /// Set the <c>ListSeparator</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetListSeparator(this Settings settings, string? value)
  {
    settings.SetFirstStringTypeElementVal<ListSeparator>(value);
  }

  /// <summary>
  /// Set the <c>DocumentId</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDocumentId(this Settings settings, int? value)
  {
    settings.SetFirstElementHexIntVal<DocumentId>(value);
  }

  /// <summary>
  /// Set the <c>DiscardImageEditingData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDiscardImageEditingData(this Settings settings, DXO10W.OnOffValues? value)
  {
    settings.SetFirstOnOffValuesElementVal<DiscardImageEditingData>(value);
  }

  /// <summary>
  /// Set the <c>DefaultImageDpi</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDefaultImageDpi(this Settings settings, int? value)
  {
    settings.SetFirstElementIntVal<DefaultImageDpi>(value);
  }

  /// <summary>
  /// Set the <c>ConflictMode</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetConflictMode(this Settings settings, DXO10W.OnOffValues? value)
  {
    settings.SetFirstOnOffValuesElementVal<ConflictMode>(value);
  }

  /// <summary>
  /// Set the <c>ChartTrackingRefBased</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetChartTrackingRefBased(this Settings settings, DX.OnOffValue? value)
  {
    settings.SetFirstOnOffValueElementVal<ChartTrackingRefBased>(value);
  }
  #endregion set settings

  /// <summary>
  /// Gets the <c>WordprocessingDocument</c> from the <c>Settings</c> object.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>wordprocessing document</result>
  public static DXPack.WordprocessingDocument? GetDocument(this Settings settings)
  {

    return settings.DocumentSettingsPart?.OpenXmlPackage as DXPack.WordprocessingDocument;
  }

  private static readonly Dictionary<string, (Type type, SettingCategory category)> PropDefs = new()
  {
    {  "ActiveWritingStyle", (typeof(ActiveWritingStyle), SettingCategory.Proofing) },
    {  "AlignBorderAndEdges", (typeof(bool?), SettingCategory.Layout) },
    {  "AlwaysMergeEmptyNamespace", (typeof(bool?), SettingCategory.CustomXml) },
    {  "AlwaysShowPlaceholderText", (typeof(bool?), SettingCategory.CustomXml) },
    {  "AttachedSchema", (typeof(string), SettingCategory.CustomXml) },
    {  "AttachedTemplate", (typeof(string), SettingCategory.Layout) },
    {  "AutoFormatOverride", (typeof(bool?), SettingCategory.Security) },
    {  "AutoHyphenation", (typeof(bool?), SettingCategory.Hyphenation) },
    {  "BookFoldPrinting", (typeof(bool?), SettingCategory.Printing) },
    {  "BookFoldPrintingSheets", (typeof(short), SettingCategory.Printing) },
    {  "BookFoldReversePrinting", (typeof(bool?), SettingCategory.Printing) },
    {  "BordersDoNotSurroundFooter", (typeof(bool?), SettingCategory.Layout) },
    {  "BordersDoNotSurroundHeader", (typeof(bool?), SettingCategory.Layout) },
    {  "Captions", (typeof(Captions), SettingCategory.Automation) },
    {  "CharacterSpacingControl", (typeof(CharacterSpacingValues), SettingCategory.Layout) },
    {  "ChartTrackingRefBased", (typeof(DX.OnOffValue), SettingCategory.Tracking) },
    {  "ClickAndTypeStyle", (typeof(string), SettingCategory.Automation) },
    {  "ColorSchemeMapping", (typeof(ColorSchemeMapping), SettingCategory.Theming) },
    {  "Compatibility", (typeof(Compatibility), SettingCategory.Compatibility) },
    {  "ConflictMode", (typeof(DXO10W.OnOffValues), SettingCategory.Load) },
    {  "ConsecutiveHyphenLimit", (typeof(ushort), SettingCategory.Hyphenation) },
    {  "DecimalSymbol", (typeof(string), SettingCategory.Automation) },
    {  "DefaultImageDpi", (typeof(int), SettingCategory.Layout) },
    {  "DefaultTableStyle", (typeof(string), SettingCategory.Layout) },
    {  "DefaultTabStop", (typeof(short), SettingCategory.Layout) },
    {  "DiscardImageEditingData", (typeof(DXO10W.OnOffValues), SettingCategory.Save) },
    {  "DisplayBackgroundShape", (typeof(bool?), SettingCategory.Layout) },
    {  "DisplayHorizontalDrawingGrid", (typeof(byte), SettingCategory.Layout) },
    {  "DisplayVerticalDrawingGrid", (typeof(byte), SettingCategory.Layout) },
    {  "DocumentId", (typeof(int), SettingCategory.Identification) },
    {  "DocumentProtection", (typeof(DocumentProtection), SettingCategory.Security) },
    {  "DocumentType", (typeof(string), SettingCategory.Identification) },
    {  "DocumentVariables", (typeof(DocumentVariables), SettingCategory.Automation) },
    {  "DoNotAutoCompressPictures", (typeof(bool?), SettingCategory.Save) },
    {  "DoNotDemarcateInvalidXml", (typeof(bool?), SettingCategory.CustomXml) },
    {  "DoNotDisplayPageBoundaries", (typeof(bool?), SettingCategory.Layout) },
    {  "DoNotHyphenateCaps", (typeof(bool?), SettingCategory.Hyphenation) },
    {  "DoNotIncludeSubdocsInStats", (typeof(bool?), SettingCategory.Automation) },
    {  "DoNotShadeFormData", (typeof(bool?), SettingCategory.Layout) },
    {  "DoNotTrackFormatting", (typeof(bool?), SettingCategory.Tracking) },
    {  "DoNotTrackMoves", (typeof(bool?), SettingCategory.Tracking) },
    {  "DoNotUseMarginsForDrawingGridOrigin", (typeof(bool?), SettingCategory.Tracking) },
    {  "DoNotValidateAgainstSchema", (typeof(bool?), SettingCategory.CustomXml) },
    {  "DrawingGridHorizontalOrigin", (typeof(int), SettingCategory.Layout) },
    {  "DrawingGridHorizontalSpacing", (typeof(int), SettingCategory.Layout) },
    {  "DrawingGridVerticalOrigin", (typeof(int), SettingCategory.Layout) },
    {  "DrawingGridVerticalSpacing", (typeof(int), SettingCategory.Layout) },
    {  "EmbedSystemFonts", (typeof(bool?), SettingCategory.Save) },
    {  "EmbedTrueTypeFonts", (typeof(bool?), SettingCategory.Save) },
    {  "EndnoteDocumentWideProperties", (typeof(EndnoteDocumentWideProperties), SettingCategory.Layout) },
    {  "EvenAndOddHeaders", (typeof(bool?), SettingCategory.Layout) },
    {  "FootnoteDocumentWideProperties", (typeof(FootnoteDocumentWideProperties), SettingCategory.Layout) },
    {  "ForceUpgrade", (typeof(bool?), SettingCategory.Load) },
    {  "FormsDesign", (typeof(bool?), SettingCategory.Load) },
    {  "GutterAtTop", (typeof(bool?), SettingCategory.Printing) },
    {  "HeaderShapeDefaults", (typeof(HeaderShapeDefaults), SettingCategory.Layout) },
    {  "HideGrammaticalErrors", (typeof(bool?), SettingCategory.Proofing) },
    {  "HideSpellingErrors", (typeof(bool?), SettingCategory.Proofing) },
    {  "HyphenationZone", (typeof(int), SettingCategory.Hyphenation) },
    {  "IgnoreMixedContent", (typeof(bool?), SettingCategory.CustomXml) },
    {  "LinkStyles", (typeof(bool?), SettingCategory.Load) },
    {  "ListSeparator", (typeof(string), SettingCategory.Automation) },
    {  "MailMerge", (typeof(MailMerge), SettingCategory.MailMerge) },
    {  "MathProperties", (typeof(MathProperties), SettingCategory.Layout) },
    {  "MirrorMargins", (typeof(bool?), SettingCategory.Layout) },
    {  "NoLineBreaksAfterKinsoku", (typeof(NoLineBreaksAfterKinsoku), SettingCategory.Layout) },
    {  "NoLineBreaksBeforeKinsoku", (typeof(NoLineBreaksBeforeKinsoku), SettingCategory.Layout) },
    {  "NoPunctuationKerning", (typeof(bool?), SettingCategory.Layout) },
    {  "PersistentDocumentId", (typeof(Guid), SettingCategory.Identification) },
    {  "PrintFormsData", (typeof(bool?), SettingCategory.Printing) },
    {  "PrintFractionalCharacterWidth", (typeof(bool?), SettingCategory.Printing) },
    {  "PrintPostScriptOverText", (typeof(bool?), SettingCategory.Printing) },
    {  "PrintTwoOnOne", (typeof(bool?), SettingCategory.Printing) },
    {  "ProofState", (typeof(ProofState), SettingCategory.Proofing) },
    {  "ReadModeInkLockDown", (typeof(ReadModeInkLockDown), SettingCategory.Layout) },
    {  "RemoveDateAndTime", (typeof(bool?), SettingCategory.Security) },
    {  "RemovePersonalInformation", (typeof(bool?), SettingCategory.Security) },
    {  "RevisionView", (typeof(RevisionView), SettingCategory.Revisions) },
    {  "Rsids", (typeof(Rsids), SettingCategory.Revisions) },
    {  "SaveFormsData", (typeof(bool?), SettingCategory.Save) },
    {  "SaveInvalidXml", (typeof(bool?), SettingCategory.Save) },
    {  "SavePreviewPicture", (typeof(bool?), SettingCategory.Save) },
    {  "SaveSubsetFonts", (typeof(bool?), SettingCategory.Save) },
    {  "SaveThroughXslt", (typeof(SaveThroughXslt), SettingCategory.Save) },
    {  "SaveXmlDataOnly", (typeof(bool?), SettingCategory.Save) },
    {  "SchemaLibrary", (typeof(SchemaLibrary), SettingCategory.CustomXml) },
    {  "ShapeDefaults", (typeof(ShapeDefaults), SettingCategory.Layout) },
    {  "ShowEnvelope", (typeof(bool?), SettingCategory.MailMerge) },
    {  "ShowXmlTags", (typeof(bool?), SettingCategory.CustomXml) },
    {  "StrictFirstAndLastChars", (typeof(bool?), SettingCategory.Layout) },
    {  "StyleLockStylesPart", (typeof(bool?), SettingCategory.Styles) },
    {  "StyleLockThemesPart", (typeof(bool?), SettingCategory.Styles) },
    {  "StylePaneFormatFilter", (typeof(StylePaneFormatFilter), SettingCategory.UI) },
    {  "StylePaneSortMethods", (typeof(string), SettingCategory.UI) },
    {  "SummaryLength", (typeof(int), SettingCategory.Automation) },
    {  "ThemeFontLanguages", (typeof(ThemeFontLanguages), SettingCategory.Theming) },
    {  "TrackRevisions", (typeof(bool?), SettingCategory.Tracking) },
    {  "UICompatibleWith97To2003", (typeof(UICompatibleWith97To2003), SettingCategory.UI) },
    {  "UpdateFieldsOnOpen", (typeof(bool?), SettingCategory.Load) },
    {  "UseXsltWhenSaving", (typeof(bool?), SettingCategory.Save) },
    {  "View", (typeof(ViewValues), SettingCategory.Layout) },
    {  "WriteProtection", (typeof(WriteProtection), SettingCategory.Security) },
    {  "Zoom", (typeof(Zoom), SettingCategory.Layout) },

  };
}