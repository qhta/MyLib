namespace Qhta.OpenXmlTools;

/// <summary>
/// Format of the text to process
/// </summary>
public record TextFormat
{
  /// <summary>
  /// Bold attribute of the text.
  /// </summary>
  public bool? Bold { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Italic attribute of the text.
  /// </summary>
  public bool? Italic { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

  /// <summary>
  /// Compare the text format with another text format.
  /// If some format attributes are not set, they are not compared.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public bool IsSame(TextFormat other)
  {
    if (Bold.HasValue && other.Bold.HasValue && Bold != other.Bold)
      return false;
    if (Italic.HasValue && other.Italic.HasValue && Italic != other.Italic)
      return false;
    return true;
  }
}