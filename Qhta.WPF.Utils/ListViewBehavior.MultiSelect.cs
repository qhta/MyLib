using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Qhta.WPF.Utils
{
  public partial class ListViewBehavior
  {
    /// <summary>
    ///   For MultiSelect behavior list view items source should implement IListSelector interface.
    /// </summary>
    public static bool GetMultiSelect(DependencyObject obj)
    {
      return (bool)obj.GetValue(MultiSelectProperty);
    }

    /// <summary>
    ///   For MultiSelect behavior list view items source should implement IListSelector interface.
    /// </summary>
    public static void SetMultiSelect(DependencyObject obj, bool value)
    {
        obj.SetValue(MultiSelectProperty, value);
    }

    /// <summary>
    ///   For MultiSelect behavior list view items source should implement IListSelector interface.
    /// </summary>
    public static readonly DependencyProperty MultiSelectProperty = DependencyProperty.RegisterAttached
      ("MultiSelect", typeof(bool), typeof(ListViewBehavior),
          new UIPropertyMetadata(false, MultiSelectChanged));

    public static void MultiSelectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (obj is ListView listView)
      {
        if ((bool)args.NewValue)
        {
          listView.SelectionMode = SelectionMode.Extended;
          listView.GotFocus += OnListViewItemGotFocus;
          listView.PreviewMouseLeftButtonDown += OnListViewItemPreviewMouseDown;
          listView.PreviewMouseLeftButtonUp += OnListViewItemPreviewMouseUp;
        }
        else
        {
          listView.GotFocus -= OnListViewItemGotFocus;
          listView.PreviewMouseLeftButtonDown -= OnListViewItemPreviewMouseDown;
          listView.PreviewMouseLeftButtonUp -= OnListViewItemPreviewMouseUp;
        }
      }
    }

    public static ListViewItem _selectListViewItemOnMouseUp;


    public static readonly DependencyProperty IsItemSelectedProperty = DependencyProperty.RegisterAttached
      ("IsItemSelected", typeof(Boolean), typeof(ListViewBehavior), new PropertyMetadata(false, OnIsItemSelectedPropertyChanged));

    public static bool GetIsItemSelected(ListViewItem element)
    {
      //return element.IsSelected;
      return (bool)element.GetValue(IsItemSelectedProperty);
    }

    public static void SetIsItemSelected(ListViewItem element, Boolean value)
    {
      if (element == null) return;
      element.IsSelected = value;
      element.SetValue(IsItemSelectedProperty, value);
    }

    public static void OnIsItemSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var listViewItem = d as ListViewItem;
      var listView = FindListView(listViewItem);
      if (listViewItem != null && listView != null)
      {
        var selectedItems = GetSelectedItems(listView);
        if (selectedItems != null)
        {
          if (GetIsItemSelected(listViewItem))
          {
            selectedItems.Add(listViewItem.DataContext);
          }
          else
          {
            selectedItems.Remove(listViewItem.DataContext);
          }
        }
      }
    }

    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached
      ("SelectedItems", typeof(IList), typeof(ListViewBehavior));

    public static IList GetSelectedItems(ListView element)
    {
      return (IList)element.GetValue(SelectedItemsProperty);
    }

    public static void SetSelectedItems(ListView element, IList value)
    {
      element.SetValue(SelectedItemsProperty, value);
    }

    public static readonly DependencyProperty StartItemProperty = DependencyProperty.RegisterAttached
      ("StartItem", typeof(ListViewItem), typeof(ListViewBehavior));


    public static ListViewItem GetStartItem(ListView element)
    {
      return (ListViewItem)element.GetValue(StartItemProperty);
    }

    public static void SetStartItem(ListView element, ListViewItem value)
    {
      element.SetValue(StartItemProperty, value);
    }


    public static void OnListViewItemGotFocus(object sender, RoutedEventArgs e)
    {
      _selectListViewItemOnMouseUp = null;

      if (e.OriginalSource is ListView) return;

      var listViewItem = FindListViewItem(e.OriginalSource as DependencyObject);
      //Debug.WriteLine($"OnListViewItemGotFocus({listViewItem})");
      if (Mouse.LeftButton == MouseButtonState.Pressed && GetIsItemSelected(listViewItem) && Keyboard.Modifiers != ModifierKeys.Control)
      {
        _selectListViewItemOnMouseUp = listViewItem;
      }
      else
        e.Handled = SelectItems(listViewItem, sender as ListView);
      //Debug.WriteLine($"GotFocus handled={e.Handled}");
    }

    public static bool SelectItems(ListViewItem listViewItem, ListView listView)
    {
      if (listViewItem != null && listView != null)
      {
        if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
        {
          return SelectMultipleItemsContinuously(listView, listViewItem, true);
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control)
        {
          return SelectMultipleItemsRandomly(listView, listViewItem);
        }
        else if (Keyboard.Modifiers == ModifierKeys.Shift)
        {
          return SelectMultipleItemsContinuously(listView, listViewItem);
        }
        else
        {
          return SelectSingleItem(listView, listViewItem);
        }
      }
      return false;
    }

    public static void OnListViewItemPreviewMouseDown(object sender, MouseEventArgs e)
    {
      var listViewItem = FindListViewItem(e.OriginalSource as DependencyObject);
      if (listViewItem != null && listViewItem.IsFocused)
        OnListViewItemGotFocus(sender, e);
    }

    public static void OnListViewItemPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      var listViewItem = FindListViewItem(e.OriginalSource as DependencyObject);
      if (listViewItem == _selectListViewItemOnMouseUp)
      {
        SelectItems(listViewItem, sender as ListView);
      }
    }

    public static ListViewItem FindListViewItem(DependencyObject dependencyObject)
    {
      if (!(dependencyObject is Visual || dependencyObject is Visual3D))
        return null;

      var ListViewItem = dependencyObject as ListViewItem;
      if (ListViewItem != null)
      {
        return ListViewItem;
      }

      return FindListViewItem(VisualTreeHelper.GetParent(dependencyObject));
    }

    public static bool SelectSingleItem(ListView listView, ListViewItem listViewItem)
    {
      //Debug.WriteLine($"SelectSingleItem({listViewItem})");
      DeselectAllItems(listView);
      if (listView.ItemsSource is IListSelector listSelector)
        listSelector.SelectItem(listViewItem.DataContext ?? listViewItem, true);
      else
        SetIsItemSelected(listViewItem, true);
      SetStartItem(listView, listViewItem);
      return true;
    }

    public static void DeselectAllItems(ListView listView)
    {
      if (listView != null)
      {
        if (listView.ItemsSource is IListSelector listSelector)
        {
          listSelector.SelectAll(false);
        }
        else
        {
          for (int i = 0; i < listView.Items.Count; i++)
          {
            var item = listView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
            if (item != null)
            {
              SetIsItemSelected(item, false);
            }
          }
        }
      }
    }

    public static void DeselectAllItemsExcept(ListView listView, IEnumerable<ListViewItem> exceptViewItems)
    {
      if (listView != null)
      {
        for (int i = 0; i < listView.Items.Count; i++)
        {
          var dataItem = listView.Items[i];
          var viewItem = listView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
          if (viewItem != null && !exceptViewItems.Contains(viewItem))
          {
            SetIsItemSelected(viewItem, false);
          }
          else
            if (listView.ItemsSource is IListSelector listSelector)
          {
            listSelector.SelectItem(dataItem,false);
          }
        }
      }
    }

    public static ListView FindListView(DependencyObject dependencyObject)
    {
      if (dependencyObject == null)
      {
        return null;
      }

      var ListView = dependencyObject as ListView;

      return ListView ?? FindListView(VisualTreeHelper.GetParent(dependencyObject));
    }

    public static bool SelectMultipleItemsRandomly(ListView listView, ListViewItem listViewItem)
    {
      //Debug.WriteLine($"SelectMultipleItemsRandomly({listViewItem})");
      // Do not change IsSelected here as it conflicts with ListView original behavior
      //SetIsItemSelected(listViewItem, !GetIsItemSelected(listViewItem));
      if (GetStartItem(listView) == null || Keyboard.Modifiers == ModifierKeys.Control)
      {
        if (GetIsItemSelected(listViewItem))
        {
          SetStartItem(listView, listViewItem);
        }
      }
      else
      {
        if (GetSelectedItems(listView).Count == 0)
        {
          SetStartItem(listView, null);
        }
      }
      return true;
    }

    public static bool SelectMultipleItemsContinuously(ListView listView, ListViewItem listViewItem, bool shiftControl = false)
    {
      //Debug.WriteLine($"SelectMultipleItemsContinuously({listViewItem})");
      ListViewItem startItem = GetStartItem(listView);
      if (startItem != null)
      {
        if (startItem == listViewItem)
        {
          SelectSingleItem(listView, listViewItem);
          return true;
        }

        ICollection<ListViewItem> allItems = new List<ListViewItem>();
        GetAllItems(listView, allItems);
        DeselectAllItemsExcept(listView, allItems);
        bool isBetween = false;
        foreach (var item in allItems)
        {
          if (item == listViewItem || item == startItem)
          {
            // toggle to true if first element is found and
            // back to false if last element is found
            isBetween = !isBetween;

            // set boundary element
            SetIsItemSelected(item, true);
            continue;
          }

          if (isBetween)
          {
            SetIsItemSelected(item, true);
            continue;
          }

          if (!shiftControl)
            SetIsItemSelected(item, false);
        }
        return true;
      }
      return false;
    }

    public static void GetAllItems(ListView listView, ICollection<ListViewItem> allItems)
    {
      if (listView != null)
      {
        for (int i = 0; i < listView.Items.Count; i++)
        {
          var item = listView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
          if (item != null)
          {
            allItems.Add(item);
          }
        }
      }
    }

  }
}
