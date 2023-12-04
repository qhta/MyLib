namespace Qhta.WPF.Utils;

/// <summary>
/// DynamicResourceExtension class that implements dynamic resource binding with optional converter (as normal Binding).
/// </summary>
public class DynamicResourceBinding : DynamicResourceExtension
{
  #region Internal Classes

  private class DynamicResourceBindingSource : Freezable
  {
    public static readonly DependencyProperty ResourceReferenceExpressionProperty = DependencyProperty.Register(
        nameof(ResourceReferenceExpression),
        typeof(object),
        typeof(DynamicResourceBindingSource),
        new FrameworkPropertyMetadata());

    public object ResourceReferenceExpression
    {
      get { return GetValue(ResourceReferenceExpressionProperty); }
      set { SetValue(ResourceReferenceExpressionProperty, value); }
    }

    protected override Freezable CreateInstanceCore()
    {
      return new DynamicResourceBindingSource();
    }
  }

  #endregion Internal Classes

  /// <summary>
  /// Default constructor.
  /// </summary>
  public DynamicResourceBinding() { }

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="resourceKey"></param>
  public DynamicResourceBinding(string resourceKey)
  : base(resourceKey) { }

  /// <summary>
  /// Optional converter of the resource value.
  /// </summary>
  public IValueConverter? Converter { get; set; }

  /// <summary>
  /// Optional parameter of the converter.
  /// </summary>
  public object? ConverterParameter { get; set; }

  /// <summary>
  /// Optional culture of the conversion.
  /// </summary>
  public CultureInfo? ConverterCulture { get; set; }

  /// <summary>
  /// String format to convert data.
  /// </summary>
  public string? StringFormat { get; set; }

  /// <summary>
  /// Provides resource value using service provider.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <returns></returns>
  public override object ProvideValue(IServiceProvider serviceProvider)
  {
    // Get the expression representing the DynamicResource
    var resourceReferenceExpression = base.ProvideValue(serviceProvider);

    // If there's no converter, nor StringFormat, just return it (Matches standard DynamicResource behavior}
    if (Converter == null && StringFormat == null)
      return resourceReferenceExpression;

    // Create the Freezable-based object and set its ResourceReferenceExpression property directly to the 
    // result of base.ProvideValue (held in resourceReferenceExpression). Then add it to the target FrameworkElement's
    // Resources collection (using itself as its key for uniqueness) so it participates in the resource lookup chain.
    var dynamicResourceBindingSource = new DynamicResourceBindingSource() { ResourceReferenceExpression = resourceReferenceExpression };

    // Get the target FrameworkElement so we have access to its Resources collection
    // Note: targetFrameworkElement may be null in the case of setters. Still trying to figure out how to handle them.
    // For now, they just fall back to looking up at the app level
    var targetInfo = (IProvideValueTarget?)serviceProvider.GetService(typeof(IProvideValueTarget));
    var targetFrameworkElement = targetInfo?.TargetObject as FrameworkElement;
    targetFrameworkElement?.Resources.Add(dynamicResourceBindingSource, dynamicResourceBindingSource);

    // Now since we have a source object which has a DependencyProperty that's set to the value of the
    // DynamicResource we're interested in, we simply use that as the source for a new binding,
    // passing in all of the other binding-related properties.
    var binding = new Binding()
    {
      Path               = new PropertyPath(DynamicResourceBindingSource.ResourceReferenceExpressionProperty),
      Source             = dynamicResourceBindingSource,
      Converter          = Converter,
      ConverterParameter = ConverterParameter,
      ConverterCulture   = ConverterCulture,
      StringFormat       = StringFormat,
      Mode               = BindingMode.OneWay
    };

    // Now we simply return the result of the new binding's ProvideValue
    // method (or the binding itself if the target is not a FrameworkElement)
    return (targetFrameworkElement != null)
        ? binding.ProvideValue(serviceProvider)
        : binding;
  }
}
