using System;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace Qhta.WPF.Converters;

/// <summary>
/// Value converter that retrieves a localized resource string for an enum value.
/// </summary>
public class EnumToResourceConverter : IValueConverter
{
  /// <summary>
  /// Resource type that contains the localized strings for the enum values.
  /// </summary>
  public Type ResourceType { [DebuggerStepThrough] get; set; } = null!;

  /// <summary>
  /// Suffix to append to the enum value name when looking up the resource key.
  /// </summary>
  public string? Suffix { [DebuggerStepThrough] get; set; } = null!;

  private ResourceManager? resourceMan = null;
  private ResourceManager _resourceManager
  {
    get
    {
      if (object.ReferenceEquals(resourceMan, null))
      {
        var baseName = ResourceType.FullName!;
        resourceMan = new ResourceManager(baseName, ResourceType.Assembly);
      }
      return resourceMan;
    }
  }

  /// <summary>
  /// Converts an enum value to its corresponding localized resource string.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    // Get the resource key based on the enum value
    string? resourceKey = value?.ToString();
    if (resourceKey==null)
      return null;
    resourceKey += Suffix;

    // Retrieve the translation from the resource file

    string? translation = _resourceManager.GetString(resourceKey, culture);
    if (translation == null) 
      Debug.Assert(true);
    //Debug.WriteLine($"{resourceKey} [{culture}] = {translation}");
    return (translation ?? value?.ToString()); // Fallback to the enum value if no translation is found
  }

  /// <summary>
  /// Unimplemented method for converting back from the target type to the source type.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="targetType"></param>
  /// <param name="parameter"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException("ConvertBack is not supported.");
  }
}