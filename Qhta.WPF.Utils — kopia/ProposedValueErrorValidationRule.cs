namespace Qhta.WPF.Utils;

/// <summary>
/// ValidationRule that checks a proposed property value.
/// </summary>
public class ProposedValueErrorValidationRule : ValidationRule
{
  private readonly DependencyObject targetObject;
  private readonly DependencyProperty targetProperty;

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="targetObject"></param>
  /// <param name="targetProperty"></param>
  /// <exception cref="ArgumentNullException"></exception>
  public ProposedValueErrorValidationRule(DependencyObject targetObject, DependencyProperty targetProperty)
      : base(ValidationStep.RawProposedValue, true)
  {
    if (targetObject == null)
      throw new ArgumentNullException("targetObject");
    if (targetProperty == null)
      throw new ArgumentNullException("targetProperty");

    this.targetObject = targetObject;
    this.targetProperty = targetProperty;
  }

  /// <summary>
  /// Defined Validate method.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="cultureInfo"></param>
  /// <returns></returns>
  public override ValidationResult Validate(object value, CultureInfo cultureInfo)
  {
    var expression = BindingOperations.GetBindingExpression(this.targetObject, this.targetProperty);
    if (expression != null)
    {
      var sourceItem = expression.DataItem as IProposedValueErrorInfo;
      if (sourceItem != null)
      {
        var propertyName = expression.ParentBinding.Path != null ? expression.ParentBinding.Path.Path : null;
        if (propertyName != null)
        {
          var error = sourceItem.GetError(propertyName, value, cultureInfo);
          if (error != null)
            return new ValidationResult(false, error);
        }
      }
    }
    return ValidationResult.ValidResult;
  }
}
