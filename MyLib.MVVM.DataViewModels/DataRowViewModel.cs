using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public class DataRowViewModel: ViewModel
  {
    public virtual bool HasRowDetails => false;
  }
}
