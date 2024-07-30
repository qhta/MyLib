namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for working with OpenXml documents.
/// </summary>
public static class DocumentTools
{
  /// <summary>
  /// Checks if the document has core properties.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static bool HasCoreProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.CoreFilePropertiesPart != null;
  }

  /// <summary>
  /// Gets the core properties of the document. If the document does not have core properties, they are created.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument to get the properties from.</param>
  /// <returns></returns>
  public static DXPack.IPackageProperties GetCoreProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.PackageProperties;
  }

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
    var part = wordDoc.ExtendedFilePropertiesPart;
    if (part == null)
    {
      part = wordDoc.AddExtendedFilePropertiesPart();
    }
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
  /// Checks if the document has custom file properties.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static bool HasCustomFileProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.CustomFilePropertiesPart?.Properties != null;
  }

  /// <summary>
  /// Gets the custom file properties of the document. If the document does not have custom file properties,
  /// they are created.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument to get the properties from.</param>
  /// <returns></returns>
  public static DXCP.Properties GetCustomFileProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    var part = wordDoc.CustomFilePropertiesPart;
    if (part == null)
    {
      part = wordDoc.AddCustomFilePropertiesPart();
    }
    var properties = part.Properties;
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (properties == null)
    {
      properties = new DXCP.Properties();
      part.Properties = properties;
    }
    return properties;
  }

  /// <summary>
  /// Gets all the properties of the document to manage them in a uniform way.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static DocumentProperties GetDocumentProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return new DocumentProperties (wordDoc);
  }
}
