using System.Diagnostics;
using System.Windows;

using Qhta.DeepCopy;
using Qhta.MVVM;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Commands;

/// <summary>
/// Command to create a new writing system.
/// </summary>
public class NewWritingSystemCommand : RelayCommand<WritingSystemType?>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="NewWritingSystemCommand"/> class.
  /// </summary>
  public NewWritingSystemCommand()
    : base(NewWritingSystemCommandExecute)
  {
  }
  /// <summary>
  /// Gets a new instance of <see cref="WritingSystemViewModel"/> initialized with a new <see cref="WritingSystem"/> entity.
  /// </summary>
  private static WritingSystemViewModel NewWritingSystem
  {
    get
    {
      var data = new WritingSystem();
      var viewModel = new WritingSystemViewModel(data);
      return viewModel;
    }
  }

  private static void NewWritingSystemCommandExecute(WritingSystemType? newType)
  {
    var vm = FindEmptyWritingSystemViewModel();
    if (vm == null)
    {
      // No empty writing system view model found, create a new one
      vm = NewWritingSystem;
    }
    else
    {
      var existingItem = vm;
      vm = (WritingSystemViewModel?)DeepCopier.CopyFrom(existingItem); // Create a copy of the existing empty writing system view model
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

  private static WritingSystemViewModel? FindEmptyWritingSystemViewModel()
  {
    var existingItem = _ViewModels.Instance.WritingSystems.LastOrDefault(item => item.Name!.StartsWith("<"));
    return existingItem;
  }
}