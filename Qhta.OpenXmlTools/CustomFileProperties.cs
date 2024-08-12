using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with custom file properties of a document.
/// </summary>
public static class CustomFileProperties
{
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
  /// Get the count of the custom file properties.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <returns></returns>
#pragma warning disable OOXML0001
  public static int Count(this DXCP.Properties customFileProperties)
#pragma warning restore OOXML0001
    => customFileProperties.Elements<DXCP.CustomDocumentProperty>().Count();

  /// <summary>
  /// Get the names of the custom file properties.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <returns></returns>
  public static string[] GetNames(this DXCP.Properties customFileProperties)
    => customFileProperties.Elements<DXCP.CustomDocumentProperty>().Select(item => item.Name!.Value!.Trim()).ToArray();


  /// <summary>
  /// Get the type of property with its name.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static Type GetType(this DXCP.Properties customFileProperties, string propertyName)
  {
    var property = customFileProperties.Elements<DXCP.CustomDocumentProperty>()
      .FirstOrDefault(item => item.Name?.Value == propertyName);
    if (property == null)
      throw new ArgumentException($"Property {propertyName} not found");
    var type = property.FirstChild?.GetType();
    if (type == null)
      return typeof(object);
    return VTVariantTools.VTTypeToType[type];

  }

  /// <summary>
  /// Gets the value of a custom file property.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static object? GetValue(this DXCP.Properties customFileProperties, string propertyName)
  {
    var property = customFileProperties.Elements<DXCP.CustomDocumentProperty>().FirstOrDefault(item => item.Name?.Value == propertyName);
    if (property != null)
    {
      var value = property.FirstChild;
      if (value != null)
        try
        {
          return value.GetVariantValue();
        }
        catch
        {
          return null;
        }
    }
    return null;
  }

  /// <summary>
  /// Sets the value of a custom file property.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <param name="value"></param>
  public static void SetValue(this DXCP.Properties customFileProperties, string propertyName, object? value)
  {
    var property = customFileProperties.Elements<DXCP.CustomDocumentProperty>().FirstOrDefault(item => item.Name?.Value == propertyName);
    if (property != null)
    {
      property.FirstChild?.Remove();
      if (value != null)
      {
        var element = VTVariantTools.CreateVariant(value);
        property.AddChild(element);
      }
      else
      {
        customFileProperties.RemoveChild(property);
      }
    }
    else
    if (value != null)
    {
      var element = VTVariantTools.CreateVariant(value);
      var pid = 2;
      if (customFileProperties.Any())
        pid = (customFileProperties.Elements<DXCP.CustomDocumentProperty>().Max(item => item.PropertyId) ?? 1) + 1;
      property = new DXCP.CustomDocumentProperty(element) { FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}", PropertyId = pid, Name = propertyName };
      customFileProperties.Append(property);
    }
  }

  /// <summary>
  /// Add the new custom file property.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <param name="propertyType"></param>
  /// <param name="value"></param>
  /// <returns>true if the addition was successful, false if the property with the same name already exists</returns>
  public static bool Add(this DXCP.Properties customFileProperties, string propertyName, Type propertyType, object? value = null)
  {
    var property = customFileProperties.Elements<DXCP.CustomDocumentProperty>().FirstOrDefault(item => item.Name?.Value == propertyName);
    if (property != null)
      return false;

    var element = VTVariantTools.CreateVariant(propertyType, value);
    var pid = 2;
    if (customFileProperties.Any())
      pid = (customFileProperties.Elements<DXCP.CustomDocumentProperty>().Max(item => item.PropertyId) ?? 1) + 1;
    property = new DXCP.CustomDocumentProperty(element) { FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}", PropertyId = pid, Name = propertyName };
    customFileProperties.Append(property);
    return true;

  }

  /// <summary>
  /// Removes a custom file property.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns>true if the addition was successful, false if the property with the given name does not exist</returns>
  public static bool Remove(this DXCP.Properties customFileProperties, string propertyName)
  {
    var property = customFileProperties.Elements<DXCP.CustomDocumentProperty>().FirstOrDefault(item => item.Name?.Value == propertyName);
    if (property == null)
      return false;

    property.Remove();
    return true;
  }

  /// <summary>
  /// Renames a custom file property.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <param name="newName"></param>
  /// <returns>true if the addition was successful, false if the property with the given name does not exist</returns>
  public static bool Rename(this DXCP.Properties customFileProperties, string propertyName, string newName)
  {
    var property = customFileProperties.Elements<DXCP.CustomDocumentProperty>().FirstOrDefault(item => item.Name?.Value == propertyName);
    if (property == null)
      return false;

    property.Name = newName;
    return true;
  }


  /// <summary>
  /// Changes a custom file property type.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <param name="newType"></param>
  /// <returns>true if the addition was successful, false if the property with the given name does not exist</returns>
  public static bool ChangeType(this DXCP.Properties customFileProperties, string propertyName, Type newType)
  {
    var property = customFileProperties.Elements<DXCP.CustomDocumentProperty>().FirstOrDefault(item => item.Name?.Value == propertyName);
    if (property == null)
      return false;

    var value = property.FirstChild?.GetVariantValue();
    var element = VTVariantTools.CreateVariant(newType, value);
    property.FirstChild?.Remove();
    property.AddChild(element);
    return true;
  }

  /// <summary>
  /// Removes all custom file properties.
  /// </summary>
  /// <param name="customFileProperties"></param>
  public static void Clear(this DXCP.Properties customFileProperties)
  {
    customFileProperties.RemoveAllChildren();
  }
}
