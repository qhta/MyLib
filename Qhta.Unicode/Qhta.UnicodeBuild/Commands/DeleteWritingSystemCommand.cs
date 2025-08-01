using System.Diagnostics;
using System.Windows;

using Qhta.MVVM;
using Qhta.UnicodeBuild.ViewModels;

namespace Qhta.UnicodeBuild.Commands;

/// <summary>
/// Command to delete a writing system.
/// </summary>
public class DeleteWritingSystemCommand : RelayCommand<WritingSystemViewModel>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DeleteWritingSystemCommand"/> class.
  /// </summary>
  public DeleteWritingSystemCommand() : base(DeleteWritingSystemCommandExecute)
  {
  }

  private static void DeleteWritingSystemCommandExecute(WritingSystemViewModel? item)
  {
    if (item == null) return;
    if (!CanDeleteWritingSystem(item))
    {
      MessageBox.Show(Resources.Strings.CannotDeleteWritingSystem, Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
      return;
    }
    // Writing system entity is not really deleted, just marked as deleted
    item.Name = "<deleted>";
    if (item.Parent != null)
    {
      item.Parent.Children?.Remove(item);
      item.Parent.NotifyPropertyChanged(nameof(WritingSystemViewModel.Children));
    }
    foreach (var prop in item.Model.GetType().GetProperties())
    {
      if (prop.Name != nameof(WritingSystemViewModel.Id) && prop.Name != nameof(WritingSystemViewModel.Name) && prop.CanWrite)
      {
        prop.SetValue(item.Model, null);
      }
    }
  }

  private static bool CanDeleteWritingSystem(WritingSystemViewModel? item)
  {
    if (item == null) return false;

    var ok = !item.IsUsed;
    return ok;
  }
}
