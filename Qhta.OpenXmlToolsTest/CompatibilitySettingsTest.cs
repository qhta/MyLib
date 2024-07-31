using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Qhta.TextUtils;

namespace Qhta.OpenXmlToolsTest;

public class CompatibilitySettingsTest

{
  public void CompatibilitySettingsReadTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {

      DocumentCompatibilitySettingsReadTest(wordDoc);
    }
  }


  public void CompatibilitySettingsWriteTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document))
    {
      var mainDocumentPart = wordDoc.AddMainDocumentPart();
      var document = mainDocumentPart.Document = new DXW.Document();
      document.Body = new DXW.Body();
      CompatibilitySettingsWriteTest(wordDoc);
    }
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      DocumentCompatibilitySettingsReadTest(wordDoc);
    }
  }

  public void DocumentCompatibilitySettingsReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Compatibility settings read test:");
    if (!wordDoc.HasCompatibilitySettings())
    {
      Console.WriteLine("No compatibility settings found");
      return;
    }

    var compatibilitySettings = wordDoc.GetCompatibilitySettings();
    var count = compatibilitySettings.Count();
    Console.WriteLine($"Compatibility settings count = {count}");
    var propNames = compatibilitySettings.GetNames().ToArray();
    foreach (var propName in propNames)
    {
      var value = compatibilitySettings.GetValue(propName);
      if (value is OpenXmlCompositeElement)
        Console.WriteLine($"{propName}:\r\n{value.AsString(1)}");
      else
        Console.WriteLine($"{propName}: {value.AsString()}");
    }
    Console.WriteLine();
  }


  public void CompatibilitySettingsWriteTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Compatibility settings write test:");
    var compatibilitySettings = wordDoc.GetCompatibilitySettings();
    foreach (var propName in compatibilitySettings.GetNames(true))
    {
      var propType = compatibilitySettings.GetType(propName);
      var value = TestTools.CreateNewPropertyValue(propName, propType);
      if (propName == "CompatibilityMode")
        value = 15;
      if (value != null)
      {
        compatibilitySettings.SetValue(propName, value);
        Console.WriteLine($"writing {propName}: {value.AsString()}");
      }
    }
    Console.WriteLine();
  }


}
