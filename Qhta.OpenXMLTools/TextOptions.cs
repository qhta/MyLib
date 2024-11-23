using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Options for getting text.
/// </summary>
public record TextOptions
{
  /// <summary>
  /// Options to get only plain text. All non-text elements are ignored.
  /// Text is returned as is. Symbols like new line, tab, page break, column break, line break, carriage return are replaced by replacement strings, which are as follows:
  /// <list type="table">
  ///   <item>
  ///      <term>New line</term>
  ///      <description>\r\n</description>
  ///   </item>
  ///   <item>
  ///      <term>Paragraph separator</term>
  ///      <description>\r\n</description>
  ///   </item>
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
    IgnoreEmptyParagraphs = true
  };

  /// <summary>
  /// Options to get tabbed text.
  /// Such characters as &lt;, &gt;, &amp; are replaced with html entities.
  /// Paragraphs and line breaks are represented with HTML tags.
  /// Tables are represented as HTML tables.
  /// </summary>
  public static readonly TextOptions TabbedText = new TextOptions()
  {
    UseHtmlEntities = true,
    UseHtmlParagraphs = true,
    ParagraphSeparator = "<p/>",
    BreakLineTag = "<br/>",
    TabTag = "<t/>",
    BreakColumnTag = "<v/>",
    BreakPageTag = "<f/>",
    CarriageReturnTag = "<r/>",
    UseHtmlTables = true,
  };

  /// <summary>
  /// Options to get full text. All non-text elements are replaced with tags.
  /// Html entities are used.
  /// </summary>
  public static TextOptions FullText { get; set; } = TabbedText with
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

  #region control characters
  /// <summary>
  /// Tag to mark a new line.
  /// </summary>
  public string NewLine { get; set; } = "\r\n";

  /// <summary>
  /// Tag to mark a tab character.
  /// </summary>
  public string TabTag { get; set; } = "\u0009"; // \h

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
  #endregion

  #region Plain text options

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
  public bool IncludeFieldFormula { get; set; } = false;

  /// <summary>
  /// Tag to start a field formula.
  /// </summary>
  public string FieldStartTag { get; set; } = "«";

  /// <summary>
  /// Tag to separate a field formula command from result.
  /// </summary>
  public string FieldResultTag { get; set; } = "|";

  /// <summary>
  /// Tag to end a field formula.
  /// </summary>
  public string FieldEndTag { get; set; } = "»";

  /// <summary>
  /// Tag to start a footnote reference.
  /// </summary>
  public string FootnoteRefStart { get; set; } = "[FootnoteRef ";

  /// <summary>
  /// Tag to end a footnote reference.
  /// </summary>
  public string FootnoteRefEnd { get; set; } = "]";

  /// <summary>
  /// Tag to start an endnote reference.
  /// </summary>
  public string EndnoteRefStart { get; set; } = "[EndnoteRef";

  /// <summary>
  /// Tag to end an end note reference.
  /// </summary>
  public string EndnoteRefEnd { get; set; } = "]";

  /// <summary>
  /// Tag to start a comment reference.
  /// </summary>
  public string CommentRefStart { get; set; } = "[CommentRef";

  /// <summary>
  /// Tag to end a comment reference.
  /// </summary>
  public string CommentRefEnd { get; set; } = "]";

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

  #region drawing options
  /// <summary>
  /// Include drawings in the text. Drawings are included as Xml.
  /// </summary>
  public bool IncludeDrawings { get; set; } = false;

  /// <summary>
  /// Include drawing as DrawingReplacementTags.
  /// </summary>
  public bool IgnoreDrawingContents { get; set; } = false;

  /// <summary>
  /// Tag to replace a drawing.
  /// </summary>
  public string DrawingSubstituteTag { get; set; } = "<drawing/>";

  /// <summary>
  /// Tag to start a Table.
  /// </summary>
  public string DrawingStartTag { get; set; } = "<drawing>";

  /// <summary>
  /// Tag to end a Table.
  /// </summary>
  public string DrawingEndTag { get; set; } = "</drawing>";
  #endregion

  /// <summary>
  /// Include other members of the element.
  /// </summary>
  public bool IncludeOtherMembers { get; set; }

}
