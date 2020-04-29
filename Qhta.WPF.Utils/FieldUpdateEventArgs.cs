using System.ComponentModel;
using System.Reflection;

namespace Qhta.WPF.Utils
{
  public class FieldUpdateEventArgs: CancelEventArgs
  {
    public FieldUpdateEventArgs(object targetObject, PropertyInfo property, object newValue, object oldValue)
    {
      TargetObject = targetObject;
      Property = property;
      NewValue = newValue;
      OldValue = oldValue;
    }

    public object TargetObject { get; set; }
    public PropertyInfo Property { get; private set; }
    public object NewValue { get; private set; }
    public object OldValue { get; private set; }
  }
}
