using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public static class BindingExpressionUtils
  {
    public static string GetString(this BindingExpressionBase expression)
    {
      if (expression is BindingExpression expr)
      {
        var str = expr.DataItem?.ToString();
        return str;
      }
      return expression.GetType().Name;
    }

    public static BindingExpression RefreshBinding (this DependencyObject obj, DependencyProperty property, object newDataItem)
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
}
