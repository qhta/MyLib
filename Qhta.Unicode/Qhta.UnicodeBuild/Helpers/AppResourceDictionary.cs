using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Helpers
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

  }
}