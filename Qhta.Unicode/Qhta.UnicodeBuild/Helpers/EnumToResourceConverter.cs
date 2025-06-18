using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Windows.Data;
using Qhta.TextUtils;

namespace Qhta.UnicodeBuild.Helpers;

public class EnumToResourceConverter : IValueConverter
{
  public Type ResourceType { get; set; } = null!;
  private ResourceManager? resourceMan = null;
  private ResourceManager _resourceManager
  {
    get
    {
      if (object.ReferenceEquals(resourceMan, null))
      {
        var baseName = ResourceType.FullName!;
        ResourceManager temp = new ResourceManager(baseName, ResourceType.Assembly);
        resourceMan = temp;
      }
      return resourceMan;
    }
  }

  // Initialize the ResourceManager for the resource file

  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value == null) return null;

    // Get the resource key based on the enum value
    string? resourceKey = value.ToString()?.TitleCase();
    if (resourceKey==null)
      return null;
    if (parameter is string param)
    {
      resourceKey = $"{resourceKey}{param}";
    }
    // Retrieve the translation from the resource file

    string? translation = _resourceManager.GetString(resourceKey, culture);
    Debug.WriteLine($"{resourceKey} [{culture}] = {translation}");
    return (translation ?? value.ToString())?.ToLower(); // Fallback to the enum value if no translation is found
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException("ConvertBack is not supported.");
  }
}