using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Qhta.MVVM;
using Qhta.SF.WPF.Tools;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.Windows.Shared;
using Qhta.UnicodeBuild.Helpers;
using Qhta.WPF.Utils;

namespace Qhta.UnicodeBuild
{
  /// <summary>
  /// 
  /// </summary>
  public partial class AppResourceDictionary : ResourceDictionary
  {
    /// <summary>
    /// Handler methods for various events in the application.
    /// </summary>
    public AppResourceDictionary()
    {
      InitializeComponent();
    }


    private T? FirstDescendant<T>(DependencyObject? parent, Predicate<T> predicate) where T : DependencyObject
    {
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

    //private void WrapButton_CheckedChanged(object sender, RoutedEventArgs e)
    //{
    //  if (sender is ToggleButton button)
    //    if (button.DataContext is ILongTextViewModel viewModel)
    //    {
    //      var dataGrid = button.FindParent<SfDataGrid>();
    //      if (dataGrid != null)
    //      {
    //        var rowIndex = dataGrid.ResolveToRowIndex(viewModel);
    //        //Debug.WriteLine($"Resolved row index: {rowIndex}");
    //        dataGrid.InvalidateRowHeight(rowIndex);
    //        //Debug.WriteLine($"dataGrid.InvalidateRowHeight({rowIndex}) invoked");
    //        dataGrid.UpdateLayout();
    //        dataGrid.View.Refresh();
    //      }
    //    }
    //}


  }
}
