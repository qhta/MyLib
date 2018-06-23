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
         //, INotifyPropertyChanged
         where ItemType : class, IValidated, ISelectable
  {
    public ListViewModel()
    {
    //  _Items.CollectionChanged+=_Items_CollectionChanged;
    //  _Items.PropertyChanged+=_Items_PropertyChanged;
    }

    public ListViewModel(ViewModel parentViewModel): this()
    {
      ParentViewModel = parentViewModel;
    }

    public ViewModel ParentViewModel;

    public ItemType SelectedItem
    {
      get
      {
        ItemType selectedItem = null;
        //try
        //{
        //  selectedItem = Items.ToList().Where(item => item.IsSelected).FirstOrDefault();
        //}
        //catch
        //{

        //}
        return selectedItem;
      }
      //set
      //{
      //  //try
      //  //{
      //  //  foreach (var item in Items.ToList())
      //  //    item.IsSelected = item.Equals(value);
      //  ////  //NotifyPropertyChanged("SelectedItem");
      //  ////  //NotifySelectionChanged();
      //  //}
      //  //catch
      //  //{

      //  //}
      //}
    }

  }
}
