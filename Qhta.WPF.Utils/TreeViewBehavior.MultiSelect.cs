using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Qhta.WPF.Utils
{
  public static partial class TreeViewBehavior
  {
    private static TreeViewItem _selectTreeViewItemOnMouseUp;

    #region MultiSelect property
    /// <summary>
    /// For MultiSelect behavior not only tree view items source should implement IListSelector interface, 
    /// but also all tree view item items sources.
    /// </summary>
    public static bool GetMultiSelect(DependencyObject obj)
    {
      return (bool)obj.GetValue(MultiSelectProperty);
    }

    /// <summary>
    /// For MultiSelect behavior not only tree view items source should implement IListSelector interface, 
    /// but also all tree view item items sources.
    /// </summary>
    public static void SetMultiSelect(DependencyObject obj, bool value)
    {
      obj.SetValue(MultiSelectProperty, value);
    }

    /// <summary>
    /// For MultiSelect behavior not only tree view items source should implement IListSelector interface, 
    /// but also all tree view item items sources.
    /// </summary>
    public static readonly DependencyProperty MultiSelectProperty = DependencyProperty.RegisterAttached
      ("MultiSelect", typeof(bool), typeof(TreeViewBehavior),
          new UIPropertyMetadata(false, MultiSelectChanged));

    public static void MultiSelectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (obj is TreeView treeView)
      {
        if ((bool)args.NewValue)
        {
          treeView.GotFocus += OnTreeViewItemGotFocus;
          treeView.PreviewMouseLeftButtonDown += OnTreeViewItemPreviewMouseDown;
          treeView.PreviewMouseLeftButtonUp += OnTreeViewItemPreviewMouseUp;
        }
        else
        {
          treeView.GotFocus -= OnTreeViewItemGotFocus;
          treeView.PreviewMouseLeftButtonDown -= OnTreeViewItemPreviewMouseDown;
          treeView.PreviewMouseLeftButtonUp -= OnTreeViewItemPreviewMouseUp;
        }
      }
    }
    #endregion

    #region IsItemSelected property
    public static readonly DependencyProperty IsItemSelectedProperty = DependencyProperty.RegisterAttached
      ("IsItemSelected", typeof(Boolean), typeof(TreeViewBehavior), new PropertyMetadata(false, OnIsItemSelectedPropertyChanged));

    public static bool GetIsItemSelected(TreeViewItem element)
    {
      return (bool)element.GetValue(IsItemSelectedProperty);
    }

    public static void SetIsItemSelected(TreeViewItem element, Boolean value)
    {
      if (element == null) return;
      element.SetValue(IsItemSelectedProperty, value);
    }

    public static void OnIsItemSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var treeViewItem = d as TreeViewItem;
      var treeView = FindTreeView(treeViewItem);
      if (treeViewItem != null && treeView != null)
      {
        var selectedItems = GetSelectedItems(treeView);
        if (selectedItems != null)
        {
          if (GetIsItemSelected(treeViewItem))
          {
            selectedItems.Add(treeViewItem.Header);
          }
          else
          {
            selectedItems.Remove(treeViewItem.Header);
          }
        }
      }
    }
    #endregion

    #region SelectedItems property
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached
      ("SelectedItems", typeof(IList), typeof(TreeViewBehavior));

    public static IList GetSelectedItems(TreeView element)
    {
      return (IList)element.GetValue(SelectedItemsProperty);
    }

    public static void SetSelectedItems(TreeView element, IList value)
    {
      element.SetValue(SelectedItemsProperty, value);
    }
    #endregion

    #region CurrentItem property

    public static readonly DependencyProperty CurrentItemProperty = DependencyProperty.RegisterAttached
      ("CurrentItem", typeof(object), typeof(TreeViewBehavior),
          new UIPropertyMetadata(null, CurrentItemChanged));
    
    public static object CurrentItem(TreeView treeView)
    {
      return treeView.GetValue(CurrentItemProperty);
    }

    public static void SetCurrentItem(TreeView obj, object treeView)
    {
      if (treeView != null)
      {
        //Debug.WriteLine($"TreeViewItemBehavior.SetSelectedItem({value})");
        obj.SetValue(CurrentItemProperty, treeView);
      }
    }

    public static void UpdateCurrentItem(TreeView treeView, object value)
    {
      //Debug.WriteLine($"TreeViewBehavior.UpdateCurrentItem({value})");
      treeView.SetCurrentValue(CurrentItemProperty, value);
    }
    #endregion

    #region UseCurrentItemChangedEvent property
    public static readonly DependencyProperty UseCurrentItemChangeEventProperty = DependencyProperty.RegisterAttached
      ("UseCurrentItemChangeEvent", typeof(bool), typeof(TreeViewBehavior),
          new UIPropertyMetadata(false));
    public static bool GetUseCurrentItemChangeEvent(DependencyObject obj)
    {
      return (bool)obj.GetValue(UseCurrentItemChangeEventProperty);
    }

    public static void SetUseCurrentItemChangeEvent(DependencyObject obj, bool value)
    {
      obj.SetValue(UseCurrentItemChangeEventProperty, value);
    }
    #endregion

    #region CurrentItemChanged event
    public static readonly RoutedEvent CurrentItemChangeEvent = EventManager.RegisterRoutedEvent
      ("CurrentItemChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewBehavior));

    public static void AddCurrentItemChangeHandler(DependencyObject obj, RoutedEventHandler handler)
    {
      if (obj is UIElement element)
        element.AddHandler(TreeViewBehavior.CurrentItemChangeEvent, handler);
    }

    public static void RemoveCurrentItemChangeHandler(DependencyObject obj, RoutedEventHandler handler)
    {
      if (obj is UIElement element)
        element.RemoveHandler(TreeViewBehavior.CurrentItemChangeEvent, handler);
    }

    private static void CurrentItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (obj is TreeView treeView)
      {
        if (GetUseCurrentItemChangeEvent(treeView))
          treeView.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(args.OldValue, args.NewValue)
          { RoutedEvent = TreeViewBehavior.CurrentItemChangeEvent, Source = obj });
      }
    }
    #endregion

    #region StartItem property
    public static readonly DependencyProperty StartItemProperty = DependencyProperty.RegisterAttached
      ("StartItem", typeof(object), typeof(TreeViewBehavior));


    public static object GetStartItem(TreeView element)
    {
      return element.GetValue(StartItemProperty);
    }

    public static void SetStartItem(TreeView element, object value)
    {
      //Debug.WriteLine($"TreeViewBehavior.MultiSelect.SetStartItem({value})");
      element.SetValue(StartItemProperty, value);
    }
    #endregion

    #region TreeViewItem event handlers
    public static void OnTreeViewItemPreviewMouseDown(object sender, MouseEventArgs e)
    {
      var treeViewItem = FindTreeViewItem(e.OriginalSource as DependencyObject);

      if (treeViewItem != null && treeViewItem.IsFocused)
        OnTreeViewItemGotFocus(sender, e);
    }

    public static void OnTreeViewItemPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      var treeViewItem = FindTreeViewItem(e.OriginalSource as DependencyObject);

      if (treeViewItem == _selectTreeViewItemOnMouseUp)
      {
        SelectItems(treeViewItem, sender as TreeView);
      }
    }

    public static void OnTreeViewItemGotFocus(object sender, RoutedEventArgs e)
    {
      _selectTreeViewItemOnMouseUp = null;

      if (e.OriginalSource is TreeView) return;

      var treeViewItem = FindTreeViewItem(e.OriginalSource as DependencyObject);
      if (Mouse.LeftButton == MouseButtonState.Pressed && GetIsItemSelected(treeViewItem) && Keyboard.Modifiers != ModifierKeys.Control)
      {
        _selectTreeViewItemOnMouseUp = treeViewItem;
        return;
      }
      if (sender is TreeView treeView)
      {
        if (SelectItems(treeViewItem, treeView))
        {
          if (treeView.ItemsSource is IListSelector listSelector)
            listSelector.NotifySelectionChanged();
        }
      }
    }
    #endregion

    #region Find methods
    public static TreeViewItem FindTreeViewItem(DependencyObject dependencyObject)
    {
      if (!(dependencyObject is Visual || dependencyObject is Visual3D))
        return null;
      var treeViewItem = dependencyObject as TreeViewItem;
      if (treeViewItem != null)
      {
        return treeViewItem;
      }
      return FindTreeViewItem(VisualTreeHelper.GetParent(dependencyObject));
    }

    public static TreeView FindTreeView(DependencyObject dependencyObject)
    {
      if (dependencyObject == null)
        return null;
      var treeView = dependencyObject as TreeView;
      return treeView ?? FindTreeView(VisualTreeHelper.GetParent(dependencyObject));
    }
    #endregion

    #region SelectItems methods
    public static bool SelectItems(TreeViewItem treeViewItem, TreeView treeView)
    {
      if (treeViewItem != null && treeView != null)
      {
        if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
        {
          return SelectMultipleItemsContinuously(treeView, treeViewItem, true);
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control)
        {
          return SelectMultipleItemsRandomly(treeView, treeViewItem);
        }
        else if (Keyboard.Modifiers == ModifierKeys.Shift)
        {
          return SelectMultipleItemsContinuously(treeView, treeViewItem);
        }
        else
        {
          return SelectSingleItem(treeView, treeViewItem);
        }
      }
      return false;
    }

    public static bool SelectSingleItem(TreeView treeView, TreeViewItem treeViewItem)
    {
      //Debug.WriteLine($"SelectSingleItem({treeViewItem})");
      DeselectAllItems(treeView, null);
      if (treeView.ItemsSource is IListSelector listSelector)
        listSelector.SelectItem(treeViewItem.DataContext ?? treeViewItem, true);
      SetSelectedItem(treeView, treeViewItem.DataContext ?? treeViewItem);
      SetStartItem(treeView, treeViewItem.DataContext ?? treeViewItem);
      UpdateCurrentItem(treeView, treeViewItem.DataContext ?? treeViewItem);
      return true;
    }

    public static void DeselectAllItems(TreeView treeView, TreeViewItem treeViewItem)
    {
      if (treeView != null)
      {
        if (treeView.ItemsSource is IListSelector listSelector)
        {
          listSelector.SelectAll(false);
        }
        else
        {
          for (int i = 0; i < treeViewItem.Items.Count; i++)
          {
            var item = treeViewItem.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
            if (item != null)
            {
              SetIsItemSelected(item, false);
              DeselectAllItems(null, item);
            }
          }
        }
      }
    }

    public static bool SelectMultipleItemsRandomly(TreeView treeView, TreeViewItem treeViewItem)
    {
      //Debug.WriteLine($"SelectMultipleItemsRandomly({treeViewItem})");
      SetIsItemSelected(treeViewItem, !GetIsItemSelected(treeViewItem));
      if (GetStartItem(treeView) == null || Keyboard.Modifiers == ModifierKeys.Control)
      {
        //if (GetIsItemSelected(treeViewItem))
        {
          SetStartItem(treeView, treeViewItem.DataContext ?? treeViewItem);
        }
      }
      else
      {
        if (GetSelectedItems(treeView).Count == 0)
        {
          SetStartItem(treeView, null);
        }
      }
      UpdateCurrentItem(treeView, treeViewItem.DataContext ?? treeViewItem);
      return true;
    }

    public static bool SelectMultipleItemsContinuously(TreeView treeView, TreeViewItem treeViewItem, bool shiftControl = false)
    {
      //Debug.WriteLine($"SelectMultipleItemsContinuously({treeViewItem})");
      var startItem = GetStartItem(treeView);

      if (startItem == null)
      {
        SelectSingleItem(treeView, treeViewItem);
        return true;
      }
      else
      {
        if (startItem == treeViewItem || startItem == treeViewItem.DataContext)
        {
          SelectSingleItem(treeView, treeViewItem);
          return true;
        }
        var isSelected = true;// GetIsItemSelected(treeViewItem);
        ICollection<TreeViewItem> allItems = new List<TreeViewItem>();
        GetAllItems(treeView, null, allItems);
        //DeselectAllItems(treeView, null);
        bool isBetween = false;
        foreach (var item in allItems)
        {
          if (item == treeViewItem || item == startItem || item.DataContext == startItem)
          {
            // toggle to true if first element is found and
            // back to false if last element is found
            isBetween = !isBetween;

            // set boundary element
            SetIsItemSelected(item, isSelected);
            continue;
          }

          if (isBetween)
          {
            SetIsItemSelected(item, isSelected);
            continue;
          }

          //if (!shiftControl)
          //  SetIsItemSelected(item, false);
        }
        UpdateCurrentItem(treeView, treeViewItem.DataContext ?? treeViewItem);
      }
      //Debug.WriteLine($"SelectMultipleItemsContinuously({treeViewItem}) end");
      return true;
    }

    public static void GetAllItems(TreeView treeView, TreeViewItem treeViewItem, ICollection<TreeViewItem> allItems)
    {
      if (treeView != null)
      {
        for (int i = 0; i < treeView.Items.Count; i++)
        {
          var item = treeView.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
          if (item != null)
          {
            allItems.Add(item);
            GetAllItems(null, item, allItems);
          }
        }
      }
      else
      {
        for (int i = 0; i < treeViewItem.Items.Count; i++)
        {
          var item = treeViewItem.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
          if (item != null)
          {
            allItems.Add(item);
            GetAllItems(null, item, allItems);
          }
        }
      }
    }
    #endregion

  }
}
