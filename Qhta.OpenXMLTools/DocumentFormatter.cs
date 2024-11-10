using System;

using Qhta.TextUtils;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A composite tool for formatting a Wordprocessing document.
/// </summary>
public class DocumentFormatter
{
  /// <summary>
  /// Determines the level of verbosity of the cleaner.
  /// </summary>
  public int VerboseLevel { get; set; }

  private Dictionary<string, int> PropertyCounts = new();

  /// <summary>
  /// Format the document using all the methods available.
  /// </summary>
  /// <param name="fileName"></param>
  public void FormatDocument(string fileName)
  {
    using var wordDoc = DXPack.WordprocessingDocument.Open(fileName, true);
    FormatBodyParagraphs(wordDoc);
  }

  /// <summary>
  /// Format body paragraphs.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void FormatBodyParagraphs(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nFormatting paragraphs");
    var body = wordDoc.GetBody();
    var count = FormatParagraphs(body);
    //var headers = wordDoc.GetHeaders().ToList();
    //foreach (var header in headers)
    //{
    //  count += FormatParagraphs(header);
    //}
    //var footers = wordDoc.GetFooters().ToList();
    //foreach (var footer in wordDoc.GetFooters())
    //{
    //  count += FormatParagraphs(footer);
    //}
    if (VerboseLevel > 0)
      Console.WriteLine($" {count} paragraphs formatted.");
  }

  /// <summary>
  /// Format paragraphs in the element.
  /// </summary>
  /// <param name="body"></param>
  /// <returns>count of trimmed paragraphs</returns>
  public int FormatParagraphs(DX.OpenXmlCompositeElement body)
  {
    var paragraphs = body.Descendants<DXW.Paragraph>().ToList();
    var count = 0;
    foreach (var paragraph in paragraphs)
      if (ScanParagraph(paragraph))
        count++;
    return count;
  }

  /// <summary>
  /// Scan a paragraph for properties.
  /// Find the properties of the paragraph and count them.
  /// Properties counts are stored in the PropertyCounts dictionary.
  /// </summary>
  /// <param name="paragraph"></param>
  /// <returns></returns>
  private bool ScanParagraph(DXW.Paragraph paragraph)
  {
    var properties = paragraph.ParagraphProperties;
    if (properties == null)
      return false;

    foreach (var element in properties.Elements())
    {
      var propName = element.LocalName;
      if (PropertyCounts.ContainsKey(propName))
        PropertyCounts[propName]++;
      else
        PropertyCounts[propName] = 1;
    }
    return true;
  }

}