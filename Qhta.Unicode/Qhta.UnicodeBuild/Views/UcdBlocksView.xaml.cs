using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;

using Qhta.Unicode.Models;
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
      if (e.RowIndex > 0 && sender is SfDataGrid dataGrid)
      {
        if (dataGrid.View.Records[e.RowIndex - 1].Data is UcdBlockViewModel record)
        {
          if (record.Comment == null)
            return;
          var wrapColumn = dataGrid.Columns.FirstOrDefault(col => col.MappingName == "Comment");
          if (wrapColumn == null)
            return;
          var maxWidth = wrapColumn.ActualWidth - 6;
          var formattedText = new FormattedText(
            record.Comment,
            System.Globalization.CultureInfo.CurrentCulture,
            System.Windows.FlowDirection.LeftToRight, // Fixed the error by qualifying with type name
            new Typeface("Segoe UI"),
            12,
            Brushes.Black,
            new NumberSubstitution(),
            1);
          formattedText.MaxTextWidth = maxWidth;
          e.Height = formattedText.Height + 6; // Add some padding
          e.Handled = true;
        }
      }
    }
  }
}
