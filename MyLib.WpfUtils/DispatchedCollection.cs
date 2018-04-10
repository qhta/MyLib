using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyLib.WpfUtils
{
  public class DispatchedCollection<T>: ObservableCollection<T>
  {

    public Dispatcher _Dispatcher = Application.Current.Dispatcher;

    public new void Add(T item)
    {
      if (Dispatcher.CurrentDispatcher==_Dispatcher)
      {
        lock (this)
          base.Add(item);
      }
      else
      {
        var action = new Action<T>(Add);
        _Dispatcher.BeginInvoke(action, new object[] { item });
      }
    }

    public new void Insert(int index, T item)
    {
      if (Dispatcher.CurrentDispatcher==_Dispatcher)
      {
        lock (this)
          base.Insert(index, item);
      }
      else
      {
        var action = new Action<int, T>(Insert);
        _Dispatcher.BeginInvoke(action, new object[] { index, item });
      }
    }

    //public void Sort(IComparer<T> comparer)
    //{
    //  if (Dispatcher.CurrentDispatcher==_Dispatcher)
    //  {
    //    DoSort(comparer);
    //  }
    //  else
    //  {
    //    var action = new Action<IComparer<T>>(DoSort);
    //    _Dispatcher.BeginInvoke(action, new object[] { comparer });
    //  }
    //}

    //private void DoSort(IComparer<T> comparer)
    //{
    //  for (int i=0; i<Count-1; i++)
    //    for (int j = i; j<Count-1; j++)
    //  {
    //      int cmp = comparer.Compare(this[i], this[i+1]);
    //      if (cmp>0)
    //      {
    //        Debug.WriteLine($"{i}->{i+1}");
    //        Move(i+1, i);
    //      }
    //  }
    //}
  }
}
