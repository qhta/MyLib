using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public abstract class DataSetViewModel: DataViewModel
  {
    public virtual List<DataColumnViewModel> VisibleColumns { get; set; }

    public virtual bool CanExpandRows { get; set; }
  }
}
