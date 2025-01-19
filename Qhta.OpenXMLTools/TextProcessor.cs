using System;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Class to process text in a document.
/// </summary>
public class TextProcessor
{
  private readonly FormattedText FormattedText;

  /// <summary>
  /// Construct a formatted text using paragraph as a context element.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="mode"></param>
  public TextProcessor(DXW.Paragraph paragraph, FormattedTextMode mode)
  {
    FormattedText = new FormattedText(paragraph, mode);
  }

  private void NormalizeSpacesAtStart(string text, int j, WhitespaceOptions options)
  {
    if (options.Start == WsMode.Remove)
      RemoveText(0, j);
  }


  private void RemoveText(int pos, int length)
  {
    var curLength = 0;
    for (int i = 0; i < FormattedText.Count; i++)
    {
      var kvp = FormattedText.ElementAt(i);
      if (kvp.Text.Length > pos)
      {
        var member = kvp.Run;
        var text = kvp.Text;
        if (curLength + text.Length <= pos)
        {
          curLength += text.Length;
        }
        else
        if (curLength + text.Length <= pos + length)
        {
          member.SetText(text.Substring(0, pos), TextOptions.FullText);
          length -= text.Length - pos;
          pos = 0;
        }
        else
        {
          member.SetText(text.Substring(0, pos) + text.Substring(pos + length), TextOptions.FullText);
          break;
        }
      }
    }
  }

}
