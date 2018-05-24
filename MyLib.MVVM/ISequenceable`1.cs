using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public interface ISequenceable<T> where T: class
  {
    T Next { get; set; }
    T Prior { get; set; }
  }
}
