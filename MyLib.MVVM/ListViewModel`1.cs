using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public class ListViewModel<ItemType> : ListViewModel, IListViewModel,
    IEnumerable<ItemType>, INotifyCollectionChanged, INotifySelectionChanged where ItemType : class, IValidated, ISelectable
  {
    public ListViewModel(ViewModel parentViewModel) : base(parentViewModel)
    {
      _Items.CollectionChanged+=_Items_CollectionChanged;
      _Items.PropertyChanged+=_Items_PropertyChanged;
    }

    public override Type GetItemType()
    {
      return typeof(ItemType);
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
      NotifyPropertyChanged("ValidItemsCount");
      NotifyPropertyChanged("InvalidItemsCount");
    }

    private void _Items_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyPropertyChanged(e.PropertyName);
      switch (e.PropertyName)
      {
        case "IsValid":
          NotifyPropertyChanged("ValidItemsCount");
          NotifyPropertyChanged("InvalidItemsCount");
          break;
        case "IsSelected":
          NotifyPropertyChanged("SelectedItemsCount");
          break;
      }
    }

    IEnumerable<object> IListViewModel.Items => this.Items;

    public DispatchedCollection<ItemType> Items => _Items;
    protected DispatchedCollection<ItemType> _Items = new DispatchedCollection<ItemType>();

    public event NotifySelectionChangedEventHandler SelectionChanged;

    public override bool? IsValid { get => Items!=null && Items.Where(item => item.IsValid==false).FirstOrDefault()==null; set => _IsValid = value; }
    private bool? _IsValid;

    public IEnumerator<ItemType> GetEnumerator()
    {
      return ((IEnumerable<ItemType>)Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<ItemType>)Items).GetEnumerator();
    }

    public int Count => Items.Count;

    public int ValidItemsCount => Items.Where(item => item.IsValid==true).Count();

    public int InvalidItemsCount => Items.Where(item => item.IsValid==false).Count();

    public int SelectedItemsCount => Items.Where(item => item.IsSelected).Count();

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

    public string SortedBy
    {
      get { return _SortedBy; }
      set
      {
        if (_SortedBy!=value)
        {
          _SortedBy=value;
          NotifyPropertyChanged("SortedBy");
        }
      }
    }
    private string _SortedBy;

    public List<ItemType> GetItemsList()
    {
      var items = Items.ToList();
      if (SortedBy!=null)
      {
        IOrderedEnumerable<ItemType> orderedItems = null;
        List<String> sortedColumns = SortedBy.Split(new char[] { ';', ',' }).ToList();
        foreach (var column in sortedColumns)
        {
          string columnName = column;
          ListSortDirection? direction = null;
          if (column.EndsWith("(desc)"))
          {
            columnName = column.Substring(0, column.Length-"(desc)".Length).Trim();
            direction = ListSortDirection.Descending;
          }
          else
          if (column.EndsWith("(asc)"))
          {
            columnName = column.Substring(0, column.Length-"(asc)".Length).Trim();
            direction = ListSortDirection.Ascending;
          }
          if (direction==ListSortDirection.Descending)
          {
            if (orderedItems==null)
              orderedItems=items.OrderByDescending(item => item.GetType().GetProperty(columnName).GetValue(item));
            else
              orderedItems=orderedItems.OrderByDescending(item => item.GetType().GetProperty(columnName).GetValue(item));
          }
          else
          {
            if (orderedItems==null)
              orderedItems=items.OrderBy(item => item.GetType().GetProperty(columnName).GetValue(item));
            else
              orderedItems=orderedItems.OrderBy(item => item.GetType().GetProperty(columnName).GetValue(item));
          }
        }
        return orderedItems.ToList();
      }
      return items;
    }

    Action FindNextItemDelegate;

    public void FindNextItem()
    {
      FindNextItemDelegate?.Invoke();
    }

    public void FindFirstItem(object pattern, IEnumerable<string> propNames)
    {
      if (pattern is ItemType typedPattern)
        FindFirstItem(typedPattern, propNames);
    }

    public void FindFirstItem(ItemType pattern, IEnumerable<string> propNames)
    {
      Pattern=null;
      PatternPropNames = propNames;
      var properties = typeof(ItemType).GetProperties().Where(prop => propNames.Contains(prop.Name)).ToArray();
      var selectedItem = SelectedItem;
      if (selectedItem==null)
      {
        var selectedItems = GetItemsList().Where(item => SameAs(item, pattern, properties)).ToList();
        var firstSelectedItem = selectedItems.FirstOrDefault();
        if (firstSelectedItem!=null)
        {
          SelectedItem = firstSelectedItem;
          Pattern = pattern;
          FindNextItemDelegate=FindNextPatternItem;
        }
      }
      else
      {
        var selectedItems = GetItemsList().Where(item => SameAs(item, pattern, properties)).ToList();
        var selectedItemIndex = selectedItems.IndexOf(selectedItem);
        if (selectedItemIndex<selectedItems.Count-1)
        {
          SelectedItem = selectedItems[selectedItemIndex+1];
          Pattern = pattern;
          FindNextItemDelegate=FindNextPatternItem;
        }
      }
    }

    ItemType Pattern;
    IEnumerable<string> PatternPropNames;

    public void FindNextPatternItem()
    {
      if (Pattern!=null)
        FindFirstItem(Pattern, PatternPropNames);
    }

    protected virtual bool SameAs(ItemType item, ItemType pattern, IEnumerable<PropertyInfo> properties)
    {
      foreach (var property in properties)
      {
        var patternValue = property.GetValue(pattern);
        if (patternValue!=null)
        {
          var itemValue = property.GetValue(item);
          if (itemValue!=null)
            if (!itemValue.Equals(patternValue))
              return false;
        }
      }
      return true;
    }

    public void FindFirstInvalidItem()
    {
      var invalidItems = GetItemsList().Where(item => item.IsValid==false).ToList();
      var firstInvalidItem = invalidItems.FirstOrDefault();
      if (firstInvalidItem!=null)
      {
        SelectedItem = firstInvalidItem;
        FindNextItemDelegate=FindNextInvalidItem;
      }
    }

    public void FindNextInvalidItem()
    {
      var selectedItem = SelectedItem;
      if (selectedItem==null)
        FindFirstInvalidItem();
      else
      {
        var invalidItems = GetItemsList().Where(item => item.IsValid==false).ToList();
        var invalidItemIndex = invalidItems.IndexOf(selectedItem);
        if (invalidItemIndex<invalidItems.Count-1)
          SelectedItem = invalidItems[invalidItemIndex+1];
      }
    }
  }
}
