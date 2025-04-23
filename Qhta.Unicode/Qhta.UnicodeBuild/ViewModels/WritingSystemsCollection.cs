using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public class WritingSystemsCollection : ObservableCollection<WritingSystemViewModel>
{
  public WritingSystemsCollection()
  {
    EvaluateIsUsedCommand = new RelayCommand(EvaluateIsUsed);
  }

  public WritingSystemsCollection(IEnumerable<WritingSystem> ws) : this()
  {
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

  public WritingSystemViewModel Add(WritingSystem ws)
  {
    var vm = _ViewModels.Instance.AllWritingSystems.FirstOrDefault(item => item.Id == ws.Id);
    if (vm == null)
    {
      vm = new WritingSystemViewModel(ws);
      _ViewModels.Instance.AllWritingSystems.Add(vm);
    }
    return vm;
  }

  /// <summary>
  /// Adds a WritingSystemViewModel to the collection and subscribes to its PropertyChanged event.
  /// </summary>
  public new void Add(WritingSystemViewModel vm)
  {
    int n = this.Count;
    for (int i=0; i < Count; i++)
    {
      if (String.Compare(this[i].Name, vm.Name, StringComparison.InvariantCulture)>0)
      {
        n = i;
        break;
      }
    }
    base.Insert(n,vm);
    vm.PropertyChanged += VM_PropertyChanged;
  }


  private void VM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is WritingSystemViewModel vm && e.PropertyName == nameof(WritingSystemViewModel.Parent))
    {
      Debug.WriteLine($"Parent changed to {vm.Parent?.Name}");
    }
  }

  public ICommand EvaluateIsUsedCommand { get; }

  private void EvaluateIsUsed()
  {
    foreach (var vm in this)
    {
      vm.IsUsed = vm.Model.UcdBlocks?.Count > 0 || vm.Model.Children?.Count > 0 || vm.Model.UcdRanges?.Count > 0;
    }
  }
}