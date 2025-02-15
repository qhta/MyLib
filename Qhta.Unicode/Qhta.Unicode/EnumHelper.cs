namespace Qhta.Unicode;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

/// <summary>
/// Helper class for working with enums
/// </summary>
public static class EnumHelper
{
  /// <summary>
  /// Parse an enum value from a string using EnumMemberAttribute or the field name
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="value"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  public static T? ParseEnum<T>(string value) where T : Enum
  {
    foreach (var field in typeof(T).GetFields())
    {
      if (Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute)) is EnumMemberAttribute attribute)
      {
        if (attribute.Value == value)
        {
          return (T?)field.GetValue(null);
        }
      }
      else if (field.Name == value)
      {
        return (T?)field.GetValue(null);
      }
    }
    throw new ArgumentException($"Unknown value: {value}", nameof(value));
  }
}
