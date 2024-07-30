using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with core properties of a document.
/// </summary>
public static class CorePropertiesTools
{
  /// <summary>
  /// Get the count of all the core properties.
  /// </summary>
  /// <param name="coreProperties"></param>
  /// <param name="all">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
#pragma warning disable OOXML0001
  public static int Count(this DXPack.IPackageProperties coreProperties, bool all = false)
#pragma warning restore OOXML0001
  {
    if (all)
      return PropTypes.Count;
    return PropTypes.Count(item => coreProperties.GetValue(item.Key) != null);
  }


  /// <summary>
  /// Get the names of all the core properties.
  /// </summary>
  /// <param name="coreProperties"></param>
  /// <param name="all">specifies if all property names should be listed or non-empty ones</param>
  /// <returns></returns>
#pragma warning disable OOXML0001
  public static string[] GetNames(this DXPack.IPackageProperties coreProperties, bool all = false)
#pragma warning restore OOXML0001
  {
    if (all)
      return PropTypes.Keys.ToArray();
    return PropTypes.Where(item => coreProperties.GetValue(item.Key) != null).Select(item => item.Key).ToArray();
  }

  /// <summary>
  /// Get the type of property with its name.
  /// </summary>
  /// <param name="coreProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
#pragma warning disable OOXML0001
  public static Type GetType(this DXPack.IPackageProperties coreProperties, string propertyName)
#pragma warning restore OOXML0001
  {
    if (PropTypes.TryGetValue(propertyName, out var type))
      return type;
    throw new ArgumentException($"Property {propertyName} not found");
  }

  /// <summary>
  /// Gets the value of a core property.
  /// </summary>
  /// <param name="coreProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
#pragma warning disable OOXML0001
  public static object? GetValue(this DXPack.IPackageProperties coreProperties, string propertyName)
#pragma warning restore OOXML0001
  {
    switch (propertyName)
    {
      case "Title":
        return coreProperties.Title;
      case "Subject":
        return coreProperties.Subject;
      case "Creator":
        return coreProperties.Creator;
      case "Description":
        return coreProperties.Description;
      case "Keywords":
        return coreProperties.Keywords;
      case "Category":
        return coreProperties.Category;
      case "ContentStatus":
        return coreProperties.ContentStatus;
      case "ContentType":
        return coreProperties.ContentType;
      case "Created":
        return coreProperties.Created;
      case "Modified":
        return coreProperties.Modified;
      case "LastModifiedBy":
        return coreProperties.LastModifiedBy;
      case "Revision":
        return coreProperties.Revision;
      case "Version":
        return coreProperties.Version;
      case "LastPrinted":
        return coreProperties.LastPrinted;
      case "Language":
        return coreProperties.Language;
      case "Identifier":
        return coreProperties.Identifier;
      default:
        return null;
    }
  }

  /// <summary>
  /// Sets the value of a core property.
  /// </summary>
  /// <param name="coreProperties"></param>
  /// <param name="propertyName"></param>
  /// <param name="value"></param>
#pragma warning disable OOXML0001
  public static void SetValue(this DXPack.IPackageProperties coreProperties, string propertyName, object? value)
#pragma warning restore OOXML0001
  {
    switch (propertyName)
    {
      case "Title":
        coreProperties.Title = (string?)value;
        break;
      case "Subject":
        coreProperties.Subject = (string?)value;
        break;
      case "Creator":
        coreProperties.Creator = (string?)value;
        break;
      case "Description":
        coreProperties.Description = (string?)value;
        break;
      case "Keywords":
        coreProperties.Keywords = (string?)value;
        break;
      case "Category":
        coreProperties.Category = (string?)value;
        break;
      case "ContentStatus":
        coreProperties.ContentStatus = (string?)value;
        break;
      case "ContentType":
        coreProperties.ContentType = (string?)value;
        break;
      case "Created":
        coreProperties.Created = (DateTime?)value;
        break;
      case "Modified":
        coreProperties.Modified = (DateTime?)value;
        break;
      case "LastModifiedBy":
        coreProperties.LastModifiedBy = (string?)value;
        break;
      case "Revision":
        coreProperties.Revision = value?.ToString();
        break;
      case "Version":
        coreProperties.Version = (string?)value;
        break;
      case "LastPrinted":
        coreProperties.LastPrinted = (DateTime?)value;
        break;
      case "Language":
        coreProperties.Language = (string?)value;
        break;
      case "Identifier":
        coreProperties.Identifier = (string?)value;
        break;
    }
  }

  private static readonly Dictionary<string, Type> PropTypes = new()
  {
    {"Category", typeof(String) },
    {"ContentStatus", typeof(String) },
    {"ContentType", typeof(String) },
    {"Created", typeof(DateTime) },
    {"Creator", typeof(String) },
    {"Description", typeof(String) },
    {"Identifier", typeof(String) },
    {"Keywords", typeof(String) },
    {"Language", typeof(String) },
    {"LastModifiedBy", typeof(String) },
    {"LastPrinted", typeof(DateTime) },
    {"Modified", typeof(DateTime) },
    {"Revision", typeof(int) },
    {"Subject", typeof(String) },
    {"Title", typeof(String) },
    {"Version", typeof(String) },
  };

}
