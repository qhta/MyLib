namespace Qhta.OpenXmlTools;

/// <summary>
/// Represents a run-text pair in the FormattingText class.
/// </summary>
public record RunText
{
  /// <summary>
  /// Run element.
  /// </summary>
  public readonly DXW.Run Run;
  /// <summary>
  /// Text of the run element.
  /// </summary>
  public string Text;

  /// <summary>
  /// Construct a RunText object.
  /// </summary>
  /// <param name="run"></param>
  /// <param name="text"></param>
  public RunText(DXW.Run run, string text)
  {
    Run = run;
    Text = text;
  }
}

/// <summary>
/// Represents a list of run-text pairs.
/// </summary>
public class FormattedText: List<RunText>
{
  /// <summary>
  /// Set the text of the specified item and pass it to the Run element.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="text"></param>
  public void SetText(int index, string text)
  {
    this[index].Text = text;
    this[index].Run.SetText(text);
  }
}