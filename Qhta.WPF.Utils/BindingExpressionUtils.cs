namespace Qhta.WPF.Utils;

/// <summary>
/// Utility class for BindingExpressions. It defines GetString and RefreshBinding methods.
/// </summary>
public static class BindingExpressionUtils
{
  /// <summary>
  /// Gets a string value from this <see cref="BindingExpressionBase"/> expression DataItem property.
  /// </summary>
  /// <param name="expression"></param>
  /// <returns></returns>
  public static string? GetString(this BindingExpressionBase expression)
  {
    if (expression is BindingExpression expr)
    {
      var str = expr.DataItem?.ToString();
      return str;
    }
    return expression.GetType().Name;
  }

  /// <summary>
  /// Creates a new <see cref="BindingExpression"/> for the specific property of this object 
  /// </summary>
  /// <param name="obj">Must be a dependency object</param>
  /// <param name="property">Must be a dependency property</param>
  /// <param name="newDataItem">Defines source for new binding</param>
  /// <returns></returns>
  public static BindingExpression? RefreshBinding (this DependencyObject obj, DependencyProperty property, object newDataItem)
  {
    BindingExpression binding = BindingOperations.GetBindingExpression(obj, property);
    if (binding != null)
    {
      //Debug.WriteLine($"      binding.IsDirty={binding.IsDirty}");
      //Debug.WriteLine($"      binding.DataItem={binding.DataItem}");
      //Debug.WriteLine($"      BindingOperations.ClearBinding(this.ValueBox, TextBox.TextProperty)");
      BindingOperations.ClearBinding(obj, property);
      Binding newBinding = new Binding(binding.ParentBinding.Path.Path);
      newBinding.Source = newDataItem;
      newBinding.Converter = binding.ParentBinding.Converter;
      newBinding.ConverterParameter = binding.ParentBinding.ConverterParameter;
      BindingOperations.SetBinding(obj, property, newBinding);
      binding = BindingOperations.GetBindingExpression(obj, property);
      return binding;
    }
    return null;
  }
}
