using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Options for getting text.
/// </summary>
[DebuggerStepThrough]
public record TextOptions
{

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
    UseHtmlEntities = false,
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
  public static TextOptions ParaText { get; set; } = PlainText with
  {
    IncludeDrawings = true,
    IgnoreDrawingContents = true,
    IncludeOtherMembers = true,
  };

  /// <summary>
  /// Options to get tabbed text.
  /// Such characters as &lt;, &gt;, &amp; are replaced with html entities.
  /// Paragraphs and line breaks are represented with HTML tags.
  /// Tables are represented as HTML tables.
  /// </summary>
  public static readonly TextOptions XmlTaggedText = new TextOptions()
  {
    UseHtmlEntities = true,
    UseHtmlParagraphs = true,
    UseHtmlTables = true,
    TabChar = "<tab/>",
    ParagraphSeparator = "<p/>",
    BreakLineTag = "<br/>",
    BreakColumnTag = "<v/>",
    BreakPageTag = "<f/>",
    CarriageReturnTag = "<r/>",
  };

  /// <summary>
  /// Options to get full text. All non-text elements are replaced with tags.
  /// Html entities are used.
  /// </summary>
  public static TextOptions FullText { get; set; } = XmlTaggedText with
  {
    IncludeDrawings = true,
    IncludeOtherMembers = true,
  };

  /// <summary>
  /// Options to get full text. All non-text elements are replaced with tags.
  /// Html entities are used.
  /// </summary>
  public static TextOptions FormattedText { get; set; } = XmlTaggedText with
  {
    IncludeDrawings = true,
    IncludeOtherMembers = true,
  };

  //#region indenting
  ///// <summary>
  ///// Use indenting.
  ///// </summary>
  //public bool UseIndenting { get; set; } = true;

  ///// <summary>
  ///// IndentUnit unit string.
  ///// </summary>
  //public string IndentUnit { get; set; } = "  ";

  ///// <summary>
  ///// The number of indent unit string to insert.
  ///// </summary>
  //public int IndentLevel { get; set; } = 0;

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
  public bool OuterText { get; set; }

  #region control characters
  /// <summary>
  /// Tag to mark a new line.
  /// </summary>
  public string NewLine { get; set; } = "\r\n";

  /// <summary>
  /// Tag to mark a tab character.
  /// </summary>
  public string TabChar { get; set; } = "\u0009"; // \h

  /// <summary>
  /// Tag to mark a line break.
  /// </summary>
  public string BreakLineTag { get; set; } = "\u000A"; // \n

  /// <summary>
  /// Tag to mark a column break.
  /// </summary>
  public string BreakColumnTag { get; set; } = "\u000B"; // \v

  /// <summary>
  /// Tag to mark a page break.
  /// </summary>
  public string BreakPageTag { get; set; } = "\u000C"; // \f

  /// <summary>
  /// Tag to mark a carriage return.
  /// </summary>
  public string CarriageReturnTag { get; set; } = "\u000D"; // \r

  /// <summary>
  /// Tag to mark a soft hyphen.
  /// </summary>
  public string SoftHyphenTag { get; set; } = "\u00AD";

  /// <summary>
  /// Tag to mark a non-break hyphen.
  /// </summary>
  public string NoBreakHyphenTag { get; set; } = "\u2011";

  /// <summary>
  /// Positional tab replacement char.
  /// </summary>
  public string PositionalTabChar { get; set; } = "\uE009";

  #endregion

  #region Plain text options

  /// <summary>
  /// Tag to mark other object in plain text.
  /// </summary>
  public string OtherObjectSubstituteTag { get; set; } = "<other>";

  /// <summary>
  /// Tag to mark a page number.
  /// </summary>
  public string PageNumberTag { get; set; } = "<pgNum>";

  /// <summary>
  /// Tag to mark a last rendered page break.
  /// </summary>
  public string LastRenderedPageBreakTag { get; set; } = "<lrPgBreak>";

  /// <summary>
  /// Ignore empty paragraphs in plain text.
  /// </summary>
  public bool IgnoreEmptyParagraphs { get; set; }

  /// <summary>
  /// Tag to insert between paragraphs.
  /// </summary>
  public string ParagraphSeparator { get; set; } = "\r\n";

  /// <summary>
  /// Tag to insert between table in plain text.
  /// </summary>
  public string TableSeparator { get; set; } = "\r\n\r\n";

  /// <summary>
  /// Tag to insert between table cells in plain text.
  /// </summary>
  public string TableRowSeparator { get; set; } = "\r\n";

  /// <summary>
  /// Tag to insert between table cells in plain text.
  /// </summary>
  public string TableCellSeparator { get; set; } = "\t";
  #endregion

  #region HTML options
  /// <summary>
  /// Convert plain text to HTML entities.
  /// </summary>
  public bool UseHtmlEntities { get; set; }

  /// <summary>
  /// Convert Run properties to HTML formatting tags.
  /// </summary>
  public bool UseHtmlFormatting { get; set; }

  /// <summary>
  /// Tag to start bold formatting.
  /// </summary>
  public string BoldStartTag { get; set; } = "<b>";

  /// <summary>
  /// Tag to end bold formatting.
  /// </summary>
  public string BoldEndTag { get; set; } = "</b>";

  /// <summary>
  /// Tag to start italic formatting.
  /// </summary>
  public string ItalicStartTag { get; set; } = "<i>";

  /// <summary>
  /// Tag to end italic formatting.
  /// </summary>
  public string ItalicEndTag { get; set; } = "</i>";

  /// <summary>
  /// Tag to start superscript formatting.
  /// </summary>
  public string SuperscriptStartTag { get; set; } = "<sup>";

  /// <summary>
  /// Tag to end superscript formatting.
  /// </summary>
  public string SuperscriptEndTag { get; set; } = "</sup>";

  /// <summary>
  /// Tag to start subscript formatting.
  /// </summary>
  public string SubscriptStartTag { get; set; } = "<sub>";

  /// <summary>
  /// Tag to end subscript formatting.
  /// </summary>
  public string SubscriptEndTag { get; set; } = "</sub>";

  /// <summary>
  /// Use HTML paragraph tags instead of paragraph separators.
  /// </summary>
  public bool UseHtmlParagraphs { get; set; }

  /// <summary>
  /// Tag to start a run.
  /// </summary>
  public string TextStartTag { get; set; } = "<t>";

  /// <summary>
  /// Tag to end a run.
  /// </summary>
  public string TextEndTag { get; set; } = "</t>";

  /// <summary>
  /// Tag to start a run.
  /// </summary>
  public string RunStartTag { get; set; } = "<r>";

  /// <summary>
  /// Tag to end a run.
  /// </summary>
  public string RunEndTag { get; set; } = "</r>";

  /// <summary>
  /// Tag to start a paragraph.
  /// </summary>
  public string ParagraphStartTag { get; set; } = "<p>";

  /// <summary>
  /// Tag to end a paragraph.
  /// </summary>
  public string ParagraphEndTag { get; set; } = "</p>";
  #endregion

  #region Table options
  /// <summary>
  /// Use HTML Table tags.
  /// </summary>
  public bool UseHtmlTables { get; set; }

  /// <summary>
  /// Include table as TableSubstituteTag.
  /// </summary>
  public bool IgnoreTableContents { get; set; }

  /// <summary>
  /// Tag to represent empty Table.
  /// </summary>
  public string TableSubstituteTag { get; set; } = "<table/>";

  /// <summary>
  /// Tag to start a Table.
  /// </summary>
  public string TableStartTag { get; set; } = "<table>";

  /// <summary>
  /// Tag to end a Table.
  /// </summary>
  public string TableEndTag { get; set; } = "</table>";

  /// <summary>
  /// Tag to start a table row.
  /// </summary>
  public string TableRowStartTag { get; set; } = "<tr>";

  /// <summary>
  /// Tag to end a table row.
  /// </summary>
  public string TableRowEndTag { get; set; } = "</tr>";

  /// <summary>
  /// Tag to start a table cell.
  /// </summary>
  public string TableCellStartTag { get; set; } = "<td>";

  /// <summary>
  /// Tag to end a table cell.
  /// </summary>
  public string TableCellEndTag { get; set; } = "</td>";
  #endregion

  /// <summary>
  /// Include formula command text.
  /// </summary>
  public bool IncludeFieldFormula { get; set; }

  /// <summary>
  /// Tag to start a field formula.
  /// </summary>
  public string FieldStartTag { get; set; } = "<field/>";

  /// <summary>
  /// Tag to start a field code start.
  /// </summary>
  public string FieldCodeStart { get; set; } = "<instr>";

  /// <summary>
  /// Tag to start a field code end.
  /// </summary>
  public string FieldCodeEnd { get; set; } = "</instr>";
 
  /// <summary>
  /// Tag to separate a field formula command from result.
  /// </summary>
  public string FieldResultTag { get; set; } = "<result/>";

  /// <summary>
  /// Tag to end a field formula.
  /// </summary>
  public string FieldEndTag { get; set; } = "<field/>";

  /// <summary>
  /// Tag to start a footnote reference.
  /// </summary>
  public string FootnoteRefStart { get; set; } = "<footnoteRef ";

  /// <summary>
  /// Tag to end a footnote reference.
  /// </summary>
  public string FootnoteRefEnd { get; set; } = "/>";

  /// <summary>
  /// Tag to start an endnote reference.
  /// </summary>
  public string EndnoteRefStart { get; set; } = "<endnoteRef ";

  /// <summary>
  /// Tag to end an end note reference.
  /// </summary>
  public string EndnoteRefEnd { get; set; } = "/>";

  /// <summary>
  /// Tag to start a comment reference.
  /// </summary>
  public string CommentRefStart { get; set; } = "<commentRef ";

  /// <summary>
  /// Tag to end a comment reference.
  /// </summary>
  public string CommentRefEnd { get; set; } = "/>";

  /// <summary>
  /// Tag to mark an annotation reference mark.
  /// </summary>
  public string AnnotationRefMark { get; set; } = "<annotationRefMark/>";

  /// <summary>
  /// Tag to mark an endnote reference mark.
  /// </summary>
  public string FootnoteRefMark { get; set; } = "<footnoteRefMark/>";

  /// <summary>
  /// Tag to mark an endnote reference mark.
  /// </summary>
  public string EndnoteRefMark { get; set; } = "<endnoteRefMark/>";

  /// <summary>
  /// Tag to mark a footnotes/endnotes separator mark.
  /// </summary>
  public string FootnoteSepMark { get; set; } = "<footnoteSepMark/>";

  /// <summary>
  /// Tag to mark a continuation separator mark.
  /// </summary>
  public string ContinuationSepMark { get; set; } = "<contSepMark/>";

  #region Numbering options
  /// <summary>
  /// Include paragraph numbering string at the beginning of paragraph text.
  /// </summary>
  public bool IncludeParagraphNumbering { get; set; } = false;

  /// <summary>
  /// Should numbered list be indented on each level.
  /// </summary>
  public bool IndentNumberingLists { get; set; } = false;

  /// <summary>
  /// Tag to start a paragraph numbering.
  /// </summary>
  public string NumberingStartTag { get; set; } = "";

  /// <summary>
  /// Tag to end a paragraph numbering.
  /// </summary>
  public string NumberingEndTag { get; set; } = "\t";
  #endregion

  #region deleted text options

  /// <summary>
  /// Deleted text start tag.
  /// </summary>
  public string DeletedTextStartTag { get; set; } = "<del>";

  /// <summary>
  /// Deleted text end tag.
  /// </summary>
  public string DeletedTextEndTag { get; set; } = "</del>";


  /// <summary>
  /// Deletion start tag.
  /// </summary>
  public string DeletedInstrStartTag { get; set; } = "<del>";

  /// <summary>
  /// Deletion end tag.
  /// </summary>
  public string DeletedInstrEndTag { get; set; } = "</del>";
  #endregion
  #region embedded object options
  /// <summary>
  /// Include embedded objects in the text. Objects are included as Xml.
  /// </summary>
  public bool IncludeEmbeddedObjects { get; set; } = false;

  /// <summary>
  /// Ignore embedded objects content.
  /// </summary>
  public bool IgnoreEmbeddedObjectContent { get; set; } = false;

  /// <summary>
  /// Tag to replace an object.
  /// </summary>
  public string EmbeddedObjectSubstituteTag { get; set; } = "<object/>";

  /// <summary>
  /// Tag to start an object.
  /// </summary>
  public string EmbeddedObjectStartTag { get; set; } = "<object>";

  /// <summary>
  /// Tag to end an object.
  /// </summary>
  public string EmbeddedObjectEndTag { get; set; } = "</object>";

  #endregion

  #region drawing options
  /// <summary>
  /// Include drawings in the text. Drawings are included as Xml.
  /// </summary>
  public bool IncludeDrawings { get; set; } = false;

  /// <summary>
  /// Ignore drawings content.
  /// </summary>
  public bool IgnoreDrawingContents { get; set; } = false;

  /// <summary>
  /// Tag to replace a drawing.
  /// </summary>
  public string DrawingSubstituteTag { get; set; } = "<drawing/>";

  /// <summary>
  /// Tag to start a drawing.
  /// </summary>
  public string DrawingStartTag { get; set; } = "<drawing>";

  /// <summary>
  /// Tag to end a drawing.
  /// </summary>
  public string DrawingEndTag { get; set; } = "</drawing>";

  /// <summary>
  /// Tag to show blip linked object.
  /// </summary>
  public string BlipTag { get; set; } = "<blip/>";
  #endregion

  /// <summary>
  /// Include other members of the element.
  /// </summary>
  public bool IncludeOtherMembers { get; set; }

  /// <summary>
  /// Ignore other members content.
  /// </summary>
  public bool IgnoreOtherMembersContent { get; set; }

  /// <summary>
  /// Determine if the member properties should be included.
  /// </summary>
  public bool IncludeMemberProperties { get; set; }

}
