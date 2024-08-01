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
  /// Get the count of the document properties.
  /// </summary>
  /// <param name="filter">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public int Count(ItemFilter filter = ItemFilter.Defined)
  {
    int count = 0;
    if (WordDoc.HasCoreProperties())
      count+= WordDoc.GetCoreProperties().Count(filter);
    if (WordDoc.HasExtendedFileProperties())
      count += WordDoc.GetExtendedFileProperties().Count(filter);
    if (WordDoc.HasCustomFileProperties())
      count += WordDoc.GetCustomFileProperties().Count();
    return count;
  }

  /// <summary>
  /// Get the names of the document properties.
  /// </summary>
  /// <param name="filter">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
  public string[] GetNames(ItemFilter filter = ItemFilter.Defined)
  {
    List<string> names = new();
    names.AddRange(WordDoc.GetCoreProperties().GetNames(filter));
    names.AddRange(WordDoc.GetExtendedFileProperties().GetNames(filter));
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
    if (CoreProperties.GetNames(ItemFilter.All).Contains(propName))
      return CoreProperties.GetType(propName);
    if (ExtendedProperties.GetNames(ItemFilter.All).Contains(propName))
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
    if (CoreProperties.GetNames(ItemFilter.All).Contains(propertyName))
      return CoreProperties.GetValue(propertyName);
    if (ExtendedProperties.GetNames(ItemFilter.All).Contains(propertyName))
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
    if (CoreProperties.GetNames(ItemFilter.All).Contains(propertyName))
      CoreProperties.SetValue(propertyName, value);
    else
    {
      if (ExtendedProperties.GetNames(ItemFilter.All).Contains(propertyName))
        ExtendedProperties.SetValue(propertyName, value);

      else
      {
        CustomProperties.SetValue(propertyName, value);
      }
    }
  }
}
