using System.Globalization;
using System.Reflection;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;

using Qhta.OpenXmlTools;

using static Qhta.OpenXmlToolsTest.TestTools;

namespace Qhta.OpenXmlToolsTest;
public class PropertiesTest
{

  public void DocumentPropertiesReadTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      CorePropertiesDirectReadTest(wordDoc);
      ExtendedFilePropertiesDirectReadTest(wordDoc);
      CustomFilePropertiesDirectReadTest(wordDoc);
      DocumentPropertiesReadTest(wordDoc);
    }
  }

  public void CorePropertiesDirectReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Core properties read test:");
    var coreProperties = wordDoc.GetCoreProperties();
#pragma warning disable OOXML0001
    foreach (var property in typeof(IPackageProperties).GetProperties())
#pragma warning restore OOXML0001
    {
      var value = property.GetValue(coreProperties);
      Console.WriteLine($"{property.Name}: {value.AsString()}");
    }
    Console.WriteLine();
  }

  public void ExtendedFilePropertiesDirectReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Extended file properties read test:");
    var appProperties = wordDoc.GetExtendedFileProperties();
    if (appProperties == null)
    {
      Console.WriteLine("No extended file properties found");
    }
    else
      foreach (var property in typeof(DocumentFormat.OpenXml.ExtendedProperties.Properties)
                 .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
      {
        if (property.CanRead && property.CanWrite && !property.Name.EndsWith("Part"))
        {
          // var element = appProperties.Elements().FirstOrDefault(item => item.LocalName == property.Name);
          object? element = property.GetValue(appProperties);
          object? value = null;
          if (element is OpenXmlLeafTextElement textElement)
          {
            if (intProperties.Contains(property.Name))
              value = int.Parse(textElement.InnerText);
            else if (boolProperties.Contains(property.Name))
              value = bool.Parse(textElement.InnerText);
            else
              value = textElement.InnerText;
          }
          else if (element is OpenXmlCompositeElement compositeElement)
            value = compositeElement.FirstChild.GetVariantValue();
          Console.WriteLine($"{property.Name}: {value.AsString()}");
        }
      }
    Console.WriteLine();
  }

  public void CustomFilePropertiesDirectReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Custom file properties read test:");
    var customProperties = wordDoc.GetCustomFileProperties();
    if (customProperties == null)
    {
      Console.WriteLine("No custom file properties found");
    }
    else
      foreach (var customProperty in customProperties.Elements<DXCP.CustomDocumentProperty>())
      {
        var value = customProperty.FirstChild.GetVariantValue();
        Console.WriteLine($"{customProperty.Name}: {value.AsString()}");
      }
    Console.WriteLine();
  }

  public void DocumentPropertiesReadTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Document properties read test:");
    var documentProperties = wordDoc.GetDocumentProperties();

    foreach (var propName in documentProperties.GetNames())
    {
      Console.WriteLine($"{propName}: {documentProperties.GetValue(propName)}");
    }
    Console.WriteLine();
  }

  public void DocumentPropertiesWriteTest(string filename)
  {
    using (var wordDoc = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document))
    {
      var mainDocumentPart = wordDoc.AddMainDocumentPart();
      var document = mainDocumentPart.Document = new Document();
      var body = document.Body = new Body();
      CorePropertiesDirectWriteTest(wordDoc);
      ExtendedFilePropertiesDirectWriteTest(wordDoc);
      CustomFilePropertiesDirectWriteTest(wordDoc);
    }
    using (var wordDoc = WordprocessingDocument.Open(filename, false))
    {
      CorePropertiesDirectReadTest(wordDoc);
      ExtendedFilePropertiesDirectReadTest(wordDoc);
      CustomFilePropertiesDirectReadTest(wordDoc);
    }
  }

  public void CorePropertiesDirectWriteTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Core properties write test:");
    var coreProperties = wordDoc.GetCoreProperties();
#pragma warning disable OOXML0001
    foreach (var property in typeof(IPackageProperties).GetProperties())
#pragma warning restore OOXML0001
    {
      var value = CreateNewPropertyValue(property.Name, property.PropertyType);
      if (property.Name == "Revision")
        value = "1";
      if (property.PropertyType == typeof(string))
      {
        property.SetValue(coreProperties, value);
        Console.WriteLine($"{property.Name}: {property.GetValue(coreProperties)}");
      }
    }
    Console.WriteLine();
  }

  public void ExtendedFilePropertiesDirectWriteTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Extended file properties write test:");
    var appProperties = wordDoc.GetExtendedFileProperties();
    if (appProperties == null)
    {
      var extendedFilePropertiesPart = wordDoc.AddExtendedFilePropertiesPart();
      appProperties = new DocumentFormat.OpenXml.ExtendedProperties.Properties();
      extendedFilePropertiesPart.Properties = appProperties;
    }
    foreach (var property in typeof(DocumentFormat.OpenXml.ExtendedProperties.Properties)
               .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
    {
      if (property.CanWrite)
      {
        var propertyType = property.PropertyType;
        if (propertyType.BaseType == typeof(OpenXmlLeafTextElement))
        {
          var element = Activator.CreateInstance(propertyType) as OpenXmlLeafTextElement;
          var value = CreateNewPropertyValue(property.Name, typeof(string));
          if (property.Name == "ApplicationVersion")
            value = "1.0";
          else if (intProperties.Contains(property.Name))
            value = 1;
          else if (boolProperties.Contains(property.Name))
            value = false;
          if (value == null) continue;
          if (value is bool)
            value = value.ToString()!.ToLower();
          var valueText = value.ToString();
          element!.Text = valueText!;
          property.SetValue(appProperties, element);
          Console.WriteLine($"{property.Name}: {value.AsString()}");
        }
      }
    }
    Console.WriteLine();
  }


  public void CustomFilePropertiesDirectWriteTest(WordprocessingDocument wordDoc)
  {
    Console.WriteLine("Custom file properties write test:");
    var customProperties = wordDoc.GetCustomFileProperties();
    if (customProperties == null)
    {
      var customFilePropertiesPart = wordDoc.AddCustomFilePropertiesPart();
      customProperties = new DocumentFormat.OpenXml.CustomProperties.Properties();
      customFilePropertiesPart.Properties = customProperties;
    }
    var propId = 2;
    foreach (var data in vtTestData)
    {
      var propertyType = data.Key;
      var value = data.Value;
      OpenXmlElement? dataInstance = CreateVariantElement(propertyType, value);
      if (dataInstance != null)
      {
        var customProperty = new DocumentFormat.OpenXml.CustomProperties.CustomDocumentProperty
        {
          Name = propertyType.Name.Substring(2),
          FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}",
          PropertyId = propId++
        };
        customProperty.AppendChild(dataInstance);
        customProperties.AppendChild(customProperty);
        Console.WriteLine($"{propertyType.Name}: {value.AsString()}");
      }
    }

    Console.WriteLine();
  }


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
    return null;
  }

  private static readonly string[] intProperties = new string[]
  {
    "Characters", "CharactersWithSpaces","Lines", "Pages", "Paragraphs",
    "Revision", "TotalTime", "Words", "Slides", "HiddenSlides", "MMClips", "MultimediaClips", "Notes" , "DocumentSecurity",
  };

  private static readonly string[] boolProperties = new string[]
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
