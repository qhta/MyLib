using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public interface IListViewModel
  {

    ViewModel ParentViewModel { get; }

    string SortedBy { get; set; }

    void FindFirstItem(object pattern, IEnumerable<string> propNames);

    void FindNextItem();

    void FindFirstInvalidItem();

    void FindNextInvalidItem();

    IEnumerable<object> Items { get; }
  }
}