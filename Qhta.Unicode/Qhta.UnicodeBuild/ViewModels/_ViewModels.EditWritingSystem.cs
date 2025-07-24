using System.Diagnostics;
using System.Windows;
using Qhta.MVVM;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  /// <summary>
  /// Command to edit a writing system.
  /// </summary>
  public IRelayCommand EditWritingSystemCommand { [DebuggerStepThrough] get; }

  private void EditWritingSystemCommandExecute(WritingSystemViewModel? item)
  {
    var vmWindow = new Views.EditWritingSystemWindow
    {
      AddMode = false,
      DataContext = item,
      Owner = Application.Current.MainWindow,
    };
    vmWindow.ShowDialog();
  }


}