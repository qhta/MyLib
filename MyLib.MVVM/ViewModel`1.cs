using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public class ViewModel<ItemType>: ViewModel
  {
    public ItemType Model { get; set; }
  }
}
