using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public abstract class DataTreeViewModel: DataViewModel
  {
    public virtual bool CanExpandItems { get; set; }
  }
}
