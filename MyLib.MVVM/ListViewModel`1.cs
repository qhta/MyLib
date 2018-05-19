using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public class ListViewModel<ItemType> : ListViewModel, IEnumerable<ItemType>, INotifyCollectionChanged, INotifySelectionChanged
    where ItemType: IValidated, ISelectable
  {
    public ListViewModel()
    {
      _Items.CollectionChanged+=_Items_CollectionChanged;
      _Items.PropertyChanged+=_Items_PropertyChanged;
    }


    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add
      {
        ((INotifyCollectionChanged)Items).CollectionChanged+=value;
      }

      remove
      {
        ((INotifyCollectionChanged)Items).CollectionChanged-=value;
      }
    }

    private void _Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      NotifyPropertyChanged("Count");
    }

    private void _Items_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyPropertyChanged(e.PropertyName);
    }


    public DispatchedCollection<ItemType> Items => _Items;
    protected DispatchedCollection<ItemType> _Items = new DispatchedCollection<ItemType>();

    public event NotifySelectionChangedEventHandler SelectionChanged;

    public override bool IsValid => Items!=null && Items.Where(item => !item.IsValid).FirstOrDefault()==null;

    public IEnumerator<ItemType> GetEnumerator()
    {
      return ((IEnumerable<ItemType>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<ItemType>)Items).GetEnumerator();
    }

    public int Count => Items.Count;

    public IEnumerable<ItemType> SelectedItems
    {
      get
      {
        return _Items.Where(item => item.IsSelected) ?? new ItemType[0];
      }
      set
      {
        List<ItemType> unselectedItems = new List<ItemType>();
        List<ItemType> selectedItems = new List<ItemType>();
        foreach (var item in _Items)
        {
          bool wasSelected = item.IsSelected;
          item.IsSelected = value!=null && value.Contains(item);
          if (!wasSelected && item.IsSelected)
            selectedItems.Add(item);
          else if (wasSelected && !item.IsSelected)
            unselectedItems.Add(item);
        }
        NotifyPropertyChanged("SelectedItems");
        if (SelectionChanged!=null &&  (unselectedItems.Count!=0  || selectedItems.Count!=0))
          base.Dispatch(() => SelectionChanged(this, new NotifySelectionChangedEventArgs(selectedItems, unselectedItems)));          
      }
    }

    public ItemType SelectedItem
    {
      get
      {
        return _Items.Where(item => item.IsSelected).FirstOrDefault();
      }
      set
      {
        foreach (var item in _Items)
          item.IsSelected = item.Equals(value);
        NotifyPropertyChanged("SelectedItem");
      }
    }
  }
}
