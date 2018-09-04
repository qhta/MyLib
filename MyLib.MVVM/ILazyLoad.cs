using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public interface ILazyLoad
  {
    bool LoadOnInit { get; set; }
    bool IsLoading { get; }
    bool IsLoaded { get; }

    Task StartLoading();
  }
}
