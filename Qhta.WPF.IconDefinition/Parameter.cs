using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Qhta.WPF
{
  [ContentProperty("Value")]
  public class Parameter: DependencyObject
  {
    #region Type property
    public Type Type;

    public static DependencyProperty TypeProperty = DependencyProperty.Register
      ("Type", typeof(Type), typeof(Parameter));
    #endregion

    #region Value property
    public object Value
    {
      get => GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
    }

    public static DependencyProperty ValueProperty = DependencyProperty.Register
      ("Value", typeof(Object), typeof(Parameter));
    #endregion
  }
}
