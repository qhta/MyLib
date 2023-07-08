using System.Windows;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{

  public class DummyDO : FrameworkElement
  {
    public object Value
    {
      get { return (object)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }
    public static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register("Value", typeof(object), typeof(DummyDO), new PropertyMetadata(null));
  }

  public static class BindingEvaluator
  {
    public static object GetValue(this Binding b)
    {
      DummyDO d = new DummyDO();
      BindingOperations.SetBinding(d, DummyDO.ValueProperty, b);
      return d.Value;
    }

    public static object GetValue(this Binding b, object myObject)
    {
      Binding b1 = new Binding(b.Path.Path)
      {
        Source = myObject,
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

}

