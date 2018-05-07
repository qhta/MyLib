using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public class DataColumnViewModel: ViewModel
  {
    public string Property { get; set; }
    public string Header { get; set; }
    public Double MinWidth { get; set; }
    public Double MaxWidth { get; set; }
    public Double Width { get; set; }

    public static implicit operator DataColumnViewModel(string property)
    {
      return new DataColumnViewModel { Property = property, Header = property };
    }
  }
}
