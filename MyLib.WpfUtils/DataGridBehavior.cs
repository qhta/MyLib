using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using MyLib.MVVM;

namespace MyLib.WpfUtils
{
  /// <summary>
  /// Collection of DataGrid behavior
  /// </summary>
  public static class DataGridBehavior
  {
    #region DisplayRowNumberOffset

    /// <summary>
    /// Sets the starting value of the row header if enabled
    /// </summary>
    public static DependencyProperty DisplayRowNumberOffsetProperty =
        DependencyProperty.RegisterAttached("DisplayRowNumberOffset",
                                            typeof(int),
                                            typeof(DataGridBehavior),
                                            new FrameworkPropertyMetadata(0, OnDisplayRowNumberOffsetChanged));

    public static int GetDisplayRowNumberOffset(DependencyObject target)
    {
      return (int)target.GetValue(DisplayRowNumberOffsetProperty);
    }

    public static void SetDisplayRowNumberOffset(DependencyObject target, int value)
    {
      target.SetValue(DisplayRowNumberOffsetProperty, value);
    }

    private static void OnDisplayRowNumberOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      DataGrid dataGrid = target as DataGrid;
      int offset = (int)e.NewValue;

      if (GetDisplayRowNumber(target))
      {
        GetVisualChildCollection<DataGridRow>(dataGrid).
                ForEach(d => d.Header = d.GetIndex() + offset);
      }
    }

    #endregion

    #region DisplayRowNumber

    /// <summary>
    /// Enable display of row header automatically
    /// </summary>
    /// <remarks>
    /// Source: 
    /// </remarks>
    public static DependencyProperty DisplayRowNumberProperty =
        DependencyProperty.RegisterAttached("DisplayRowNumber",
                                            typeof(bool),
                                            typeof(DataGridBehavior),
                                            new FrameworkPropertyMetadata(false, OnDisplayRowNumberChanged));

    public static bool GetDisplayRowNumber(DependencyObject target)
    {
      return (bool)target.GetValue(DisplayRowNumberProperty);
    }

    public static void SetDisplayRowNumber(DependencyObject target, bool value)
    {
      target.SetValue(DisplayRowNumberProperty, value);
    }

    private static void OnDisplayRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      DataGrid dataGrid = target as DataGrid;
      if ((bool)e.NewValue == true)
      {
        int offset = GetDisplayRowNumberOffset(target);

        EventHandler<DataGridRowEventArgs> loadedRowHandler = null;
        loadedRowHandler = (object sender, DataGridRowEventArgs ea) =>
        {
          if (GetDisplayRowNumber(dataGrid) == false)
          {
            dataGrid.LoadingRow -= loadedRowHandler;
            return;
          }
          ea.Row.Header = ea.Row.GetIndex() + offset;
        };
        dataGrid.LoadingRow += loadedRowHandler;

        ItemsChangedEventHandler itemsChangedHandler = null;
        itemsChangedHandler = (object sender, ItemsChangedEventArgs ea) =>
        {
          if (GetDisplayRowNumber(dataGrid) == false)
          {
            dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
            return;
          }
          GetVisualChildCollection<DataGridRow>(dataGrid).
              ForEach(d => d.Header = d.GetIndex() + offset);
        };
        dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
      }
    }

    #endregion // DisplayRowNumber

    #region IsBroughtIntoViewWhenSelected

    public static DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
        DependencyProperty.RegisterAttached("IsBroughtIntoViewWhenSelected",
                                            typeof(bool),
                                            typeof(DataGridBehavior),
                                            new FrameworkPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

    public static bool GetIsBroughtIntoViewWhenSelected(DependencyObject target)
    {
      return (bool)target.GetValue(IsBroughtIntoViewWhenSelectedProperty);
    }

    public static void SetIsBroughtIntoViewWhenSelected(DependencyObject target, bool value)
    {
      target.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
    }

    private static void OnIsBroughtIntoViewWhenSelectedChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      DataGrid dataGrid = target as DataGrid;
      if ((bool)e.NewValue == true)
      {
        //int offset = GetDisplayRowNumberOffset(target);

        EventHandler<DataGridRowEventArgs> loadedRowHandler = null;
        loadedRowHandler = (object sender, DataGridRowEventArgs ea) =>
        {
          if (GetIsBroughtIntoViewWhenSelected(dataGrid) == false)
          {
            dataGrid.LoadingRow -= loadedRowHandler;
            return;
          }
          ea.Row.Selected+=DataGridRow_Selected;
          if (ea.Row.DataContext is ISelectable selectable && ea.Row.DataContext is INotifyPropertyChanged notifyPropertyChanged)
            notifyPropertyChanged.PropertyChanged+=DataContext_PropertyChanged;
          else
            ea.Row.DataContextChanged+=DataRow_DataContextChanged;
        };
        dataGrid.LoadingRow += loadedRowHandler;

        //ItemsChangedEventHandler itemsChangedHandler = null;
        //itemsChangedHandler = (object sender, ItemsChangedEventArgs ea) =>
        //{
        //  if (GetIsBroughtIntoViewWhenSelected(dataGrid) == false)
        //  {
        //    dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
        //    return;
        //  }
        //  GetVisualChildCollection<DataGridRow>(dataGrid).
        //      ForEach(d => d.Header = d.GetIndex() + offset);
        //};
        //dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
      }
    }

    private static void DataRow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      Debug.WriteLine($"{sender.GetType().Name}.DataContextChanged({e.NewValue})");
      if (e.NewValue!=null)
        if (e.NewValue is ISelectable selectable && e.NewValue is INotifyPropertyChanged notifyPropertyChanged)
          notifyPropertyChanged.PropertyChanged+=DataContext_PropertyChanged;
    }

    private static void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Debug.WriteLine($"{sender.GetType().Name}.PropertyChanged({e.PropertyName})");
      if (e.PropertyName=="IsSelected")
      {
        ISelectable selectable = sender as ISelectable;
        if (selectable.IsSelected)
        {
          Debug.WriteLine("Selected");
        }
      }
    }

    private static void DataGridRow_Selected(object sender, RoutedEventArgs e)
    {
      DataGridRow item = e.OriginalSource as DataGridRow;
      if (item != null)
        item.BringIntoView();
    }
    #endregion

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

    //#region IsBroughtIntoViewWhenSelected

    //public static bool GetIsBroughtIntoViewWhenSelected(DataGridRow dataGridRow)
    //{
    //  return (bool)dataGridRow.GetValue(IsBroughtIntoViewWhenSelectedProperty);
    //}

    //public static void SetIsBroughtIntoViewWhenSelected(
    //  DataGridRow dataGridRow, bool value)
    //{
    //  dataGridRow.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
    //}

    //public static DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
    //    DependencyProperty.RegisterAttached("IsBroughtIntoViewWhenSelected",
    //                                        typeof(bool),
    //                                        typeof(DataGridBehavior),
    //                                        new FrameworkPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

    ////public static DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
    ////    DependencyProperty.RegisterAttached(
    ////    "IsBroughtIntoViewWhenSelected",
    ////    typeof(bool),
    ////    typeof(DataGridBehavior),
    ////    new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

    //static void OnIsBroughtIntoViewWhenSelectedChanged(
    //  DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    //{
    //  DataGridRow item = depObj as DataGridRow;
    //  if (item == null)
    //    return;

    //  if (e.NewValue is bool == false)
    //    return;

    //  if ((bool)e.NewValue)
    //    item.Selected += OnDataGridRowSelected;
    //  else
    //    item.Selected -= OnDataGridRowSelected;
    //}

    //static void OnDataGridRowSelected(object sender, RoutedEventArgs e)
    //{
    //  // Only react to the Selected event raised by the TreeViewItem
    //  // whose IsSelected property was modified. Ignore all ancestors
    //  // who are merely reporting that a descendant's Selected fired.
    //  if (!Object.ReferenceEquals(sender, e.OriginalSource))
    //    return;

    //  DataGridRow item = e.OriginalSource as DataGridRow;
    //  if (item != null)
    //    item.BringIntoView();
    //}

    //#endregion // IsBroughtIntoViewWhenSelected

  }
}
