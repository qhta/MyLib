namespace Qhta.WPF.Utils;

public partial class ListViewBehavior
{
  private static ListViewItem? _selectListViewItemOnMouseUp;

  #region MultiSelect property
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

  private static void MultiSelectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
  #endregion

  #region IsItemSelected property

  /// <summary>
  /// Getter for IsItemSelected property.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool GetIsItemSelected(ListViewItem element)
  {
    //return element.IsSelected;
    return (bool)element.GetValue(IsItemSelectedProperty);
  }

  /// <summary>
  /// Setter for IsItemSelected property.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetIsItemSelected(ListViewItem element, Boolean value)
  {
    if (element == null) return;
    element.IsSelected = value;
    element.SetValue(IsItemSelectedProperty, value);
  }

  /// <summary>
  /// Dependency property for IsItemSelected property
  /// </summary>
  public static readonly DependencyProperty IsItemSelectedProperty = DependencyProperty.RegisterAttached
    ("IsItemSelected", typeof(Boolean), typeof(ListViewBehavior), new PropertyMetadata(false, OnIsItemSelectedPropertyChanged));

  private static void OnIsItemSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    var listViewItem = d as ListViewItem;
    if (listViewItem == null) return;
    var listView = FindParentListView(listViewItem);
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
  #endregion

  #region SelectedItems property

  /// <summary>
  /// Getter for SelectedItems property.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static IList GetSelectedItems(ListView element)
  {
    return (IList)element.GetValue(SelectedItemsProperty);
  }

  /// <summary>
  /// Setter for SelectedItems property.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetSelectedItems(ListView element, IList value)
  {
    element.SetValue(SelectedItemsProperty, value);
  }

  /// <summary>
  /// Dependency property to store SelectedItems property.
  /// </summary>
  public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached
    ("SelectedItems", typeof(IList), typeof(ListViewBehavior));

  #endregion

  #region CurrentItem property

  /// <summary>
  /// Getter for CurrentItem property.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static object GetCurrentItem(ListView element)
  {
    var value = element.GetValue(CurrentItemProperty);
    //Debug.WriteLine($"ListViewBehavior.GetCurrentItem({value})");
    return value;

  }

  /// <summary>
  /// Setter for CurrentItem property.
  /// </summary>
  /// <param name="listView"></param>
  /// <param name="value"></param>
  public static void SetCurrentItem(ListView listView, object value)
  {
    //Debug.WriteLine($"ListViewBehavior.SetCurrentItem({value})");
    listView.SetValue(CurrentItemProperty, value);
  }

  /// <summary>
  /// Updater for CurrentItem property.
  /// </summary>
  /// <param name="listView"></param>
  /// <param name="value"></param>
  public static void UpdateCurrentItem(ListView listView, object value)
  {
    //Debug.WriteLine($"ListViewBehavior.UpdateCurrentItem({value})");
    listView.SetCurrentValue(CurrentItemProperty, value);
  }
  /// <summary>
  /// Dependency property to store CurrentItem property.
  /// </summary>
  public static readonly DependencyProperty CurrentItemProperty = DependencyProperty.RegisterAttached
    ("CurrentItem", typeof(object), typeof(ListViewBehavior), new PropertyMetadata(null, CurrentItemChanged));


  #endregion

  #region UseCurrentItemChangeEvent property

  /// <summary>
  /// Setter for UseCurrentItemChangedEvent property.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetUseCurrentItemChangeEvent(DependencyObject obj)
  {
    return (bool)obj.GetValue(UseCurrentItemChangeEventProperty);
  }

  /// <summary>
  /// Getter for UseCurrentItemChangeEvent property
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetUseCurrentItemChangeEvent(DependencyObject obj, bool value)
  {
    obj.SetValue(UseCurrentItemChangeEventProperty, value);
  }

  /// <summary>
  /// Dependency property to store UseCurrentItemChangeEvent property.
  /// </summary>
  public static readonly DependencyProperty UseCurrentItemChangeEventProperty = DependencyProperty.RegisterAttached
    ("UseCurrentItemChangeEvent", typeof(bool), typeof(ListViewBehavior),
        new UIPropertyMetadata(false));
  #endregion

  #region CurrentItemChange event

  /// <summary>
  /// Add method for CurrentItemChange event handler.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void AddCurrentItemChangeHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.AddHandler(ListViewBehavior.CurrentItemChangeEvent, handler);
  }

  /// <summary>
  /// Remove method for CurrentItemChange event handler.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void RemoveCurrentItemChangeHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.RemoveHandler(ListViewBehavior.CurrentItemChangeEvent, handler);
  }

  /// <summary>
  /// Routed event to store CurrentItemChange event handler.
  /// </summary>
  public static readonly RoutedEvent CurrentItemChangeEvent = EventManager.RegisterRoutedEvent
    ("CurrentItemChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListViewBehavior));


  private static void CurrentItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
  {
    //Debug.WriteLine($"ListViewBehavior.CurrentItemChanged({obj})");
    if (obj is ListView listView)
    {
      if (GetUseCurrentItemChangeEvent(listView))
        listView.RaiseEvent(new RoutedPropertyChangedEventArgs<object>(args.OldValue, args.NewValue)
        { RoutedEvent = ListViewBehavior.CurrentItemChangeEvent, Source = obj });
    }
  }
  #endregion

  #region StartItem property

  /// <summary>
  /// Getter for StartItem property.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static object? GetStartItem(ListView element)
  {
    return element.GetValue(StartItemProperty);
  }

  /// <summary>
  /// Setter for StartItem property.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetStartItem(ListView element, object? value)
  {
    element.SetValue(StartItemProperty, value);
  }

  /// <summary>
  /// Dependency property to store StartItem property.
  /// </summary>
  public static readonly DependencyProperty StartItemProperty = DependencyProperty.RegisterAttached
    ("StartItem", typeof(object), typeof(ListViewBehavior));

  #endregion

  #region ListViewItem event handlers

  /// <summary>
  /// Handle method for ListViewItem GotFocus event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  public static void OnListViewItemGotFocus(object sender, RoutedEventArgs args)
  {
    _selectListViewItemOnMouseUp = null;

    if (args.OriginalSource is ListView) return;

    if (sender is ListView listView && args.OriginalSource is DependencyObject dependencyObjectSource)
    {
      var listViewItem = FindParentListViewItem(dependencyObjectSource);
      if (listViewItem != null)
      {
        //Debug.WriteLine($"OnListViewItemGotFocus({listViewItem})");
        if (Mouse.LeftButton == MouseButtonState.Pressed && GetIsItemSelected(listViewItem) && Keyboard.Modifiers != ModifierKeys.Control)
          _selectListViewItemOnMouseUp = listViewItem;
        else
          args.Handled = SelectItems(listView, listViewItem);
      }
    }
    //Debug.WriteLine($"GotFocus handled={e.Handled}");
  }

  /// <summary>
  /// Handle method for ListViewItem PreviewMouseDown event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  public static void OnListViewItemPreviewMouseDown(object sender, MouseEventArgs args)
  {
    if (args.OriginalSource is DependencyObject dependencyObjectSource)
    {
      var listViewItem = FindParentListViewItem(dependencyObjectSource);
      if (listViewItem != null && listViewItem.IsFocused)
        OnListViewItemGotFocus(sender, args);
    }
  }

  /// <summary>
  /// Handle method for ListViewItem PreviewMouseUp event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  public static void OnListViewItemPreviewMouseUp(object sender, MouseButtonEventArgs args)
  {
    if (sender is ListView listView && args.OriginalSource is DependencyObject dependencyObjectSource)
    {
      var listViewItem = FindParentListViewItem(dependencyObjectSource);
      if (listViewItem != null && listViewItem == _selectListViewItemOnMouseUp)
      {
        SelectItems(listView, listViewItem);
      }
    }
  }
  #endregion

  #region Find methods

  /// <summary>
  /// Helper method to find parent ListView of the dependency object.
  /// </summary>
  /// <param name="dependencyObject"></param>
  /// <returns></returns>
  public static ListView? FindParentListView(DependencyObject dependencyObject)
  {
    if (dependencyObject == null)
    {
      return null;
    }
    var ListView = dependencyObject as ListView;
    return ListView ?? FindParentListView(VisualTreeHelper.GetParent(dependencyObject));
  }

  /// <summary>
  /// Helper method to find parent ListViewItem of the dependency object.
  /// </summary>
  /// <param name="dependencyObject"></param>
  /// <returns></returns>
  public static ListViewItem? FindParentListViewItem(DependencyObject dependencyObject)
  {
    if (!(dependencyObject is Visual || dependencyObject is Visual3D))
      return null;
    var ListViewItem = dependencyObject as ListViewItem;
    if (ListViewItem != null)
    {
      return ListViewItem;
    }
    return FindParentListViewItem(VisualTreeHelper.GetParent(dependencyObject));
  }

  #endregion

  #region SelectItems methods

  /// <summary>
  /// Helper method to select items in a ListView when a ListViewItem is currently pointed by mouse.
  /// </summary>
  /// <param name="listView"></param>
  /// <param name="listViewItem"></param>
  /// <returns></returns>
  public static bool SelectItems(ListView listView, ListViewItem listViewItem)
  {
    if (listViewItem != null && listView != null)
    {
      if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
      {
        return SelectMultipleItemsContinuously(listView, listViewItem, true);
      }
      else if (Keyboard.Modifiers == ModifierKeys.Control)
      {
        return SelectMultipleItems(listView, listViewItem);
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

  /// <summary>
  /// Helper method to select a single item in a ListView when a ListViewItem is currently pointed by mouse.
  /// </summary>
  /// <param name="listView"></param>
  /// <param name="listViewItem"></param>
  /// <returns></returns>
  public static bool SelectSingleItem(ListView listView, ListViewItem listViewItem)
  {
    //Debug.WriteLine($"SelectSingleItem({listViewItem})");
    DeselectAllItems(listView);
    if (listView.ItemsSource is IListSelector listSelector)
      listSelector.SelectItem(listViewItem.DataContext ?? listViewItem, true);
    else
      SetIsItemSelected(listViewItem, true);
    SetStartItem(listView, listViewItem.DataContext ?? listViewItem);
    UpdateCurrentItem(listView, listViewItem.DataContext ?? listViewItem);
    return true;
  }

  /// <summary>
  /// Helper method to deselect all items in a ListView.
  /// </summary>
  /// <param name="listView"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Helper method to deselect all items in a ListView except specific view items list.
  /// </summary>
  /// <param name="listView"></param>
  /// <param name="exceptViewItems"></param>
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
          listSelector.SelectItem(dataItem, false);
        }
      }
    }
  }

  /// <summary>
  /// Helper method to select multiple items in a list view when ListViewItem is currently pointed by mouse.
  /// </summary>
  /// <param name="listView"></param>
  /// <param name="listViewItem"></param>
  /// <returns></returns>
  public static bool SelectMultipleItems(ListView listView, ListViewItem listViewItem)
  {
    //Debug.WriteLine($"SelectMultipleItemsRandomly({listViewItem})");
    // Do not change IsSelected here as it conflicts with ListView original behavior
    //SetIsItemSelected(listViewItem, !GetIsItemSelected(listViewItem));
    if (GetStartItem(listView) == null || Keyboard.Modifiers == ModifierKeys.Control)
    {
      if (GetIsItemSelected(listViewItem))
      {
        SetStartItem(listView, listViewItem.DataContext ?? listViewItem);
      }
    }
    else
    {
      if (GetSelectedItems(listView).Count == 0)
      {
        SetStartItem(listView, null);
      }
    }
    UpdateCurrentItem(listView, listViewItem.DataContext ?? listViewItem);
    return true;
  }

  /// <summary>
  /// Helper method to select multiple items in a list view when ListViewItem is currently pointed by mouse.
  /// All items from StartItem to the current list view item are selected.
  /// </summary>
  /// <param name="listView"></param>
  /// <param name="listViewItem"></param>
  /// <param name="shiftControl">Is Shift key is currently pressed</param>
  /// <returns></returns>
  public static bool SelectMultipleItemsContinuously(ListView listView, ListViewItem listViewItem, bool shiftControl = false)
  {
    //Debug.WriteLine($"SelectMultipleItemsContinuously({listViewItem})");
    object? startItem = GetStartItem(listView);
    if (startItem != null)
    {
      if (startItem == listViewItem || startItem == listViewItem.DataContext)
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
        if (item == listViewItem || item == startItem || item.DataContext == startItem)
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
      UpdateCurrentItem(listView, listViewItem.DataContext ?? listViewItem);
      return true;
    }
    return false;
  }

  /// <summary>
  /// Helper method to get all items of the ListView in the order specified by ItemContainerGenerator.
  /// </summary>
  /// <param name="listView"></param>
  /// <param name="allItems"></param>
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
  #endregion

}
