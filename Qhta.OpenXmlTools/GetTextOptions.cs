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
  /// Tag to mark a new line.
  /// </summary>
  public string NewLine { get; set; } = "\r\n";

  /// <summary>
  /// Tag to mark a tab character.
  /// </summary>
  public string TabTag { get; set; } = "\u0009";

  /// <summary>
  /// Tag to mark a page break.
  /// </summary>
  public string BreakPageTag { get; set; } = "\u000B";

  /// <summary>
  /// Tag to mark a column break.
  /// </summary>
  public string BreakColumnTag { get; set; } = "\u000C";

  /// <summary>
  /// Tag to mark a line break.
  /// </summary>
  public string BreakLineTag { get; set; } = "\u000A";

  /// <summary>
  /// Tag to mark a carriage return.
  /// </summary>
  public string CarriageReturnTag { get; set; } = "\u000D";

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

  /// <summary>
  /// Put a table in a separate line
  /// </summary>
  public bool TableInSeparateLine { get; set; } = true;

  /// <summary>
  /// Put a table row in a separate line
  /// </summary>
  public bool TableRowInSeparateLine { get; set; } = true;

  /// <summary>
  /// Put a table cell in a separate line
  /// </summary>
  public bool TableCellInSeparateLine { get; set; } = true;
  
  /// <summary>
  /// Should table content be indented.
  /// </summary>
  public bool IndentTableContent { get; set; } = true;

  /// <summary>
  /// Indent unit string.
  /// </summary>
  public string Indent { get; set; } = "  ";

  /// <summary>
  /// The number of indent unit string to insert.
  /// </summary>
  public int IndentLevel { get; set; } = 0;

  /// <summary>
  /// Include paragraph numbering string at the beginning of paragraph text.
  /// </summary>
  public bool IncludeParagraphNumbering { get; set; } = true;

  /// <summary>
  /// Should numbered list be indented on each level.
  /// </summary>
  public bool IndentNumberingLists { get; set; } = true;

  /// <summary>
  /// Tag to start a paragraph numbering.
  /// </summary>
  public string NumberingStartTag { get; set; } = "";

  /// <summary>
  /// Tag to end a paragraph numbering.
  /// </summary>
  public string NumberingEndTag { get; set; } = "\t";

  /// <summary>
  /// Tag to start a paragraph.
  /// </summary>
  public string ParagraphStartTag { get; set; } = "\r\n";

  /// <summary>
  /// Tag to end a paragraph.
  /// </summary>
  public string ParagraphEndTag { get; set; } = "\r\n";

}
