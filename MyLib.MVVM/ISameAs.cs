using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{

  public interface ISameAs<T>
  {
    bool SameAs(T other);
  }
}
