using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlToolsTest;


public class StylesTest

{
  public void StylesReadTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      StylesNamesTest(wordDoc);
      StylesReadTest(wordDoc);
    }
  }

  public void StylesWriteTest(string sourceFilename, string filename)
  {
    using (var sourceDoc = WordprocessingDocument.Open(sourceFilename, false))
    using (var wordDoc = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document))
    {
      StylesWriteTest(sourceDoc, wordDoc);
    }
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      StylesReadTest(wordDoc);
    }
  }
  public void StylesNamesTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Styles names test:");
    if (!wordDoc.HasStyles())
    {
      Console.WriteLine("No styles found");
      return;
    }

    var styles = wordDoc.GetStyles();
    var count = styles.Count();
    Console.WriteLine($"Defined styles count = {count}");
    var styleNames = styles.GetNames().ToArray();
    foreach (var styleName in styleNames)
    {
      var styleType = styles.GetType(styleName);
      var styleTypeStr = styleType.ToString();
      if (styleTypeStr.Length < 12)
        styleTypeStr += new string(' ', 12 - styleTypeStr.Length);
      var isHeading = styles.IsHeading(styleName);
      Console.WriteLine($" {styleTypeStr}\t{styleName}\t\t{(isHeading ? "It is heading" : "")}");
    }
    count = styles.Count(ItemFilter.All);
    Console.WriteLine($"All styles count = {count}");
    styleNames = styles.GetNames(ItemFilter.All).ToArray();
    foreach (var styleName in styleNames)
    {
      var styleType = styles.GetType(styleName);
      var styleTypeStr = styleType.ToString();
      if (styleTypeStr.Length < 12)
        styleTypeStr += new string(' ', 12 - styleTypeStr.Length);
      var isHeading = styles.IsHeading(styleName);
      Console.WriteLine($" {styleTypeStr}\t{styleName}\t\t{(isHeading ? "It is heading" : "")}");
    }
    Console.WriteLine();
  }

  public void StylesReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Styles read test:");
    if (!wordDoc.HasStyles())
    {
      Console.WriteLine("No styles found");
      return;
    }

    var styles = wordDoc.GetStyles();
    var count = styles.Count();
    Console.WriteLine($"Styles count = {count}");
    var styleNames = styles.GetNames().ToArray();
    foreach (var styleName in styleNames)
    {
      var style = styles.GetStyle(styleName);
      if (style == null)
        Console.WriteLine($"{styleName} not found");
      else
        Console.WriteLine($"{styleName}:\r\n{style.AsString(1)}");
    }
    Console.WriteLine();
  }


  public void StylesWriteTest(WordprocessingDocument sourceDoc, WordprocessingDocument targetDoc)
  {
    Console.WriteLine("Styles write test:");
    var sourceStyles = sourceDoc.GetStyles();
    var targetStyles = targetDoc.GetStyles();
    foreach (var styleName in sourceStyles.GetNames(ItemFilter.All))
    {
      var sourceStyle = sourceStyles.GetStyle(styleName);
      if (sourceStyle == null)
        throw new Exception($"Source style \"{styleName}\" not found");
      Console.WriteLine($"{styleName} copied");
      var targetStyle = (Style)sourceStyle.CloneNode(true);
      targetStyles.SetStyle(targetStyle);
    }
    Console.WriteLine();
  }

}
