using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib.OrdNumbers;

namespace MyLib.MVVM
{
  public interface INumbered
  {
    int Number { get; set; }

    OrdNum OrdNumber { get; }
  }
}
