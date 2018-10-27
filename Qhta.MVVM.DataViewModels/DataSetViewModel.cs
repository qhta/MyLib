using System.Collections.Generic;

namespace Qhta.MVVM
{
  public abstract class DataSetViewModel: DataViewModel
  {
    public virtual List<DataColumnViewModel> VisibleColumns { get; set; }

    public virtual bool CanExpandRows { get; set; }
  }
}
