namespace Qhta.WPF.Utils.Views;
/// <summary>
/// Interaction logic for GenericColumnFilterView.xaml
/// </summary>
public partial class GenericColumnFilterView : UserControl
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public GenericColumnFilterView()
  {
    InitializeComponent();
  }
  private void ColumnSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
  {
    if (DataContext is GenericColumnFilterViewModel viewModel &&
      args.AddedItems.Count == 1 && args.AddedItems[0] is FilterableColumnInfo info)
    {
      Debug.WriteLine($"ColumnSelectionBox_SelectionChanged");
      Debug.WriteLine($"OldColumn={viewModel.Column}");
      Debug.WriteLine($"NewColumn={info}");
      if (viewModel.Column != info)
      {
        viewModel.Column = info;
        viewModel.SpecificFilter = viewModel.CreateSpecificFilter();
      }
    }
  }

}
