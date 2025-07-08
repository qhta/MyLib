using System.Diagnostics;
using System.Net.Mime;

namespace Qhta.UnicodeBuild.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class OrderedObservableCollection<T>(Func<T, object> keySelector, IComparer<object>? comparer = null)
  : ObservableCollection<T>
{
  public bool IsLoaded
  //{
  //  get;
  //  set;
  //}
  {
    get => _IsLoaded;
    set
    {
      if (value != _IsLoaded)
      {
        _IsLoaded = value;
        if (_IsLoaded)
        {
          // Recalculate the row header column width when loaded
          //_RowHeaderColumnWidth = CalculateRowHeaderColumnWidth();
          OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(Count)));

        }
        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(IsLoaded)));
        //OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(RowHeaderColumnWidth)));
      }
    }
  }
  private bool _IsLoaded;

  private readonly Func<T, object> _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

  protected override void InsertItem(int index, T item)
  {
    var oldIndex = IndexOf(item);
    if (oldIndex >= 0)
    {
      if (oldIndex == index)
      {
        //Debug.WriteLine($"{item} already in collection at {oldIndex}");
        return; // Already in the correct position
      }
      else
      {
        // Remove the item from its old position
        RemoveAt(oldIndex);
        //Debug.WriteLine($"{item} removed from position {oldIndex}");
      }
    }
    index = GetInsertIndex(item);
    base.InsertItem(index, item);
    //Debug.WriteLine($"{item} added at {index}");
  }

  protected override void SetItem(int index, T item)
  {
    RemoveAt(index);
    InsertItem(GetInsertIndex(item), item);
  }

  private int GetInsertIndex(T item)
  {
    var key = _keySelector(item);
    for (int i = 0; i < Count; i++)
    {
      var comparison = comparer?.Compare(key, _keySelector(this[i])) ?? Comparer<object>.Default.Compare(key, _keySelector(this[i]));
      if (comparison < 0)
        return i;
    }
    return Count;
  }

  //public Double RowHeaderColumnWidth
  //{
  //  get
  //  {
  //    if (!IsLoaded)
  //      return 50;
  //    if (_RowHeaderColumnWidth == 0)
  //      _RowHeaderColumnWidth = CalculateRowHeaderColumnWidth();
  //    return _RowHeaderColumnWidth;
  //  }
  //}

  //private double _RowHeaderColumnWidth;

  //public virtual Double CalculateRowHeaderColumnWidth()
  //{
  //  //if (Count == 0)
  //    return 50; // Default width if no items
  //  var count = DataRecordsCount; 
  //  var digits = (int)Math.Round(Math.Log10(Convert.ToDouble(count)));
  //  if (digits < 0)
  //    digits = 1;
  //  var text = new string('8', digits);
  //  text = " " + text+" "; // Add a space for padding
  //  var width = TextSizeEvaluator.EvaluateTextWidth(text);// 12 pixels per digit, plus 6 for padding
  //  return width;
  //}

  //public virtual int DataRecordsCount => Count;
}                         