using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with custom file properties of a document.
/// </summary>
public static class CustomFileProperties
{
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
    throw new ArgumentException("Property name not found.", nameof(propertyName));
  }

  /// <summary>
  /// Gets the value of a custom file property.
  /// </summary>
  /// <param name="customFileProperties"></param>
  /// <param name="propertyName"></param>
  /// <returns></returns>
  public static object? GetValue (this DXCP.Properties customFileProperties, string propertyName)
  { 
    var property = customFileProperties.Elements<DXCP.CustomDocumentProperty>().FirstOrDefault(item => item.Name?.Value == propertyName);
    if (property != null)
    {
      var value = property.FirstChild;
      if (value != null)
        value.GetVariantValue();
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
        if (value !=null)
        {
          var element = VTVariantTools.CreateVariant(value);
          property.AddChild(element);
        }
        else
        {
          customFileProperties.RemoveChild(property);
        }
      }
      if (value != null)
      {
        var element = VTVariantTools.CreateVariant(value);
        property = new DXCP.CustomDocumentProperty(element);
        customFileProperties.AppendChild(property);
      }
  }


}
