using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlToolsTest;
public class MathPropertiesTest

{

  public void MathPropertiesReadTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      DocumentMathPropertiesReadTest(wordDoc);
      DocumentMathPropertiesReadTest(wordDoc);
    }
  }


  public void MathPropertiesWriteTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document))
    {
      var mainDocumentPart = wordDoc.AddMainDocumentPart();
      var document = mainDocumentPart.Document = new Document();
      document.Body = new Body();
      DocumentMathPropertiesWriteTest(wordDoc);
    }
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      DocumentMathPropertiesReadTest(wordDoc);
    }
  }


  public void DocumentMathPropertiesReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Math properties read test:");
    if (!wordDoc.HasMathProperties())
    {
      Console.WriteLine("No Math properties found");
      return;
    }

    var documentMathProperties = wordDoc.GetMathProperties();
    var count = documentMathProperties.Count();
    Console.WriteLine($"Math properties count = {count}");
    var propNames = documentMathProperties.GetNames().ToArray();
    foreach (var propName in propNames)
    {
      var value = documentMathProperties.GetValue(propName);
      if (value is OpenXmlCompositeElement)
        Console.WriteLine($"{propName}:\r\n{value.AsString(1)}");
      else
        Console.WriteLine($"{propName}: {value.AsString()}");
    }
    Console.WriteLine();
  }


  public void DocumentMathPropertiesWriteTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Math properties write test:");
    var MathProperties = wordDoc.GetMathProperties();
    foreach (var propName in MathProperties.GetNames(true))
    {
      var propType = MathProperties.GetType(propName);
      var value = TestTools.CreateNewPropertyValue(propName, propType);
      //if (propName == "RemovePersonalInformation"
      //    || propName == "HideSpellingErrors" || propName == "HideGrammaticalErrors"
      //    || propName == "SaveFormsData"
      //    || propName == "TrackRevisions"
      //    || propName == "DoNotTrackMoves" || propName == "DoNotTrackFormatting" || propName == "DoNotTrackComments"
      //    || propName == "UpdateFieldsOnOpen"
      //    )
      //  value = false;
      //else if (propName == "AttachedTemplate")
      //  value = @"file:///C:\Users\qhta1\AppData\Roaming\Microsoft\Templates\ECMA.dotx";
      if (value != null)
      {
        MathProperties.SetValue(propName, value);
        Console.WriteLine($"writing {propName}: {value.AsString()}");
      }
    }
    Console.WriteLine();
  }


}
