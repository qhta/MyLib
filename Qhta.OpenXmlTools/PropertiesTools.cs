using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Methods for working with properties.
/// </summary>
public static class PropertiesTools
{
  /// <summary>
  /// Determines if a property is obsolete.
  /// </summary>
  /// <param name="property"></param>
  /// <returns></returns>
  public static bool IsObsolete(this PropertyInfo property)
  {
    return property.GetCustomAttribute<ObsoleteAttribute>() != null;
  }
}
