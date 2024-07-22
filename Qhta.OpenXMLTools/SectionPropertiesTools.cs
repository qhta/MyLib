using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml;

namespace Qhta.OpenXmlTools;

public static class SectionPropertiesTools
{
  #region Get elements
  public static IEnumerable<HeaderReference> GetHeaderReferences(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<HeaderReference>();
  }

  public static IEnumerable<FooterReference> GetFooterReferences(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<FooterReference>();
  }

  public static FootnoteProperties GetFootnoteProperties(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<FootnoteProperties>().FirstOrDefault();
  }

  public static EndnoteProperties GetEndnoteProperties(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<EndnoteProperties>().FirstOrDefault();
  }

  public static SectionType GetSectionType(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<SectionType>().FirstOrDefault();
  }

  public static PageSize GetPageSize(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PageSize>().FirstOrDefault();
  }

  public static PageMargin GetPageMargin(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PageMargin>().FirstOrDefault();
  }

  public static PaperSource GetPaperSource(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PaperSource>().FirstOrDefault();
  }

  public static PageBorders GetPageBorders(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PageBorders>().FirstOrDefault();
  }

  public static LineNumberType GetLineNumberType(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<LineNumberType>().FirstOrDefault();
  }

  public static PageNumberType GetPageNumberType(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PageNumberType>().FirstOrDefault();
  }

  public static Columns GetColumns(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<Columns>().FirstOrDefault();
  }

  public static FormProtection GetFormProtection(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<FormProtection>().FirstOrDefault();
  }

  public static VerticalTextAlignmentOnPage GetVerticalTextAlignmentOnPage(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<VerticalTextAlignmentOnPage>().FirstOrDefault();
  }

  public static NoEndnote GetNoEndnote(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<NoEndnote>().FirstOrDefault();
  }

  public static TitlePage GetTitlePage(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<TitlePage>().FirstOrDefault();
  }

  public static TextDirection GetTextDirection(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<TextDirection>().FirstOrDefault();
  }

  public static BiDi GetBiDi(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<BiDi>().FirstOrDefault();
  }

  public static GutterOnRight GetGutterOnRight(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<GutterOnRight>().FirstOrDefault();
  }


  public static DocGrid GetDocGrid(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<DocGrid>().FirstOrDefault();
  }

  public static PrinterSettingsReference GetPrinterSettingsReference(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<PrinterSettingsReference>().FirstOrDefault();
  }

  public static FootnoteColumns GetFootnoteColumns(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<FootnoteColumns>().FirstOrDefault();
  }

  public static SectionPropertiesChange GetSectionPropertiesChange(this SectionProperties sectionProperties)
  {
    return sectionProperties.Elements<SectionPropertiesChange>().FirstOrDefault();
  }
  #endregion

  #region Set elements
  public static void SetHeaderReferences(this SectionProperties sectionProperties, IEnumerable<HeaderReference>? value)
  {
    foreach (var item in sectionProperties.Elements<HeaderReference>().ToArray())
      item.Remove();
    if (value != null)
      foreach (var item in value)
        sectionProperties.AddChild(item);
  }

  public static void SetFooterReferences(this SectionProperties sectionProperties, IEnumerable<FooterReference>? value)
  {
    foreach (var item in sectionProperties.Elements<FooterReference>().ToArray())
      item.Remove();
    if (value != null)
      foreach (var item in value) 
        sectionProperties.AddChild(item);
  }

  public static void SetFootnoteProperties(this SectionProperties sectionProperties, FootnoteProperties? value)
  {
    var element = sectionProperties.Elements<FootnoteProperties>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetEndnoteProperties(this SectionProperties sectionProperties, EndnoteProperties? value)
  {
    var element = sectionProperties.Elements<EndnoteProperties>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetSectionType(this SectionProperties sectionProperties, SectionType? value)
  {
    var element = sectionProperties.Elements<SectionType>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetPageSize(this SectionProperties sectionProperties, PageSize? value)
  {
    var element = sectionProperties.Elements<PageSize>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetPageMargin(this SectionProperties sectionProperties, PageMargin? value)
  {
    var element = sectionProperties.Elements<PageMargin>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetPaperSource(this SectionProperties sectionProperties, PaperSource? value)
  {
    var element = sectionProperties.Elements<PaperSource>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetPageBorders(this SectionProperties sectionProperties, PageBorders? value)
  {
    var element = sectionProperties.Elements<PageBorders>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetLineNumberType(this SectionProperties sectionProperties, LineNumberType? value)
  {
    var element = sectionProperties.Elements<LineNumberType>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetPageNumberType(this SectionProperties sectionProperties, PageNumberType? value)
  {
    var element = sectionProperties.Elements<PageNumberType>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetColumns(this SectionProperties sectionProperties, Columns? value)
  {
    var element = sectionProperties.Elements<Columns>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetFormProtection(this SectionProperties sectionProperties, FormProtection? value)
  {
    var element = sectionProperties.Elements<FormProtection>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetVerticalTextAlignmentOnPage(this SectionProperties sectionProperties, VerticalTextAlignmentOnPage? value)
  {
    var element = sectionProperties.Elements<VerticalTextAlignmentOnPage>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetNoEndnote(this SectionProperties sectionProperties, NoEndnote? value)
  {
    var element = sectionProperties.Elements<NoEndnote>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetTitlePage(this SectionProperties sectionProperties, TitlePage? value)
  {
    var element = sectionProperties.Elements<TitlePage>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetTextDirection(this SectionProperties sectionProperties, TextDirection? value)
  {
    var element = sectionProperties.Elements<TextDirection>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetBiDi(this SectionProperties sectionProperties, BiDi? value)
  {
    var element = sectionProperties.Elements<BiDi>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetGutterOnRight(this SectionProperties sectionProperties, GutterOnRight? value)
  {
    var element = sectionProperties.Elements<GutterOnRight>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }


  public static void SetDocGrid(this SectionProperties sectionProperties, DocGrid? value)
  {
    var element = sectionProperties.Elements<DocGrid>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetPrinterSettingsReference(this SectionProperties sectionProperties, PrinterSettingsReference? value)
  {
    var element = sectionProperties.Elements<PrinterSettingsReference>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetFootnoteColumns(this SectionProperties sectionProperties, FootnoteColumns? value)
  {
    var element = sectionProperties.Elements<FootnoteColumns>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }

  public static void SetSectionPropertiesChange(this SectionProperties sectionProperties, SectionPropertiesChange? value)
  {
    var element = sectionProperties.Elements<SectionPropertiesChange>().FirstOrDefault();
    if (element != null)
      element.Remove();
    if (value != null)
      sectionProperties.AddChild(value);
  }
  #endregion

  public static Body? GetBody(this SectionProperties sectionProperties)
  {
    OpenXmlElement? parent = sectionProperties.Parent;
    while (parent != null && !(parent is Body))
      parent = parent.Parent;
    var element = parent as Body;
    return element;
  }
}