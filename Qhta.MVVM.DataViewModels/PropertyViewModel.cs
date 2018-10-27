using System;
using System.Reflection;

namespace Qhta.MVVM
{
  public class PropertyViewModel: VisibleViewModel<PropertyInfo>
  {
    public string Name => Model.Name;
    public Type Type => Model.PropertyType;
    public object Instance { get; set; }
    public object Value
    {
      get
      {
        return Model.GetValue(Instance, new object[0]);
      }
    }
  }
}
