using System;

namespace Qhta.OpenXmlTools;
/// <summary>
/// Manages the properties of a document in a uniform way.
/// </summary>
public class DocumentProperties
{
  /// <summary>
  /// Initializes a new instance of the DocumentProperties class.
  /// </summary>
  /// <param name="wordDoc"></param>
  public DocumentProperties(DXPack.WordprocessingDocument wordDoc)
  {
    CoreProperties = wordDoc.PackageProperties;
    ExtendedProperties = wordDoc.ExtendedFilePropertiesPart?.Properties;
    CustomProperties = wordDoc.CustomFilePropertiesPart?.Properties;
  }

  /// <summary>
  /// Holds the core properties of the document.
  /// </summary>
#pragma warning disable OOXML0001
  public DXPack.IPackageProperties CoreProperties { get; private set; }
#pragma warning restore OOXML0001

  /// <summary>
  /// Holds the extended properties of the document.
  /// </summary>
  public DXEP.Properties? ExtendedProperties { get; private set; }

  /// <summary>
  /// Holds the custom properties of the document.
  /// </summary>
  public DXCP.Properties? CustomProperties { get; private set; }

  /// <summary>
  /// Get the names of all the document properties.
  /// </summary>
  /// <returns></returns>
  public string[] GetNames()
  {
    List<string> names = new();
    names.AddRange(CoreProperties.GetNames());
    if (ExtendedProperties != null)
      names.AddRange(ExtendedProperties.GetNames());
    if (CustomProperties != null)
      names.AddRange(CustomProperties.GetNames());
    return names.ToArray();
  }


  /// <summary>
  /// Get the type of the document property
  /// </summary>
  /// <returns></returns>
  public Type GetType(string propName)
  {
    if (CoreProperties.GetNames().Contains(propName))
      return CoreProperties.GetType(propName);
    if (ExtendedProperties != null && ExtendedProperties.GetNames().Contains(propName))
      return ExtendedProperties.GetType(propName);
    if (CustomProperties != null)
      return CustomProperties.GetType(propName);
    throw new ArgumentException("Property name not found.", nameof(propName));
  }

  /// <summary>
  /// Gets the value of a document property.
  /// </summary>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public object? GetValue(string propertyName)
  {
    if (CoreProperties.GetNames().Contains(propertyName))
      return CoreProperties.GetValue(propertyName);
    if (ExtendedProperties != null && ExtendedProperties.GetNames().Contains(propertyName))
      return ExtendedProperties.GetValue(propertyName);
    if (CustomProperties != null)
      return CustomProperties.GetValue(propertyName);
    return null;
  }
}
