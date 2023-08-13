namespace Qhta.WPF.Utils;

/// <summary>
/// Helper class that defines dummy object used in <see cref="BindingEvaluator"/>.
/// </summary>
public class DummyDO : FrameworkElement
{
  /// <summary>
  /// Dummy Value property
  /// </summary>
  public object Value
  {
    get { return (object)GetValue(ValueProperty); }
    set { SetValue(ValueProperty, value); }
  }

  /// <summary>
  /// Dependency property to store Value property.
  /// </summary>
  public static readonly DependencyProperty ValueProperty =
    DependencyProperty.Register("Value", typeof(object), typeof(DummyDO), new PropertyMetadata(null));
}

/// <summary>
/// Utility class to evaluate this Binding value. Defines GetValue methods.
/// It is used in ClipboardUtils.
/// </summary>
public static class BindingEvaluator
{
  /// <summary>
  /// Gets dummy object Value property for this Binding.
  /// </summary>
  /// <param name="b"></param>
  /// <returns></returns>
  public static object GetValue(this Binding b)
  {
    DummyDO d = new DummyDO();
    BindingOperations.SetBinding(d, DummyDO.ValueProperty, b);
    return d.Value;
  }

  /// <summary>
  /// Gets dummy object value for this Binding using source object.
  /// </summary>
  /// <param name="b"></param>
  /// <param name="sourceObject"></param>
  /// <returns></returns>
  public static object GetValue(this Binding b, object sourceObject)
  {
    Binding b1 = new Binding(b.Path.Path)
    {
      Source = sourceObject,
      Mode=BindingMode.OneTime,
      Converter = b.Converter,
      ConverterParameter = b.ConverterParameter,
      ConverterCulture = b.ConverterCulture,
      StringFormat = b.StringFormat
    };
    DummyDO tc = new DummyDO { DataContext = b1 };
    BindingOperations.SetBinding(tc, DummyDO.ValueProperty, b1);
    var value = tc.Value;
    return value;
  }

}

