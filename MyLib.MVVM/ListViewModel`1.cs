using System;
using System.Collections;
using System.Collections.Generic;
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
  public class ListViewModel<ItemType> : ListViewModel, IListViewModel,
    IEnumerable<ItemType>, INotifyCollectionChanged, INotifySelectionChanged where ItemType : class, IValidated, ISelectable
  {
    public ListViewModel(ViewModel parentViewModel) : base(parentViewModel)
    {
      _Items.CollectionChanged+=_Items_CollectionChanged;
      _Items.PropertyChanged+=_Items_PropertyChanged;
    }

    public Type GetItemType()
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
      if (e.Action==NotifyCollectionChangedAction.Add)
      {
        foreach (var item in e.NewItems)
        {
          if (item is INotifyPropertyChanged notifyingItem)
            notifyingItem.PropertyChanged += Item_PropertyChanged;
        }
      }
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
          if (!inSelectAll)
          {
            NotifyPropertyChanged("SelectedItem");
            NotifyPropertyChanged("SelectedItems");
            NotifySelectionChanged();
          }
          break;
      }
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

    IEnumerable<object> IListViewModel.SelectedItems { get => SelectedItems; set => SelectedItems=value.Cast<ItemType>(); }

    public IEnumerable<ItemType> SelectedItems
    {
      get
      {
        return _Items.Where(item => item.IsSelected) ?? new ItemType[0];
      }
      set
      {
        foreach (var item in _Items)
        {
          item.IsSelected = value!=null && value.Contains(item);
        }
        NotifyPropertyChanged("SelectedItems");
        NotifySelectionChanged();
      }
    }

    object IListViewModel.SelectedItem { get => SelectedItem; set => SelectedItem=value as ItemType; }

    public ItemType SelectedItem
    {
      get
      {
        return _Items.ToList().Where(item => item.IsSelected).FirstOrDefault();
      }
      set
      {
        foreach (var item in _Items.ToList())
          item.IsSelected = item.Equals(value);
        NotifyPropertyChanged("SelectedItem");
        NotifySelectionChanged();
      }
    }

    public void NotifySelectionChanged()
    {
      List<ItemType> unselectedItems = new List<ItemType>();
      List<ItemType> selectedItems = new List<ItemType>();
      bool selectionChangeDetected = false;
      foreach (var item in _Items.ToList())
      {
        bool wasSelected = previouslySelectedItems.Contains(item);
        if (!wasSelected && item.IsSelected)
        {
          selectedItems.Add(item);
          previouslySelectedItems.Add(item);
          selectionChangeDetected = true;
        }
        else if (wasSelected && !item.IsSelected)
        {
          unselectedItems.Add(item);
          previouslySelectedItems.Remove(item);
          selectionChangeDetected = true;
        }
      }
      if (selectionChangeDetected)
      {
        //Debug.WriteLine($"NotifySelectionChanged selected={selectedItems.Count},unselected={unselectedItems.Count}");
        if (SelectionChanged!=null &&  (unselectedItems.Count!=0  || selectedItems.Count!=0))
          base.Dispatch(() => SelectionChanged(this, new NotifySelectionChangedEventArgs(selectedItems, unselectedItems)));
      }
    }

    private HashSet<ItemType> previouslySelectedItems = new HashSet<ItemType>();
    private bool inSelectAll;
    public void SelectAll(bool select)
    {
      if (inSelectAll)
        return;
      inSelectAll=true;
      foreach (var item in _Items.ToList())
        item.IsSelected=select;
      inSelectAll=false;
      NotifySelectionChanged();
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

    class MyComparer : IComparer<ItemType>
    {
      public MyComparer(PropertyInfo propInfo)
      {
        myPropInfo = propInfo;
      }

      PropertyInfo myPropInfo;

      public int Compare(ItemType x, ItemType y)
      {
        IComparable value1 = myPropInfo.GetValue(x) as IComparable;
        IComparable value2 = myPropInfo.GetValue(y) as IComparable;
        if (value1!=value2)
          return value1.CompareTo(value2);
        else
          return 0;
      }
    }

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
          var propertyInfo = typeof(ItemType).GetProperty(columnName);
          if (propertyInfo!=null)
          {
            if (orderedItems==null)
            {
              if (direction==ListSortDirection.Descending)
                orderedItems=items.OrderByDescending(item => propertyInfo.GetValue(item));
              else
                orderedItems=items.OrderBy(item => propertyInfo.GetValue(item));
            }
            else
            {
              IComparer<ItemType> comparer = new MyComparer(propertyInfo);
              orderedItems=orderedItems.CreateOrderedEnumerable<ItemType>(item => item, comparer, false);
            }
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

    public void FindFirstItem(Expression<Func<object, bool>> expression)
    {
      var predicate = expression.Compile();
      FindFirstItem(predicate);
    }

    public void FindFirstItem(Func<object, bool> predicate)
    {
      Predicate = null;
      Pattern=null;
      var selectedItem = SelectedItem;
      if (selectedItem==null)
      {
        var selectedItems = GetItemsList().Where(predicate).Cast<ItemType>().ToList();
        var firstSelectedItem = selectedItems.FirstOrDefault();
        if (firstSelectedItem!=null)
        {
          SelectedItem = firstSelectedItem;
          Predicate = predicate;
          FindNextItemDelegate=FindNextItemWithPredicate;
        }
      }
      else
      {
        var selectedItems = GetItemsList().Where(predicate).Cast<ItemType>().ToList();
        var selectedItemIndex = selectedItems.IndexOf(selectedItem);
        if (selectedItemIndex<selectedItems.Count-1)
        {
          SelectedItem = selectedItems[selectedItemIndex+1];
          Predicate = predicate;
          FindNextItemDelegate=FindNextItemWithPredicate;
        }
      }
    }

    public void FindFirstItem(object pattern, IEnumerable<string> propNames)
    {
      if (pattern is ItemType typedPattern)
        FindFirstItem(typedPattern, propNames);
    }

    public void FindFirstItem(ItemType pattern, IEnumerable<string> propNames)
    {
      Predicate = null;
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
          FindNextItemDelegate=FindNextItemWithPattern;
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
          FindNextItemDelegate=FindNextItemWithPattern;
        }
      }
    }

    Func<object, bool> Predicate;
    ItemType Pattern;
    IEnumerable<string> PatternPropNames;

    public void FindNextItemWithPredicate()
    {
      if (Predicate!=null)
        FindFirstItem(Predicate);
    }

    public void FindNextItemWithPattern()
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
