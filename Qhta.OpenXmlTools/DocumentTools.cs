namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for working with OpenXml documents.
/// </summary>
public static class DocumentTools
{

  /// <summary>
  /// Gets the core properties of the document.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument to get the properties from.</param>
  /// <returns></returns>
  public static DXPack.IPackageProperties GetCoreProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.PackageProperties;
  }

  /// <summary>
  /// Gets the extended file properties of the document.
  /// </summary>
  /// <param name="wordDoc">The WordprocessingDocument to get the properties from.</param>
  /// <returns></returns>
  public static DXEP.Properties? GetExtendedFileProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.ExtendedFilePropertiesPart?.Properties;
  }

  /// <summary>
  /// Gets the custom file properties of the document.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static DXCP.Properties? GetCustomFileProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.CustomFilePropertiesPart?.Properties;
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
