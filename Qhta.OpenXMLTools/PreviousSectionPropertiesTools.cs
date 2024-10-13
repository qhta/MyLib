using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing PreviousSectionProperties element.
/// </summary>
public static class PreviousSectionPropertiesTools
{
  #region Get elements

  /// <summary>
  /// Get all <c>HeaderReference</c> elements from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static IEnumerable<HeaderReference> GetHeaderReferences(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<HeaderReference>();
  }

  /// <summary>
  /// Get all <c>FooterReference</c> elements from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static IEnumerable<FooterReference> GetFooterReferences(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<FooterReference>();
  }

  /// <summary>
  /// Get the first <c>FootnoteProperties</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static FootnoteProperties? GetFootnoteProperties(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<FootnoteProperties>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>EndnoteProperties</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static EndnoteProperties? GetEndnoteProperties(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<EndnoteProperties>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>SectionType</c> value from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static SectionMarkValues? GetSectionType(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<SectionType>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>PageSize</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static PageSize? GetPageSize(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<PageSize>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>PageMargin</c> from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static PageMargin? GetPageMargin(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<PageMargin>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>PaperSource</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static PaperSource? GetPaperSource(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<PaperSource>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>PageBorders</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static PageBorders? GetPageBorders(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<PageBorders>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>LineNumberType</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static LineNumberType? GetLineNumberType(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<LineNumberType>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>PageNumberType</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static PageNumberType? GetPageNumberType(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<PageNumberType>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>columns</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static Columns? GetColumns(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<Columns>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>FormProtection</c> from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static bool? GetFormProtection(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<FormProtection>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>VerticalTextAlignmentOnPage</c> value from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static VerticalJustificationValues? GetVerticalTextAlignmentOnPage(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<VerticalTextAlignmentOnPage>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>NoEndnote</c> value from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static bool? IsNoEndnote(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<NoEndnote>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>TitlePage</c> value from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static bool? IsTitlePage(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<TitlePage>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>TextDirection</c> value from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static TextDirectionValues? GetTextDirection(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<TextDirection>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>TitlePage</c> value from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static bool? GetTitlePage(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<TitlePage>().FirstOrDefault()?.Val?.Value;
  }


  /// <summary>
  /// Get the first <c>NoEndnote</c> value from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static bool? GetNoEndnote(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<NoEndnote>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>BiDi</c> value from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static bool? GetBiDi(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<BiDi>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>GutterOnRight</c> value from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static bool? GetGutterOnRight(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<GutterOnRight>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>DocGrid</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static DocGrid? GetDocGrid(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<DocGrid>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>PrinterSettingsReference</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static PrinterSettingsReference? GetPrinterSettingsReference(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<PrinterSettingsReference>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>FootnoteColumns</c> element from the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static FootnoteColumns? GetFootnoteColumns(this PreviousSectionProperties PreviousSectionProperties)
  {
    return PreviousSectionProperties.Elements<FootnoteColumns>().FirstOrDefault();
  }

  #endregion

  #region Set elements

  /// <summary>
  /// Set all <c>HeaderReference</c> elements in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetHeaderReferences(this PreviousSectionProperties PreviousSectionProperties, IEnumerable<HeaderReference>? value)
  {
    PreviousSectionProperties.SetAllElements(value);
  }

  /// <summary>
  ///  Set all <c>FooterReference</c> elements in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetFooterReferences(this PreviousSectionProperties PreviousSectionProperties, IEnumerable<FooterReference>? value)
  {
    PreviousSectionProperties.SetAllElements(value);
  }

  /// <summary>
  /// Set <c>FootnoteProperties</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetFootnoteProperties(this PreviousSectionProperties PreviousSectionProperties, FootnoteProperties? value)
  {
    PreviousSectionProperties.SetAllElements(value);
    ;
  }

  /// <summary>
  /// Set <c>EndnoteProperties</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetEndnoteProperties(this PreviousSectionProperties PreviousSectionProperties, EndnoteProperties? value)
  {
    PreviousSectionProperties.SetAllElements(value);
  }

  /// <summary>
  /// Set <c>SectionType</c> value in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetSectionType(this PreviousSectionProperties PreviousSectionProperties, SectionMarkValues? value)
  {
    PreviousSectionProperties.SetFirstElementVal<SectionType, SectionMarkValues?>(value);
  }

  /// <summary>
  /// Set <c>PageSize</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPageSize(this PreviousSectionProperties PreviousSectionProperties, PageSize? value)
  {
    PreviousSectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PageMargin</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPageMargin(this PreviousSectionProperties PreviousSectionProperties, PageMargin? value)
  {
    PreviousSectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PaperSource</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPaperSource(this PreviousSectionProperties PreviousSectionProperties, PaperSource? value)
  {
    PreviousSectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PageBorders</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPageBorders(this PreviousSectionProperties PreviousSectionProperties, PageBorders? value)
  {
    PreviousSectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>LineNumberType</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetLineNumberType(this PreviousSectionProperties PreviousSectionProperties, LineNumberType? value)
  {
    PreviousSectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PageNumberType</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPageNumberType(this PreviousSectionProperties PreviousSectionProperties, PageNumberType? value)
  {
    PreviousSectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>Columns</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetColumns(this PreviousSectionProperties PreviousSectionProperties, Columns? value)
  {
    PreviousSectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>FormProtection</c> value in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetFormProtection(this PreviousSectionProperties PreviousSectionProperties, bool? value)
  {
    PreviousSectionProperties.SetOnOffTypeElement<FormProtection>(value);
  }

  /// <summary>
  /// Set <c>VerticalTextAlignmentOnPage</c> value in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetVerticalTextAlignmentOnPage(this PreviousSectionProperties PreviousSectionProperties, VerticalJustificationValues? value)
  {
    PreviousSectionProperties.SetFirstElementVal<VerticalTextAlignmentOnPage, VerticalJustificationValues?>(value);
  }

  /// <summary>
  /// Set <c>NoEndnote</c> value in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetNoEndnote(this PreviousSectionProperties PreviousSectionProperties, bool? value)
  {
    PreviousSectionProperties.SetOnOffTypeElement<NoEndnote>(value);
  }

  /// <summary>
  /// Set <c>TitlePage</c> value in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetTitlePage(this PreviousSectionProperties PreviousSectionProperties, bool? value)
  {
    PreviousSectionProperties.SetOnOffTypeElement<TitlePage>(value);
  }

  /// <summary>
  /// Set <c>TextDirection</c> value in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetTextDirection(this PreviousSectionProperties PreviousSectionProperties, TextDirectionValues? value)
  {
    PreviousSectionProperties.SetFirstElementVal<TextDirection, TextDirectionValues?>(value);
  }

  /// <summary>
  /// Set <c>BiDi</c> value in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetBiDi(this PreviousSectionProperties PreviousSectionProperties, bool? value)
  {
    PreviousSectionProperties.SetOnOffTypeElement<BiDi>(value);
  }

  /// <summary>
  /// Set <c>GutterOnRight</c> value in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetGutterOnRight(this PreviousSectionProperties PreviousSectionProperties, bool? value)
  {
    PreviousSectionProperties.SetOnOffTypeElement<GutterOnRight>(value);
  }

  /// <summary>
  /// Set <c>DocGrid</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>

  public static void SetDocGrid(this PreviousSectionProperties PreviousSectionProperties, DocGrid? value)
  {
    PreviousSectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PrinterSettingsReference</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPrinterSettingsReference(this PreviousSectionProperties PreviousSectionProperties, PrinterSettingsReference? value)
  {
    PreviousSectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>FootnoteColumns</c> element in the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <param name="value"></param>
  public static void SetFootnoteColumns(this PreviousSectionProperties PreviousSectionProperties, FootnoteColumns? value)
  {
    PreviousSectionProperties.SetFirstElement(value);

  }
  #endregion

  /// <summary>
  /// Recursively get the parent <c>Body</c> element of the section properties.
  /// </summary>
  /// <param name="PreviousSectionProperties"></param>
  /// <returns></returns>
  public static Body? GetBody(this PreviousSectionProperties PreviousSectionProperties)
  {
    OpenXmlElement? parent = PreviousSectionProperties.Parent;
    while (parent != null && !(parent is Body))
      parent = parent.Parent;
    var element = parent as Body;
    return element;
  }
}