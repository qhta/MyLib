using System;

using DocumentFormat.OpenXml.Spreadsheet;

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
    WordDoc = wordDoc;
    CoreProperties = wordDoc.PackageProperties;
    ExtendedProperties = wordDoc.ExtendedFilePropertiesPart?.Properties;
    CustomProperties = wordDoc.CustomFilePropertiesPart?.Properties;
  }

  /// <summary>
  /// Holds the WordprocessingDocument object.
  /// </summary>
  public DXPack.WordprocessingDocument WordDoc { get; private set; }

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
  /// Get the count of all the document properties.
  /// </summary>
  /// <param name="all">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public int Count(bool all = false)
  {
    int count = CoreProperties.Count(all);
    if (ExtendedProperties != null)
      count += ExtendedProperties.Count(all);
    if (CustomProperties != null)
      count += CustomProperties.Count();
    return count;
  }

  /// <summary>
  /// Get the names of all the document properties.
  /// </summary>
  /// <param name="all">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public string[] GetNames(bool all = false)
  {
    List<string> names = new();
    names.AddRange(CoreProperties.GetNames(all));
    if (ExtendedProperties != null)
      names.AddRange(ExtendedProperties.GetNames(all));
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
    if (CoreProperties.GetNames(true).Contains(propName))
      return CoreProperties.GetType(propName);
    if (ExtendedProperties != null && ExtendedProperties.GetNames(true).Contains(propName))
      return ExtendedProperties.GetType(propName);
    if (CustomProperties != null)
    {
      var vType = CustomProperties.GetType(propName);
      return VTVariantTools.VTTypeToType.TryGetValue(vType, out var aType) ? aType : vType;
    }
    throw new ArgumentException("Property name not found.", nameof(propName));
  }

  /// <summary>
  /// Gets the value of a document property.
  /// </summary>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public object? GetValue(string propertyName)
  {
    if (CoreProperties.GetNames(true).Contains(propertyName))
      return CoreProperties.GetValue(propertyName);
    if (ExtendedProperties != null && ExtendedProperties.GetNames(true).Contains(propertyName))
      return ExtendedProperties.GetValue(propertyName);
    if (CustomProperties != null)
      return CustomProperties.GetValue(propertyName);
    return null;
  }

  /// <summary>
  /// Sets the value of a document property.
  /// </summary>
  /// <param name="propertyName"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public void SetValue(string propertyName, object? value)
  {
    if (CoreProperties.GetNames(true).Contains(propertyName))
      CoreProperties.SetValue(propertyName, value);
    else
    {
      if (ExtendedProperties == null)
      {
        var extendedFilePropertiesPart = WordDoc.AddExtendedFilePropertiesPart();
        var appProperties = new DocumentFormat.OpenXml.ExtendedProperties.Properties();
        extendedFilePropertiesPart.Properties = appProperties;
        ExtendedProperties = appProperties;
      }
      if (ExtendedProperties.GetNames(true).Contains(propertyName))
        ExtendedProperties.SetValue(propertyName, value);

      else
      {
        if (CustomProperties == null)
        {
          var customFilePropertiesPart = WordDoc.AddCustomFilePropertiesPart();
          var customProperties = new DocumentFormat.OpenXml.CustomProperties.Properties();
          customFilePropertiesPart.Properties = customProperties;
          CustomProperties = customProperties;
        }
        CustomProperties.SetValue(propertyName, value);
      }
    }
  }
}
