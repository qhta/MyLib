using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public class ListViewModel<ItemType> : DispatchedCollection<ItemType>
         where ItemType : class, IValidated, ISelectable
  {
    public ListViewModel()
    {
    }

    public ListViewModel(ViewModel parentViewModel): this()
    {
      ParentViewModel = parentViewModel;
    }

    public ViewModel ParentViewModel;

    public IEnumerable<ItemType> Items => this;

    public ItemType SelectedItem
    {
      get
      {
        ItemType selectedItem = null;
        selectedItem = Items.ToList().Where(item => item.IsSelected).FirstOrDefault();
        return selectedItem;
      }
      set
      {
        try
        {
          foreach (var item in Items.ToList())
            item.IsSelected = item.Equals(value);
        }
        catch
        {

        }
      }
    }

  }
}
