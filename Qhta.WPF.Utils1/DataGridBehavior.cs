using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Qhta.MVVM;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Collection of DataGrid behavior
  /// </summary>
  public static class DataGridBehavior
  {
    //#region DisplayRowNumberOffset

    ///// <summary>
    ///// Sets the starting value of the row header if enabled
    ///// </summary>
    //public static DependencyProperty DisplayRowNumberOffsetProperty =
    //    DependencyProperty.RegisterAttached("DisplayRowNumberOffset",
    //                                        typeof(int),
    //                                        typeof(DataGridBehavior),
    //                                        new FrameworkPropertyMetadata(0, OnDisplayRowNumberOffsetChanged));

    //public static int GetDisplayRowNumberOffset(DependencyObject target)
    //{
    //  return (int)target.GetValue(DisplayRowNumberOffsetProperty);
    //}

    //public static void SetDisplayRowNumberOffset(DependencyObject target, int value)
    //{
    //  target.SetValue(DisplayRowNumberOffsetProperty, value);
    //}

    //private static void OnDisplayRowNumberOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    //{
    //  DataGrid dataGrid = target as DataGrid;
    //  int offset = (int)e.NewValue;

    //  if (GetDisplayRowNumber(target))
    //  {
    //    GetVisualChildCollection<DataGridRow>(dataGrid).
    //            ForEach(d => d.Header = d.GetIndex() + offset);
    //  }
    //}

    //#endregion

    //#region DisplayRowNumber

    ///// <summary>
    ///// Enable display of row header automatically
    ///// </summary>
    ///// <remarks>
    ///// Source: 
    ///// </remarks>
    //public static DependencyProperty DisplayRowNumberProperty =
    //    DependencyProperty.RegisterAttached("DisplayRowNumber",
    //                                        typeof(bool),
    //                                        typeof(DataGridBehavior),
    //                                        new FrameworkPropertyMetadata(false, OnDisplayRowNumberChanged));

    //public static bool GetDisplayRowNumber(DependencyObject target)
    //{
    //  return (bool)target.GetValue(DisplayRowNumberProperty);
    //}

    //public static void SetDisplayRowNumber(DependencyObject target, bool value)
    //{
    //  target.SetValue(DisplayRowNumberProperty, value);
    //}

    //private static void OnDisplayRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    //{
    //  DataGrid dataGrid = target as DataGrid;
    //  if ((bool)e.NewValue == true)
    //  {
    //    int offset = GetDisplayRowNumberOffset(target);

    //    EventHandler<DataGridRowEventArgs> loadedRowHandler = null;
    //    loadedRowHandler = (object sender, DataGridRowEventArgs ea) =>
    //    {
    //      if (GetDisplayRowNumber(dataGrid) == false)
    //      {
    //        dataGrid.LoadingRow -= loadedRowHandler;
    //        return;
    //      }
    //      ea.Row.Header = ea.Row.GetIndex() + offset;
    //    };
    //    dataGrid.LoadingRow += loadedRowHandler;

    //    ItemsChangedEventHandler itemsChangedHandler = null;
    //    itemsChangedHandler = (object sender, ItemsChangedEventArgs ea) =>
    //    {
    //      if (GetDisplayRowNumber(dataGrid) == false)
    //      {
    //        dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
    //        return;
    //      }
    //      GetVisualChildCollection<DataGridRow>(dataGrid).
    //          ForEach(d => d.Header = d.GetIndex() + offset);
    //    };
    //    dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
    //  }
    //}
    //#endregion // DisplayRowNumber

    //#region IsSelectable
    //public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.RegisterAttached(
    //    "IsSelectable",
    //    typeof(object),
    //    typeof(DataGridBehavior),
    //    new PropertyMetadata(default(object), OnIsSelectableChanged));

    //public static object GetIsSelectable(DependencyObject target)
    //{
    //  return (object)target.GetValue(IsSelectableProperty);
    //}

    //public static void SetIsSelectable(DependencyObject target, object value)
    //{
    //  target.SetValue(IsSelectableProperty, value);
    //}

    //static void OnIsSelectableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    //{
    //  var dataGrid = sender as DataGrid;
    //  if (dataGrid == null || e.NewValue == null)
    //    return;
    //  dataGrid.Dispatcher.Invoke((Action)(() =>
    //  {
    //    try
    //    {
    //      //dataGrid.SelectedIndex=-1;
    //    }
    //    catch
    //    {
    //    }
    //  }));
    //}

    //#endregion IsSelectable

    #region ScrollIntoView
    public static readonly DependencyProperty ScrollIntoViewProperty = DependencyProperty.RegisterAttached(
        "ScrollIntoView",
        typeof(object),
        typeof(DataGridBehavior),
        new PropertyMetadata(default(object), OnScrollIntoViewChanged));

    public static object GetScrollIntoView(DependencyObject target)
    {
      return (object)target.GetValue(ScrollIntoViewProperty);
    }

    public static void SetScrollIntoView(DependencyObject target, object value)
    {
      target.SetValue(ScrollIntoViewProperty, value);
    }

    static void OnScrollIntoViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var dataGrid = sender as DataGrid;
      if (dataGrid == null || e.NewValue == null)
        return;
      dataGrid.Dispatcher.Invoke((Action)(() =>
      {
        try
        {
          dataGrid.UpdateLayout();
          dataGrid.ScrollIntoView(e.NewValue, null);
          dataGrid.UpdateLayout();
        }
        catch
        {
        }
      }));
    }

    #endregion ScrollIntoView

    #region SortingEventHandler
    public static readonly DependencyProperty SortingEventHandlerProperty = DependencyProperty.RegisterAttached(
        "SortingEventHandler",
        typeof(object),
        typeof(DataGridBehavior),
        new PropertyMetadata(default(object), OnSortingEventHandlerChanged));


    public static object GetSortingEventHandler(DependencyObject target)
    {
      return (object)target.GetValue(SortingEventHandlerProperty);
    }

    public static void SetSortingEventHandler(DependencyObject target, object value)
    {
      target.SetValue(SortingEventHandlerProperty, value);
    }

    static void OnSortingEventHandlerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var dataGrid = sender as DataGrid;
      if (dataGrid == null || e.NewValue == null)
        return;
      dataGrid.Dispatcher.Invoke((Action)(() =>
      {
        if (e.NewValue is DataGridSortingEventHandler handler)
          dataGrid.Sorting+=handler;
        else
          dataGrid.Sorting+=DataGrid_Sorting;
      }));
    }

    static void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
      var dataGrid = sender as DataGrid;
      if (dataGrid.DataContext is IListViewModel listViewModel)
        if (e.Column is DataGridTextColumn textColumn && textColumn.Binding is Binding binding)
        {
          List<string> sortedColumns = new List<string>();
          foreach (var column in dataGrid.Columns)
          {
            var sortDirection = column.SortDirection;
            if (sortDirection!=null)
            {
              if (column==e.Column)
              {
                //swap sort direction
                if (sortDirection==ListSortDirection.Descending)
                  sortedColumns.Add($"{column.SortMemberPath}(asc)");
                else
                  sortedColumns.Add($"{column.SortMemberPath}(desc)");
              }
              else
              {
                // columns sorted previously
                if (sortDirection==ListSortDirection.Descending)
                  sortedColumns.Add($"{column.SortMemberPath}(desc)");
                else
                  sortedColumns.Add($"{column.SortMemberPath}(asc)");
              }
            }
          }
          var curColumn = e.Column;
          if (curColumn.SortDirection==null)
            sortedColumns.Add($"{curColumn.SortMemberPath}");
          listViewModel.SortedBy = string.Join(";", sortedColumns);
          //Debug.WriteLine($"SortedBy({listViewModel.SortedBy})");
        }
    }

    #endregion SortingEventHandler

    #region Get Visuals

    private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
    {
      List<T> visualCollection = new List<T>();
      GetVisualChildCollection(parent as DependencyObject, visualCollection);
      return visualCollection;
    }

    private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
    {
      int count = VisualTreeHelper.GetChildrenCount(parent);
      for (int i = 0; i < count; i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(parent, i);
        if (child is T)
        {
          visualCollection.Add(child as T);
        }
        if (child != null)
        {
          GetVisualChildCollection(child, visualCollection);
        }
      }
    }

    #endregion // Get Visuals

  }
}
