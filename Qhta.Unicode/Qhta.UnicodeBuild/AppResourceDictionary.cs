using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.Windows.Shared;

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

    private T? FirstDescendant<T>(DependencyObject? parent, Predicate<T> predicate) where T : DependencyObject
    {
      //OrientedCellsPanel
      //GridDetailsViewExpanderCell
      var parentObject = parent;
      if (parent is T content)
      {
        if (predicate(content))
          return content;
      }
      if (parentObject is ContentControl contentControl)
      {
        return FirstDescendant<T>(contentControl.Content as DependencyObject, predicate);
      }
      if (parentObject is ItemsControl itemsControl)
      {
        foreach (var item in itemsControl.Items)
        {
          var result = FirstDescendant<T>(item as DependencyObject, predicate);
          if (result != null)
            return result;
        }
        return null;
      }
      if (parentObject is Panel panel)
      {
        foreach (var item in panel.Children)
        {
          var result = FirstDescendant<T>(item as DependencyObject, predicate);
          if (result != null)
            return result;
        }
        return null;
      }
      Debug.WriteLine($"parent is {parent?.GetType()}");
      return null;
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
            var cell = FindParent<GridCell>(button);
            if (cell != null)
            {
              var dataGrid = FindParent<SfDataGrid>(cell);
              if (dataGrid != null)
              {
                popup.PlacementTarget = cell;
                popup.Placement = PlacementMode.Bottom;
                popup.VerticalOffset = -cell.ActualHeight;
                popup.Width = cell.ActualWidth;
                popup.IsOpen = true;
              }
            }
          }
        }
      }
    }

    private void GridSplitter_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      var gridSplitter = sender as GridSplitter;
      if (gridSplitter == null)
        return;
      var parent = FindParent<Grid>(gridSplitter);
      if (parent != null)
      {
        Debug.WriteLine($"GridSplitter size changed: {e.NewSize}");
        parent.Height=e.NewSize.Height;
      }
    }
  }
}
