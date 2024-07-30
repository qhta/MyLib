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
  }

  /// <summary>
  /// Holds the WordprocessingDocument object.
  /// </summary>
  public DXPack.WordprocessingDocument WordDoc { get; private set; }

  DXPack.IPackageProperties CoreProperties => WordDoc.GetCoreProperties();
  DXEP.Properties ExtendedProperties => WordDoc.GetExtendedFileProperties();
  DXCP.Properties CustomProperties => WordDoc.GetCustomFileProperties();

  /// <summary>
  /// Get the count of all the document properties.
  /// </summary>
  /// <param name="all">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public int Count(bool all = false)
  {
    int count = 0;
    if (WordDoc.HasCoreProperties())
      count+= WordDoc.GetCoreProperties().Count(all);
    if (WordDoc.HasExtendedFileProperties())
      count += WordDoc.GetExtendedFileProperties().Count(all);
    if (WordDoc.HasCustomFileProperties())
      count += WordDoc.GetCustomFileProperties().Count();
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
    names.AddRange(WordDoc.GetCoreProperties().GetNames(all));
    names.AddRange(WordDoc.GetExtendedFileProperties().GetNames(all));
    if (WordDoc.HasCustomFileProperties())
      names.AddRange(WordDoc.GetCustomFileProperties().GetNames());
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
    if (ExtendedProperties.GetNames(true).Contains(propName))
      return ExtendedProperties.GetType(propName);
    var vType = CustomProperties.GetType(propName);
    return VTVariantTools.VTTypeToType.TryGetValue(vType, out var aType) ? aType : vType;
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
    if (ExtendedProperties.GetNames(true).Contains(propertyName))
      return ExtendedProperties.GetValue(propertyName);
    return CustomProperties.GetValue(propertyName);
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
      if (ExtendedProperties.GetNames(true).Contains(propertyName))
        ExtendedProperties.SetValue(propertyName, value);

      else
      {
        CustomProperties.SetValue(propertyName, value);
      }
    }
  }
}
