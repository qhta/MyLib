using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.VariantTypes;
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
    //using (var wordDoc = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document))
    //{
    //  var mainDocumentPart = wordDoc.AddMainDocumentPart();
    //  var document = mainDocumentPart.Document = new Document();
    //  var body = document.Body = new Body();
    //  SettingsDirectWriteTest(wordDoc);
    //  ExtendedFileSettingsDirectWriteTest(wordDoc);
    //  CustomFileSettingsDirectWriteTest(wordDoc);
    //  DocumentSettingsWriteTest(wordDoc);
    //}
    //using (var wordDoc = WordprocessingDocument.Open(filename, false))
    //{
    //  SettingsDirectReadTest(wordDoc);
    //  ExtendedFileSettingsDirectReadTest(wordDoc);
    //  CustomFileSettingsDirectReadTest(wordDoc);
    //  DocumentSettingsReadTest(wordDoc);
    //}
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
    var documentSettings = wordDoc.MainDocumentPart?.DocumentSettingsPart?.Settings;
    
    if (documentSettings == null)
    {
      Console.WriteLine("No document settings found");
      return;
    }
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


  public void SettingWriteTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Core properties write test:");
    var settings = wordDoc.GetSettings();
    foreach (var propName in settings.GetNames(true))
    {
      var propType = settings.GetType(propName);
      var value = CreateNewPropertyValue(propName, propType);
      if (value!=null)
      {
        settings.SetValue(propName, value);
        Console.WriteLine($"writing {propName}: {value.AsString()}");
      }
    }
    Console.WriteLine();
  }

  //public void DocumentSettingsWriteTest(WordprocessingDocument wordDoc)
  //{
  //  Console.WriteLine("Document properties write test:");
  //  var documentSettings = wordDoc.GetDocumentSettings();
  //  foreach (var propName in documentSettings.GetNames())
  //  {
  //    var type = documentSettings.GetType(propName);
  //    var value = CreateNewPropertyValue(propName, type);
  //    Console.WriteLine($"writing {propName}: {value}");
  //    documentSettings.SetValue(propName, value);
  //  }
  //  foreach (var data in vtTestData)
  //  {

  //    var propertyType = data.Key;
  //    var propName = propertyType.Name.Substring(2) + "_new";
  //    var value = data.Value;
  //    OpenXmlElement? dataInstance = CreateVariantElement(propertyType, value);
  //    if (dataInstance != null)
  //    {
  //      documentSettings.SetValue(propName, value);
  //      Console.WriteLine($"writing {propName}: {value.AsString()}");
  //    }
  //  }
  //  Console.WriteLine();
  //}

  private object? CreateNewPropertyValue(string propertyName, Type propertyType)
  {
    if (propertyType.Name.StartsWith("Nullable"))
      propertyType = propertyType.GenericTypeArguments[0];
    if (propertyType == typeof(string))
      return propertyName + "_string";
    if (propertyType == typeof(DateTime))
      return DateTime.Now;
    if (propertyType == typeof(int))
      return 100_000;
    if (propertyType == typeof(bool))
      return true;
    if (propertyType == typeof(decimal))
      return 1.2m;
    return null;
  }

  private static readonly string[] intSettings = new string[]
  {
    "Characters", "CharactersWithSpaces","Lines", "Pages", "Paragraphs",
    "Revision", "TotalTime", "Words", "Slides", "HiddenSlides", "MMClips", "MultimediaClips", "Notes" , "DocumentSecurity",
  };

  private static readonly string[] boolSettings = new string[]
  {
    "ScaleCrop", "LinksUpToDate", "SharedDoc", "HyperlinksChanged", "SharedDocument"
  };

  private OpenXmlElement? CreateVariantElement(Type propertyType, object value)
  {
    OpenXmlElement? dataInstance = VTVariantTools.CreateVariant(propertyType, value);
    return dataInstance;
  }

  private static readonly Dictionary<Type, object> vtTestData = new()
  {
    { typeof(VTLPWSTR), "Gżegżółka" },
    { typeof(VTInt32), 100_000 },
    { typeof(VTBool), true },
    { typeof(VTFileTime), DateTime.Now },
    { typeof(VTDecimal), 123.45m},
    { typeof(VTDouble), 123.45e123d},
    { typeof(VTFloat), 123.45e25f},
    { typeof(VTByte), (sbyte)-128},
    { typeof(VTUnsignedByte), (byte)255},
    { typeof(VTShort), Int16.MinValue},
    { typeof(VTUnsignedShort),UInt16.MaxValue},
    { typeof(VTUnsignedInt32), UInt32.MaxValue},
    { typeof(VTInt64), Int64.MinValue},
    { typeof(VTUnsignedInt64), UInt64.MaxValue},
    { typeof(VTVector), new object[] { "Test1", 123, true } },
    //{ typeof(VTArray), new string[] { "Test1", "Test2", "Test3" } },

    //{ typeof(VTLPSTR), "Long String" },
    //{ typeof(VTBString), "Binary_String\t" },
    //{ typeof(VTStreamData), "StreamData" },
    //{ typeof(VTOStreamData), "OStreamData" },
    //{ typeof(VTVStreamData), "VStreamData" },


    //{ typeof(VTNull), null },
    //{ typeof(VTEmpty), null },
    //{ typeof(VTError), "Error" },
    //{ typeof(VTStream), "Stream" },
    //{ typeof(VTOStream), "OStream" },
    //{ typeof(VTVStream), "VStream" },
    //{ typeof(VTVStreams), "VStreams" },
    //{ typeof(VTStorage), "Storage" },
    //{ typeof(VTVStorage), "VStorage" },
    //{ typeof(VTVStorages), "VStorages" },
    //{ typeof(VTVersionedStream), "VersionedStream" },
    //{ typeof(VTVersionedStreams), "VersionedStreams
  };
}
