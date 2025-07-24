using System.Diagnostics;
using System.Windows;
using Qhta.DeepCopy;
using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  /// <summary>
  /// Command to create a new writing system.
  /// </summary>
  public IRelayCommand NewWritingSystemCommand { [DebuggerStepThrough] get; }

  private void NewWritingSystemCommandExecute(WritingSystemType? newType)
  {
    var vm = FindEmptyWritingSystemViewModel();
    // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
    if (vm == null)
    {
      // No empty writing system view model found, create a new one
      vm = NewWritingSystem;
    }
    else
    {
      var existingItem = vm;
      vm = (WritingSystemViewModel?)DeepCopier
        .CopyFrom(existingItem); // Create a copy of the existing empty writing system view model
    }
    if (vm is null) return;
    vm.Type = newType;
    var vmWindow = new Views.EditWritingSystemWindow
    {
      AddMode = true,
      DataContext = vm,
      Owner = Application.Current.MainWindow,
    };
    vmWindow.ShowDialog();
  }

  private WritingSystemViewModel? FindEmptyWritingSystemViewModel()
  {
    var existingItem = _ViewModels.Instance.WritingSystems.LastOrDefault(item => item.Name!.StartsWith("<"));
    return existingItem;
  }

  /// <summary>
  /// Gets a new instance of <see cref="WritingSystemViewModel"/> initialized with a new <see cref="WritingSystem"/> entity.
  /// </summary>
  public WritingSystemViewModel NewWritingSystem
  {
    get
    {
      var data = new WritingSystem();
      var viewModel = new WritingSystemViewModel(data);
      return viewModel;
    }
  }

}