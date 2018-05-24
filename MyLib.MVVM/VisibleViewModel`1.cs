using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public class VisibleViewModel<ItemType> : VisibleViewModel, IVisible, IExpandable, ISelectable
  {
    public VisibleViewModel()
    {
    }

    public VisibleViewModel(ViewModel parentViewModel)
    {
      ParentViewModel = parentViewModel;
    }

    public ViewModel ParentViewModel { get; private set; }

    public ItemType Model { get; set; }

  }
}
