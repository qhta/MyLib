using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Qhta.MVVM
{
  public interface IListViewModel
  {
    Type GetItemType();

    IEnumerable<object> GetItems();

    IViewModel ParentViewModel { get; }

    string SortedBy { get; set; }

    void FindFirstItem(object pattern, IEnumerable<string> propNames);

    void FindFirstItem(Expression<Func<object, bool>> expression);

    void FindNextItem();

    void FindFirstInvalidItem();

    void FindNextInvalidItem();

    IEnumerable<object> Items { get; }

    object CurrentItem { get; set; }
    IEnumerable<object> SelectedItems { get; set; }

    void SelectAll(bool select);
  }
}