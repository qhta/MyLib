namespace Qhta.WPF.Utils;

/// <summary>
/// Markup extension to be used to show localized enum names.
/// </summary>
public sealed class EnumerateExtension : MarkupExtension
{
  /// <summary>
  /// Enum type to get names
  /// </summary>
  public Type Type { get; set; }

  /// <summary>
  /// Initializing constructor
  /// </summary>
  /// <param name="type"></param>
  public EnumerateExtension(Type type)
  {
    this.Type = type;
  }

  /// <summary>
  /// Function to provide enum names.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <returns></returns>
  public override object ProvideValue(IServiceProvider serviceProvider)
  {
    string[] names = Enum.GetNames(Type);
    string[] values = new string[names.Length];

    for (int i = 0; i < names.Length; i++)
    {
      values[i] = CommonStrings.ResourceManager.GetString(names[i]) ?? names[i];
    }

    return values;
  }
}
