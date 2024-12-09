namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing SearchText element.
/// </summary>
public static class RunTextTools
{
  /// <summary>
  /// Checks if the run text is empty.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DXW.Text? element)
  {
    if (element == null)
      return true;
    return element.Text.Trim()=="";
  }

  /// <summary>
  /// Split the run runText at the specified index, which is the number of characters from the beginning of the runText.
  /// Split is not possible if the index is at the beginning or end of the runText.
  /// Returns the second part of the runText.
  /// </summary>
  /// <param name="runText">Run runText element to process</param>
  /// <param name="index">Char position number</param>
  /// <param name="options">Options for runText extraction</param>
  /// <returns>Next, newly created runText (or null) if split is not available</returns>
  public static DXW.Text? SplitAt(this DXW.Text runText, int index, TextOptions options)
  {
    
    var textValue = runText.Text;
    if (index <= 0 || index >= textValue.Length)
      return null;

    var newTextValue = textValue.Substring(index);
    var newText = new DXW.Text(newTextValue);
    runText.Text = textValue.Substring(0, index);
    return newText;
  }

}