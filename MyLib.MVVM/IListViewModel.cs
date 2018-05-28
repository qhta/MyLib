using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public interface IListViewModel
  {
    Type GetItemType();

    ViewModel ParentViewModel { get; }

    string SortedBy { get; set; }

    void FindFirstItem(object pattern, IEnumerable<string> propNames);

    void FindFirstItem(Expression<Func<object, bool>> expression);

    void FindNextItem();

    void FindFirstInvalidItem();

    void FindNextInvalidItem();

    IEnumerable<object> Items { get; }

    object SelectedItem { get; set; }
    IEnumerable<object> SelectedItems { get; set; }

    void SelectAll(bool select);
  }
}