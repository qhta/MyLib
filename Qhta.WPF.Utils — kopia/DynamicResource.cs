namespace Qhta.WPF.Utils;

/// <summary>
/// Markup extension class that defines a dynamic resource.
/// </summary>
public class DynamicResource : MarkupExtension
{
  /// <summary>
  /// Ininitalizing constructor.
  /// </summary>
  /// <param name="resourceName"></param>
  public DynamicResource(string resourceName)
  {
    ResourceName = resourceName;
  }

  /// <summary>
  /// Name of the resource.
  /// </summary>
  public string ResourceName { get; set; }

  /// <summary>
  /// Provides value of the resource using service provider.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <returns></returns>
  public override object? ProvideValue(IServiceProvider serviceProvider)
  {
    try
    {
      object result = Application.Current.Resources[ResourceName];
      return result;
    }
    catch
    {

    }
    return null;
  }

}
