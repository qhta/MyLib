using System;
using System.Text;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Mode to get text.
/// </summary>
public enum FormattedTextMode
{
  /// <summary>
  /// Get plain text.
  /// </summary>
  PlainText,
  /// <summary>
  /// Get text with tags starting with '\'.
  /// </summary>
  RichText,
  /// <summary>
  /// Get text with XML tags.
  /// </summary>
  XmlTagged,
}

/// <summary>
/// Options for getting text.
/// </summary>
[DebuggerStepThrough]
public record TextOptions
{

  private static readonly PlainTextWriter PlainTextWriter = new PlainTextWriter();
  private static readonly RichTextWriter RichTextWriter = new RichTextWriter();
  private static readonly XmlTaggedTextWriter XmlTaggedTextWriter = new XmlTaggedTextWriter();

  private static readonly PlainTextReader PlainTextReader = new PlainTextReader();
  private static readonly RichTextReader RichTextReader = new RichTextReader();
  private static readonly XmlTaggedTextReader XmlTaggedTextReader = new XmlTaggedTextReader();

  /// <summary>
  /// 
  /// </summary>
  public FormattedTextMode Mode { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = FormattedTextMode.PlainText;

  /// <summary>
  /// Get the text writer for the current mode.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public OpenXmlTextWriter GetTextWriter()
  {
    OpenXmlTextWriter result = Mode switch
    {
      FormattedTextMode.PlainText => PlainTextWriter,
      FormattedTextMode.RichText => RichTextWriter,
      FormattedTextMode.XmlTagged => XmlTaggedTextWriter,
      _ => throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null),
    };
    result.Options = this;
    return result;
  }

  /// <summary>
  /// Get the text reader for the current mode.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public OpenXmlTextReader GetTextReader()
  {
    OpenXmlTextReader result = Mode switch
    {
      FormattedTextMode.PlainText => PlainTextReader,
      FormattedTextMode.RichText => RichTextReader,
      FormattedTextMode.XmlTagged => XmlTaggedTextReader,
      _ => throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null),
    };
    result.Options = this;
    return result;
  }
  /// <summary>
  /// Tag to mark special characters.
  /// </summary>
  public record FormattedTextTags
  {
    /// <summary>
    /// Represents a tab element.
    /// </summary>
    public string TabTag = "<t>";
    /// <summary>
    /// Represents a line break element.
    /// </summary>
    public string BreakLineTag = "<br>";
    /// <summary>
    /// Represents a column break element.
    /// </summary>
    public string BreakColumnTag = "<cb>";
    /// <summary>
    /// Represents a page break element.
    /// </summary>
    public string BreakPageTag = "<pb>";
    /// <summary>
    /// Represents a carriage return element.
    /// </summary>
    public string CarriageReturnTag = "<cr>";
    /// <summary>
    /// Represents a soft hyphen element.
    /// </summary>
    public string SoftHyphenTag = "<sh>";
    /// <summary>
    /// Represents a non-break hyphen element.
    /// </summary>
    public string NoBreakHyphenTag = "<nbh>";
    /// <summary>
    /// Represents a positional tab element.
    /// </summary>
    public string PositionalTabTag = "<pt>";
    /// <summary>
    /// Represents an annotation reference mark element.
    /// </summary>
    public string AnnotationReferenceMarkTag = "<arm>";
    /// <summary>
    /// Represents a footnote reference mark element.
    /// </summary>
    public string FootnoteReferenceMarkTag = "<frm>";
    /// <summary>
    /// Represents an endnote reference mark element.
    /// </summary>
    public string EndnoteReferenceMarkTag = "<erm>";
    /// <summary>
    /// Represents a separator mark element.
    /// </summary>
    public string SeparatorMarkTag = "<sep>";
    /// <summary>
    ///  Represents a continuation separator mark element.
    /// </summary>
    public string ContinuationSeparatorMarkTag = "<csep>";
    /// <summary>
    /// Represents a last rendered page break element.
    /// </summary>
    public string LastRenderedPageBreakTag = "<lrpb>";
    /// <summary>
    /// Represents a page number element.
    /// </summary>
    public string PageNumberTag = "<pn>";
    /// <summary>
    /// Represents a DayLong element.
    /// </summary>
    public string DayLongTag = "<dayLong>";
    /// <summary>
    /// Represents a DayShort element.
    /// </summary>
    public string DayShortTag = "<dayShort>";
    /// <summary>
    /// Represents a MonthLong element.
    /// </summary>
    public string MonthLongTag = "<monthLong>";
    /// <summary>
    /// Represents a MonthShort element.
    /// </summary>
    public string MonthShortTag = "<monthShort>";
    /// <summary>
    /// Represents a YearLong element.
    /// </summary>
    public string YearLongTag = "<yearLong>";
    /// <summary>
    /// Represents a YearShort element.
    /// </summary>
    public string YearShortTag = "<yearShort>";
    /// <summary>
    /// Represents a field char begin.
    /// </summary>
    public string FieldCharBeginTag = "<fb>";
    /// <summary>
    /// Represents a field char separate.
    /// </summary>
    public string FieldCharSeparateTag = "<fs>";
    /// <summary>
    /// Represents a field char end.
    /// </summary>
    public string FieldCharEndTag = "<fe>";
    /// <summary>
    /// Tag to insert between paragraphs.
    /// </summary>
    public string ParagraphSeparator = "<p>";
    /// <summary>
    /// Tag to insert between runs.
    /// </summary>
    public string RunSeparator = "<r>";
  }

  /// <summary>
  /// Special characters tags for plain text.
  /// </summary>
  public static readonly FormattedTextTags PlainTextTags = new()
  {
    TabTag = "\t",
    BreakLineTag = "\n",
    BreakColumnTag = "\v",
    BreakPageTag = "\f",
    CarriageReturnTag = "\r",
    SoftHyphenTag = "\u00AD",
    NoBreakHyphenTag = "\u2011",
    PositionalTabTag = "\uE009",
    AnnotationReferenceMarkTag = "\uE00A",
    LastRenderedPageBreakTag = "\uE00B",
    ContinuationSeparatorMarkTag = "\uE00C",
    SeparatorMarkTag = "\uE00D",
    EndnoteReferenceMarkTag = "\uE00E",
    FootnoteReferenceMarkTag = "\uE00F",
    PageNumberTag = "\uE010",
    DayLongTag = "\uE011",
    DayShortTag = "\uE012",
    MonthLongTag = "\uE013",
    MonthShortTag = "\uE014",
    YearLongTag = "\uE015",
    YearShortTag = "\uE016",
    FieldCharBeginTag = "\uE021",
    FieldCharSeparateTag = "\uE022",
    FieldCharEndTag = "\uE023",
    ParagraphSeparator = "\u2029",
    RunSeparator = "\u2028",
  };

  /// <summary>
  /// Special characters tags for rich text.
  /// </summary>
  public static readonly FormattedTextTags RichTextTags = new()
  {
    TabTag = @"\t",
    BreakLineTag = @"\n",
    BreakColumnTag = @"\v",
    BreakPageTag = @"\f",
    CarriageReturnTag = @"\r",
    SoftHyphenTag = @"\softHyphen",
    NoBreakHyphenTag = @"\noBreakHyphen",
    PositionalTabTag = @"\ptab",
    AnnotationReferenceMarkTag = @"\an",
    LastRenderedPageBreakTag = @"\page",
    ContinuationSeparatorMarkTag = @"\csep",
    SeparatorMarkTag = @"\sep",
    EndnoteReferenceMarkTag = @"\en",
    FootnoteReferenceMarkTag = @"\fn",
    PageNumberTag = @"\pn",
    DayLongTag = @"\dayLong",
    DayShortTag = @"\dayShort",
    MonthLongTag = @"\monthLong",
    MonthShortTag = @"\monthShort",
    YearLongTag = @"\yearLong",
    YearShortTag = @"\yearShort",
    FieldCharBeginTag = @"\fb",
    FieldCharSeparateTag = @"\fs",
    FieldCharEndTag = @"\fc",
    ParagraphSeparator = @"\p",
    RunSeparator = @"\run",
  };
  /// <summary>
  /// Special characters tags for XML-tagged text.
  /// </summary>
  public static readonly FormattedTextTags XmlTags = new()
  {
    TabTag = "<t/>",
    BreakLineTag = "<n/>",
    BreakColumnTag = "<v/>",
    BreakPageTag = "<f/>",
    CarriageReturnTag = "<r/>",
    SoftHyphenTag = "<sh/>",
    NoBreakHyphenTag = "<nbh/>",
    PositionalTabTag = "<pt/>",
    AnnotationReferenceMarkTag = "<an/>",
    LastRenderedPageBreakTag = "<pb/>",
    ContinuationSeparatorMarkTag = "<cont/>",
    SeparatorMarkTag = "<sep/>",
    EndnoteReferenceMarkTag = "<en/>",
    FootnoteReferenceMarkTag = "<fn/>",
    PageNumberTag = "<pn/>",
    DayLongTag = "<DDDD/>",
    DayShortTag = "<DD/>",
    MonthLongTag = "<MMMM/>",
    MonthShortTag = "<MM/>",
    YearLongTag = "<YYYY/>",
    YearShortTag = "<YY/>",
    FieldCharBeginTag = "<fcb/>",
    FieldCharSeparateTag = "<fcs/>",
    FieldCharEndTag = "<fce/>",
  };

  /// <summary>
  /// Configured property set of special characters tags.
  /// </summary>
  public FormattedTextTags Tags { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = null!;

  /// <summary>
  /// Marker to precede a tag.
  /// </summary>
  public const char EscChar = '\u001b';

  /// <summary>
  /// Options to get only plain text. All non-text elements are returned as a single control character \u001B (ESC).
  /// SearchText is returned as is.
  /// Symbols like new line, tab, page break, column break, line break, carriage return are replaced by control characters,
  /// which are as follows:
  /// <list type="table">
  ///   <item>
  ///      <term>Tab character</term>
  ///      <description>\t = \u0009</description>
  ///   </item>
  ///   <item>
  ///      <term>Line break</term>
  ///      <description>\n = \u000A</description>
  ///   </item>
  ///   <item>
  ///      <term>Column break</term>
  ///      <description>\v = \u000B</description>
  ///   </item>
  ///   <item>
  ///      <term>Page break</term>
  ///      <description>\f = \u000C</description>
  ///   </item>
  ///   <item>
  ///      <term>Carriage return</term>
  ///      <description>\r = \u000D</description>
  ///   </item>
  /// </list>
  /// Positional tabulators are treated as tab characters.
  /// </summary>
  public static readonly TextOptions PlainText = new TextOptions
  {
    Tags = PlainTextTags,
    Mode = FormattedTextMode.PlainText,
    UseHtmlFormatting = false,
    UseHtmlParagraphs = false,
    UseHtmlTables = false,
    IncludeFieldFormula = false,
    IncludeParagraphNumbering = false,
    IncludeDrawings = false,
    IgnoreTableContents = true,
    IgnoreEmptyParagraphs = true,
    IncludeOtherMembers = false,
  };

  /// <summary>
  /// Options to get paragraph text.
  /// </summary>
  public static TextOptions ParaText { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = PlainText with
  {
    IncludeDrawings = true,
    IgnoreDrawingContents = true,
    IncludeOtherMembers = true,
  };

  /// <summary>
  /// Options to get rich text.
  /// Rich text tags are control words starting with '\' with optional parameters in curly braces
  /// </summary>
  public static readonly TextOptions RichText = ParaText with
  {
    Tags = RichTextTags,
    Mode = FormattedTextMode.RichText,
    UseHtmlFormatting = false,
    UseHtmlParagraphs = false,
    UseHtmlTables = false,
    IncludeFieldFormula = false,
    IncludeParagraphNumbering = false,
    IncludeDrawings = false,
    IgnoreTableContents = true,
    IgnoreEmptyParagraphs = true,
    IncludeOtherMembers = false,
  };

  /// <summary>
  /// Options to get xml-tagged text.
  /// Such characters as &lt;, &gt;, &amp; are replaced with html entities.
  /// Paragraphs and line breaks are represented with HTML tags.
  /// Tables are represented as HTML tables.
  /// </summary>
  public static readonly TextOptions XmlTaggedText = new TextOptions()
  {
    Tags = XmlTags,
    Mode = FormattedTextMode.XmlTagged,
    UseHtmlParagraphs = true,
    UseHtmlTables = true,
  };

  /// <summary>
  /// Options to get full text. All non-text elements are replaced with tags.
  /// Html entities are used.
  /// </summary>
  public static TextOptions FullText { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = XmlTaggedText with
  {
    IncludeDrawings = true,
    IncludeOtherMembers = true,
  };

  /// <summary>
  /// Options to get full text. All non-text elements are replaced with tags.
  /// Html entities are used.
  /// </summary>
  public static TextOptions FormattedText { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = XmlTaggedText with
  {
    IncludeDrawings = true,
    IncludeOtherMembers = true,
  };

  //#region indenting
  ///// <summary>
  ///// Use indenting.
  ///// </summary>
  //public bool UseIndenting { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

  ///// <summary>
  ///// IndentUnit unit string.
  ///// </summary>
  //public string IndentUnit { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "  ";

  ///// <summary>
  ///// The number of indent unit string to insert.
  ///// </summary>
  //public int IndentLevel { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = 0;

  ///// <summary>
  ///// Get the indent string.
  ///// </summary>
  ///// <returns></returns>
  //public string GetIndent()
  //{
  //  var sb = new StringBuilder();
  //  for (int i = 0; i < IndentLevel; i++)
  //    sb.Append(IndentUnit);
  //  return sb.ToString();
  //}
  //#endregion

  /// <summary>
  /// Options important for getting text from paragraphs.
  /// </summary>
  public bool OuterText { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  #region control characters
  /// <summary>
  /// Tag to mark a new line.
  /// </summary>
  public string NewLine { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "\r\n";

  /// <summary>
  /// Tag to mark a TabChar
  /// </summary>
  public string TabTag => Tags.TabTag;

  /// <summary>
  /// Tag to mark a line break.
  /// </summary>
  public string BreakLineTag => Tags.BreakLineTag;

  /// <summary>
  /// Tag to mark a column break.
  /// </summary>
  public string BreakColumnTag => Tags.BreakColumnTag; 

  /// <summary>
  /// Tag to mark a page break.
  /// </summary>
  public string BreakPageTag => Tags.BreakPageTag;

  /// <summary>
  /// Tag to mark a carriage return.
  /// </summary>
  public string CarriageReturnTag => Tags.CarriageReturnTag;

  /// <summary>
  /// Tag to mark a soft hyphen.
  /// </summary>
  public string SoftHyphenTag => Tags.SoftHyphenTag;

  /// <summary>
  /// Tag to mark a non-break hyphen.
  /// </summary>
  public string NoBreakHyphenTag => Tags.NoBreakHyphenTag;

  /// <summary>
  /// Tag to mark a PositionalTab.
  /// </summary>
  public string PositionalTabTag => Tags.PositionalTabTag;
  
  /// <summary>
  /// Tag to mark a last rendered page break.
  /// </summary>
  public string LastRenderedPageBreakTag => Tags.LastRenderedPageBreakTag;

  /// <summary>
  /// Tag to mark a page number.
  /// </summary>
  public string PageNumberTag => Tags.PageNumberTag;

  /// <summary>
  /// Tag to mark a DayLong element.
  /// </summary>
  public string DayLongTag => Tags.DayLongTag;

  /// <summary>
  /// Tag to mark a DayShort element.
  /// </summary>
  public string DayShortTag => Tags.DayShortTag;

  /// <summary>
  /// Tag to mark a MonthLong element.
  /// </summary>
  public string MonthLongTag => Tags.MonthLongTag;

  /// <summary>
  /// Tag to mark a MonthShort element.
  /// </summary>
  public string MonthShortTag => Tags.MonthShortTag;

  /// <summary>
  /// Tag to mark a YearLong element.
  /// </summary>
  public string YearLongTag => Tags.YearLongTag;

  /// <summary>
  /// Tag to mark a YearShort element.
  /// </summary>
  public string YearShortTag => Tags.YearShortTag;

    /// <summary>
  /// Tag to mark a FieldChar element with FieldCharType = begin.
  /// </summary>
  public string FieldCharBeginTag => Tags.FieldCharBeginTag;

  /// <summary>
  /// Tag to mark a FieldChar element with FieldCharType = separate.
  /// </summary>
  public string FieldCharSeparateTag => Tags.FieldCharSeparateTag;

  /// <summary>
  /// Tag to mark a FieldChar element with FieldCharType = end.
  /// </summary>
  public string FieldCharEndTag => Tags.FieldCharEndTag;
  #endregion

  #region Plain text options

  /// <summary>
  /// Tag to mark other object.
  /// </summary>
  public string OtherObjectSubstituteTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<other>";

  /// <summary>
  /// Ignore empty paragraphs in plain text.
  /// </summary>
  public bool IgnoreEmptyParagraphs { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Tag to insert between paragraphs.
  /// </summary>
  public string ParagraphSeparator => Tags.ParagraphSeparator;

  /// <summary>
  /// Tag to insert between table in plain text.
  /// </summary>
  public string TableSeparator { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "\r\n\r\n";

  /// <summary>
  /// Tag to insert between table cells in plain text.
  /// </summary>
  public string TableRowSeparator { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "\r\n";

  /// <summary>
  /// Tag to insert between table cells in plain text.
  /// </summary>
  public string TableCellSeparator { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "\t";
  #endregion

  #region HTML options

  /// <summary>
  /// Convert text to string using "C" like escape sequences.
  /// </summary>
  public bool UseEscapeSequences { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

  /// <summary>
  /// Convert text to string using control character names.
  /// </summary>
  public bool UseControlCharNames { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

  /// <summary>
  /// Convert text to string using separator character names.
  /// Separator characters are characters of category "Zs" (space separators), "Zl" (line separators), and "Zp" (paragraph separators).
  /// Space itself (U+0020) is not included, but governed by UseOtherCharNames option.
  /// </summary>
  public bool UseSeparatorNames { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

  /// <summary>
  /// Convert text to string using dash character names.
  /// </summary>
  public bool UseDashNames { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

  /// <summary>
  /// Convert text to string using format character names.
  /// </summary>
  public bool UseFormatCharNames { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

  /// <summary>
  /// Convert text to string using superscript/subscript character names.
  /// </summary>
  public bool UseSupSubCharNames { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

  /// <summary>
  /// Convert text to string using roman character names.
  /// </summary>
  public bool UseRomanCharNames { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

  /// <summary>
  /// Convert text to string using other character names.
  /// </summary>
  public bool UseOtherCharNames { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = true;

  /// <summary>
  /// Convert text to string using Ascii letters and digits character codes.
  /// </summary>
  public bool UseAlphanumericCodes { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

  /// <summary>
  /// Convert text to string using character functions (where possible).
  /// </summary>
  public bool UseCharFunctions { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

  /// <summary>
  /// Convert text to string using Html entities.
  /// </summary>
  public bool UseHtmlEntities { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Convert Run properties to HTML formatting tags.
  /// </summary>
  public bool UseHtmlFormatting { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Tag to start bold formatting.
  /// </summary>
  public string BoldStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<b>";

  /// <summary>
  /// Tag to end bold formatting.
  /// </summary>
  public string BoldEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</b>";

  /// <summary>
  /// Tag to start italic formatting.
  /// </summary>
  public string ItalicStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<i>";

  /// <summary>
  /// Tag to end italic formatting.
  /// </summary>
  public string ItalicEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</i>";

  /// <summary>
  /// Tag to start superscript formatting.
  /// </summary>
  public string SuperscriptStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<sup>";

  /// <summary>
  /// Tag to end superscript formatting.
  /// </summary>
  public string SuperscriptEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</sup>";

  /// <summary>
  /// Tag to start subscript formatting.
  /// </summary>
  public string SubscriptStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<sub>";

  /// <summary>
  /// Tag to end subscript formatting.
  /// </summary>
  public string SubscriptEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</sub>";

  /// <summary>
  /// Use HTML paragraph tags instead of paragraph separators.
  /// </summary>
  public bool UseHtmlParagraphs { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Tag to start a run.
  /// </summary>
  public string TextStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<t>";

  /// <summary>
  /// Tag to end a run.
  /// </summary>
  public string TextEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</t>";

  /// <summary>
  /// Tag to start a run.
  /// </summary>
  public string RunStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<r>";

  /// <summary>
  /// Tag to end a run.
  /// </summary>
  public string RunEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</r>";

  /// <summary>
  /// Tag to start a run.
  /// </summary>
  public string RunSeparator => Tags.RunSeparator;

  /// <summary>
  /// Tag to start a paragraph.
  /// </summary>
  public string ParagraphStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<p>";

  /// <summary>
  /// Tag to end a paragraph.
  /// </summary>
  public string ParagraphEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</p>";
  #endregion

  #region Table options
  /// <summary>
  /// Use HTML Table tags.
  /// </summary>
  public bool UseHtmlTables { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Include table as TableSubstituteTag.
  /// </summary>
  public bool IgnoreTableContents { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Tag to represent empty Table.
  /// </summary>
  public string TableSubstituteTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<table/>";

  /// <summary>
  /// Tag to start a Table.
  /// </summary>
  public string TableStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<table>";

  /// <summary>
  /// Tag to end a Table.
  /// </summary>
  public string TableEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</table>";

  /// <summary>
  /// Tag to start a table row.
  /// </summary>
  public string TableRowStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<tr>";

  /// <summary>
  /// Tag to end a table row.
  /// </summary>
  public string TableRowEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</tr>";

  /// <summary>
  /// Tag to start a table cell.
  /// </summary>
  public string TableCellStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<td>";

  /// <summary>
  /// Tag to end a table cell.
  /// </summary>
  public string TableCellEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</td>";
  #endregion

  /// <summary>
  /// Include formula command text.
  /// </summary>
  public bool IncludeFieldFormula { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Include formula result text.
  /// </summary>
  public bool IncludeFieldResult { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Tag to start a field.
  /// </summary>
  public string FieldStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<field>";

  /// <summary>
  /// Tag to start a field code start.
  /// </summary>
  public string FieldCodeStart { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<instr>";

  /// <summary>
  /// Tag to start a field code end.
  /// </summary>
  public string FieldCodeEnd { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</instr>";
 
  /// <summary>
  /// Tag to separate a field formula command from result.
  /// </summary>
  public string FieldResultTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<result/>";

  /// <summary>
  /// Tag to end a field formula.
  /// </summary>
  public string FieldEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<field/>";

  /// <summary>
  /// Tag to start a footnote reference.
  /// </summary>
  public string FootnoteRefStart { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<footnoteRef ";

  /// <summary>
  /// Tag to end a footnote reference.
  /// </summary>
  public string FootnoteRefEnd { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "/>";

  /// <summary>
  /// Tag to start an endnote reference.
  /// </summary>
  public string EndnoteRefStart { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<endnoteRef ";

  /// <summary>
  /// Tag to end an end note reference.
  /// </summary>
  public string EndnoteRefEnd { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "/>";

  /// <summary>
  /// Tag to start a comment reference.
  /// </summary>
  public string CommentRefStart { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<commentRef ";

  /// <summary>
  /// Tag to end a comment reference.
  /// </summary>
  public string CommentRefEnd { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "/>";

  /// <summary>
  /// Tag to mark an annotation reference mark.
  /// </summary>
  public string AnnotationReferenceMarkTag => Tags.AnnotationReferenceMarkTag;

  /// <summary>
  /// Tag to mark an endnote reference mark.
  /// </summary>
  public string FootnoteReferenceMarkTag => Tags.FootnoteReferenceMarkTag;

  /// <summary>
  /// Tag to mark an endnote reference mark.
  /// </summary>
  public string EndnoteReferenceMarkTag => Tags.EndnoteReferenceMarkTag;

  /// <summary>
  /// Tag to mark a footnotes/endnotes separator mark.
  /// </summary>
  public string SeparatorMarkTag => Tags.SeparatorMarkTag;
  /// <summary>
  /// Tag to mark a continuation separator mark.
  /// </summary>
  public string ContinuationSeparatorMarkTag => Tags.ContinuationSeparatorMarkTag;

  #region Numbering options
  /// <summary>
  /// Include paragraph numbering string at the beginning of paragraph text.
  /// </summary>
  public bool IncludeParagraphNumbering { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

  /// <summary>
  /// Should numbered list be indented on each level.
  /// </summary>
  public bool IndentNumberingLists { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

  /// <summary>
  /// Tag to start a paragraph numbering.
  /// </summary>
  public string NumberingStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "";

  /// <summary>
  /// Tag to end a paragraph numbering.
  /// </summary>
  public string NumberingEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "\t";
  #endregion

  #region deleted text options

  /// <summary>
  /// Deleted text start tag.
  /// </summary>
  public string DeletedTextStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<del>";

  /// <summary>
  /// Deleted text end tag.
  /// </summary>
  public string DeletedTextEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</del>";


  /// <summary>
  /// Deletion start tag.
  /// </summary>
  public string DeletedInstrStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<del>";

  /// <summary>
  /// Deletion end tag.
  /// </summary>
  public string DeletedInstrEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</del>";
  #endregion
  #region embedded object options
  /// <summary>
  /// Include embedded objects in the text. Objects are included as Xml.
  /// </summary>
  public bool IncludeEmbeddedObjects { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

  /// <summary>
  /// Ignore embedded objects content.
  /// </summary>
  public bool IgnoreEmbeddedObjectContent { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

  /// <summary>
  /// Tag to replace an object.
  /// </summary>
  public string EmbeddedObjectSubstituteTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<object/>";

  /// <summary>
  /// Tag to start an object.
  /// </summary>
  public string EmbeddedObjectStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<object>";

  /// <summary>
  /// Tag to end an object.
  /// </summary>
  public string EmbeddedObjectEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</object>";

  #endregion

  #region drawing options
  /// <summary>
  /// Include drawings in the text. Drawings are included as Xml.
  /// </summary>
  public bool IncludeDrawings { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

  /// <summary>
  /// Ignore drawings content.
  /// </summary>
  public bool IgnoreDrawingContents { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = false;

  /// <summary>
  /// Tag to replace a drawing.
  /// </summary>
  public string DrawingSubstituteTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<drawing/>";

  /// <summary>
  /// Tag to start a drawing.
  /// </summary>
  public string DrawingStartTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<drawing>";

  /// <summary>
  /// Tag to end a drawing.
  /// </summary>
  public string DrawingEndTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "</drawing>";

  /// <summary>
  /// Tag to show blip linked object.
  /// </summary>
  public string BlipTag { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = "<blip/>";
  #endregion

  /// <summary>
  /// Include other members of the element.
  /// </summary>
  public bool IncludeOtherMembers { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Ignore other members content.
  /// </summary>
  public bool IgnoreOtherMembersContent { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Determine if the member properties should be included.
  /// </summary>
  public bool IncludeMemberProperties { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

}
