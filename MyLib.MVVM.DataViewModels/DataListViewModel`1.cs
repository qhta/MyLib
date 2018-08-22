using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MyLib.MVVM
{
  public class DataListViewModel<ItemType>: ListViewModel<ItemType> where ItemType: class, IValidated, ISelectable
  {

  }
}
