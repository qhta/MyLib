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
  #region get settings
  /// <summary>
  /// Get the <c>BordersDoNotSurroundHeader</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetBordersDoNotSurroundHeader(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<BordersDoNotSurroundHeader>();
  }

  /// <summary>
  /// Get the <c>DrawingGridHorizontalSpacing</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static int? GetDrawingGridHorizontalSpacing(this Settings settings)
  {
    return settings.GetFirstTwipsMeasureTypeElementVal<DrawingGridHorizontalSpacing>();
  }

  /// <summary>
  /// Get the <c>DocumentProtection</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetPersistentDocumentId(this Settings settings)
  {
    return settings.GetFirstElementVal<PersistentDocumentId>();
  }

  /// <summary>
  /// Get the <c>View</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static ViewValues? GetView(this Settings settings)
  {
    return settings.GetFirstElementVal<View, ViewValues>();
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
    return settings.GetFirstOnOffElementVal<RemovePersonalInformation>();
  }

  /// <summary>
  /// Get the <c>RemoveDateAndTime</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetRemoveDateAndTime(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<RemoveDateAndTime>();
  }

  /// <summary>
  /// Get the <c>DoNotDisplayPageBoundaries</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotDisplayPageBoundaries(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<DoNotDisplayPageBoundaries>();
  }

  /// <summary>
  /// Get the <c>DisplayBackgroundShape</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDisplayBackgroundShape(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<DisplayBackgroundShape>();
  }

  /// <summary>
  /// Get the <c>PrintPostScriptOverText</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetPrintPostScriptOverText(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<PrintPostScriptOverText>();
  }

  /// <summary>
  /// Get the <c>PrintFractionalCharacterWidth</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetPrintFractionalCharacterWidth(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<PrintFractionalCharacterWidth>();
  }

  /// <summary>
  /// Get the <c>PrintFormsData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetPrintFormsData(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<PrintFormsData>();
  }

  /// <summary>
  /// Get the <c>EmbedTrueTypeFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetEmbedTrueTypeFonts(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<EmbedTrueTypeFonts>();
  }

  /// <summary>
  /// Get the <c>EmbedSystemFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetEmbedSystemFonts(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<EmbedSystemFonts>();
  }

  /// <summary>
  /// Get the <c>SaveSubsetFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetSaveSubsetFonts(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<SaveSubsetFonts>();
  }

  /// <summary>
  /// Get the <c>SaveFormsData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetSaveFormsData(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<SaveFormsData>();
  }

  /// <summary>
  /// Get the <c>MirrorMargins</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetMirrorMargins(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<MirrorMargins>();
  }

  /// <summary>
  /// Get the <c>AlignBordersAndEdges</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetAlignBorderAndEdges(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<AlignBorderAndEdges>();
  }

  /// <summary>
  /// Get the <c>BordersDoNotSurroundFooter</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetBordersDoNotSurroundFooter(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<BordersDoNotSurroundFooter>();
  }

  /// <summary>
  /// Get the <c>DoNotSuppressParagraphBorders</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetGutterAtTop(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<GutterAtTop>();
  }

  /// <summary>
  /// Get the <c>HideSpellingErrors</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetHideSpellingErrors(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<HideSpellingErrors>();
  }

  /// <summary>
  /// Get the <c>HideGrammaticalErrors</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetHideGrammaticalErrors(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<HideGrammaticalErrors>();
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
    return settings.GetFirstOnOffElementVal<FormsDesign>();
  }

  /// <summary>
  /// Get the <c>AttachedTemplate</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetAttachedTemplate(this Settings settings)
  {
    return settings.GetFirstRelationshipElementId<AttachedTemplate>();
  }

  /// <summary>
  /// Get the <c>LinkStyles</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetLinkStyles(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<LinkStyles>();
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
    return settings.GetFirstElementVal<StylePaneSortMethods>();
  }

  /// <summary>
  /// Get the <c>DocumentType</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static string? GetDocumentType(this Settings settings)
  {
    return settings.GetFirstElementVal<DocumentType>();
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
    return settings.GetFirstOnOffElementVal<TrackRevisions>();
  }

  /// <summary>
  /// Get the <c>DoNotTrackMoves</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotTrackMoves(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<DoNotTrackMoves>();
  }

  /// <summary>
  /// Get the <c>DoNotTrackFormatting</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotTrackFormatting(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<DoNotTrackFormatting>();
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
    return settings.GetFirstOnOffElementVal<AutoFormatOverride>();
  }

  /// <summary>
  /// Get the <c>StyleLockTheme</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetStyleLockThemesPart(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<StyleLockThemesPart>();
  }

  /// <summary>
  /// Get the <c>StyleLockQFSet</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetStyleLockStylesPart(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<StyleLockStylesPart>();
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
    return settings.GetFirstOnOffElementVal<AutoHyphenation>();
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
  public static int? GetHyphenationZone(this Settings settings)
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
    return settings.GetFirstOnOffElementVal<DoNotHyphenateCaps>();
  }

  /// <summary>
  /// Get the <c>DoNotLeaveBackslashAlone</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetShowEnvelope(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<ShowEnvelope>();
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
    return settings.GetFirstOnOffElementVal<EvenAndOddHeaders>();
  }

  /// <summary>
  /// Get the <c>BookFoldPrintingSheets</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetBookFoldReversePrinting(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<BookFoldReversePrinting>();
  }

  /// <summary>
  /// Get the <c>BookFoldPrinting</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetBookFoldPrinting(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<BookFoldPrinting>();
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
  public static int? GetDrawingGridVerticalSpacing(this Settings settings)
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
    return settings.GetFirstOnOffElementVal<DoNotUseMarginsForDrawingGridOrigin>();
  }

  /// <summary>
  /// Get the <c>DoNotShadeFormData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static int? GetDrawingGridHorizontalOrigin(this Settings settings)
  {
    return settings.GetFirstTwipsMeasureTypeElementVal<DrawingGridHorizontalOrigin>();
  }

  /// <summary>
  /// Get the <c>DrawingGridVerticalOrigin</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static int? GetDrawingGridVerticalOrigin(this Settings settings)
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
    return settings.GetFirstOnOffElementVal<DoNotShadeFormData>();
  }

  /// <summary>
  /// Get the <c>NoPunctuationKerning</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetNoPunctuationKerning(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<NoPunctuationKerning>();
  }

  /// <summary>
  /// Get the <c>CharacterSpacingControl</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static CharacterSpacingValues? GetCharacterSpacingControl(this Settings settings)
  {
    return settings.GetFirstElementVal<CharacterSpacingControl, CharacterSpacingValues>();
  }

  /// <summary>
  /// Get the <c>PrintTwoOnOne</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetPrintTwoOnOne(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<PrintTwoOnOne>();
  }

  /// <summary>
  /// Get the <c>StrictFirstAndLastChars</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetStrictFirstAndLastChars(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<StrictFirstAndLastChars>();
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
    return settings.GetFirstOnOffElementVal<SavePreviewPicture>();
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotValidateAgainstSchema(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<DoNotValidateAgainstSchema>();
  }

  /// <summary>
  /// Get the <c>SaveInvalidXml</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetSaveInvalidXml(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<SaveInvalidXml>();
  }

  /// <summary>
  /// Get the <c>IgnoreMixedContent</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetIgnoreMixedContent(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<IgnoreMixedContent>();
  }

  /// <summary>
  ///   Get the <c>AlwaysShowPlaceholderText</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetAlwaysShowPlaceholderText(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<AlwaysShowPlaceholderText>();
  }

  /// <summary>
  /// Get the <c>DoNotDemarcateInvalidXml</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotDemarcateInvalidXml(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<DoNotDemarcateInvalidXml>();
  }

  /// <summary>
  /// Get the <c>SaveXmlDataOnly</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetSaveXmlDataOnly(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<SaveXmlDataOnly>();
  }

  /// <summary>
  /// Get the <c>UseXsltWhenSaving</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetUseXsltWhenSaving(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<UseXsltWhenSaving>();
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
    return settings.GetFirstOnOffElementVal<ShowXmlTags>();
  }

  /// <summary>
  /// Get the <c>AlwaysMergeEmptyNamespace</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetAlwaysMergeEmptyNamespace(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<AlwaysMergeEmptyNamespace>();
  }

  /// <summary>
  /// Get the <c>UpdateFieldsOnOpen</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetUpdateFieldsOnOpen(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<UpdateFieldsOnOpen>();
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
    return settings.GetFirstElementVal<AttachedSchema>();
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
    return settings.GetFirstOnOffElementVal<DoNotIncludeSubdocsInStats>();
  }

  /// <summary>
  /// Get the <c>DoNotAutoCompressPictures</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <result>result value</result>
  public static bool? GetDoNotAutoCompressPictures(this Settings settings)
  {
    return settings.GetFirstOnOffElementVal<DoNotAutoCompressPictures>();
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
  public static int? GetDocumentId(this Settings settings)
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
    settings.SetFirstOnOffElementVal<BordersDoNotSurroundHeader>(value);
  }

  /// <summary>
  /// Set the <c>DrawingGridHorizontalSpacing</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDrawingGridHorizontalSpacing(this Settings settings, int? value)
  {
    settings.SetFirstTwipsMeasureTypeElementVal<DrawingGridHorizontalSpacing>(value);
  }

  /// <summary>
  /// Set the <c>DocumentProtection</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetPersistentDocumentId(this Settings settings, string? value)
  {
    settings.SetFirstElementVal<PersistentDocumentId>(value);
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
    settings.SetFirstOnOffElementVal<RemovePersonalInformation>(value);
  }

  /// <summary>
  /// Set the <c>RemoveDateAndTime</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetRemoveDateAndTime(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<RemoveDateAndTime>(value);
  }

  /// <summary>
  /// Set the <c>DoNotDisplayPageBoundaries</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotDisplayPageBoundaries(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<DoNotDisplayPageBoundaries>(value);
  }

  /// <summary>
  /// Set the <c>DisplayBackgroundShape</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDisplayBackgroundShape(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<DisplayBackgroundShape>(value);
  }

  /// <summary>
  /// Set the <c>PrintPostScriptOverText</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetPrintPostScriptOverText(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<PrintPostScriptOverText>(value);
  }

  /// <summary>
  /// Set the <c>PrintFractionalCharacterWidth</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetPrintFractionalCharacterWidth(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<PrintFractionalCharacterWidth>(value);
  }

  /// <summary>
  /// Set the <c>PrintFormsData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetPrintFormsData(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<PrintFormsData>(value);
  }

  /// <summary>
  /// Set the <c>EmbedTrueTypeFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetEmbedTrueTypeFonts(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<EmbedTrueTypeFonts>(value);
  }

  /// <summary>
  /// Set the <c>EmbedSystemFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetEmbedSystemFonts(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<EmbedSystemFonts>(value);
  }

  /// <summary>
  /// Set the <c>SaveSubsetFonts</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSaveSubsetFonts(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<SaveSubsetFonts>(value);
  }

  /// <summary>
  /// Set the <c>SaveFormsData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSaveFormsData(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<SaveFormsData>(value);
  }

  /// <summary>
  /// Set the <c>MirrorMargins</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetMirrorMargins(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<MirrorMargins>(value);
  }

  /// <summary>
  /// Set the <c>AlignBordersAndEdges</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAlignBorderAndEdges(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<AlignBorderAndEdges>(value);
  }

  /// <summary>
  /// Set the <c>BordersDoNotSurroundFooter</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetBordersDoNotSurroundFooter(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<BordersDoNotSurroundFooter>(value);
  }

  /// <summary>
  /// Set the <c>DoNotSuppressParagraphBorders</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetGutterAtTop(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<GutterAtTop>(value);
  }

  /// <summary>
  /// Set the <c>HideSpellingErrors</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetHideSpellingErrors(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<HideSpellingErrors>(value);
  }

  /// <summary>
  /// Set the <c>HideGrammaticalErrors</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetHideGrammaticalErrors(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<HideGrammaticalErrors>(value);
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
    settings.SetFirstOnOffElementVal<FormsDesign>(value);
  }

  /// <summary>
  /// Set the <c>AttachedTemplate</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAttachedTemplate(this Settings settings, string? value)
  {
    settings.SetFirstRelationshipElementId<AttachedTemplate>(value);
  }

  /// <summary>
  /// Set the <c>LinkStyles</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetLinkStyles(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<LinkStyles>(value);
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
    settings.SetFirstElementVal<StylePaneSortMethods>(value);
  }

  /// <summary>
  /// Set the <c>DocumentType</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDocumentType(this Settings settings, string? value)
  {
    settings.SetFirstElementVal<DocumentType>(value);
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
    settings.SetFirstOnOffElementVal<TrackRevisions>(value);
  }

  /// <summary>
  /// Set the <c>DoNotTrackMoves</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotTrackMoves(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<DoNotTrackMoves>(value);
  }

  /// <summary>
  /// Set the <c>DoNotTrackFormatting</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotTrackFormatting(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<DoNotTrackFormatting>(value);
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
    settings.SetFirstOnOffElementVal<AutoFormatOverride>(value);
  }

  /// <summary>
  /// Set the <c>StyleLockTheme</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetStyleLockThemesPart(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<StyleLockThemesPart>(value);
  }

  /// <summary>
  /// Set the <c>StyleLockQFSet</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetStyleLockStylesPart(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<StyleLockStylesPart>(value);
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
    settings.SetFirstOnOffElementVal<AutoHyphenation>(value);
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
  public static void SetHyphenationZone(this Settings settings, int? value)
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
    settings.SetFirstOnOffElementVal<DoNotHyphenateCaps>(value);
  }

  /// <summary>
  /// Set the <c>DoNotLeaveBackslashAlone</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetShowEnvelope(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<ShowEnvelope>(value);
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
    settings.SetFirstOnOffElementVal<EvenAndOddHeaders>(value);
  }

  /// <summary>
  /// Set the <c>BookFoldPrintingSheets</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetBookFoldReversePrinting(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<BookFoldReversePrinting>(value);
  }

  /// <summary>
  /// Set the <c>BookFoldPrinting</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetBookFoldPrinting(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<BookFoldPrinting>(value);
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
  public static void SetDrawingGridVerticalSpacing(this Settings settings, int? value)
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
    settings.SetFirstOnOffElementVal<DoNotUseMarginsForDrawingGridOrigin>(value);
  }

  /// <summary>
  /// Set the <c>DoNotShadeFormData</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDrawingGridHorizontalOrigin(this Settings settings, int? value)
  {
    settings.SetFirstTwipsMeasureTypeElementVal<DrawingGridHorizontalOrigin>(value);
  }

  /// <summary>
  /// Set the <c>DrawingGridVerticalOrigin</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDrawingGridVerticalOrigin(this Settings settings, int? value)
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
    settings.SetFirstOnOffElementVal<DoNotShadeFormData>(value);
  }

  /// <summary>
  /// Set the <c>NoPunctuationKerning</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetNoPunctuationKerning(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<NoPunctuationKerning>(value);
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
    settings.SetFirstOnOffElementVal<PrintTwoOnOne>(value);
  }

  /// <summary>
  /// Set the <c>StrictFirstAndLastChars</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetStrictFirstAndLastChars(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<StrictFirstAndLastChars>(value);
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
    settings.SetFirstOnOffElementVal<SavePreviewPicture>(value);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotValidateAgainstSchema(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<DoNotValidateAgainstSchema>(value);
  }

  /// <summary>
  /// Set the <c>SaveInvalidXml</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSaveInvalidXml(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<SaveInvalidXml>(value);
  }

  /// <summary>
  /// Set the <c>IgnoreMixedContent</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetIgnoreMixedContent(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<IgnoreMixedContent>(value);
  }

  /// <summary>
  ///   Set the <c>AlwaysShowPlaceholderText</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAlwaysShowPlaceholderText(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<AlwaysShowPlaceholderText>(value);
  }

  /// <summary>
  /// Set the <c>DoNotDemarcateInvalidXml</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotDemarcateInvalidXml(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<DoNotDemarcateInvalidXml>(value);
  }

  /// <summary>
  /// Set the <c>SaveXmlDataOnly</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetSaveXmlDataOnly(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<SaveXmlDataOnly>(value);
  }

  /// <summary>
  /// Set the <c>UseXsltWhenSaving</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetUseXsltWhenSaving(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<UseXsltWhenSaving>(value);
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
    settings.SetFirstOnOffElementVal<ShowXmlTags>(value);
  }

  /// <summary>
  /// Set the <c>AlwaysMergeEmptyNamespace</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetAlwaysMergeEmptyNamespace(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<AlwaysMergeEmptyNamespace>(value);
  }

  /// <summary>
  /// Set the <c>UpdateFieldsOnOpen</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetUpdateFieldsOnOpen(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<UpdateFieldsOnOpen>(value);
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
    settings.SetFirstElementVal<AttachedSchema>(value);
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
    settings.SetFirstOnOffElementVal<DoNotIncludeSubdocsInStats>(value);
  }

  /// <summary>
  /// Set the <c>DoNotAutoCompressPictures</c> setting value.
  /// </summary>
  /// <param name="settings"></param>
  /// <param name="value">value to set</param>
  public static void SetDoNotAutoCompressPictures(this Settings settings, bool? value)
  {
    settings.SetFirstOnOffElementVal<DoNotAutoCompressPictures>(value);
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
}