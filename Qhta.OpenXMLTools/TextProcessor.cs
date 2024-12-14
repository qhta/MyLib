using System;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Class to process text in a document.
/// </summary>
public class TextProcessor
{

  //private readonly TextOptions GetTextOptions = TextOptions.PlainText;
  //private readonly FormattedText FormattedText = new();

  ///// <summary>
  ///// Construct a text processor using paragraph as a context element.
  ///// </summary>
  ///// <param name="paragraph"></param>
  //public TextProcessor(DXW.Paragraph paragraph)
  //{
  //  foreach (var member in paragraph.Elements<DXW.Run>())
  //  {
  //    var text = member.GetText(GetTextOptions);
  //    FormattedText.Add(new RunText(member, text));
  //  }
  //}

  ///// <summary>
  ///// Get text of the context and fill FormattedText dictionary.
  ///// </summary>
  ///// <returns></returns>
  //public string GetText()
  //{
  //  var sb = new StringBuilder();
  //  foreach (var item in FormattedText)
  //  {
  //    sb.Append(item.Text);
  //  }
  //  return sb.ToString();
  //}

  ///// <summary>
  ///// Search the text with the given format.
  ///// </summary>
  ///// <param name="searchText"></param>
  ///// <param name="searchFormat"></param>
  ///// <param name="options"></param>
  ///// <returns>character position of the text or -1 if not found</returns>
  //public int Search(string searchText, TextFormat? searchFormat, FindAndReplaceOptions? options = null)
  // => Search(0, searchText, searchFormat, options);

  ///// <summary>
  ///// Search the text with the given format starting at the given startPosition.
  ///// </summary>
  ///// <param name="startPosition"></param>
  ///// <param name="searchText"></param>
  ///// <param name="searchFormat"></param>
  ///// <param name="options"></param>
  ///// <returns>character startPosition of the text or -1 if not found</returns>
  //public int Search(int startPosition, string searchText, TextFormat? searchFormat, FindAndReplaceOptions? options = null)
  //{
  //  var findWholeWordsOnly = options?.FindWholeWordsOnly ?? false;
  //  var matchCaseInsensitive = options?.MatchCaseInsensitive ?? false;
  //  var stringComparison =
  //    matchCaseInsensitive ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
  //  if (searchFormat == null)
  //  {
  //    var searchInText = GetText();
  //    if (findWholeWordsOnly)
  //    {
  //      searchInText = '\0' + searchInText + '\0';
  //      var k = searchInText.IndexOf(searchText, startPosition, stringComparison);
  //      while (k > 0)
  //      {
  //        if (char.IsLetterOrDigit(searchInText[k - 1]) || k + searchText.Length < searchInText.Length && char.IsLetterOrDigit(searchInText[k + searchText.Length]))
  //          k = searchInText.IndexOf(searchText, k + 1, stringComparison);
  //        else
  //          return k - 1;
  //      }
  //      return -1;
  //    }
  //    else
  //      return searchInText.IndexOf(searchText, startPosition, stringComparison);
  //  }
  //  var searchTextLength = searchText.Length;
  //  var sumLength = 0;
  //  for (int i = 0; i < FormattedText.Count; i++)
  //  {
  //    var itemText = FormattedText[i].Text;
  //    if (sumLength + itemText.Length > startPosition)
  //    {
  //      if (searchFormat.IsSame(FormattedText[i].Run.GetFormat()))
  //      {
  //        var searchInText = itemText;
  //        if (findWholeWordsOnly)
  //        {
  //          if (i > 0)
  //            searchInText = FormattedText[i - 1].Text.LastOrDefault() + searchInText;
  //          else
  //            searchInText = '\0' + searchInText;
  //        }
  //        int j = i + 1;
  //        while (searchInText.Length > searchTextLength && j < FormattedText.Count && searchFormat.IsSame(FormattedText[j].Run.GetFormat()))
  //        {
  //          searchInText += FormattedText[j].Text;
  //          j++;
  //        }
  //        if (findWholeWordsOnly)
  //        {
  //          if (i < FormattedText.Count - 1)
  //            searchInText = searchInText + FormattedText[i + 1].Text.LastOrDefault();
  //          else
  //            searchInText = searchInText + '\0';
  //        }
  //        if (searchInText.Length >= searchTextLength)
  //        {
  //          var k = searchInText.IndexOf(searchText, stringComparison);
  //          while (k >= 0)
  //          {
  //            if (findWholeWordsOnly && k > 0)
  //            {
  //              if (char.IsLetterOrDigit(searchInText[k - 1]) || char.IsLetterOrDigit(searchInText[k + searchTextLength]))
  //                k = searchInText.IndexOf(searchText, k + 1, stringComparison);
  //              else
  //                return sumLength + k - 1;
  //            }
  //            else
  //              return sumLength + k;
  //          }
  //        }
  //      }
  //    }
  //    sumLength += itemText.Length;
  //  }
  //  return -1;
  //}

  ///// <summary>
  ///// Replace the first occurrence of the search text with the replacement text.
  ///// </summary>
  ///// <param name="searchText"></param>
  ///// <param name="replacementText"></param>
  ///// <param name="options"></param>
  ///// <returns></returns>
  //public bool Replace(string searchText, string replacementText, FindAndReplaceOptions? options = null)
  //  => Replace(searchText, null, replacementText, null, options);

  ///// <summary>
  ///// Replace the first occurrence of the search text with the formatted replacement text.
  ///// </summary>
  ///// <param name="searchText"></param>
  ///// <param name="replacementText"></param>
  ///// <param name="replacementFormat"></param>
  ///// <param name="options"></param>
  ///// <returns></returns>
  //public bool Replace(string searchText, string replacementText, TextFormat? replacementFormat, FindAndReplaceOptions? options = null)
  //  => Replace(searchText, null, replacementText, replacementFormat, options);

  ///// <summary>
  ///// Replace the first occurrence of the formatted search text with the formatted replacement text.
  ///// </summary>
  ///// <param name="searchText"></param>
  ///// <param name="replacementText"></param>
  ///// <param name="searchFormat"></param>
  ///// <param name="replacementFormat"></param>
  ///// <param name="options"></param>
  ///// <returns></returns>
  //public bool Replace(string searchText, TextFormat? searchFormat, string replacementText, TextFormat? replacementFormat, FindAndReplaceOptions? options = null)
  //{
  //  var k = Search(0, searchText, searchFormat, options);
  //  if (k >= 0)
  //  {
  //    if (options?.MatchCaseInsensitive == true)
  //    {
  //      var foundText = GetText().Substring(k, searchText.Length);
  //      if (foundText != searchText)
  //      {
  //        if (foundText.IsUppercase())
  //          replacementText = replacementText.ToUpper();
  //        else if (foundText.IsLowercase())
  //          replacementText = replacementText.ToLower();
  //        else if (foundText.IsTitlecase())
  //          replacementText = replacementText.TitleCase();
  //      }
  //    }
  //    return ReplaceAt(k, searchText.Length, replacementText, replacementFormat);
  //  }
  //  return false;
  //}

  ///// <summary>
  ///// Replace the text at given position and of given length with the replacement text.
  ///// </summary>
  ///// <param name="length"></param>
  ///// <param name="replacementText"></param>
  ///// <param name="position"></param>
  ///// <param name="replacementFormat"></param>
  ///// <param name="options"></param>
  ///// <returns></returns>
  //public bool ReplaceAt(int position, int length, string replacementText, TextFormat? replacementFormat = null, FindAndReplaceOptions? options = null)
  //{
  //  var sumLength = 0;
  //  var selectedItem = -1;
  //  for (int i = 0; i < FormattedText.Count; i++)
  //  {
  //    var itemText = FormattedText[i].Text;
  //    if (sumLength + itemText.Length > position)
  //    {
  //      selectedItem = i;
  //      break;
  //    }
  //    sumLength += itemText.Length;
  //  }
  //  if (selectedItem >= 0)
  //  {
  //    while (selectedItem < FormattedText.Count && (length > 0 || replacementText.Length > 0))
  //    {
  //      var itemText = FormattedText[selectedItem].Text;
  //      var itemOldLength = itemText.Length;
  //      var itemPosition = position - sumLength;
  //      var itemRestLength = itemText.Length - itemPosition;
  //      var delLength = length;
  //      if (itemRestLength < length)
  //      {
  //        delLength = itemRestLength;
  //        length -= delLength;
  //      }
  //      else
  //      {
  //        length = 0;
  //      }
  //      if (delLength > 0)
  //      {
  //        itemText = itemText.Remove(itemPosition, delLength);
  //        FormattedText.SetText(selectedItem, itemText);
  //      }
  //      var nextItem = selectedItem + 1;
  //      if (replacementText.Length > 0)
  //      {
  //        if (replacementFormat != null && !replacementFormat.IsSame(FormattedText[selectedItem].Run.GetFormat()))
  //        {
  //          var selectedRun = FormattedText[selectedItem].Run;
  //          if (itemPosition == 0)
  //          {
  //            var newRun = new DXW.Run();
  //            newRun.AppendText(replacementText);
  //            newRun.SetFormat(replacementFormat);
  //            selectedRun.InsertBeforeSelf(newRun);
  //            FormattedText.Insert(selectedItem, new RunText(newRun, replacementText));
  //          }
  //          else if (itemPosition == itemText.Length)
  //          {
  //            var newRun = new DXW.Run();
  //            newRun.AppendText(replacementText);
  //            newRun.SetFormat(replacementFormat);
  //            selectedRun.InsertAfterSelf(newRun);
  //            FormattedText.Insert(selectedItem + 1, new RunText(newRun, replacementText));
  //            nextItem++;
  //          }
  //          else
  //          {
  //            var tailRun = selectedRun.SplitAt(itemPosition, TextOptions.PlainText);
  //            var newRun = new DXW.Run();
  //            newRun.AppendText(replacementText);
  //            newRun.SetFormat(replacementFormat);
  //            FormattedText[selectedItem].Run.InsertAfterSelf(newRun);
  //            FormattedText.Insert(selectedItem + 1, new RunText(newRun, replacementText));
  //            nextItem++;
  //            selectedItem++;
  //            if (tailRun != null)
  //            {
  //              newRun.InsertAfterSelf(tailRun);
  //              FormattedText.Insert(selectedItem, new RunText(tailRun, tailRun.GetText(GetTextOptions)));
  //            }
  //          }
  //        }
  //        else
  //        {
  //          itemText = itemText.Insert(itemPosition, replacementText);
  //          FormattedText.SetText(selectedItem, itemText);
  //        }
  //        replacementText = String.Empty;
  //      }
  //      if (length == 0)
  //        break;
  //      position += delLength;
  //      sumLength += itemOldLength;
  //      selectedItem = nextItem;
  //    }
  //    return true;
  //  }
  //  return false;
  //}

  ///// <summary>
  ///// Remove exceeding whitespaces from all paragraphs in the context.
  ///// </summary>
  //public void NormalizeWhitespaces(WhitespaceOptions options)
  //{
  //  var text = GetText();
  //  var startSpacesLength = 0;
  //  var endSpacesLength = 0;
  //  for (int i = 0; i < text.Length; i++)
  //  {
  //    if (char.IsWhiteSpace(text[i]))
  //    {
  //      startSpacesLength++;
  //    }
  //    else break;
  //  }
  //  if (startSpacesLength < text.Length)
  //  {
  //    for (int i = text.Length - 1; i >= 0; i--)
  //    {
  //      if (char.IsWhiteSpace(text[i]))
  //      {
  //        endSpacesLength++;
  //      }
  //      else break;
  //    }
  //  }
  //}

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
