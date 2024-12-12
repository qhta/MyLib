using System;
using System.Security.Principal;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Class to process text in a document.
/// </summary>
public class TextProcessor
{

  private readonly TextOptions GetTextOptions = TextOptions.PlainText;
  private readonly FormattedText FormattedText = new();
  /// <summary>
  /// Search options - whole words only.
  /// </summary>
  public bool FindWholeWordsOnly { get; set; }

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
    if (searchFormat == null)
    {
      var compareText = GetText();
      if (FindWholeWordsOnly)
      {
        compareText += '\0' + compareText + '\0';
        var k = compareText.IndexOf(searchText, startPosition);
        while (k >= 0)
        {
          if (k > 0 && char.IsLetterOrDigit(compareText[k - 1]) || k + searchText.Length < compareText.Length && char.IsLetterOrDigit(compareText[k + searchText.Length]))
            k = compareText.IndexOf(searchText, k + 1);
          else
            return k;
        }
        return -1;
      }
      else
        return compareText.IndexOf(searchText, startPosition);
    }
    var searchTextLength = searchText.Length;
    var sumLength = 0;
    for (int i = 0; i < FormattedText.Count; i++)
    {
      var itemText = FormattedText[i].Text;
      if (sumLength + itemText.Length > startPosition)
      {
        if (searchFormat.IsSame(FormattedText[i].Run.GetFormat()))
        {
          var textToCompare = itemText;
          if (FindWholeWordsOnly)
          {
            if (i > 0 && searchFormat.IsSame(FormattedText[i - 1].Run.GetFormat()))
              textToCompare = FormattedText[i - 1].Text.LastOrDefault() + textToCompare;
            else
              textToCompare = '\0' + textToCompare;
          }
          int j = i + 1;
          while (textToCompare.Length > searchTextLength && j < FormattedText.Count && searchFormat.IsSame(FormattedText[j].Run.GetFormat()))
          {
            textToCompare += FormattedText[j].Text;
            j++;
          }
          if (FindWholeWordsOnly)
          {
            if (i < FormattedText.Count - 1 &&
                searchFormat.IsSame(FormattedText[i + 1].Run.GetFormat()))
              textToCompare = textToCompare + FormattedText[i + 1].Text.LastOrDefault();
            else
              textToCompare = textToCompare + '\0';
          }
          if (textToCompare.Length >= searchTextLength)
          {
            var k = textToCompare.IndexOf(searchText);
            while (k >= 0)
            {
              if (FindWholeWordsOnly && k > 0)
              {
                if (char.IsLetterOrDigit(textToCompare[k - 1]) || char.IsLetterOrDigit(textToCompare[k + searchTextLength]))
                  k = textToCompare.IndexOf(searchText, k + 1);
                else
                  return sumLength + k - 1;
              }
              else
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
    var k = Search(0, searchText, searchFormat);
    if (k >= 0)
    {
      return ReplaceAt(k, searchText.Length, replacementText, replacementFormat);
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
    for (int i = 0; i < FormattedText.Count; i++)
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
          FormattedText.SetText(selectedItem, itemText);
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
              var newRun = new DXW.Run();
              newRun.AppendText(replacementText);
              newRun.SetFormat(replacementFormat);
              FormattedText[selectedItem].Run.InsertAfterSelf(newRun);
              FormattedText.Insert(selectedItem + 1, new RunText(newRun, replacementText));
              nextItem++;
              selectedItem++;
              if (tailRun != null)
              {
                newRun.InsertAfterSelf(tailRun);
                FormattedText.Insert(selectedItem, new RunText(tailRun, tailRun.GetText(GetTextOptions)));
              }
            }
          }
          else
          {
            itemText = itemText.Insert(itemPosition, replacementText);
            FormattedText.SetText(selectedItem, itemText);
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
