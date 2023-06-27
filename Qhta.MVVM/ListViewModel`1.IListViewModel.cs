using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Qhta.MVVM
{
  public partial class ListViewModel<ItemType> : IListViewModel
  {
    /// <summary>
    /// Enumerable items.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<object> GetItems()
    {
      return Items;
    }

    Action? FindNextItemDelegate;

    /// <summary>
    /// A method to switch to the next item.
    /// </summary>
    public void FindNextItem()
    {
      FindNextItemDelegate?.Invoke();
    }


    /// <summary>
    /// A method to switch to the first item that fullfills the expression.
    /// </summary>
    /// <param name="expression"></param>
    public void FindFirstItem(Expression<Func<object, bool>> expression)
    {
      var predicate = expression.Compile();
      FindFirstItem(predicate);
    }

    /// <summary>
    /// A method to switch to the first item that fullfills the predicate.
    /// </summary>
    /// <param name="predicate"></param>
    public void FindFirstItem(Func<object, bool> predicate)
    {
      Predicate = null;
      Pattern = null;
      var currentItem = CurrentItem;
      if (currentItem == null)
      {
        var selectedItems = GetItemsList().Where(predicate).Cast<ItemType>().ToList();
        var firstSelectedItem = selectedItems.FirstOrDefault();
        if (firstSelectedItem != null)
        {
          SelectCurrentItem(firstSelectedItem);
          Predicate = predicate;
          FindNextItemDelegate = FindNextItemWithPredicate;
        }
      }
      else
      {
        var selectedItems = GetItemsList().Where(predicate).Cast<ItemType>().ToList();
        var selectedItemIndex = selectedItems.IndexOf(currentItem);
        if (selectedItemIndex < selectedItems.Count - 1)
        {
          SelectCurrentItem(selectedItems[selectedItemIndex + 1]);
          Predicate = predicate;
          FindNextItemDelegate = FindNextItemWithPredicate;
        }
      }
    }

    /// <summary>
    /// A method to switch to the first item with a specific pattern and selected propNames.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="propNames"></param>
    public void FindFirstItem(object pattern, IEnumerable<string> propNames)
    {
      if (pattern is ItemType typedPattern)
        FindFirstItem(typedPattern, propNames);
    }

    /// <summary>
    /// A method to switch to the first item with a specific pattern and selected propNames.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="propNames"></param>
    public void FindFirstItem(ItemType pattern, IEnumerable<string> propNames)
    {
      Predicate = null;
      Pattern = null;
      PatternPropNames = propNames;
      var properties = typeof(ItemType).GetProperties().Where(prop => propNames.Contains(prop.Name)).ToArray();
      var currentItem = CurrentItem;
      if (currentItem == null)
      {
        var selectedItems = GetItemsList().Where(item => SameAs(item, pattern, properties)).ToList();
        var firstSelectedItem = selectedItems.FirstOrDefault();
        if (firstSelectedItem != null)
        {
          SelectCurrentItem(firstSelectedItem);
          Pattern = pattern;
          FindNextItemDelegate = FindNextItemWithPattern;
        }
      }
      else
      {
        var selectedItems = GetItemsList().Where(item => SameAs(item, pattern, properties)).ToList();
        var selectedItemIndex = selectedItems.IndexOf(currentItem);
        if (selectedItemIndex < selectedItems.Count - 1)
        {
          SelectCurrentItem(selectedItems[selectedItemIndex + 1]);
          Pattern = pattern;
          FindNextItemDelegate = FindNextItemWithPattern;
        }
      }
    }

    Func<object, bool>? Predicate;
    ItemType? Pattern;
    IEnumerable<string>? PatternPropNames;

    /// <summary>
    /// Find next item with previously defined predicate.
    /// </summary>
    public void FindNextItemWithPredicate()
    {
      if (Predicate != null)
        FindFirstItem(Predicate);
    }

    /// <summary>
    /// Find next item with previously defined pattern.
    /// </summary>
    public void FindNextItemWithPattern()
    {
      if (Pattern != null && PatternPropNames != null)
        FindFirstItem(Pattern, PatternPropNames);
    }

    /// <summary>
    /// A method to check if the modeled item is same as other item type.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="pattern"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    protected virtual bool SameAs(ItemType item, ItemType pattern, IEnumerable<PropertyInfo> properties)
    {
      foreach (var property in properties)
      {
        var patternValue = property.GetValue(pattern);
        if (patternValue != null)
        {
          var itemValue = property.GetValue(item);
          if (itemValue != null)
            if (!itemValue.Equals(patternValue))
              return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Find first invalid item.
    /// </summary>
    public void FindFirstInvalidItem()
    {
      var invalidItems = GetItemsList().Where(item => item.IsValid == false).ToList();
      var firstInvalidItem = invalidItems.FirstOrDefault();
      if (firstInvalidItem != null)
      {
        SelectCurrentItem(firstInvalidItem);
        FindNextItemDelegate = FindNextInvalidItem;
      }
    }

    /// <summary>
    /// Find next invalid item.
    /// </summary>
    public void FindNextInvalidItem()
    {
      var selectedItem = CurrentItem;
      if (selectedItem == null)
        FindFirstInvalidItem();
      else
      {
        var invalidItems = GetItemsList().Where(item => item.IsValid == false).ToList();
        var invalidItemIndex = invalidItems.IndexOf(selectedItem);
        if (invalidItemIndex < invalidItems.Count - 1)
          SelectCurrentItem(invalidItems[invalidItemIndex + 1]);
      }
    }

    /// <summary>
    /// Returns a type of the item.
    /// </summary>
    /// <returns></returns>
    public Type GetItemType()
    {
      return typeof(ItemType);
    }

    /// <summary>
    /// Get list of modeled items.
    /// </summary>
    /// <returns></returns>
    public List<ItemType> GetItemsList()
    {
      var items = Values.ToList();
      if (SortedBy != null)
      {
        IOrderedEnumerable<ItemType>? orderedItems = null;
        List<String> sortedColumns = SortedBy.Split(new char[] { ';', ',' }).ToList();
        foreach (var column in sortedColumns)
        {
          string columnName = column;
          ListSortDirection? direction = null;
          if (column.EndsWith("(desc)"))
          {
            columnName = column.Substring(0, column.Length - "(desc)".Length).Trim();
            direction = ListSortDirection.Descending;
          }
          else
          if (column.EndsWith("(asc)"))
          {
            columnName = column.Substring(0, column.Length - "(asc)".Length).Trim();
            direction = ListSortDirection.Ascending;
          }
          var propertyInfo = typeof(ItemType).GetProperty(columnName);
          if (propertyInfo != null)
          {
            if (orderedItems == null)
            {
              if (direction == ListSortDirection.Descending)
                orderedItems = items.OrderByDescending(item => propertyInfo.GetValue(item));
              else
                orderedItems = items.OrderBy(item => propertyInfo.GetValue(item));
            }
            else
            {
              IComparer<ItemType> comparer = new MyComparer(propertyInfo);
              orderedItems = orderedItems.CreateOrderedEnumerable<ItemType>(item => item, comparer, false);
            }
          }
        }
        if (orderedItems != null)
          return orderedItems.ToList();
      }
      return items;
    }

    class MyComparer : IComparer<ItemType>
    {
      public MyComparer(PropertyInfo propInfo)
      {
        myPropInfo = propInfo;
      }

      PropertyInfo myPropInfo;

      public int Compare(ItemType? x, ItemType? y)
      {
        if (x != null && y != null)
        {
          var value1 = (IComparable?)myPropInfo.GetValue(x);
          var value2 = (IComparable?)myPropInfo.GetValue(y);
          if (value1 != value2 && value1 != null)
            return value1.CompareTo(value2);
        }
        return 0;
      }
    }

    /// <summary>
    /// Specifes the criteria of sorting
    /// </summary>
    public string? SortedBy
    {
      get { return _SortedBy; }
      set
      {
        if (_SortedBy != value)
        {
          _SortedBy = value;
          NotifyPropertyChanged(nameof(SortedBy));
        }
      }
    }
    private string? _SortedBy;

    IEnumerable<object> IListViewModel.SelectedItems { get => SelectedItems; set { } /*SelectedItems=value.Cast<ItemType>();*/ }

    /// <summary>
    /// Returns a list of selected items.
    /// </summary>
    public IEnumerable<ItemType> SelectedItems { get => _SelectedItems; set { } }
    private List<ItemType> _SelectedItems = new List<ItemType>();

    private void SelectCurrentItem(ItemType value)
    {
      SelectedItem = value;
      CurrentItem = value;
    }

    object? IListViewModel.CurrentItem { get => this.CurrentItem; set => CurrentItem = value as ItemType; }

    /// <summary>
    /// Enumerable of all items.
    /// </summary>
    public IEnumerable<object> Items => Values;

    private bool inSelectAll;

    /// <summary>
    /// A method of selecting or deselecting items.
    /// </summary>
    /// <param name="select"></param>
    public void SelectAll(bool select)
    {
      if (inSelectAll)
        return;
      inSelectAll = true;
      foreach (var item in Values.ToList())
        item.IsSelected = select;
      inSelectAll = false;
      NotifySelectionChanged();
    }

    private HashSet<ItemType> previouslySelectedItems = new HashSet<ItemType>();

    /// <summary>
    /// A method of notification that selection has been changed.
    /// </summary>
    public void NotifySelectionChanged()
    {
      List<ItemType> unselectedItems = new List<ItemType>();
      List<ItemType> selectedItems = new List<ItemType>();
      bool selectionChangeDetected = false;
      foreach (var item in Values.ToList())
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
        if (_SelectionChanged != null && (unselectedItems.Count != 0 || selectedItems.Count != 0))
          base.Dispatch(() => _SelectionChanged(this, new SelectionChangedEventArgs(selectedItems, unselectedItems)));
      }
    }
  }
}
