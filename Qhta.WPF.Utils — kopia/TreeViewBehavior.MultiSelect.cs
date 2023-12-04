namespace Qhta.WPF.Utils;

public static partial class TreeViewBehavior
{
  private static TreeViewItem? _selectTreeViewItemOnMouseUp;

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

  private static void MultiSelectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
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

  /// <summary>
  /// Getter for IsItemSelected property.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static bool GetIsItemSelected(TreeViewItem element)
  {
    return (bool)element.GetValue(IsItemSelectedProperty);
  }

  /// <summary>
  /// Setter for IsItemSelected property.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetIsItemSelected(TreeViewItem element, Boolean value)
  {
    if (element == null) return;
    element.SetValue(IsItemSelectedProperty, value);
  }

  /// <summary>
  /// Dependency property to store IsItemSelected property.
  /// </summary>
  public static readonly DependencyProperty IsItemSelectedProperty = DependencyProperty.RegisterAttached
    ("IsItemSelected", typeof(Boolean), typeof(TreeViewBehavior), new PropertyMetadata(false, OnIsItemSelectedPropertyChanged));

  private static void OnIsItemSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is TreeViewItem treeViewItem)
    {
      var treeView = FindTreeView(treeViewItem);
      if (treeView != null)
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
  }
  #endregion

  #region SelectedItems property

  /// <summary>
  /// Getter for SelectedItems property.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static IList GetSelectedItems(TreeView element)
  {
    return (IList)element.GetValue(SelectedItemsProperty);
  }

  /// <summary>
  /// Setter for SelectedItmes property.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetSelectedItems(TreeView element, IList value)
  {
    element.SetValue(SelectedItemsProperty, value);
  }

  /// <summary>
  /// Dependency property to store SelectedItems property.
  /// </summary>
  public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached
    ("SelectedItems", typeof(IList), typeof(TreeViewBehavior));

  #endregion

  #region CurrentItem property

  /// <summary>
  /// Getter for CurrentItem property.
  /// </summary>
  /// <param name="treeView"></param>
  /// <returns></returns>
  public static object CurrentItem(TreeView treeView)
  {
    return treeView.GetValue(CurrentItemProperty);
  }

  /// <summary>
  /// Setter for CurrentItem property.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="treeView"></param>
  public static void SetCurrentItem(TreeView obj, object treeView)
  {
    if (treeView != null)
    {
      //Debug.WriteLine($"TreeViewItemBehavior.SetSelectedItem({value})");
      obj.SetValue(CurrentItemProperty, treeView);
    }
  }

  /// <summary>
  /// Update method for CurrentItem.
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="value"></param>
  public static void UpdateCurrentItem(TreeView treeView, object value)
  {
    //Debug.WriteLine($"TreeViewBehavior.UpdateCurrentItem({value})");
    treeView.SetCurrentValue(CurrentItemProperty, value);
  }

  /// <summary>
  /// Dependency property to store CurrentItem property.
  /// </summary>
  public static readonly DependencyProperty CurrentItemProperty = DependencyProperty.RegisterAttached
    ("CurrentItem", typeof(object), typeof(TreeViewBehavior),
        new UIPropertyMetadata(null, CurrentItemChanged));

  #endregion

  #region UseCurrentItemChangedEvent property

  /// <summary>
  /// Getter for UseCurrentItemChangeEvent property.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetUseCurrentItemChangeEvent(DependencyObject obj)
  {
    return (bool)obj.GetValue(UseCurrentItemChangeEventProperty);
  }

  /// <summary>
  /// Setter for UseCurrentItemChangeEvent property.
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
    ("UseCurrentItemChangeEvent", typeof(bool), typeof(TreeViewBehavior),
        new UIPropertyMetadata(false));

  #endregion

  #region CurrentItemChanged event

  /// <summary>
  /// Add method for CurrentItemChange event handler.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void AddCurrentItemChangeHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.AddHandler(TreeViewBehavior.CurrentItemChangeEvent, handler);
  }

  /// <summary>
  /// Remove method for CurrentItemChange event handler.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void RemoveCurrentItemChangeHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.RemoveHandler(TreeViewBehavior.CurrentItemChangeEvent, handler);
  }

  /// <summary>
  /// Routed event to store CurrentItemChange event.
  /// </summary>
  public static readonly RoutedEvent CurrentItemChangeEvent = EventManager.RegisterRoutedEvent
    ("CurrentItemChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewBehavior));

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

  /// <summary>
  /// Getter for StartItem property.
  /// </summary>
  /// <param name="element"></param>
  /// <returns></returns>
  public static object? GetStartItem(TreeView element)
  {
    return element.GetValue(StartItemProperty);
  }

  /// <summary>
  /// Setter for StartItem property.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="value"></param>
  public static void SetStartItem(TreeView element, object? value)
  {
    //Debug.WriteLine($"TreeViewBehavior.MultiSelect.SetStartItem({value})");
    element.SetValue(StartItemProperty, value);
  }

  /// <summary>
  /// Dependency property to store StartItem property.
  /// </summary>
  public static readonly DependencyProperty StartItemProperty = DependencyProperty.RegisterAttached
    ("StartItem", typeof(object), typeof(TreeViewBehavior));
  #endregion

  #region TreeViewItem event handlers

  private static void OnTreeViewItemPreviewMouseDown(object sender, MouseEventArgs args)
  {
    if (args.OriginalSource is DependencyObject dependencyObject)
    {
      var treeViewItem = FindTreeViewItem(dependencyObject);

      if (treeViewItem != null && treeViewItem.IsFocused)
        OnTreeViewItemGotFocus(sender, args);
    }
  }

  private static void OnTreeViewItemPreviewMouseUp(object sender, MouseButtonEventArgs args)
  {
    if (args.OriginalSource is DependencyObject dependencyObject)
    {
      var treeViewItem = FindTreeViewItem(dependencyObject);


      if (treeViewItem == _selectTreeViewItemOnMouseUp && sender is TreeView treeView)
      {
        SelectItems(treeView, treeViewItem);
      }
    }
  }

  private static void OnTreeViewItemGotFocus(object sender, RoutedEventArgs args)
  {
    if (args.OriginalSource is DependencyObject dependencyObject)
    {
      _selectTreeViewItemOnMouseUp = null;

      if (args.OriginalSource is TreeView) return;

      var treeViewItem = FindTreeViewItem(dependencyObject);
      if (treeViewItem != null)
      {
        if (Mouse.LeftButton == MouseButtonState.Pressed && GetIsItemSelected(treeViewItem) && Keyboard.Modifiers != ModifierKeys.Control)
        {
          _selectTreeViewItemOnMouseUp = treeViewItem;
          return;
        }
      }
      if (sender is TreeView treeView)
      {
        if (SelectItems(treeView, treeViewItem))
        {
          if (treeView.ItemsSource is IListSelector listSelector)
            listSelector.NotifySelectionChanged();
        }
      }
    }
  }
  #endregion

  #region Find methods

  /// <summary>
  /// Find TreeViewItem parent of the dependency object.
  /// </summary>
  /// <param name="dependencyObject"></param>
  /// <returns></returns>
  public static TreeViewItem? FindTreeViewItem(DependencyObject dependencyObject)
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

  /// <summary>
  /// Find TreeView parent of the dependency object.
  /// </summary>
  /// <param name="dependencyObject"></param>
  /// <returns></returns>
  public static TreeView? FindTreeView(DependencyObject dependencyObject)
  {
    if (dependencyObject == null)
      return null;
    var treeView = dependencyObject as TreeView;
    return treeView ?? FindTreeView(VisualTreeHelper.GetParent(dependencyObject));
  }
  #endregion

  #region SelectItems methods

  /// <summary>
  /// Selecte items of the tree view.
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="treeViewItem"></param>
  /// <returns></returns>
  public static bool SelectItems(TreeView treeView, TreeViewItem? treeViewItem)
  {
    if (treeViewItem != null && treeView != null)
    {
      if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
      {
        return SelectMultipleItemsContinuously(treeView, treeViewItem, true);
      }
      else if (Keyboard.Modifiers == ModifierKeys.Control)
      {
        return SelectMultipleItems(treeView, treeViewItem);
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

  /// <summary>
  /// Select a single tree view item.
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="treeViewItem"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Deselect all tree view items.
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="treeViewItem"></param>
  public static void DeselectAllItems(TreeView? treeView, TreeViewItem? treeViewItem)
  {
    if (treeView != null && treeView.ItemsSource is IListSelector listSelector)
    {
      listSelector.SelectAll(false);
    }
    else if (treeViewItem != null)
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

  /// <summary>
  /// Selects multiple tree view items
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="treeViewItem"></param>
  /// <returns></returns>
  public static bool SelectMultipleItems(TreeView treeView, TreeViewItem treeViewItem)
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

  /// <summary>
  /// Selects multiple tree view items in a continuous range.
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="treeViewItem"></param>
  /// <param name="shiftControl"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Get all items of the tree view.
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="treeViewItem"></param>
  /// <param name="allItems"></param>
  public static void GetAllItems(TreeView? treeView, TreeViewItem? treeViewItem, ICollection<TreeViewItem> allItems)
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
    else if (treeViewItem!=null)
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
