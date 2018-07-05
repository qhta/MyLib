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
using System.Windows.Threading;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public partial class ListViewModel<ItemType> : DispatchedCollection<ItemType>, INotifySelectionChanged, IViewModel, ISelectable
         where ItemType : class, IValidated, ISelectable
  {
    public ListViewModel()
    {
    }

    public ListViewModel(IViewModel parentViewModel) : this()
    {
      ParentViewModel = parentViewModel;
    }

    public IViewModel ParentViewModel { get; private set; }

    public ItemType SelectedItem
    {
      get
      {
        ItemType selectedItem = null;
        selectedItem = Values.ToList().FirstOrDefault(item => item.IsSelected);
        return selectedItem;
      }
      set
      {
        foreach (var item in Values.ToList())
          item.IsSelected = item.Equals(value);
      }
    }

    public int ValidItemsCount
    {
      get
      {
        return Values.ToList().Where(item => item.IsValid==true).Count();
      }
    }

    public int InvalidItemsCount
    {
      get
      {
        return Values.ToList().Where(item => item.IsValid==false).Count();
      }
    }

    protected override void AfterCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      base.AfterCollectionChanged(e);
      NotifyPropertyChanged(nameof(ValidItemsCount));
      NotifyPropertyChanged(nameof(InvalidItemsCount));
      if (e.Action==NotifyCollectionChangedAction.Add)
      {
        foreach (var item in e.NewItems)
        {
          if (item is INotifyPropertyChanged notifyPropertyChangedItem)
            notifyPropertyChangedItem.PropertyChanged+=Item_PropertyChanged;
          if (item is INumbered numberedItem)
            numberedItem.Number=Values.Count();
        }
      }
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "IsValid":
          NotifyPropertyChanged(nameof(ValidItemsCount));
          NotifyPropertyChanged(nameof(InvalidItemsCount));
          break;
        case "IsSelected":
          NotifySelectionChanged(sender);
          break;
        default:
          break;
      }
    }

    public void NotifySelectionChanged(object item)
    {
      List<object> selectedItems = new List<object>();
      List<object> unselectedItems = new List<object>();
      if (item is ISelectable selectable)
      {
        if (selectable.IsSelected)
          selectedItems.Add(item);
        else
          unselectedItems.Add(item);
      }
      OnSelectionChanged(new SelectionChangedEventArgs(selectedItems, unselectedItems));
    }

    public event SelectionChangedEventHandler SelectionChanged
    {
      add { _SelectionChanged+=value; }
      remove { _SelectionChanged-=value; }
    }
    protected event SelectionChangedEventHandler _SelectionChanged;

    public virtual void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      if (_SelectionChanged != null)
      {
        if (Dispatcher.CurrentDispatcher==DispatchedObject.ApplicationDispatcher)
        {
          _SelectionChanged.Invoke(this, e);
          AfterSelectionChanged(e);
        }
        else
        {
          var action = new Action<NotifyCollectionChangedEventArgs>(OnCollectionChanged);
          DispatchedObject.ApplicationDispatcher.Invoke(action, new object[] { e });
        }
      }
      else
        AfterSelectionChanged(e);
    }

    protected virtual void AfterSelectionChanged(SelectionChangedEventArgs e)
    {
      NotifyPropertyChanged(nameof(SelectedItem));
    }

    public ItemType CurrentItem
    {
      get => _CurrentItem;
      set
      {
        if (_CurrentItem!=value)
        {
          var oldValue = _CurrentItem;
          _CurrentItem=value;
          NotifyPropertyChanged(nameof(CurrentItem));
          if (CurrentItemChanged!=null)
            CurrentItemChanged(this, new CurrentItemChangedEventArgs(_CurrentItem, oldValue));
        }
      }
    }
    private ItemType _CurrentItem;

    public event CurrentItemChangedEventHandler CurrentItemChanged;

    public int SelectedIndex
    {
      get => _SelectedIndex;
      set
      {
        if (_SelectedIndex!=value)
        {
          _SelectedIndex=value;
          NotifyPropertyChanged(nameof(SelectedIndex));
          if (_SelectedIndex>=0)
            CurrentItem = Values.ToList()[_SelectedIndex];
        }
      }
    }

    public bool IsSelected
    {
      get => _IsSelected;
      set
      {
        if (_IsSelected!=value)
        {
          _IsSelected = value;
          NotifyPropertyChanged(nameof(IsSelected));
        }
      }
    }
    private bool _IsSelected = false;

    private int _SelectedIndex = -1;

  }
}
