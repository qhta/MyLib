using System;

namespace Qhta.WPF.Controls
{
  public class ValueChangedEventArgs<ValueType>: EventArgs
  {
    public ValueChangedEventArgs(ValueType newValue)
    {
      NewValue = newValue;
    }

    public ValueType NewValue { get; private set; }
  }

  public delegate void ValueChangedEventHandler<ValueType>(object sender, ValueChangedEventArgs<ValueType> args);
}
