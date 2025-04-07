using System.Windows.Controls;

using Qhta.UnicodeBuild.Helpers;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Views
{
  /// <summary>
  /// Interaction logic for UcdRangesView.xaml
  /// </summary>
  public partial class UcdRangesView : UserControl
  {
    public UcdRangesView()
    {
      InitializeComponent();
    }

    private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
    {
      LongTextColumn.DataGrid_OnQueryRowHeight(sender, e);
    }
  }
}
