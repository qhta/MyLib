using System.Xml.Linq;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Class to find (and replace) text in a document.
/// </summary>
public class Finder
{

  /// <summary>
  /// Construct a finder using context element.
  /// </summary>
  /// <param name="contextElement"></param>
  public Finder(DX.OpenXmlCompositeElement contextElement)
  {
    var members = contextElement.GetMembers().ToArray();
    var startMember = members.FirstOrDefault();
    var endMember = members.LastOrDefault();
    ContextRange = new Range(startMember, endMember);
  }

  /// <summary>
  /// Construct a finder using context range.
  /// </summary>
  /// <param name="contextRange"></param>
  public Finder(Range contextRange)
  {
    ContextRange = contextRange;
  }

  /// <summary>
  /// ContextRange for the search.
  /// </summary>
  public Range ContextRange { get; set; }

  /// <summary>
  /// Options for get text to search.
  /// </summary>

  public TextOptions TextOptions
  {
    [DebuggerStepThrough]
    get;
    [DebuggerStepThrough]
    set;
  } = TextOptions.PlainText;

  /// <summary>
  /// Text to search for.
  /// </summary>
  public string? SearchText { get; set; }

  /// <summary>
  /// Determines if the search must consider formatting.
  /// </summary>
  public bool Format { get; set; }

  /// <summary>
  /// SearchText to replace.
  /// </summary>
  public string? Replacement { get; set; }

  /// <summary>
  /// Realize the search and replace.
  /// </summary>
  /// <returns>if finder founds text and/or formatting</returns>
  public bool? Execute()
  {
    if (SearchText == null)
      return false;

    bool found = false;
    var paragraphs = ContextRange.GetParagraphs(true).ToList();

    foreach (var paragraph in paragraphs)
    {
      if (Replacement != null)
      {
        if (ReplaceText(paragraph, SearchText, Replacement))
          found = true;
      }
      else
      {
        if (FindText(paragraph, SearchText))
          found = true;
        break;
      }
    }
    return found;
  }

  private bool FindText(DXW.Paragraph paragraph, string sText)
  {
    var aText = paragraph.GetText();
    var k = aText.IndexOf(sText);
    if (k != -1)
    {
      return true;
    }
    return false;
  }

  /// <summary>
  /// Find the text in the paragraph and replace it with the replacement text.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <param name="sText"></param>
  /// <param name="replacement"></param>
  /// <returns></returns>
  private bool ReplaceText(DXW.Paragraph paragraph, string sText, string replacement)
  {
    var textOptions = TextOptions.ParaText;
    bool found = false;
    var members = paragraph.GetFlattenedMembers().ToArray();
    foreach (var member in members)
    {
      var aText = member.GetText(textOptions);
      var k = Find(aText, sText);
      if (k != -1)
      {
        if (k + sText.Length <= aText.Length)
        {
          found = true;
          var newText = aText.Substring(0, k) + replacement + aText.Substring(k + sText.Length);
          member.SetText(newText, textOptions);
          break;
        }
        else
        {
          var sTextMatchLength = sText.Length - aText.Length - k;
          var nextElement = member.NextSiblingMember();
          while (nextElement != null && sTextMatchLength > 0)
          {
            aText = nextElement.GetText(TextOptions);
            sText = sText.Substring(sText.Length - sTextMatchLength);
            k = Find(aText, sText);
            if (k == -1)
              break;
            if (k + sText.Length <= aText.Length)
            {
              found = true;
              break;
            }
            sTextMatchLength = sText.Length - aText.Length - k;
            nextElement = nextElement.NextSiblingMember();
          }
        }
      }
    }
    return found;
  }

  /// <summary>
  /// Check if the input text matches start of the search text.
  /// Determines if the input text contains completely the search text
  /// or if the input text ends with a start of the search text of any length.
  /// </summary>
  /// <param name="inText">SearchText to search in</param>
  /// <param name="sText">SearchText to search for</param>
  /// <returns>The index of the first char that matches the search string or -1 if not found</returns>
  private int Find(string inText, string sText)
  {
    for (int i = 0; i < inText.Length; i++)
    {
      if (inText[i] == sText[0])
      {
        int j = 1;
        while (j < sText.Length && i + j < inText.Length && inText[i + j] == sText[j])
        {
          j++;
        }
        if (j == sText.Length)
          return i;
        if (i + j == inText.Length)
          return i;
      }
    }
    return -1;
  }
}