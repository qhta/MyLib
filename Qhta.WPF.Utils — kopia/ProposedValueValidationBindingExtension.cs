namespace Qhta.WPF.Utils;

/// <summary>
/// Markup extension that defines proposed value validation binding.
/// </summary>
public sealed class ProposedValueValidationBindingExtension : MarkupExtension
{
  private readonly Binding binding;

  /// <summary>
  /// Initialization constructor.
  /// </summary>
  /// <param name="binding"></param>
  /// <exception cref="ArgumentNullException"></exception>
  public ProposedValueValidationBindingExtension(Binding binding)
  {
    if (binding == null)
      throw new ArgumentNullException("binding");

    this.binding = binding;
  }

  /// <summary>
  /// Provides value using service provider.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <returns></returns>
  public override object ProvideValue(IServiceProvider serviceProvider)
  {
    var provideValueTarget = serviceProvider != null ? serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget : null;
    if (provideValueTarget != null 
      && provideValueTarget.TargetObject is DependencyObject dependencyTargetObject 
      && provideValueTarget.TargetProperty is DependencyProperty dependencyTargetProperty)
      this.binding.ValidationRules.Add(new ProposedValueErrorValidationRule(dependencyTargetObject, dependencyTargetProperty));

    return this.binding.ProvideValue(serviceProvider);
  }
}
