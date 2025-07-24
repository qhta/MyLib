using System.Diagnostics;
using System.Windows;

using Qhta.MVVM;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  /// <summary>
  /// Command to delete a writing system.
  /// </summary>
  public RelayCommand<WritingSystemViewModel> DeleteWritingSystemCommand { [DebuggerStepThrough] get; }


  private void DeleteWritingSystemCommandExecute(WritingSystemViewModel? item)
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

  private bool CanDeleteWritingSystem(WritingSystemViewModel? item)
  {
    if (item == null) return false;

    var ok = !item.IsUsed;
    return ok;
  }
}