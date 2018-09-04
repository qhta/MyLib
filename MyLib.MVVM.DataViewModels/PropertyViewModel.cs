using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MyLib.MVVM
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
