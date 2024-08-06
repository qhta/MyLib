namespace Qhta.OpenXmlTools;

/// <summary>
/// Options for getting text.
/// </summary>
public record GetTextOptions
{
  /// <summary>
  /// Default options.
  /// </summary>
  public static GetTextOptions Default { get; set; } = new GetTextOptions();

  /// <summary>
  /// Tag to mark a tab character.
  /// </summary>
  public string TabTag { get; set; } = "\t";

  /// <summary>
  /// Tag to mark a page break.
  /// </summary>
  public string BreakPageTag { get; set; } = "\f";

  /// <summary>
  /// Tag to mark a column break.
  /// </summary>

  public string BreakColumnTag { get; set; } = "\n";

  /// <summary>
  /// Tag to mark a line break.
  /// </summary>
  public string BreakLineTag { get; set; } = "\r";

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
  public string FootnoteRefStart { get; set; } = "[";

  /// <summary>
  /// Tag to end a footnote reference.
  /// </summary>
  public string FootnoteRefEnd { get; set; } = "]";

  /// <summary>
  /// Tag to start an endnote reference.
  /// </summary>
  public string EndnoteRefStart { get; set; } = "[";

  /// <summary>
  /// Tag to end an end note reference.
  /// </summary>
  public string EndnoteRefEnd { get; set; } = "]";

  /// <summary>
  /// Tag to start a comment reference.
  /// </summary>
  public string CommentRefStart { get; set; } = "[";

  /// <summary>
  /// Tag to end a comment reference.
  /// </summary>
  public string CommentRefEnd { get; set; } = "]";

  /// <summary>
  /// Tag to start a table.
  /// </summary>
  public string TableStartTag { get; set; } = "<table>";

  /// <summary>
  /// Tag to end a table.
  /// </summary>
  public string TableEndTag { get; set; } = "</table>\r\n";

  /// <summary>
  /// Tag to start a table row.
  /// </summary>
  public string TableRowStartTag { get; set; } = "<tr>";

  /// <summary>
  /// Tag to end a table row.
  /// </summary>
  public string TableRowEndTag { get; set; } = "</tr>\r\n";

  /// <summary>
  /// Tag to start a table cell.
  /// </summary>
  public string TableCellStartTag { get; set; } = "<td>";

  /// <summary>
  /// Tag to end a table cell.
  /// </summary>
  public string TableCellEndTag { get; set; } = "</td>\r\n";

  /// <summary>
  /// Include paragraph numbering string at the beginning of paragraph text.
  /// </summary>
  public bool IncludeParagraphNumbering { get; set; } = true;

  /// <summary>
  /// Tag to start a paragraph.
  /// </summary>
  public string ParagraphStartTag { get; set; } = "\r\n";

  /// <summary>
  /// Tag to end a paragraph.
  /// </summary>
  public string ParagraphEndTag { get; set; } = "\r\n";

}
