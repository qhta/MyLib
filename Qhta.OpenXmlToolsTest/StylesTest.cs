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

  public void StylesWriteTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document))
    {
      Styles(wordDoc);
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
        styleTypeStr += new string(' ',12 - styleTypeStr.Length);
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
      Console.WriteLine($" {styleTypeStr}\t{styleName}\t\t{(isHeading ? "It is heading" :"")}");
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
      Console.WriteLine($"{styleName}:\r\n{style.AsString(1)}");
    }
    Console.WriteLine();
  }


  public void Styles(WordprocessingDocument wordDoc)
  {
    //Console.WriteLine("Styles write test:");
    //var styles = wordDoc.GetStyles();
    //foreach (var propName in styles.GetNames(ItemFilter.All))
    //{
    //  var propType = styles.GetType(propName);
    //  var value = TestTools.CreateNewPropertyValue(propName, propType);
    //  if (propName == "CompatibilityMode")
    //    value = 15;
    //  if (value != null)
    //  {
    //    styles.SetValue(propName, value);
    //    Console.WriteLine($"writing {propName}: {value.AsString()}");
    //  }
    //}
    //Console.WriteLine();
  }

}
