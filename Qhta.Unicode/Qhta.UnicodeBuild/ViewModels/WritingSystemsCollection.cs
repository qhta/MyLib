using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Syncfusion.Windows.Controls.Input;

namespace Qhta.UnicodeBuild.ViewModels;

public class WritingSystemsCollection : OrderedObservableCollection<WritingSystemViewModel>
{
  public WritingSystemsCollection(): base((item)=>item.Name)
  {
    EvaluateIsUsedCommand = new RelayCommand(EvaluateIsUsed);
  }

  public WritingSystemsCollection(WritingSystemViewModel parent, IEnumerable<WritingSystem> ws) : this()
  {
    Parent = parent;
    foreach (var w in ws)
    {
      var vm = _ViewModels.Instance.AllWritingSystems.FirstOrDefault(item => item.Id == w.Id);
      if (vm == null)
      {
        vm = new WritingSystemViewModel(w);
        if (this != _ViewModels.Instance.AllWritingSystems) _ViewModels.Instance.AllWritingSystems.Add(vm);
      }
      Add(vm);
    }
  }

  public WritingSystemViewModel? Parent { get; }

  public WritingSystemViewModel Add(WritingSystem ws)
  {
    //IsAdded = true;
    var vm = _ViewModels.Instance.AllWritingSystems.FirstOrDefault(item => item.Id == ws.Id);
    if (vm == null)
    {
      vm = new WritingSystemViewModel(ws);
      _ViewModels.Instance.AllWritingSystems.Add(vm);
    }
    //IsAdded = false;
    return vm;
  }

  ///// <summary>
  ///// Adds a WritingSystemViewModel to the collection and subscribes to its PropertyChanged event.
  ///// </summary>
  //public new void Add(WritingSystemViewModel item)
  //{
  //  IsAdded = true;
  //  int index = this.Count;
  //  this.Remove(item);
  //  for (int i=0; i < Count; i++)
  //  {
  //    if (String.Compare(this[i].Name, item.Name, StringComparison.InvariantCulture)>0)
  //    {
  //      index = i;
  //      break;
  //    }
  //  }
  //  Debug.WriteLine($"base.InsertItem({index}, {item})");
  //  base.InsertItem(index,item);
  //  IsAdded = false;
  //}

  //private bool IsAdded;

  //protected override void InsertItem(int index, WritingSystemViewModel item)
  //{
  //  //if (!IsAdded) // i.e. if it is moved by dragging
  //  //{
  //  //  Debug.WriteLine($"InsertItem({index}, {item})");
  //  //  item.Parent = Parent;
  //  //}
  //  //else
  //  {
  //    Debug.WriteLine($"InsertItem({index}, {item}) - not setting parent");
  //    base.InsertItem(index, item);
  //  }
  //}

  //private void VM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  //{
  //  if (sender is WritingSystemViewModel vm && e.PropertyName == nameof(WritingSystemViewModel.Parent))
  //  {
  //    Debug.WriteLine($"Parent changed to {vm.Parent?.Name}");
  //  }
  //}

  public ICommand EvaluateIsUsedCommand { get; }

  private void EvaluateIsUsed()
  {
    foreach (var vm in this)
    {
      vm.IsUsed = vm.Model.UcdBlocks?.Count > 0 || vm.Model.Children?.Count > 0 || vm.Model.UcdRanges?.Count > 0;
    }
  }
}