using System.Windows.Controls;

using Qhta.UnicodeBuild.Helpers;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Views;
/// <summary>
/// Interaction logic for CodePointsView.xaml
/// </summary>
public partial class UcdCodePointsView : UserControl
{
  public UcdCodePointsView()
  {
    InitializeComponent();
  }


  private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    LongTextColumn.DataGrid_OnQueryRowHeight(sender, e);
  }
}
