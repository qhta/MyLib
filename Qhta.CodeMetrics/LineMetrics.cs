namespace Qhta.CodeMetrics
{
  /// <summary>
  /// Metrics for code lines.
  /// TotalLines = EmptyLines + CommentLines + MeaningfulLines + OtherLines.
  /// MeaningfulLines are those, which contain letters or digits.
  /// </summary>
  public class LineMetrics
  {
    /// <summary>
    /// Total count of lines
    /// </summary>
    public int TotalLines { get; set; }
    /// <summary>
    /// Count of empty lines - with no visible characters
    /// </summary>
    public int EmptyLines { get; set; }
    /// <summary>
    /// Count of commenting lines - containing only comments
    /// </summary>
    public int CommentLines { get; set; }
    /// <summary>
    /// Count of meaningful lines - containing letters or digits
    /// </summary>
    public int MeaningfulLines { get; set; }
    /// <summary>
    /// Count of other lines - not containing letters or digits
    /// </summary>
    public int OtherLines => TotalLines-EmptyLines-CommentLines-MeaningfulLines;
    /// <summary>
    /// Mean length of meaningful lines (starting and ending whitespaces not counted)
    /// </summary>
    public int MeanLineLength { get; set; }
  }
}
