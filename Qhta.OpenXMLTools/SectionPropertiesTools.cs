using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing SectionProperties element.
/// </summary>
public static class SectionPropertiesTools
{
  #region Get elements

  /// <summary>
  /// Get all <c>HeaderReference</c> elements from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static IEnumerable<HeaderReference> GetHeaderReferences(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<HeaderReference>();
  }

  /// <summary>
  /// Get all <c>FooterReference</c> elements from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static IEnumerable<FooterReference> GetFooterReferences(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<FooterReference>();
  }

  /// <summary>
  /// Get the first <c>FootnoteProperties</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static FootnoteProperties? GetFootnoteProperties(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<FootnoteProperties>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>EndnoteProperties</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static EndnoteProperties? GetEndnoteProperties(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<EndnoteProperties>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>SectionType</c> value from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static SectionMarkValues? GetSectionType(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<SectionType>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the page size from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static (int? Width, int? Height) GetPageSize(this SectionProperties sectionProperties)
  {
    var pageSize = sectionProperties.Elements<PageSize>().FirstOrDefault();
    return ((int?)pageSize?.Width?.Value, (int?)pageSize?.Height?.Value);
  }

  /// <summary>
  /// Set the page size to the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="width"></param>
  /// <param name="height"></param>
  /// <returns></returns>
  public static void SetPageSize(this SectionProperties sectionProperties, int? width, int? height)
  {
    var pageSize = sectionProperties.Elements<PageSize>().FirstOrDefault();
    if (width == null && height == null)
    {
      if (pageSize != null)
        pageSize.Remove();
    }
    if (pageSize== null)
    {
      pageSize = new PageSize();
      sectionProperties.Append(pageSize);
    }
    pageSize.Width = (uint?)width;
    pageSize.Height = (uint?)height;
  }

  /// <summary>
  /// Get the first <c>PageMargin</c> from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static PageMargin? GetPageMargin(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PageMargin>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>PaperSource</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static PaperSource? GetPaperSource(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PaperSource>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>PageBorders</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static PageBorders? GetPageBorders(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PageBorders>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>LineNumberType</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static LineNumberType? GetLineNumberType(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<LineNumberType>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>PageNumberType</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static PageNumberType? GetPageNumberType(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PageNumberType>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>columns</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static Columns? GetColumns(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<Columns>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>FormProtection</c> from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static bool? GetFormProtection(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<FormProtection>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>VerticalTextAlignmentOnPage</c> value from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static VerticalJustificationValues? GetVerticalTextAlignmentOnPage(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<VerticalTextAlignmentOnPage>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>NoEndnote</c> value from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static bool? IsNoEndnote(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<NoEndnote>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>TitlePage</c> value from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static bool? IsTitlePage(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<TitlePage>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>TextDirection</c> value from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static TextDirectionValues? GetTextDirection(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<TextDirection>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>TitlePage</c> value from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static bool? GetTitlePage(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<TitlePage>().FirstOrDefault()?.Val?.Value;
  }


  /// <summary>
  /// Get the first <c>NoEndnote</c> value from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static bool? GetNoEndnote(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<NoEndnote>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>BiDi</c> value from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static bool? GetBiDi(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<BiDi>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>GutterOnRight</c> value from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static bool? GetGutterOnRight(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<GutterOnRight>().FirstOrDefault()?.Val?.Value;
  }

  /// <summary>
  /// Get the first <c>DocGrid</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static DocGrid? GetDocGrid(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<DocGrid>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>PrinterSettingsReference</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static PrinterSettingsReference? GetPrinterSettingsReference(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PrinterSettingsReference>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>FootnoteColumns</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static DXO13W.FootnoteColumns? GetFootnoteColumns(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<DXO13W.FootnoteColumns>().FirstOrDefault();
  }

  /// <summary>
  /// Get the first <c>SectionPropertiesChange</c> element from the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static SectionPropertiesChange? GetSectionPropertiesChange(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<SectionPropertiesChange>().FirstOrDefault();
  }
  #endregion

  #region Set elements

  /// <summary>
  /// Set all <c>HeaderReference</c> elements in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetHeaderReferences(this SectionProperties sectionProperties, IEnumerable<HeaderReference>? value)
  {
    sectionProperties.SetAllElements(value);
  }

  /// <summary>
  ///  Set all <c>FooterReference</c> elements in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetFooterReferences(this SectionProperties sectionProperties, IEnumerable<FooterReference>? value)
  {
    sectionProperties.SetAllElements(value);
  }

  /// <summary>
  /// Set <c>FootnoteProperties</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetFootnoteProperties(this SectionProperties sectionProperties, FootnoteProperties? value)
  {
    sectionProperties.SetAllElements(value);
    ;
  }

  /// <summary>
  /// Set <c>EndnoteProperties</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetEndnoteProperties(this SectionProperties sectionProperties, EndnoteProperties? value)
  {
    sectionProperties.SetAllElements(value);
  }

  /// <summary>
  /// Set <c>SectionType</c> value in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetSectionType(this SectionProperties sectionProperties, SectionMarkValues? value)
  {
    sectionProperties.SetFirstElementVal<SectionType, SectionMarkValues?>(value);
  }

  /// <summary>
  /// Set <c>PageSize</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPageSize(this SectionProperties sectionProperties, PageSize? value)
  {
    sectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PageMargin</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPageMargin(this SectionProperties sectionProperties, PageMargin? value)
  {
    sectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PaperSource</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPaperSource(this SectionProperties sectionProperties, PaperSource? value)
  {
    sectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PageBorders</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPageBorders(this SectionProperties sectionProperties, PageBorders? value)
  {
    sectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>LineNumberType</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetLineNumberType(this SectionProperties sectionProperties, LineNumberType? value)
  {
    sectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PageNumberType</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPageNumberType(this SectionProperties sectionProperties, PageNumberType? value)
  {
    sectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>Columns</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetColumns(this SectionProperties sectionProperties, Columns? value)
  {
    sectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>FormProtection</c> value in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetFormProtection(this SectionProperties sectionProperties, bool? value)
  {
    sectionProperties.SetOnOffTypeElement<FormProtection>(value);
  }

  /// <summary>
  /// Set <c>VerticalTextAlignmentOnPage</c> value in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetVerticalTextAlignmentOnPage(this SectionProperties sectionProperties, VerticalJustificationValues? value)
  {
    sectionProperties.SetFirstElementVal<VerticalTextAlignmentOnPage, VerticalJustificationValues?>(value);
  }

  /// <summary>
  /// Set <c>NoEndnote</c> value in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetNoEndnote(this SectionProperties sectionProperties, bool? value)
  {
    sectionProperties.SetOnOffTypeElement<NoEndnote>(value);
  }

  /// <summary>
  /// Set <c>TitlePage</c> value in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetTitlePage(this SectionProperties sectionProperties, bool? value)
  {
    sectionProperties.SetOnOffTypeElement<TitlePage>(value);
  }

  /// <summary>
  /// Set <c>TextDirection</c> value in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetTextDirection(this SectionProperties sectionProperties, TextDirectionValues? value)
  {
    sectionProperties.SetFirstElementVal<TextDirection, TextDirectionValues?>(value);
  }

  /// <summary>
  /// Set <c>BiDi</c> value in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetBiDi(this SectionProperties sectionProperties, bool? value)
  {
    sectionProperties.SetOnOffTypeElement<BiDi>(value);
  }

  /// <summary>
  /// Set <c>GutterOnRight</c> value in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetGutterOnRight(this SectionProperties sectionProperties, bool? value)
  {
    sectionProperties.SetOnOffTypeElement<GutterOnRight>(value);
  }

  /// <summary>
  /// Set <c>DocGrid</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>

  public static void SetDocGrid(this SectionProperties sectionProperties, DocGrid? value)
  {
    sectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>PrinterSettingsReference</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPrinterSettingsReference(this SectionProperties sectionProperties, PrinterSettingsReference? value)
  {
    sectionProperties.SetFirstElement(value);
  }

  /// <summary>
  /// Set <c>FootnoteColumns</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetFootnoteColumns(this SectionProperties sectionProperties, DXO13W.FootnoteColumns? value)
  {
    sectionProperties.SetFirstElement(value);

  }

  /// <summary>
  /// Set <c>SectionPropertiesChange</c> element in the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetSectionPropertiesChange(this SectionProperties sectionProperties, SectionPropertiesChange? value)
  {
    sectionProperties.SetFirstElement(value);

  }
  #endregion

  /// <summary>
  /// Recursively get the parent <c>Body</c> element of the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static Body? GetBody(this SectionProperties sectionProperties)
  {
    OpenXmlElement? parent = sectionProperties.Parent;
    while (parent != null && !(parent is Body))
      parent = parent.Parent;
    var element = parent as Body;
    return element;
  }


  /// <summary>
  /// Get the internal page width of the section properties.
  /// Internal page width is the width of the page without the margins.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetInternalPageWidth(this SectionProperties sectionProperties)
  {
    var pageSize = sectionProperties.Elements<DXW.PageSize>().FirstOrDefault();
    if (pageSize == null)
      return null;
    var width = (int?)pageSize.Width?.Value;
    var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
    if (pageMargin != null)
    {
      width -= (int?)pageMargin.Left?.Value;
      width -= (int?)pageMargin.Right?.Value;
      width -= (int?)pageMargin.Gutter?.Value;
    }
    return width;
  }

  /// <summary>
  /// Get the page width of the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetPageWidth(this SectionProperties sectionProperties)
  {
    return (int?)sectionProperties.Elements<DXW.PageSize>().FirstOrDefault()?.Width?.Value;
  }

  /// <summary>
  /// Set the page width to the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetPageWidth(this SectionProperties sectionProperties, int? value)
  {
    var pageSize = sectionProperties.Elements<PageSize>().FirstOrDefault();
    if (pageSize == null)
    {
      pageSize = new PageSize();
    }
    pageSize.Width = (uint?)value;
  }

  /// <summary>
  /// Get the page height of the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetPageHeight(this SectionProperties sectionProperties)
  {
    return (int?)sectionProperties.Elements<DXW.PageSize>().FirstOrDefault()?.Height?.Value;
  }

  /// <summary>
  /// Set the page height to the section properties.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetHeight(this SectionProperties sectionProperties, uint? value)
  {
    var pageSize = sectionProperties.Elements<PageSize>().FirstOrDefault();
    if (pageSize == null)
    {
      pageSize = new PageSize();
    }
    pageSize.Width = (uint?)value;
  }

  /// <summary>
  /// Get the page left margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetLeftPageMargin(this SectionProperties sectionProperties)
  {
    return (int?)sectionProperties.Elements<DXW.PageMargin>().FirstOrDefault()?.Left?.Value;
  }

  /// <summary>
  /// Set the  page left margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetLeftPageMargin(this SectionProperties sectionProperties, int? value)
  {
    var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
    if (pageMargin == null)
    {
      pageMargin = new PageMargin();
    }
    pageMargin.Left = (uint?)value;
  }

  /// <summary>
  /// Get the  page right margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetRightPageMargin(this SectionProperties sectionProperties)
  {
    return (int?)sectionProperties.Elements<DXW.PageMargin>().FirstOrDefault()?.Right?.Value;
  }

  /// <summary>
  /// Set the page right margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetRightPageMargin(this SectionProperties sectionProperties, int? value)
  {
    var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
    if (pageMargin == null)
    {
      pageMargin = new PageMargin();
    }
    pageMargin.Right = (uint?)value;
  }

  /// <summary>
  /// Get the page top margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetTopPageMargin(this SectionProperties sectionProperties)
  {
    return (int?)sectionProperties.Elements<DXW.PageMargin>().FirstOrDefault()?.Top?.Value;
  }

  /// <summary>
  /// Set the page top margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetTopPageMargin(this SectionProperties sectionProperties, int? value)
  {
    var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
    if (pageMargin == null)
    {
      pageMargin = new PageMargin();
    }
    pageMargin.Top = (int?)value;
  }

  /// <summary>
  /// Get the page bottom margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetBottomPageMargin(this SectionProperties sectionProperties)
  {
    return (int?)sectionProperties.Elements<DXW.PageMargin>().FirstOrDefault()?.Bottom?.Value;
  }

  /// <summary>
  /// Set the page bottom margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetBottomPageMargin(this SectionProperties sectionProperties, int? value)
  {
    var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
    if (pageMargin == null)
    {
      pageMargin = new PageMargin();
    }
    pageMargin.Bottom = (int?)value;
  }

  /// <summary>
  /// Get the page header margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetHeaderPageMargin(this SectionProperties sectionProperties)
  {
    return (int?)sectionProperties.Elements<DXW.PageMargin>().FirstOrDefault()?.Header?.Value;
  }

  /// <summary>
  /// Set the page header margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetHeaderPageMargin(this SectionProperties sectionProperties, int? value)
  {
    var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
    if (pageMargin == null)
    {
      pageMargin = new PageMargin();
    }
    pageMargin.Header = (uint?)value;
  }

  /// <summary>
  /// Get the page footer margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetFooterPageMargin(this SectionProperties sectionProperties)
  {
    return (int?)sectionProperties.Elements<DXW.PageMargin>().FirstOrDefault()?.Footer?.Value;
  }

  /// <summary>
  /// Set the page footer margin.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetFooterPageMargin(this SectionProperties sectionProperties, int? value)
  {
    var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
    if (pageMargin == null)
    {
      pageMargin = new PageMargin();
    }
    pageMargin.Footer = (uint?)value;
  }

  /// <summary>
  /// Get the page gutter.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <returns></returns>
  public static int? GetGutterPageMargin(this SectionProperties sectionProperties)
  {
    return (int?)sectionProperties.Elements<DXW.PageMargin>().FirstOrDefault()?.Gutter?.Value;
  }

  /// <summary>
  /// Set the page gutter.
  /// </summary>
  /// <param name="sectionProperties"></param>
  /// <param name="value"></param>
  public static void SetGutterPageMargin(this SectionProperties sectionProperties, int? value)
  {
    var pageMargin = sectionProperties.Elements<PageMargin>().FirstOrDefault();
    if (pageMargin == null)
    {
      pageMargin = new PageMargin();
    }
    pageMargin.Gutter = (uint?)value;
  }
}