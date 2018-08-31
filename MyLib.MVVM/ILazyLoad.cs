using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public interface ILazyLoad
  {
    bool LazyLoad { get; set; }
    bool IsLoading { get; }
    bool IsLoaded { get; }

    void StartLoading();
  }
}
