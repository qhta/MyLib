using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Filter for extended file properties by their application type.
/// </summary>
[Flags]
public enum AppType
{
  /// <summary>
  /// Applies to no application.
  /// </summary>
  None = 0,
  /// <summary>
  /// Applies to Word documents.
  /// </summary>
  Word = 1,
  /// <summary>
  /// Applies to presentations document.
  /// </summary>
  Presentation = 2,
  /// <summary>
  /// Applies to all applications.
  /// </summary>
  All = 3,
}

/// <summary>
/// Tools for working with extended file properties of a document.
/// </summary>
public static class ExtendedFilePropertiesTools
{

  /// <summary>
  /// Checks if the document has extended file properties.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static bool HasExtendedFileProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.ExtendedFilePropertiesPart?.Properties != null;
  }

  /// <summary>
  /// Gets the extended file properties of the document. If the document does not have extended file properties,
  /// they are created.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument to get the properties from.</param>
  /// <returns></returns>
  public static DXEP.Properties GetExtendedFileProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    var part = wordDoc.ExtendedFilePropertiesPart ?? wordDoc.AddExtendedFilePropertiesPart();
    var properties = part.Properties;
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (properties == null)
    {
      properties = new DXEP.Properties();
      part.Properties = properties;
    }
    return properties;
  }

  /// <summary>
  /// Get the count of the extended file properties.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <param name="filter">specifies if all property names should be counted or non-empty ones</param>
  /// <returns></returns>
  public static int Count(this DXEP.Properties extendedFileProperties, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.All)
      return PropTypes.Count;
    return PropTypes.Count(item => extendedFileProperties.GetValue(item.Key) != null);
  }

  /// <summary>
  /// Get the names of the extended file properties.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <param name="filter">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public static string[] GetNames(this DXEP.Properties extendedFileProperties, ItemFilter filter = ItemFilter.Defined)
  {
    if (filter == ItemFilter.All)
      return PropTypes.Keys.ToArray();
    return PropTypes.Where(item => extendedFileProperties.GetValue(item.Key) != null).Select(item => item.Key).ToArray();
  }

  /// <summary>
  /// Get the type of property with its name.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static Type GetType(this DXEP.Properties extendedFileProperties, string propertyName)
  {
    if (PropTypes.TryGetValue(propertyName, out var info))
      return info.type;
    throw new ArgumentException($"Property {propertyName} not found");
  }

  /// <summary>
  /// Check if the property is volatile.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static bool IsVolatile(this DXEP.Properties extendedFileProperties, string propertyName)
  {
    if (PropTypes.TryGetValue(propertyName, out var info))
      return info.isVolatile;
    throw new ArgumentException($"Property {propertyName} not found");
  }

  /// <summary>
  /// Check if the property is volatile.
  /// </summary>
  /// <param name="extendedFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <param name="appType"></param>
  /// <returns></returns>
  public static bool AppliesToApplication(this DXEP.Properties extendedFileProperties, string propertyName, AppType appType)
  {
    if (PropTypes.TryGetValue(propertyName, out var info))
      return (info.appType & appType)!=0;
    throw new ArgumentException($"Property {propertyName} not found");
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

  private static readonly Dictionary<string, (Type type, bool isVolatile, AppType appType)> PropTypes = new()
  {
    {"Application", (typeof(String), false, AppType.All) },
    {"ApplicationVersion", (typeof(String), false, AppType.All) },
    {"Company", (typeof(String), false, AppType.All) },
    {"Manager", (typeof(String), false, AppType.All) },
    {"SharedDocument", (typeof(bool), false, AppType.All) },
    {"DigitalSignature", (typeof(DXVT.VTBlob), false, AppType.All) },
    {"DocumentSecurity", (typeof(int), false, AppType.All) },

    {"HyperlinkBase", (typeof(String), false, AppType.All) },
    {"HyperlinkList", (typeof(DXVT.VTVector), false, AppType.All) },
    {"HyperlinksChanged", (typeof(bool), false, AppType.All) },
    {"LinksUpToDate", (typeof(bool), false, AppType.All) },

    {"ScaleCrop", (typeof(bool), false, AppType.All) },
    {"Template", (typeof(String), false, AppType.All) },
    {"HeadingPairs", (typeof(DXVT.VTVector), false, AppType.None) },
    {"TitlesOfParts", (typeof(DXVT.VTVector), false, AppType.None) },
    {"PresentationFormat", (typeof(String), false, AppType.Presentation) },

    {"TotalTime", (typeof(int), true, AppType.All) },
    {"Words", (typeof(int), true, AppType.All) },
    {"Pages", (typeof(int), true, AppType.Word) },
    {"Paragraphs", (typeof(int), true, AppType.Word) },
    {"Lines", (typeof(int), true, AppType.All) },
    {"Characters", (typeof(int), true, AppType.All) },
    {"CharactersWithSpaces", (typeof(int), true, AppType.All) },
    {"Slides", (typeof(int), true, AppType.Presentation) },
    {"HiddenSlides", (typeof(int), true, AppType.Presentation) },
    {"Notes", (typeof(int), true, AppType.Presentation) },
    {"MultimediaClips", (typeof(int), true, AppType.All) },

  };

}
