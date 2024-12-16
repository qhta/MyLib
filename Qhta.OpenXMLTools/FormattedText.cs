using System;
using System.Text;

using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;

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
public class FormattedText : List<RunText>
{
  private readonly TextOptions GetTextOptions = TextOptions.PlainText;

  /// <summary>
  /// Construct a formatted text using paragraph as a context element.
  /// </summary>
  /// <param name="paragraph"></param>
  public FormattedText(DXW.Paragraph paragraph)
  {
    foreach (var member in paragraph.Elements<DXW.Run>())
    {
      var text = member.GetText(GetTextOptions);
      this.Add(new RunText(member, text));
    }
  }

  /// <summary>
  /// Concatenate the text of all items in the list.
  /// </summary>
  /// <returns></returns>
  public string GetText()
  {
    var sb = new StringBuilder();
    foreach (var item in this)
    {
      sb.Append(item.Text);
    }
    return sb.ToString();
  }

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

  /// <summary>
  /// Set the format of the specified item passing it to the Run element.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="format"></param>
  public void SetFormat(int index, TextFormat format)
  {
    this[index].Run.SetFormat(format);
  }

  /// <summary>
  /// Insert the new run with text and format before the specified item.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="text"></param>
  /// <param name="format"></param>
  public void InsertBefore(int index, string text, TextFormat format)
  {
    var newRun = new DXW.Run();
    newRun.AppendText(text);
    newRun.SetFormat(format);
    this.Insert(index, new RunText(newRun, text));
    this[index].Run.InsertBeforeSelf(newRun);
  }


  /// <summary>
  /// Insert the new run with text and format after the specified item.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="text"></param>
  /// <param name="format"></param>
  public void InsertAfter(int index, string text, TextFormat format)
  {
    var newRun = new DXW.Run();
    newRun.AppendText(text);
    newRun.SetFormat(format);
    this.Insert(index + 1, new RunText(newRun, text));
    this[index].Run.InsertAfterSelf(newRun);
  }

  /// <summary>
  /// Insert the new run with text and format into the specified item at the given position splitting the item into two runs.
  /// </summary>
  /// <param name="index"></param>
  /// <param name="itemPosition"></param>
  /// <param name="text"></param>
  /// <param name="format"></param>
  public void InsertWithSplit(int index, int itemPosition, string text, TextFormat format)
  {
    var tailRun = this[index].Run.SplitAt(itemPosition, GetTextOptions);
    var newRun = new DXW.Run();
    newRun.AppendText(text);
    newRun.SetFormat(format);
    this.Insert(index + 1, new RunText(newRun, text));
    this[index].Run.InsertAfterSelf(newRun);
    if (tailRun != null)
    {
      this[index].Text = this[index].Text.Substring(0, itemPosition);
      this.Insert(index + 2, new RunText(tailRun, tailRun.GetText(GetTextOptions)));
      newRun.InsertAfterSelf(tailRun);
    }
  }

  /// <summary>
  /// Search the text with the given format.
  /// </summary>
  /// <param name="searchText"></param>
  /// <param name="searchFormat"></param>
  /// <param name="options"></param>
  /// <returns>character position of the text or -1 if not found</returns>
  public int Search(string searchText, TextFormat? searchFormat, FindAndReplaceOptions? options = null)
   => Search(0, searchText, searchFormat, options, out _);

  /// <summary>
  /// Search the text with the given format starting at the given startPosition.
  /// </summary>
  /// <param name="startPosition"></param>
  /// <param name="searchText"></param>
  /// <param name="searchFormat"></param>
  /// <param name="foundLength">length of found text (or 0 if not found)</param>
  /// <param name="options"></param>
  /// <returns>character startPosition of the text (or -1 if not found)</returns>
  public int Search(int startPosition, string? searchText, TextFormat? searchFormat, FindAndReplaceOptions? options, out int foundLength)
  {
    foundLength = searchText?.Length ?? 0;
    if (searchText != null && searchFormat == null)
      return SearchText(startPosition, searchText, options);
    if (searchText != null && searchFormat != null)
      return SearchTextWithFormat(startPosition, searchText, searchFormat, options);
    if (searchText == null && searchFormat != null)
      return SearchFormat(startPosition, searchFormat, options, out foundLength);
    throw new ArgumentException("Both search text and search format are null.");
  }

  /// <summary>
  /// Search the text with without format starting at the given startPosition.
  /// </summary>
  /// <param name="startPosition"></param>
  /// <param name="searchText"></param>
  /// <param name="options"></param>
  /// <returns>character startPosition of the text (or -1 if not found)</returns>
  private int SearchText(int startPosition, string searchText, FindAndReplaceOptions? options = null)
  {
    var findWholeWordsOnly = options?.FindWholeWordsOnly ?? false;
    var matchCaseInsensitive = options?.MatchCaseInsensitive ?? false;
    var stringComparison =
      matchCaseInsensitive ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
    var searchInText = GetText();
    if (findWholeWordsOnly)
    {
      searchInText = '\0' + searchInText + '\0';
      var k = searchInText.IndexOf(searchText, startPosition, stringComparison);
      while (k > 0 && k + searchText.Length < searchInText.Length)
      {
        if (!char.IsLetterOrDigit(searchInText[k - 1]) && !char.IsLetterOrDigit(searchInText[k + searchText.Length]))
          return k - 1;
        k = searchInText.IndexOf(searchText, k + 1, stringComparison);
      }
      return -1;
    }
    return searchInText.IndexOf(searchText, startPosition, stringComparison);
  }

  /// <summary>
  /// Search the text with the given format starting at the given startPosition.
  /// </summary>
  /// <param name="startPosition"></param>
  /// <param name="searchText"></param>
  /// <param name="searchFormat"></param>
  /// <param name="options"></param>
  /// <returns>character startPosition of the text (or -1 if not found)</returns>
  public int SearchTextWithFormat(int startPosition, string searchText, TextFormat searchFormat, FindAndReplaceOptions? options = null)
  {
    var findWholeWordsOnly = options?.FindWholeWordsOnly ?? false;
    var matchCaseInsensitive = options?.MatchCaseInsensitive ?? false;
    var stringComparison =
      matchCaseInsensitive ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
    var searchTextLength = searchText.Length;
    var sumLength = 0;
    for (int i = 0; i < this.Count; i++)
    {
      var itemText = this[i].Text;
      if (sumLength + itemText.Length > startPosition)
      {
        if (searchFormat.IsSame(this[i].Run.GetFormat()))
        {
          var searchInText = itemText;
          if (findWholeWordsOnly)
          {
            if (i > 0)
              searchInText = this[i - 1].Text.LastOrDefault() + searchInText;
            else
              searchInText = '\0' + searchInText;
          }
          int j = i + 1;
          while (searchInText.Length > searchTextLength && j < this.Count && searchFormat.IsSame(this[j].Run.GetFormat()))
          {
            searchInText += this[j].Text;
            j++;
          }
          if (findWholeWordsOnly)
          {
            if (i < this.Count - 1)
              searchInText = searchInText + this[i + 1].Text.LastOrDefault();
            else
              searchInText = searchInText + '\0';
          }
          if (searchInText.Length >= searchTextLength)
          {
            var k = searchInText.IndexOf(searchText, stringComparison);
            while (k >= 0)
            {
              if (findWholeWordsOnly && k > 0)
              {
                if (!char.IsLetterOrDigit(searchInText[k - 1]) && !char.IsLetterOrDigit(searchInText[k + searchTextLength]))
                  return sumLength + k - 1;
                k = searchInText.IndexOf(searchText, k + 1, stringComparison);
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
  /// Search the given format without text starting at the given startPosition.
  /// </summary>
  /// <param name="startPosition"></param>
  /// <param name="searchFormat"></param>
  /// <param name="foundLength">length of found text (or 0 if not found)</param>
  /// <param name="options"></param>
  /// <returns>character startPosition of the text (or -1 if not found)</returns>
  public int SearchFormat(int startPosition, TextFormat searchFormat, FindAndReplaceOptions? options, out int foundLength)
  {
    var findWholeWordsOnly = options?.FindWholeWordsOnly ?? false;
    var sumLength = 0;
    for (int i = 0; i < this.Count; i++)
    {
      var itemText = this[i].Text;
      if (sumLength + itemText.Length > startPosition)
      {
        if (searchFormat.IsSame(this[i].Run.GetFormat()))
        {
          var searchInText = itemText;
          if (findWholeWordsOnly)
          {
            if (i > 0)
              searchInText = this[i - 1].Text.LastOrDefault() + searchInText;
            else
              searchInText = '\0' + searchInText;
          }
          int j = i + 1;
          while (j < this.Count && searchFormat.IsSame(this[j].Run.GetFormat()))
          {
            searchInText += this[j].Text;
            j++;
          }
          if (findWholeWordsOnly)
          {
            if (i < this.Count - 1)
              searchInText = searchInText + this[i + 1].Text.LastOrDefault();
            else
              searchInText = searchInText + '\0';
          }

          if (findWholeWordsOnly)
          {
            if (!char.IsLetterOrDigit(searchInText[0]) && !char.IsLetterOrDigit(searchInText[searchInText.Length - 1]))
            {
              foundLength = searchInText.Length - 2;
              return sumLength;
            }
          }
          else
          {
            {
              foundLength = searchInText.Length;
              return sumLength;
            }
          }
        }
      }
      sumLength += itemText.Length;
    }
    foundLength = 0;
    return -1;
  }

  /// <summary>
  /// Replace the first occurrence of the search text with the replacement text.
  /// </summary>
  /// <param name="searchText"></param>
  /// <param name="replacementText"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public bool Replace(string searchText, string replacementText, FindAndReplaceOptions? options = null)
    => Replace(searchText, null, replacementText, null, options);

  /// <summary>
  /// Replace the first occurrence of the search text with the formatted replacement text.
  /// </summary>
  /// <param name="searchText"></param>
  /// <param name="replacementText"></param>
  /// <param name="replacementFormat"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public bool Replace(string searchText, string replacementText, TextFormat? replacementFormat, FindAndReplaceOptions? options = null)
    => Replace(searchText, null, replacementText, replacementFormat, options);

   /// <summary>
  /// Replace the first occurrence of the formatted search text with the formatted replacement text.
  /// </summary>
  /// <param name="searchText"></param>
  /// <param name="replacementText"></param>
  /// <param name="searchFormat"></param>
  /// <param name="replacementFormat"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public bool Replace(string? searchText, TextFormat? searchFormat, string? replacementText, TextFormat? replacementFormat, FindAndReplaceOptions? options = null)
   => Replace(0, searchText, searchFormat, replacementText, replacementFormat, options);

  /// <summary>
  /// Replace formatted search text with the formatted replacement text starting at the given position.
  /// </summary>
  /// <param name="startPosition"></param>
  /// <param name="searchText"></param>
  /// <param name="replacementText"></param>
  /// <param name="searchFormat"></param>
  /// <param name="replacementFormat"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  public bool Replace(int startPosition, string? searchText, TextFormat? searchFormat, string? replacementText, TextFormat? replacementFormat, FindAndReplaceOptions? options = null)
  {
    var foundPosition = Search(startPosition, searchText, searchFormat, options, out var foundLength);
    if (foundPosition<0)
      return false;
    if (searchText != null && replacementText == null)
      replacementText = searchText;
    if (options?.MatchCaseInsensitive == true && replacementText != null)
    {
      var foundText = GetText().Substring(foundPosition, foundLength);
      if (foundText != searchText)
      {
        if (foundText.IsUppercase())
          replacementText = replacementText.ToUpper();
        else if (foundText.IsLowercase())
          replacementText = replacementText.ToLower();
        else if (foundText.IsTitlecase())
          replacementText = replacementText.TitleCase();
      }
    }
    if (replacementText != null)
      return ReplaceTextAt(foundPosition, foundLength, replacementText, replacementFormat, options);
    if (replacementFormat != null)
      return ReplaceFormatAt(foundPosition, foundLength, replacementFormat, options);
    throw new ArgumentException("Both replacement text and replacement format are null.");
  }

  /// <summary>
  /// Replace the text at given position and of given length with the replacement text.
  /// </summary>
  /// <param name="length"></param>
  /// <param name="replacementText"></param>
  /// <param name="position"></param>
  /// <param name="replacementFormat"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private bool ReplaceTextAt(int position, int length, string replacementText, TextFormat? replacementFormat, FindAndReplaceOptions? options = null)
  {
    var sumLength = 0;
    var selectedItem = -1;
    for (int i = 0; i < this.Count; i++)
    {
      var itemText = this[i].Text;
      if (sumLength + itemText.Length > position)
      {
        selectedItem = i;
        break;
      }
      sumLength += itemText.Length;
    }
    if (selectedItem >= 0)
    {
      while (selectedItem < this.Count && (length > 0 || replacementText.Length > 0))
      {
        var itemText = this[selectedItem].Text;
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
          SetText(selectedItem, itemText);
        }
        var nextItem = selectedItem + 1;
        if (replacementText.Length > 0)
        {
          if (replacementFormat != null && !replacementFormat.IsSame(this[selectedItem].Run.GetFormat()))
          {
            if (itemPosition == 0)
            {
              InsertBefore(selectedItem, replacementText, replacementFormat);
            }
            else if (itemPosition == itemText.Length)
            {
              InsertAfter(selectedItem, replacementText, replacementFormat);
              nextItem++;
            }
            else
            {
              InsertWithSplit(selectedItem, itemPosition, replacementText, replacementFormat);
            }
          }
          else
          {
            itemText = itemText.Insert(itemPosition, replacementText);
            this.SetText(selectedItem, itemText);
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
  /// Replace the format of text at given position and of given length with the replacement format.
  /// </summary>
  /// <param name="length"></param>
  /// <param name="position"></param>
  /// <param name="replacementFormat"></param>
  /// <param name="options"></param>
  /// <returns></returns>
  private bool ReplaceFormatAt(int position, int length, TextFormat replacementFormat, FindAndReplaceOptions? options = null)
  {
    var sumLength = 0;
    var selectedItem = -1;
    for (int i = 0; i < this.Count; i++)
    {
      var itemText = this[i].Text;
      if (sumLength + itemText.Length > position)
      {
        selectedItem = i;
        break;
      }
      sumLength += itemText.Length;
    }
    if (selectedItem >= 0)
    {
      while (selectedItem < this.Count && (length > 0))
      {
        var itemText = this[selectedItem].Text;
        var itemOldLength = itemText.Length;
        length -= itemText.Length;
        var nextItem = selectedItem + 1;
        if (!replacementFormat.IsSame(this[selectedItem].Run.GetFormat()))
        {
          SetFormat(selectedItem, replacementFormat);
        }
        if (length <= 0)
          break;
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

}