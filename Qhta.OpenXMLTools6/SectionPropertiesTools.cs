using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml;

namespace Qhta.OpenXMLTools
{
  public static class SectionPropertiesTools
  {
    public static HeaderReference GetHeaderReference(this SectionProperties sectionProperties)
    {
      return sectionProperties.Elements<HeaderReference>().FirstOrDefault();
    }

    public static FooterReference GetFooterReference(this SectionProperties sectionProperties)
    {
      return sectionProperties.Elements<FooterReference>().FirstOrDefault();
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

    public static Body GetBody(this SectionProperties sectionProperties)
    {
      OpenXmlElement parent = sectionProperties.Parent;
      while (parent != null && !(parent is Body))
        parent = parent.Parent;
      return parent as Body;
    }

  }
}
