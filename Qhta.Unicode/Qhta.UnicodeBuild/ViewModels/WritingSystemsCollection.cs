using System.Collections.ObjectModel;
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

  public WritingSystemsCollection(IEnumerable<WritingSystem> ws): this()
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

  public void Add(WritingSystem ws)
  {
    Add(new WritingSystemViewModel(ws));
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