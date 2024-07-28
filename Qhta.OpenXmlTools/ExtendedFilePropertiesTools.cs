using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with extended file properties of a document.
/// </summary>
public static class ExtendedFilePropertiesTools
{
  /// <summary>
  /// Get the count of all the extended file properties.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <returns></returns>
#pragma warning disable OOXML0001
  public static int Count(this DXEP.Properties extendedFileProperties)
#pragma warning restore OOXML0001
    => PropTypes.Count;

  /// <summary>
  /// Get the names of all the extended file properties.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <returns></returns>
  public static string[] GetNames(this DXEP.Properties extendedFileProperties) 
    => PropTypes.Select(item=>item.Key.Trim()).ToArray();

  /// <summary>
  /// Get the type of property with its name.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static Type GetType(this DXEP.Properties extendedFileProperties, string propertyName)
  {
    if (PropTypes.TryGetValue(propertyName, out var type))
      return type;
    throw new ArgumentException("Property name not found.", nameof(propertyName));
  }

  /// <summary>
  /// Gets the value of an extended file property.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static object? GetValue (this DXEP.Properties extendedFileProperties, string propertyName)
  {
    switch(propertyName)
    {
       case "Application":
         return extendedFileProperties.GetFirstElementStringValue<DXEP.Application>();
      case "ApplicationVersion":
        return extendedFileProperties.GetFirstElementStringValue<DXEP.ApplicationVersion>();
      case "Characters":
         return extendedFileProperties.GetFirstElementIntValue<DXEP.Characters>(); 
       case "CharactersWithSpaces":
         return extendedFileProperties.GetFirstElementIntValue<DXEP.CharactersWithSpaces>();
       case "Company":
         return extendedFileProperties.GetFirstElementStringValue<DXEP.Company>();
      case "DigitalSignature":
         return extendedFileProperties.GetFirstElementVTBlobValue<DXEP.DigitalSignature>();
      case "DocumentSecurity":
         return extendedFileProperties.GetFirstElementStringValue<DXEP.DocumentSecurity>();
      case "HeadingPairs":
        return extendedFileProperties.GetFirstElementVTVectorValue<DXEP.HeadingPairs>();
      case "HiddenSlides":
         return extendedFileProperties.GetFirstElementIntValue<DXEP.HiddenSlides>(); ;
      case "HyperlinkBase":
        return extendedFileProperties.GetFirstElementStringValue<DXEP.HyperlinkBase>();
      case "HyperlinkList":
        return extendedFileProperties.GetFirstElementVTVectorValue<DXEP.HyperlinkList>();
      case "HyperlinksChanged":
         return extendedFileProperties.GetFirstElementBoolValue<DXEP.HyperlinksChanged>();
      case "Lines":
        return extendedFileProperties.GetFirstElementIntValue<DXEP.Lines>();
      case "LinksUpToDate":
        return extendedFileProperties.GetFirstElementBoolValue<DXEP.LinksUpToDate>();
      case "Manager":
         return extendedFileProperties.GetFirstElementStringValue<DXEP.Manager>();
      case "MultimediaClips":
        return extendedFileProperties.GetFirstElementIntValue<DXEP.MultimediaClips>();
      case "Notes":
        return extendedFileProperties.GetFirstElementIntValue<DXEP.Notes>();
      case "Pages":
        return extendedFileProperties.GetFirstElementIntValue<DXEP.Pages>();
      case "Paragraphs":
        return extendedFileProperties.GetFirstElementIntValue<DXEP.Paragraphs>();
      case "PresentationFormat":
         return extendedFileProperties.GetFirstElementStringValue<DXEP.PresentationFormat>();
      case "ScaleCrop":
        return extendedFileProperties.GetFirstElementBoolValue<DXEP.ScaleCrop>();
      case "SharedDocument":
        return extendedFileProperties.GetFirstElementBoolValue<DXEP.SharedDocument>();
      case "Slides":
         return extendedFileProperties.GetFirstElementIntValue<DXEP.Slides>();
      case "Template":
         return extendedFileProperties.GetFirstElementStringValue<DXEP.Template>();
      case "TitlesOfParts":
        return extendedFileProperties.GetFirstElementVTVectorValue<DXEP.TitlesOfParts>();
      case "TotalTime":
         return extendedFileProperties.GetFirstElementIntValue<DXEP.TotalTime>();
      case "Words":
        return extendedFileProperties.GetFirstElementIntValue<DXEP.Words>();
      default :
        return null;
    }
  }

  /// <summary>
  /// Sets the value of an extended file property.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <param name="value"></param>
  public static void SetValue(this DXEP.Properties extendedFileProperties, string propertyName, object? value)
  {
    switch (propertyName)
    {
      case "Application":
        extendedFileProperties.SetFirstElementStringValue<DXEP.Application>((string?)value);
        break;
      case "ApplicationVersion":
        extendedFileProperties.SetFirstElementStringValue<DXEP.ApplicationVersion>((string?)value);
        break;
      case "Characters":
        extendedFileProperties.SetFirstElementIntValue<DXEP.Characters>((int?)value);
        break;
      case "CharactersWithSpaces":
        extendedFileProperties.SetFirstElementIntValue<DXEP.CharactersWithSpaces>((int?)value);
        break;
      case "Company":
        extendedFileProperties.SetFirstElementStringValue<DXEP.Company>((string?)value);
        break;
      case "DigitalSignature":
        extendedFileProperties.SetFirstElementVTBlobValue<DXEP.DigitalSignature>((DXVT.VTBlob?)value);
        break;
      case "DocumentSecurity":
        extendedFileProperties.SetFirstElementIntValue<DXEP.DocumentSecurity>((int?)value);
        break;
      case "HeadingPairs":
        extendedFileProperties.SetFirstElementVTVectorValue<DXEP.HeadingPairs>((DXVT.VTVector?)value);
        break;
      case "HiddenSlides":
        extendedFileProperties.SetFirstElementIntValue<DXEP.HiddenSlides>((int?)value);
        break;
      case "HyperlinkBase":
        extendedFileProperties.SetFirstElementStringValue<DXEP.HyperlinkBase>((string?)value);
        break;
      case "HyperlinkList":
        extendedFileProperties.SetFirstElementVTVectorValue<DXEP.HyperlinkList>((DXVT.VTVector?)value);
        break;
      case "HyperlinksChanged":
        extendedFileProperties.SetFirstElementBoolValue<DXEP.HyperlinksChanged>((bool?)value);
        break;
      case "Lines":
        extendedFileProperties.SetFirstElementIntValue<DXEP.Lines>((int?)value);
        break;
      case "LinksUpToDate":
        extendedFileProperties.SetFirstElementBoolValue<DXEP.LinksUpToDate>((bool?)value);
        break;
      case "Manager":
        extendedFileProperties.SetFirstElementStringValue<DXEP.Manager>((string?)value);
        break;
      case "MultimediaClips":
        extendedFileProperties.SetFirstElementIntValue<DXEP.MultimediaClips>((int?)value);
        break;
      case "Notes":
        extendedFileProperties.SetFirstElementIntValue<DXEP.Notes>((int?)value);
        break;
      case "Pages":
        extendedFileProperties.SetFirstElementIntValue<DXEP.Pages>((int?)value);
        break;
      case "Paragraphs":
        extendedFileProperties.SetFirstElementIntValue<DXEP.Paragraphs>((int?)value);
        break;
      case "PresentationFormat":
        extendedFileProperties.SetFirstElementStringValue<DXEP.PresentationFormat>((string?)value);
        break;
      case "ScaleCrop":
        extendedFileProperties.SetFirstElementBoolValue<DXEP.ScaleCrop>((bool?)value);
        break;
      case "SharedDocument":
        extendedFileProperties.SetFirstElementBoolValue<DXEP.SharedDocument>((bool?)value);
        break;
      case "Slides":
        extendedFileProperties.SetFirstElementIntValue<DXEP.Slides>((int?)value);
        break;
      case "Template":
        extendedFileProperties.SetFirstElementStringValue<DXEP.Template>((string?)value);
        break;
      case "TitlesOfParts":
        extendedFileProperties.SetFirstElementVTVectorValue<DXEP.TitlesOfParts>((DXVT.VTVector?)value);
        break;
      case "TotalTime":
        extendedFileProperties.SetFirstElementIntValue<DXEP.TotalTime>((int?)value);
        break;
      case "Words":
        extendedFileProperties.SetFirstElementIntValue<DXEP.Words>((int?)value);
        break;
    }
  }

  private static readonly Dictionary<string, Type> PropTypes = new()
  {
    {"Application", typeof(String) },
    {"ApplicationVersion", typeof(String) },
    {"Characters", typeof(int) },
    {"CharactersWithSpaces", typeof(int) },
    {"Company", typeof(String) },
    {"DigitalSignature", typeof(DXVT.VTBlob) },
    {"DocumentSecurity", typeof(int) },
    {"HeadingPairs", typeof(DXVT.VTVector) },
    {"HiddenSlides", typeof(int) },
    {"HyperlinkBase", typeof(String) },
    {"HyperlinkList", typeof(DXVT.VTVector) },
    {"HyperlinksChanged", typeof(Boolean) },
    {"Lines", typeof(int) },
    {"LinksUpToDate", typeof(Boolean) },
    {"Manager", typeof(String) },
    {"MultimediaClips", typeof(int) },
    {"Notes", typeof(int) },
    {"Pages", typeof(int) },
    {"Paragraphs", typeof(int) },
    {"PresentationFormat", typeof(String) },
    {"ScaleCrop", typeof(Boolean) },
    {"SharedDocument", typeof(Boolean) },
    {"Slides", typeof(int) },
    {"Template", typeof(String) },
    {"TitlesOfParts", typeof(DXVT.VTVector) },
    {"TotalTime", typeof(int) },
    {"Words", typeof(int) },
  };

}
