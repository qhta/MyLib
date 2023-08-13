namespace Qhta.WPF.Utils;

/// <summary>
/// Markup extension that joins input strings.
/// </summary>
[ContentProperty(nameof(Items))]
public class JoinStrings : MarkupExtension
{
  /// <summary>
  /// Separator set between input strings.
  /// </summary>
  public string? Separator { get; set; }

  /// <summary>
  /// Input strings.
  /// </summary>
  public List<string> Items { get; private set; } = new List<string>();

  /// <summary>
  /// Provides value with service provider.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <returns></returns>
  public override object ProvideValue(IServiceProvider serviceProvider)
  {
    return ToString();
  }

  /// <summary>
  /// Joins the strings.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return String.Join(Separator, Items);
  }
}
