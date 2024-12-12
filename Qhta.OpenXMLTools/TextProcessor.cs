using System;
using System.Security.Principal;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Class to process text in a document.
/// </summary>
public class TextProcessor
{
  private record RunText
  {
    public readonly DXW.Run Run;
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

  private readonly TextOptions GetTextOptions = TextOptions.PlainText;
  private readonly List<RunText> FormattedText = new();

  /// <summary>
  /// Construct a text processor using paragraph as a context element.
  /// </summary>
  /// <param name="paragraph"></param>
  public TextProcessor(DXW.Paragraph paragraph)
  {
    foreach (var member in paragraph.Elements<DXW.Run>())
    {
      var text = member.GetText(GetTextOptions);
      FormattedText.Add(new RunText(member, text));
    }
  }

  /// <summary>
  /// Get text of the context and fill FormattedText dictionary.
  /// </summary>
  /// <returns></returns>
  public string GetText()
  {
    var sb = new StringBuilder();
    foreach (var item in FormattedText)
    {
      sb.Append(item.Text);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Search the text with the given format.
  /// </summary>
  /// <param name="searchText"></param>
  /// <param name="searchFormat"></param>
  /// <returns>character position of the text or -1 if not found</returns>
  public int Search(string searchText, TextFormat? searchFormat)
   => Search(0, searchText, searchFormat);

  /// <summary>
  /// Search the text with the given format starting at the given startPosition.
  /// </summary>
  /// <param name="startPosition"></param>
  /// <param name="searchText"></param>
  /// <param name="searchFormat"></param>
  /// <returns>character startPosition of the text or -1 if not found</returns>
  public int Search(int startPosition, string searchText, TextFormat? searchFormat)
  {
    var length = searchText.Length;
    var sumLength = 0;
    for (int i = 0; i <= FormattedText.Count; i++)
    {
      var itemText = FormattedText[i].Text;
      if (sumLength + itemText.Length > startPosition)
      {
        if (searchFormat == null || searchFormat.IsSame(FormattedText[i].Run.GetFormat()))
        {
          var textToCompare = itemText;
          int j = i + 1;
          while (textToCompare.Length > length && j < FormattedText.Count &&
                 (searchFormat == null || searchFormat.IsSame(FormattedText[j].Run.GetFormat())))
          {
            textToCompare += FormattedText[j].Text;
            j++;
          }
          itemText = FormattedText[i].Text;
          textToCompare += itemText;
          if (textToCompare.Length >= length)
          {
            var k = itemText.IndexOf(searchText);
            if (k >= 0)
            {
              return sumLength + k;
            }
          }
        }
      }
      sumLength += itemText.Length;
    }
    return -1;
  }

  /// <summary>
  /// Replace the first occurrence of the search text with the replacement text.
  /// </summary>
  /// <param name="searchText"></param>
  /// <param name="replacementText"></param>
  /// <returns></returns>
  public bool Replace(string searchText, string replacementText)
    => Replace(searchText, null, replacementText, null);

  /// <summary>
  /// Replace the first occurrence of the search text with the formatted replacement text.
  /// </summary>
  /// <param name="searchText"></param>
  /// <param name="replacementText"></param>
  /// <param name="replacementFormat"></param>
  /// <returns></returns>
  public bool Replace(string searchText, string replacementText, TextFormat? replacementFormat)
    => Replace(searchText, null, replacementText, replacementFormat);

  /// <summary>
  /// Replace the first occurrence of the formatted search text with the formatted replacement text.
  /// </summary>
  /// <param name="searchText"></param>
  /// <param name="replacementText"></param>
  /// <param name="searchFormat"></param>
  /// <param name="replacementFormat"></param>
  /// <returns></returns>
  public bool Replace(string searchText, TextFormat? searchFormat, string replacementText, TextFormat? replacementFormat)
  {
    int k;
    if (searchFormat != null)
      k = Search(0, searchText, searchFormat);
    else
    {
      var s = GetText();
      k = s.IndexOf(searchText);
    }
    if (k >= 0)
    {
      ReplaceAt(k, searchText.Length, replacementText, replacementFormat);
      return true;
    }
    return false;
  }

  /// <summary>
  /// Replace the text at given position and of given length with the replacement text.
  /// </summary>
  /// <param name="length"></param>
  /// <param name="replacementText"></param>
  /// <param name="position"></param>
  /// <param name="replacementFormat"></param>
  /// <returns></returns>
  public bool ReplaceAt(int position, int length, string replacementText, TextFormat? replacementFormat = null)
  {
    var sumLength = 0;
    var selectedItem = -1;
    for (int i = 0; i <= FormattedText.Count; i++)
    {
      var itemText = FormattedText[i].Text;
      if (sumLength + itemText.Length > position)
      {
        selectedItem = i;
        break;
      }
      sumLength += itemText.Length;
    }
    if (selectedItem >= 0)
    {
      while (selectedItem < FormattedText.Count && (length > 0 || replacementText.Length > 0))
      {
        var itemText = FormattedText[selectedItem].Text;
        var itemOldLength = itemText.Length;
        var itemPosition = position - sumLength;
        var itemRestLength = itemText.Length - itemPosition;
        var delLength = length;
        if (itemRestLength < length)
        {
          delLength = itemRestLength;
          length -= delLength;
        }
        else
        {
          length = 0;
        }
        if (delLength > 0)
        {
          itemText = itemText.Remove(itemPosition, delLength);
          FormattedText[selectedItem].Text = itemText;
          FormattedText[selectedItem].Run.SetText(itemText);
        }
        var nextItem = selectedItem + 1;
        if (replacementText.Length > 0)
        {
          if (replacementFormat != null && !replacementFormat.IsSame(FormattedText[selectedItem].Run.GetFormat()))
          {
            var selectedRun = FormattedText[selectedItem].Run;
            if (itemPosition == 0)
            {
              var newRun = new DXW.Run();
              newRun.AppendText(replacementText);
              newRun.SetFormat(replacementFormat);
              selectedRun.InsertBeforeSelf(newRun);
              FormattedText.Insert(selectedItem, new RunText(newRun, replacementText));
            }
            else if (itemPosition == itemText.Length)
            {
              var newRun = new DXW.Run();
              newRun.AppendText(replacementText);
              newRun.SetFormat(replacementFormat);
              selectedRun.InsertAfterSelf(newRun);
              FormattedText.Insert(selectedItem + 1, new RunText(newRun, replacementText));
              nextItem++;
            }
            else
            {
              var tailRun = selectedRun.SplitAt(itemPosition, TextOptions.PlainText);
              if (tailRun != null)
              {
                selectedRun.InsertBeforeSelf(tailRun);
                selectedItem++;
                FormattedText.Insert(selectedItem, new RunText(tailRun, tailRun.GetText(GetTextOptions)));
              }
              var newRun = new DXW.Run();
              newRun.AppendText(replacementText);
              newRun.SetFormat(replacementFormat);
              FormattedText[selectedItem].Run.InsertAfterSelf(newRun);
              FormattedText.Insert(selectedItem + 1, new RunText(newRun, replacementText));
              nextItem++;
            }
          }
          else
          {
            itemText = itemText.Insert(itemPosition, replacementText);
            FormattedText[selectedItem].Text = itemText;
            FormattedText[selectedItem].Run.SetText(itemText);
          }
          replacementText = String.Empty;
        }
        if (length == 0)
          break;
        position += delLength;
        sumLength += itemOldLength;
        selectedItem = nextItem;
      }
      return true;
    }
    return false;
  }

  /// <summary>
  /// Remove exceeding whitespaces from all paragraphs in the context.
  /// </summary>
  public void NormalizeWhitespaces(WhitespaceOptions options)
  {
    var text = GetText();
    var startSpacesLength = 0;
    var endSpacesLength = 0;
    for (int i = 0; i < text.Length; i++)
    {
      if (char.IsWhiteSpace(text[i]))
      {
        startSpacesLength++;
      }
      else break;
    }
    if (startSpacesLength < text.Length)
    {
      for (int i = text.Length - 1; i >= 0; i--)
      {
        if (char.IsWhiteSpace(text[i]))
        {
          endSpacesLength++;
        }
        else break;
      }
    }
  }

  //private void NormalizeSpacesAtStart(string text, int j, WhitespaceOptions options)
  //{
  //  if (options.Start == WsMode.Remove)
  //    RemoveText(0, j);
  //}


  //private void RemoveText(int pos, int length)
  //{
  //  var curLength = 0;
  //  for (int i = 0; i < FormattedText.Count; i++)
  //  {
  //    var kvp = FormattedText.ElementAt(i);
  //    if (kvp.Value.Length > pos)
  //    {
  //      var member = kvp.Key;
  //      var text = kvp.Value;
  //      if (curLength + text.Length <= pos)
  //      {
  //        curLength += text.Length;
  //      }
  //      else
  //      if (curLength + text.Length <= pos + length)
  //      {
  //        member.SetText(text.Substring(0, pos), TextOptions.FullText);
  //        length -= text.Length - pos;
  //        pos = 0;
  //      }
  //      else
  //      {
  //        member.SetText(text.Substring(0, pos) + text.Substring(pos + length), TextOptions.FullText);
  //        break;
  //      }
  //    }
  //  }
  //}

}
