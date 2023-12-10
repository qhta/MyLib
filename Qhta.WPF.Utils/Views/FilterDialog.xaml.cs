#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Qhta.WPF.Utils.ViewModels;

namespace Qhta.WPF.Utils.Views;
/// <summary>
/// Interaction logic for FilterDialog.xaml
/// </summary>
public partial class FilterDialog : ToolWindow
{
  public FilterDialog()
  {
    InitializeComponent();
    Activated += Window_Activated;
  }

  private void Window_Activated(object? sender, EventArgs args)
  {
    MinHeight = ActualHeight;
  }

  private void OkButton_Click(object sender, RoutedEventArgs args)
  {
    if (DataContext is FilterViewModel filterViewModel)
      filterViewModel.ClearOp = false;
    DialogResult = true;
    Close();
  }

  private void CancelButton_Click(object sender, RoutedEventArgs args)
  {
    if (DataContext is FilterViewModel filterViewModel)
      filterViewModel.ClearOp = false;
    DialogResult = false;
    Close();
  }

  private void ColumnSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
  {
    if (DataContext is CollectionViewFilterViewModel viewModel &&
      args.AddedItems.Count == 1 && args.AddedItems[0] is FilterableColumnInfo info)
    {
      var filter = viewModel.EditedInstance;
      if (filter is GenericColumnFilterViewModel genericFilter)
        genericFilter.PropPath = info.PropPath;
    }
  }

  private void ClearButton_Click(object sender, RoutedEventArgs e)
  {
    if (DataContext is FilterViewModel filterViewModel)
      filterViewModel.ClearOp = true;
    DialogResult = true;
    Close();
  }
}
