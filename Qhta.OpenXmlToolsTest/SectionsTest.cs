using DocumentFormat.OpenXml.Packaging;

namespace Qhta.OpenXmlToolsTest;


public class SectionsTest

{
  public void SectionsReadTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      //DocSectionPropertiesReadTest(wordDoc);
      //BodySectionPropertiesReadTest(wordDoc);
      //BodySectionRangesReadTest(wordDoc);
      //BodyFirstSectionRangeReadTest(wordDoc);
      //BodyLastSectionRangeReadTest(wordDoc);
      BodyLongestSectionRangeReadTest(wordDoc);
    }
  }

  public void SectionsWriteTest(string sourceFilename, string filename)
  {
    //using (var sourceDoc = WordprocessingDocument.Open(sourceFilename, false))
    //using (var wordDoc = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document))
    //{
    //  SectionsWriteTest(sourceDoc, wordDoc);
    //}
    //using (var wordDoc = WordprocessingDocument.Open(filename, false))
    //{
    //  SectionsReadTest(wordDoc);
    //}
  }

  public void DocSectionPropertiesReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Doc section properties read test:");
    if (!wordDoc.HasSectionProperties())
    {
      Console.WriteLine("No section properties found");
      return;
    }

    var sectionProperties = wordDoc.GetSectionProperties();
    var count = sectionProperties.Count();
    Console.WriteLine($"Sections properties count = {count}");
    foreach (var section in sectionProperties)
    {
      Console.WriteLine($"{section}:\r\n{section.AsString(1)}");
    }
    Console.WriteLine();
  }

  public void BodySectionPropertiesReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body section properties read test:");
    var body = wordDoc.GetBody();

    var section = body.GetSectionProperties();
    Console.WriteLine($"{section}:\r\n{section.AsString(1)}");
    Console.WriteLine();
  }

  public void BodySectionRangesReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body section ranges read test:");
    if (!wordDoc.HasSectionProperties())
    {
      Console.WriteLine("No section properties found");
      return;
    }

    var sectionRanges = wordDoc.GetBody().GetSectionRanges();
    var count = sectionRanges.Count();
    Console.WriteLine($"Sections ranges count = {count}");
    foreach (var section in sectionRanges)
    {
      Console.WriteLine($"{section}:\r\n{section.AsString(1, 0)}");
    }
    Console.WriteLine();
  }

  public void BodyFirstSectionRangeReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body first section range read test:");
    if (!wordDoc.HasSectionProperties())
    {
      Console.WriteLine("No section properties found");
      return;
    }

    var sectionRange = wordDoc.GetBody().GetSectionRanges().First();
    var elements = sectionRange.GetElements();
    var count = elements.Count();
    Console.WriteLine($"Section range elements count = {count}");
    foreach (var element in elements)
    {
      Console.WriteLine($"{element.AsString(1, 0)}");
    }
    Console.WriteLine();
  }

  public void BodyLastSectionRangeReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body last section range read test:");
    if (!wordDoc.HasSectionProperties())
    {
      Console.WriteLine("No section properties found");
      return;
    }

    var sectionRange = wordDoc.GetBody().GetSectionRanges().Last();
    var elements = sectionRange.GetElements();
    var count = elements.Count();
    Console.WriteLine($"Section range elements count = {count}");
    foreach (var element in elements)
    {
      Console.WriteLine($"{element.AsString(1, 0)}");
    }
    Console.WriteLine();
  }


  public void BodyLongestSectionRangeReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Body last longest range read test:");
    if (!wordDoc.HasSectionProperties())
    {
      Console.WriteLine("No section properties found");
      return;
    }

    var sectionRanges = wordDoc.GetBody().GetSectionRanges();
    var sectionRange = sectionRanges.OrderByDescending(r => r.GetElements().Count()).First();
    var elements = sectionRange.GetElements();
    var count = elements.Count();
    Console.WriteLine($"Section range elements count = {count}");
    foreach (var element in elements)
    {
      Console.WriteLine($"{element.AsString(1, 0)}");
    }
    Console.WriteLine();
  }
  //public void SectionsWriteTest(WordprocessingDocument sourceDoc, WordprocessingDocument targetDoc)
  //{
  //  Console.WriteLine("Sections write test:");
  //  var sourceSections = sourceDoc.GetSections();
  //  var targetSections = targetDoc.GetSections();
  //  foreach (var styleName in sourceSections.GetNames(ItemFilter.All))
  //  {
  //    var sourceStyle = sourceSections.GetStyle(styleName);
  //    if (sourceStyle == null)
  //      throw new Exception($"Source style \"{styleName}\" not found");
  //    Console.WriteLine($"{styleName} copied");
  //    var targetStyle = (Style)sourceStyle.CloneNode(true);
  //    targetSections.SetStyle(targetStyle);
  //  }
  //  Console.WriteLine();
  //}

}
