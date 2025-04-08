using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Views
{
  /// <summary>
  /// Interaction logic for UcdBlocksView.xaml
  /// </summary>
  public partial class UcdBlocksView : UserControl
  {
    public UcdBlocksView()
    {
      InitializeComponent();
    }

    private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
    {
      LongTextColumn.DataGrid_OnQueryRowHeight(sender, e);
    }

    private void DataGrid_OnCurrentCellValidating(object? sender, CurrentCellValidatingEventArgs e)
    {
      if (e.Column.MappingName == "Range")
      {
        var newValue = e.NewValue?.ToString();
        if (newValue != null)
        {
          if (!RangeModel.TryParse(newValue, out _))
          {
            e.ErrorMessage = "Invalid range format. Expected format: XXXX..YYYY.";
            e.IsValid = false;
          }
        }
      }
    }
  }
}

