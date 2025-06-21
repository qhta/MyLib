using System.Windows;

using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public class WritingSystemsCollection : OrderedObservableCollection<WritingSystemViewModel>
{
  public WritingSystemsCollection(): base((item)=>item.Name)
  {
    //EvaluateIsUsedCommand = new RelayCommand(EvaluateIsUsed);
    NewWritingSystemCommand = new RelayCommand<WritingSystemType?>(CreateNewWritingSystem);
    DeleteWritingSystemCommand = new RelayCommand<WritingSystemViewModel>(DeleteWritingSystem, CanDeleteWritingSystem);
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
    var vm = _ViewModels.Instance.AllWritingSystems.FirstOrDefault(item => item.Id == ws.Id);
    if (vm == null)
    {
      vm = new WritingSystemViewModel(ws);
      _ViewModels.Instance.AllWritingSystems.Add(vm);
    }
    return vm;
  }

  //public RelayCommand EvaluateIsUsedCommand { get; }

  //private void EvaluateIsUsed()
  //{
  //  foreach (var vm in this)
  //  {
  //    vm.IsUsed = vm.Model.UcdBlocks?.Count > 0 || vm.Model.Children?.Count > 0 || vm.Model.UcdRanges?.Count > 0;
  //  }
  //}

  public IRelayCommand NewWritingSystemCommand { get; }

  private void CreateNewWritingSystem(WritingSystemType? newType)
  {
    var vm = new WritingSystemViewModel(new WritingSystem());
    var model = vm.Model;
    vm.Name = "<new "+newType.ToString()+">";
    vm.Type = newType;
    var vmWindow = new Views.EditWritingSystemWindow
    {
      AddMode = true,
      DataContext = vm,
      Owner = Application.Current.MainWindow,
    };
    vmWindow.ShowDialog();
  }

  public WritingSystemViewModel? NewWritingSystem
  {
    get
    {
      var data = new WritingSystem();
      var viewModel = new WritingSystemViewModel(data);
      return viewModel;
    }
  }

  public RelayCommand<WritingSystemViewModel> DeleteWritingSystemCommand { get; }


  private void DeleteWritingSystem(WritingSystemViewModel? item)
  {
    if (item == null) return;
    if (!CanDeleteWritingSystem(item))
      MessageBox.Show("Cannot delete writing system", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
  }

  private bool CanDeleteWritingSystem(WritingSystemViewModel? item)
  {
    if (item == null) return false;

    var ok = !item.IsUsed;
    return ok;
  }
}