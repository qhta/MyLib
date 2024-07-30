using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with custom file properties of a document.
/// </summary>
public static class CustomFileProperties
{
  /// <summary>
  /// Get the count of all the custom file properties.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <returns></returns>
#pragma warning disable OOXML0001
  public static int Count(this DXCP.Properties customFileProperties)
#pragma warning restore OOXML0001
    => customFileProperties.Elements<DXCP.CustomDocumentProperty>().Count();

  /// <summary>
  /// Get the names of all the custom file properties.
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
    if (property != null)
      return property.FirstChild?.GetType() ?? typeof(object);
    throw new ArgumentException($"Property {propertyName} not found");
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
        return value.GetVariantValue();
    }
    return null;
  }

  /// <summary>
  /// Sets the value of an custom file property.
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
        pid = (customFileProperties.Elements<DXCP.CustomDocumentProperty>().Max(item => item.PropertyId)?? 1) + 1;
      property = new DXCP.CustomDocumentProperty(element) { FormatId = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}", PropertyId = pid, Name = propertyName };
      customFileProperties.Append(property);
    }
  }


}
