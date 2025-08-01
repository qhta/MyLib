using System.Diagnostics;
using System.Windows;

using Qhta.MVVM;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Commands;


/// <summary>
/// Command to edit a writing system.
/// </summary>
public class EditWritingSystemCommand : RelayCommand<WritingSystemViewModel?>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="EditWritingSystemCommand"/> class.
  /// </summary>
  public EditWritingSystemCommand() : base(EditWritingSystemCommandExecute)
  {
  }

  private static void EditWritingSystemCommandExecute(WritingSystemViewModel? item)
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