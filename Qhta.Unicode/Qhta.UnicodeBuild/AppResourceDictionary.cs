using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild
{
  public partial class AppResourceDictionary : ResourceDictionary
  {
    public AppResourceDictionary()
    {
      InitializeComponent();
    }

    private T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
      DependencyObject? parentObject = VisualTreeHelper.GetParent(child);
      if (parentObject == null) return null;

      if (parentObject is T parent)
        return parent;
      return FindParent<T>(parentObject);
    }

    private void WrapButton_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (sender is ToggleButton button)
        if (button.DataContext is ILongTextViewModel viewModel)
        {
          var dataGrid = FindParent<SfDataGrid>(button);
          if (dataGrid != null)
          {
            var rowIndex = dataGrid.ResolveToRowIndex(viewModel);
            //Debug.WriteLine($"Resolved row index: {rowIndex}");
            dataGrid.InvalidateRowHeight(rowIndex);
            //Debug.WriteLine($"dataGrid.InvalidateRowHeight({rowIndex}) invoked");
            dataGrid.UpdateLayout();
            dataGrid.View.Refresh();
          }
        }
    }

    private void ShowPopup_Click(object sender, RoutedEventArgs e)
    {
      if (sender is Button button)
      {
        if (VisualTreeHelper.GetParent(button) is Grid grid)
        {
          var popup = grid.Children.OfType<Popup>().FirstOrDefault();
          if (popup != null)
          {
            // Find the parent cell of the button
            var cell = FindParent<GridCell>(button);
            if (cell != null)
            {
              // Get the position of the cell relative to the DataGrid
              var dataGrid = FindParent<SfDataGrid>(cell);
              if (dataGrid != null)
              {
                var cellPosition = cell.TransformToAncestor(dataGrid).Transform(new Point(0, 0));

                popup.PlacementTarget= cell;
                popup.Placement = PlacementMode.Bottom;
                popup.VerticalOffset = -cell.ActualHeight;
                popup.Width = cell.ActualWidth;
                // Set the Popup position relative to the cell
                //popup.HorizontalOffset = cellPosition.X;
                //popup.VerticalOffset = cellPosition.Y;

                // Open the Popup
                popup.IsOpen = true;
                var textBox = (popup.Child as Border)?.Child as TextBox;
                if (textBox!=null)
                {
                  textBox.Focus();
                }
              }
            }
          }

        }
      }
    }
  }
}
