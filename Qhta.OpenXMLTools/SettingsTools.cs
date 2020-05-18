using System.Linq;
using DocumentFormat.OpenXml.CustomXmlSchemaReferences;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools
{
  public static class SettingsTools
  {

    public static BordersDoNotSurroundHeader GetBordersDoNotSurroundHeader(this Settings settings) { return settings.Elements<BordersDoNotSurroundHeader>().FirstOrDefault(); }
    public static DrawingGridHorizontalSpacing GetDrawingGridHorizontalSpacing(this Settings settings) { return settings.Elements<DrawingGridHorizontalSpacing>().FirstOrDefault(); }
    public static PersistentDocumentId GetPersistentDocumentId(this Settings settings) { return settings.Elements<PersistentDocumentId>().FirstOrDefault(); }
    public static View GetView(this Settings settings) { return settings.Elements<View>().FirstOrDefault(); }
    public static Zoom GetZoom(this Settings settings) { return settings.Elements<Zoom>().FirstOrDefault(); }
    public static RemovePersonalInformation GetRemovePersonalInformation(this Settings settings) { return settings.Elements<RemovePersonalInformation>().FirstOrDefault(); }
    public static RemoveDateAndTime GetRemoveDateAndTime(this Settings settings) { return settings.Elements<RemoveDateAndTime>().FirstOrDefault(); }
    public static DoNotDisplayPageBoundaries GetDoNotDisplayPageBoundaries(this Settings settings) { return settings.Elements<DoNotDisplayPageBoundaries>().FirstOrDefault(); }
    public static DisplayBackgroundShape GetDisplayBackgroundShape(this Settings settings) { return settings.Elements<DisplayBackgroundShape>().FirstOrDefault(); }
    public static PrintPostScriptOverText GetPrintPostScriptOverText(this Settings settings) { return settings.Elements<PrintPostScriptOverText>().FirstOrDefault(); }
    public static PrintFractionalCharacterWidth GetPrintFractionalCharacterWidth(this Settings settings) { return settings.Elements<PrintFractionalCharacterWidth>().FirstOrDefault(); }
    public static PrintFormsData GetPrintFormsData(this Settings settings) { return settings.Elements<PrintFormsData>().FirstOrDefault(); }
    public static EmbedTrueTypeFonts GetEmbedTrueTypeFonts(this Settings settings) { return settings.Elements<EmbedTrueTypeFonts>().FirstOrDefault(); }
    public static EmbedSystemFonts GetEmbedSystemFonts(this Settings settings) { return settings.Elements<EmbedSystemFonts>().FirstOrDefault(); }
    public static SaveSubsetFonts GetSaveSubsetFonts(this Settings settings) { return settings.Elements<SaveSubsetFonts>().FirstOrDefault(); }
    public static SaveFormsData GetSaveFormsData(this Settings settings) { return settings.Elements<SaveFormsData>().FirstOrDefault(); }
    public static MirrorMargins GetMirrorMargins(this Settings settings) { return settings.Elements<MirrorMargins>().FirstOrDefault(); }
    public static AlignBorderAndEdges GetAlignBorderAndEdges(this Settings settings) { return settings.Elements<AlignBorderAndEdges>().FirstOrDefault(); }
    public static BordersDoNotSurroundFooter GetBordersDoNotSurroundFooter(this Settings settings) { return settings.Elements<BordersDoNotSurroundFooter>().FirstOrDefault(); }
    public static GutterAtTop GetGutterAtTop(this Settings settings) { return settings.Elements<GutterAtTop>().FirstOrDefault(); }
    public static HideSpellingErrors GetHideSpellingErrors(this Settings settings) { return settings.Elements<HideSpellingErrors>().FirstOrDefault(); }
    public static HideGrammaticalErrors GetHideGrammaticalErrors(this Settings settings) { return settings.Elements<HideGrammaticalErrors>().FirstOrDefault(); }
    public static ActiveWritingStyle GetActiveWritingStyle(this Settings settings) { return settings.Elements<ActiveWritingStyle>().FirstOrDefault(); }
    public static ProofState GetProofState(this Settings settings) { return settings.Elements<ProofState>().FirstOrDefault(); }
    public static FormsDesign GetFormsDesign(this Settings settings) { return settings.Elements<FormsDesign>().FirstOrDefault(); }
    public static AttachedTemplate GetAttachedTemplate(this Settings settings) { return settings.Elements<AttachedTemplate>().FirstOrDefault(); }
    public static LinkStyles GetLinkStyles(this Settings settings) { return settings.Elements<LinkStyles>().FirstOrDefault(); }
    public static StylePaneFormatFilter GetStylePaneFormatFilter(this Settings settings) { return settings.Elements<StylePaneFormatFilter>().FirstOrDefault(); }
    public static StylePaneSortMethods GetStylePaneSortMethods(this Settings settings) { return settings.Elements<StylePaneSortMethods>().FirstOrDefault(); }
    public static DocumentType GetDocumentType(this Settings settings) { return settings.Elements<DocumentType>().FirstOrDefault(); }
    public static MailMerge GetMailMerge(this Settings settings) { return settings.Elements<MailMerge>().FirstOrDefault(); }
    public static RevisionView GetRevisionView(this Settings settings) { return settings.Elements<RevisionView>().FirstOrDefault(); }
    public static TrackRevisions GetTrackRevisions(this Settings settings) { return settings.Elements<TrackRevisions>().FirstOrDefault(); }
    public static DoNotTrackMoves GetDoNotTrackMoves(this Settings settings) { return settings.Elements<DoNotTrackMoves>().FirstOrDefault(); }
    public static DoNotTrackFormatting GetDoNotTrackFormatting(this Settings settings) { return settings.Elements<DoNotTrackFormatting>().FirstOrDefault(); }
    public static DocumentProtection GetDocumentProtection(this Settings settings) { return settings.Elements<DocumentProtection>().FirstOrDefault(); }
    public static AutoFormatOverride GetAutoFormatOverride(this Settings settings) { return settings.Elements<AutoFormatOverride>().FirstOrDefault(); }
    public static StyleLockThemesPart GetStyleLockThemesPart(this Settings settings) { return settings.Elements<StyleLockThemesPart>().FirstOrDefault(); }
    public static StyleLockStylesPart GetStyleLockStylesPart(this Settings settings) { return settings.Elements<StyleLockStylesPart>().FirstOrDefault(); }
    public static DefaultTabStop GetDefaultTabStop(this Settings settings) { return settings.Elements<DefaultTabStop>().FirstOrDefault(); }
    public static AutoHyphenation GetAutoHyphenation(this Settings settings) { return settings.Elements<AutoHyphenation>().FirstOrDefault(); }
    public static ConsecutiveHyphenLimit GetConsecutiveHyphenLimit(this Settings settings) { return settings.Elements<ConsecutiveHyphenLimit>().FirstOrDefault(); }
    public static HyphenationZone GetHyphenationZone(this Settings settings) { return settings.Elements<HyphenationZone>().FirstOrDefault(); }
    public static DoNotHyphenateCaps GetDoNotHyphenateCaps(this Settings settings) { return settings.Elements<DoNotHyphenateCaps>().FirstOrDefault(); }
    public static ShowEnvelope GetShowEnvelope(this Settings settings) { return settings.Elements<ShowEnvelope>().FirstOrDefault(); }
    public static SummaryLength GetSummaryLength(this Settings settings) { return settings.Elements<SummaryLength>().FirstOrDefault(); }
    public static ClickAndTypeStyle GetClickAndTypeStyle(this Settings settings) { return settings.Elements<ClickAndTypeStyle>().FirstOrDefault(); }
    public static DefaultTableStyle GetDefaultTableStyle(this Settings settings) { return settings.Elements<DefaultTableStyle>().FirstOrDefault(); }
    public static EvenAndOddHeaders GetEvenAndOddHeaders(this Settings settings) { return settings.Elements<EvenAndOddHeaders>().FirstOrDefault(); }
    public static BookFoldReversePrinting GetBookFoldReversePrinting(this Settings settings) { return settings.Elements<BookFoldReversePrinting>().FirstOrDefault(); }
    public static BookFoldPrinting GetBookFoldPrinting(this Settings settings) { return settings.Elements<BookFoldPrinting>().FirstOrDefault(); }
    public static BookFoldPrintingSheets GetBookFoldPrintingSheets(this Settings settings) { return settings.Elements<BookFoldPrintingSheets>().FirstOrDefault(); }
    public static WriteProtection GetWriteProtection(this Settings settings) { return settings.Elements<WriteProtection>().FirstOrDefault(); }
    public static DrawingGridVerticalSpacing GetDrawingGridVerticalSpacing(this Settings settings) { return settings.Elements<DrawingGridVerticalSpacing>().FirstOrDefault(); }
    public static DisplayHorizontalDrawingGrid GetDisplayHorizontalDrawingGrid(this Settings settings) { return settings.Elements<DisplayHorizontalDrawingGrid>().FirstOrDefault(); }
    public static DisplayVerticalDrawingGrid GetDisplayVerticalDrawingGrid(this Settings settings) { return settings.Elements<DisplayVerticalDrawingGrid>().FirstOrDefault(); }
    public static DoNotUseMarginsForDrawingGridOrigin GetDoNotUseMarginsForDrawingGridOrigin(this Settings settings) { return settings.Elements<DoNotUseMarginsForDrawingGridOrigin>().FirstOrDefault(); }
    public static DrawingGridHorizontalOrigin GetDrawingGridHorizontalOrigin(this Settings settings) { return settings.Elements<DrawingGridHorizontalOrigin>().FirstOrDefault(); }
    public static DrawingGridVerticalOrigin GetDrawingGridVerticalOrigin(this Settings settings) { return settings.Elements<DrawingGridVerticalOrigin>().FirstOrDefault(); }
    public static DoNotShadeFormData GetDoNotShadeFormData(this Settings settings) { return settings.Elements<DoNotShadeFormData>().FirstOrDefault(); }
    public static NoPunctuationKerning GetNoPunctuationKerning(this Settings settings) { return settings.Elements<NoPunctuationKerning>().FirstOrDefault(); }
    public static CharacterSpacingControl GetCharacterSpacingControl(this Settings settings) { return settings.Elements<CharacterSpacingControl>().FirstOrDefault(); }
    public static PrintTwoOnOne GetPrintTwoOnOne(this Settings settings) { return settings.Elements<PrintTwoOnOne>().FirstOrDefault(); }
    public static StrictFirstAndLastChars GetStrictFirstAndLastChars(this Settings settings) { return settings.Elements<StrictFirstAndLastChars>().FirstOrDefault(); }
    public static NoLineBreaksAfterKinsoku GetNoLineBreaksAfterKinsoku(this Settings settings) { return settings.Elements<NoLineBreaksAfterKinsoku>().FirstOrDefault(); }
    public static NoLineBreaksBeforeKinsoku GetNoLineBreaksBeforeKinsoku(this Settings settings) { return settings.Elements<NoLineBreaksBeforeKinsoku>().FirstOrDefault(); }
    public static SavePreviewPicture GetSavePreviewPicture(this Settings settings) { return settings.Elements<SavePreviewPicture>().FirstOrDefault(); }
    public static DoNotValidateAgainstSchema GetDoNotValidateAgainstSchema(this Settings settings) { return settings.Elements<DoNotValidateAgainstSchema>().FirstOrDefault(); }
    public static SaveInvalidXml GetSaveInvalidXml(this Settings settings) { return settings.Elements<SaveInvalidXml>().FirstOrDefault(); }
    public static IgnoreMixedContent GetIgnoreMixedContent(this Settings settings) { return settings.Elements<IgnoreMixedContent>().FirstOrDefault(); }
    public static AlwaysShowPlaceholderText GetAlwaysShowPlaceholderText(this Settings settings) { return settings.Elements<AlwaysShowPlaceholderText>().FirstOrDefault(); }
    public static DoNotDemarcateInvalidXml GetDoNotDemarcateInvalidXml(this Settings settings) { return settings.Elements<DoNotDemarcateInvalidXml>().FirstOrDefault(); }
    public static SaveXmlDataOnly GetSaveXmlDataOnly(this Settings settings) { return settings.Elements<SaveXmlDataOnly>().FirstOrDefault(); }
    public static UseXsltWhenSaving GetUseXsltWhenSaving(this Settings settings) { return settings.Elements<UseXsltWhenSaving>().FirstOrDefault(); }
    public static SaveThroughXslt GetSaveThroughXslt(this Settings settings) { return settings.Elements<SaveThroughXslt>().FirstOrDefault(); }
    public static ShowXmlTags GetShowXmlTags(this Settings settings) { return settings.Elements<ShowXmlTags>().FirstOrDefault(); }
    public static AlwaysMergeEmptyNamespace GetAlwaysMergeEmptyNamespace(this Settings settings) { return settings.Elements<AlwaysMergeEmptyNamespace>().FirstOrDefault(); }
    public static UpdateFieldsOnOpen GetUpdateFieldsOnOpen(this Settings settings) { return settings.Elements<UpdateFieldsOnOpen>().FirstOrDefault(); }
    public static HeaderShapeDefaults GetHeaderShapeDefaults(this Settings settings) { return settings.Elements<HeaderShapeDefaults>().FirstOrDefault(); }
    public static FootnoteDocumentWideProperties GetFootnoteDocumentWideProperties(this Settings settings) { return settings.Elements<FootnoteDocumentWideProperties>().FirstOrDefault(); }
    public static EndnoteDocumentWideProperties GetEndnoteDocumentWideProperties(this Settings settings) { return settings.Elements<EndnoteDocumentWideProperties>().FirstOrDefault(); }
    public static Compatibility GetCompatibility(this Settings settings) { return settings.Elements<Compatibility>().FirstOrDefault(); }
    public static DocumentVariables GetDocumentVariables(this Settings settings) { return settings.Elements<DocumentVariables>().FirstOrDefault(); }
    public static Rsids GetRsids(this Settings settings) { return settings.Elements<Rsids>().FirstOrDefault(); }
    public static MathProperties GetMathProperties(this Settings settings) { return settings.Elements<MathProperties>().FirstOrDefault(); }
    public static UICompatibleWith97To2003 GetUICompatibleWith97To2003(this Settings settings) { return settings.Elements<UICompatibleWith97To2003>().FirstOrDefault(); }
    public static AttachedSchema GetAttachedSchema(this Settings settings) { return settings.Elements<AttachedSchema>().FirstOrDefault(); }
    public static ThemeFontLanguages GetThemeFontLanguages(this Settings settings) { return settings.Elements<ThemeFontLanguages>().FirstOrDefault(); }
    public static ColorSchemeMapping GetColorSchemeMapping(this Settings settings) { return settings.Elements<ColorSchemeMapping>().FirstOrDefault(); }
    public static DoNotIncludeSubdocsInStats GetDoNotIncludeSubdocsInStats(this Settings settings) { return settings.Elements<DoNotIncludeSubdocsInStats>().FirstOrDefault(); }
    public static DoNotAutoCompressPictures GetDoNotAutoCompressPictures(this Settings settings) { return settings.Elements<DoNotAutoCompressPictures>().FirstOrDefault(); }
    public static ForceUpgrade GetForceUpgrade(this Settings settings) { return settings.Elements<ForceUpgrade>().FirstOrDefault(); }
    public static Captions GetCaptions(this Settings settings) { return settings.Elements<Captions>().FirstOrDefault(); }
    public static ReadModeInkLockDown GetReadModeInkLockDown(this Settings settings) { return settings.Elements<ReadModeInkLockDown>().FirstOrDefault(); }
    public static SchemaLibrary GetSchemaLibrary(this Settings settings) { return settings.Elements<SchemaLibrary>().FirstOrDefault(); }
    public static ShapeDefaults GetShapeDefaults(this Settings settings) { return settings.Elements<ShapeDefaults>().FirstOrDefault(); }
    public static DecimalSymbol GetDecimalSymbol(this Settings settings) { return settings.Elements<DecimalSymbol>().FirstOrDefault(); }
    public static ListSeparator GetListSeparator(this Settings settings) { return settings.Elements<ListSeparator>().FirstOrDefault(); }
    public static DocumentId GetDocumentId(this Settings settings) { return settings.Elements<DocumentId>().FirstOrDefault(); }
    public static DiscardImageEditingData GetDiscardImageEditingData(this Settings settings) { return settings.Elements<DiscardImageEditingData>().FirstOrDefault(); }
    public static DefaultImageDpi GetDefaultImageDpi(this Settings settings) { return settings.Elements<DefaultImageDpi>().FirstOrDefault(); }
    public static ConflictMode GetConflictMode(this Settings settings) { return settings.Elements<ConflictMode>().FirstOrDefault(); }
    public static ChartTrackingRefBased GetChartTrackingRefBased(this Settings settings) { return settings.Elements<ChartTrackingRefBased>().FirstOrDefault(); }
  }
}
