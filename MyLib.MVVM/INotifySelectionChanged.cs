using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace MyLib.MVVM
{
  public interface INotifySelectionChanged
  {
    event SelectionChangedEventHandler SelectionChanged;
  }
}
