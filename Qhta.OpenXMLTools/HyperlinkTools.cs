using System;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with OpenXml Hyperlink elements.
/// </summary>
public static class HyperlinkTools
{
  /// <summary>
  /// Get the text of the hyperlink run elements.
  /// </summary>
  /// <param name="hyperlink">source hyperlink</param>
  /// <param name="options"></param>
  /// <returns>joined text</returns>
  public static string GetText(this DXW.Hyperlink hyperlink, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    var sb = new StringBuilder();
    foreach (var item in hyperlink.Elements())
    {
      var text = item.GetText(options);
      sb.Append(text);
    }
    var result = sb.ToString();
    return result;
  }

  /// <summary>
  /// Set the text of the hyperlink.
  /// </summary>
  /// <param name="hyperlink"></param>
  /// <param name="text"></param>
  /// <param name="options"></param>
  public static void SetText(this DXW.Hyperlink hyperlink, string text, GetTextOptions? options = null)
  {
    options ??= GetTextOptions.Default;
    hyperlink.RemoveAllChildren();
    var run = new DXW.Run();
    run.SetText(text, options);
    hyperlink.AppendChild(run);
  }

  /// <summary>
  /// Checks if the hyperlink is empty.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool IsEmpty(this DXW.Hyperlink? element)
  {
    if (element == null)
      return true;
    var text = element.GetText();
    return string.IsNullOrEmpty(text);
  }

  /// <summary>
  /// Get the reference relationship of the hyperlink.
  /// </summary>
  /// <param name="hyperlink"></param>
  /// <returns></returns>
  public static DXPack.ReferenceRelationship? GetRel(this DXW.Hyperlink hyperlink)
  {
    var relId = hyperlink.Id?.Value;
    if (relId != null)
    {
      var docPart = hyperlink.GetDocumentPart();
      if (docPart != null)
      {
        var rel = docPart.GetReferenceRelationship(relId);
        return rel;
      }
    }
    return null;
  }

  /// <summary>
  /// Try to trim hyperlink text.
  /// </summary>
  /// <param name="hyperlink"></param>
  /// <returns></returns>
  public static bool TryTrim(this DXW.Hyperlink hyperlink)
  {
    var done = false;
    var hyperlinkText = hyperlink.GetText();
    var hyperlinkTextTrimmed = hyperlinkText.TrimEnd();
    if (hyperlinkText != hyperlinkTextTrimmed)
    {
      if (hyperlinkTextTrimmed == "")
      {
        hyperlink.Remove();
      }
      else
      if (hyperlink.PreviousSibling() is DXW.Hyperlink previousHyperlink && previousHyperlink.GetRel().IsEqual(hyperlink.GetRel()))
      {
        var previousHyperlinkText = previousHyperlink.GetText();
        previousHyperlinkText += hyperlinkTextTrimmed;
        previousHyperlink.SetText(previousHyperlinkText);
      }
      else
        hyperlink.SetText(hyperlinkTextTrimmed);
      done = true;
    }
    return done;
  }
}