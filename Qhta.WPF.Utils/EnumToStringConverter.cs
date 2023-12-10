namespace Qhta.WPF.Utils;

/// <summary>
/// Converter for enum names to localized strings.
/// </summary>
public sealed class EnumToStringConverter : IValueConverter
{
  /// <inheritdoc/>
  public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value == null)
    { return null; }

    var str = value.ToString()!;
    if (str!=null)
      return CommonStrings.ResourceManager.GetString(str) ?? str;
    return str;
  }

  /// <inheritdoc/>
  public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
  {
    string str = (string)value;

    if (targetType.IsNullable(out var baseType))
      targetType = baseType;
    foreach (object enumValue in Enum.GetValues(targetType))
    {
      var str1 = enumValue.ToString();
      if (str == str1 || str1!=null && str == (CommonStrings.ResourceManager.GetString(str1) ?? str1))
      { return enumValue; }
    }

    return null;
  }
}
