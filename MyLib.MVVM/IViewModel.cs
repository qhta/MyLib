using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public interface IViewModel: INotifyPropertyChanged
  {
    void NotifyPropertyChanged(string propertyName);

    bool IsValid { get; }
  }
}
