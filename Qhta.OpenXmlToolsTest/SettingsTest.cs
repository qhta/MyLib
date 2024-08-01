using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlToolsTest;
public class SettingsTest
{

  public void SettingsReadTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      DocumentSettingsDirectReadTest(wordDoc);
      DocumentSettingsReadTest(wordDoc);
    }
  }


  public void SettingsWriteTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document))
    {
      var mainDocumentPart = wordDoc.AddMainDocumentPart();
      var document = mainDocumentPart.Document = new Document();
      document.Body = new Body();
      DocumentSettingsWriteTest(wordDoc);
    }
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      DocumentSettingsReadTest(wordDoc);
    }
  }

  public void DocumentSettingsDirectReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Document settings direct read test:");
    var documentSettings = wordDoc.MainDocumentPart?.DocumentSettingsPart?.Settings;

    if (documentSettings == null)
    {
      Console.WriteLine("No document settings found");
      return;
    }
    var count = documentSettings.Count();
    Console.WriteLine($"Document settings count = {count}");
    foreach (var setting in documentSettings)
    {
      Console.WriteLine($"{setting.Prefix}:{setting.LocalName}");
      //Console.WriteLine(setting.OuterXml.Replace("<","\r\n<").Substring(2));
    }
    Console.WriteLine();
  }


  public void DocumentSettingsReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Document settings read test:");
    if (!wordDoc.HasSettings())
    {
      Console.WriteLine("No document settings found");
      return;
    }

    var documentSettings = wordDoc.GetSettings();
    var count = documentSettings.Count();
    Console.WriteLine($"Document settings count = {count}");
    var propNames = documentSettings.GetNames().ToArray();
    foreach (var propName in propNames)
    {
      var value = documentSettings.GetValue(propName);
      if (value is OpenXmlCompositeElement)
        Console.WriteLine($"{propName}:\r\n{value.AsString(1)}");
      else
        Console.WriteLine($"{propName}: {value.AsString()}");
    }
    Console.WriteLine();
  }


  public void DocumentSettingsWriteTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Document settings write test:");
    var settings = wordDoc.GetSettings();
    foreach (var propName in settings.GetNames(ItemFilter.All))
    {
      var propType = settings.GetType(propName);
      var value = TestTools.CreateNewPropertyValue(propName, propType);
      if (propName == "RemovePersonalInformation" 
          || propName == "HideSpellingErrors" || propName == "HideGrammaticalErrors"
          || propName == "SaveFormsData" 
          || propName == "TrackRevisions"
          || propName == "DoNotTrackMoves" || propName == "DoNotTrackFormatting" || propName == "DoNotTrackComments"
          || propName == "UpdateFieldsOnOpen"
          )
        value = false;
      else if (propName == "AttachedTemplate")
        value = @"file:///C:\Users\qhta1\AppData\Roaming\Microsoft\Templates\ECMA.dotx";
      if (value != null)
      {
        settings.SetValue(propName, value);
        Console.WriteLine($"writing {propName}: {value.AsString()}");
      }
    }
    Console.WriteLine();
  }


}
