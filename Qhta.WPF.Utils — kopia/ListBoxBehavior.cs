namespace Qhta.WPF.Utils;

/// <summary>
/// Behavior class that defines attached property for ListBox. It is:
/// <list type="table">
///   <item><term>MoveItemsEnabled</term><description>Enables moving items</description></item>
/// </list>
/// </summary>
public class ListBoxBehavior
{

  #region MoveItemsEnabled property
  /// <summary>
  ///   MoveItemsEnabled behavior allows ListBoxItems to be drag and drop inside list with mouse.
  /// </summary>
  public static bool GetMoveItemsEnabled(DependencyObject obj)
  {
    return (bool)obj.GetValue(MoveItemsEnabledProperty);
  }

  /// <summary>
  ///   For MoveItemsEnabled behavior list view items source should implement IListSelector interface.
  /// </summary>
  public static void SetMoveItemsEnabled(DependencyObject obj, bool value)
  {
    obj.SetValue(MoveItemsEnabledProperty, value);
  }

  /// <summary>
  ///   For MoveItemsEnabled behavior list view items source should implement IListSelector interface.
  /// </summary>
  public static readonly DependencyProperty MoveItemsEnabledProperty = DependencyProperty.RegisterAttached
    ("MoveItemsEnabled", typeof(bool), typeof(ListBoxBehavior),
        new UIPropertyMetadata(false, MoveItemsEnabledChanged));

  private static void MoveItemsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
  {
    if (obj is ListBox listBox)
    {
      if ((bool)args.NewValue)
      {
        listBox.SelectionChanged += ListBox_SelectionChanged;
        listBox.PreviewMouseLeftButtonDown += OnListBoxPreviewMouseDown;
        listBox.PreviewMouseMove += OnListBox_PreviewMouseMove;
        listBox.PreviewMouseLeftButtonUp += OnListBoxPreviewMouseUp;
        listBox.PreviewDragOver += ListBox_PreviewDragOver;
        listBox.PreviewDrop += ListBox_Drop;
      }
      else
      {
        listBox.SelectionChanged -= ListBox_SelectionChanged;
        listBox.PreviewMouseLeftButtonDown -= OnListBoxPreviewMouseDown;
        listBox.PreviewMouseMove -= OnListBox_PreviewMouseMove;
        listBox.PreviewMouseLeftButtonUp -= OnListBoxPreviewMouseUp;
        listBox.PreviewDragOver -= ListBox_PreviewDragOver;
        listBox.PreviewDrop -= ListBox_Drop;
      }
    }
  }

  private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
  {
    //if (args.AddedItems != null)
    //  foreach (var item in args.AddedItems)
    //    //Debug.WriteLine($"SelectedItems.Added({item})");
    //if (args.RemovedItems != null)
    //  foreach (var item in args.RemovedItems)
    //    //Debug.WriteLine($"SelectedItems.Removed({item})");
  }
  #endregion

  #region Find methods

  /// <summary>
  /// Finds a parent that is a ListBox of the dependency object.
  /// </summary>
  /// <param name="dependencyObject"></param>
  /// <returns></returns>
  public static ListBox? FindListBox(DependencyObject dependencyObject)
  {
    if (dependencyObject == null)
    {
      return null;
    }
    var listBox = dependencyObject as ListBox;
    return listBox ?? FindListBox(VisualTreeHelper.GetParent(dependencyObject));
  }


  /// <summary>
  /// Finds a parent that is a ListBoxItem of the dependency object.
  /// </summary>
  /// <param name="dependencyObject"></param>
  /// <returns></returns>
  public static ListBoxItem? FindListBoxItem(DependencyObject dependencyObject)
  {
    if (!(dependencyObject is Visual || dependencyObject is Visual3D))
      return null;
    var ListBoxItem = dependencyObject as ListBoxItem;
    if (ListBoxItem != null)
      return ListBoxItem;
    return FindListBoxItem(VisualTreeHelper.GetParent(dependencyObject));
  }

  #endregion

  #region Selector duplicated code - methods are made static

  private static Point? _lastMousePosition;
  private static ListBox? _lastListBox;
  private static ListBoxItem? _lastListBoxItem;

  internal static void SetInitialMousePosition(ListBox listBox, ListBoxItem item, Point pos)
  {
    _lastMousePosition = pos;
    _lastListBox = listBox;
    _lastListBoxItem = item;
  }

  const int moveStartDelta = 3;

  internal static bool DidMouseMove(ListBox listBox)
  {
    Point position = Mouse.GetPosition(listBox);
    if (Math.Abs(position.X - _lastMousePosition?.X ?? 0) >= moveStartDelta
     || Math.Abs(position.Y - _lastMousePosition?.Y ?? 0) >= moveStartDelta)
    {
      _lastMousePosition = position;
      return true;
    }

    return false;
  }

  internal static void ResetLastMousePosition()
  {
    _lastMousePosition = default(Point);
    _lastListBox = null;
    _lastListBoxItem = null;
  }
  #endregion

  #region access code for private method of ListBox 
  internal static void NotifyListItemClicked(ListBox listBox, ListBoxItem? item, MouseButton mouseButton)
  {
    var privateMethod = typeof(ListBox).GetMethod("NotifyListItemClicked", BindingFlags.NonPublic | BindingFlags.Instance);
    privateMethod?.Invoke(listBox, new object?[] { item, mouseButton });
  }

  #endregion

  #region MouseEvents handling

  private static void OnListBoxPreviewMouseDown(object sender, MouseEventArgs args)
  {
    if (sender is ListBox listBox && args.LeftButton.HasFlag(MouseButtonState.Pressed))
    {
      var pos = args.GetPosition(listBox);
      ListBoxItem? item = GetItemFromListBox(listBox, pos);
      var selectedItems = listBox.SelectedItems;
      if (item != null)
      {
        //Debug.WriteLine($"Click with {Keyboard.Modifiers} on {item}");
        SetInitialMousePosition(listBox, item, pos);
        if (item.IsSelected)
        {
          _lastListBoxItem = item;
          // We must delay deselecting clicked item as user may want to move it
          InitTimer(args);
        }
        else
        {
          KillTimer();
          // Do not call Mouse.Capture here - see NotifyListItemClicked
          NotifyListItemClicked(listBox, item, MouseButton.Left);
        }
        Mouse.Capture(listBox);
      }
    }
  }

  private static void OnListBox_PreviewMouseMove(object sender, MouseEventArgs args)
  {
    if (sender is ListBox listBox)
    {
      if (args.LeftButton.HasFlag(MouseButtonState.Pressed) && Mouse.Captured == listBox)
      {
        if (DidMouseMove(listBox))
        {
          KillTimer();
          if (_lastMousePosition != null)
          {
            object? data = GetDataFromListBox(listBox, (Point)_lastMousePosition);
            StartDrag(new DragData { Source = listBox, Data = data });
            ResetLastMousePosition();
          }
        }
      }
    }
  }

  private static void OnListBoxPreviewMouseUp(object sender, MouseButtonEventArgs args)
  {
    if (sender is ListBox listBox)
    {
      if (args.LeftButton.HasFlag(MouseButtonState.Pressed) && Mouse.Captured == listBox)
      {
        KillTimer();
        ResetLastMousePosition();
        Mouse.Capture(null);
      }
    }
  }

  private static ListBoxItem? GetItemFromListBox(ListBox listBox, Point point)
  {
    UIElement? element = listBox.InputHitTest(point) as UIElement;
    if (element != null)
    {
      while (!(element is ListBoxItem) && !(element is ListBox))
      {
        element = VisualTreeHelper.GetParent(element) as UIElement;
      }
      return element as ListBoxItem;
    }
    return null;
  }

  private static object? GetDataFromListBox(ListBox listBox, Point point)
  {
    if (listBox.SelectedItems.Count > 1)
    {
      var selectedItems = new List<object>();
      foreach (var item in listBox.SelectedItems)
      {
        selectedItems.Add(item);
      }
      return selectedItems;
    }
    else
    {
      var item = GetItemFromListBox(listBox, point);
      if (item != null)
        return item.DataContext ?? item;
    }
    return null;
  }
  #endregion

  #region Timer handling

  private static System.Threading.Timer? Timer;

  private static void InitTimer(object data)
  {
    Timer = new System.Threading.Timer(OnTimer, data, 500, Timeout.Infinite);
    //Debug.WriteLine($"Timer set");
  }

  private static void OnTimer(object? data)
  {
    //Debug.WriteLine($"Timer activated");
    if (data is MouseEventArgs args)
      _lastListBox?.Dispatcher.Invoke(() =>
      {
        NotifyListItemClicked(_lastListBox, _lastListBoxItem, MouseButton.Left);
      });
    KillTimer();
  }

  private static void KillTimer()
  {
    if (Timer != null)
    {
      Timer.Change(Timeout.Infinite, Timeout.Infinite);
      Timer = null;
      //Debug.WriteLine($"Timer killed");
    }
  }


  private static void StartDrag(DragData dragData)
  {
    LocalDragData = dragData;
    if (dragData.Source is ListBox listBox)
    {
      //Debug.WriteLine($"StartDrag ({dragData.Source}, {dragData.Data})");
      listBox.Dispatcher.Invoke(() =>
      {
        DragDrop.DoDragDrop(listBox, dragData, DragDropEffects.Move);
      });
    }
  }
  static DragData? LocalDragData;

  #endregion

  #region DragDrop event handlers

  private static void ListBox_PreviewDragOver(object sender, DragEventArgs args)
  {
    //Debug.WriteLine($"DragOver({sender})");
    if (sender is ListBox listBox)
    {
      var pos = args.GetPosition(listBox);
      var item = GetItemFromListBox(listBox, pos);
      object? targetItem;
      if (item != null)
      {
        targetItem = item.DataContext ?? item;
        //Debug.WriteLine($"can drop before item {targetItem}");
        ShowDropAtItem(listBox, item, false);
      }
      else
      {
        //Debug.WriteLine($"drag over {listBox}");
        int n = listBox.Items.Count;
        if (n > 0)
        {
          targetItem = listBox.Items[n - 1] as DependencyObject;
          //Debug.WriteLine($"can drop after item {targetItem}");
          item = targetItem as ListBoxItem;
          if (item == null)
            item = listBox.ItemContainerGenerator.ContainerFromItem(targetItem) as ListBoxItem;
        }
        if (item != null)
          ShowDropAtItem(listBox, item, true);
        else
        {
          Debug.WriteLine($"Pos.Y={pos.Y}");
          ShowDropIn(listBox, true);
        }

      }
    }
  }

  private static void ListBox_Drop(object sender, DragEventArgs args)
  {
    if (sender is ListBox listBox)
    {
      Mouse.Capture(null);
      try
      {
        HideDropPlaceholder(listBox);
        var pos = args.GetPosition(listBox);
        var item = GetItemFromListBox(listBox, pos);
        object? targetItem;
        if (item != null)
        {
          targetItem = item.DataContext ?? item;
          //Debug.WriteLine($"drop before item {targetItem}");
          DropAtItem(listBox, item, (DragData)args.Data.GetData(typeof(DragData)), false);
        }
        else
        {
          int n = listBox.Items.Count;
          if (n > 0)
          {
            targetItem = listBox.Items[n - 1] as DependencyObject;
            //Debug.WriteLine($"drop after item {targetItem}");
            item = targetItem as ListBoxItem;
            if (item == null)
              item = listBox.ItemContainerGenerator.ContainerFromItem(targetItem) as ListBoxItem;
          }
          if (item != null)
            DropAtItem(listBox, item, (DragData)args.Data.GetData(typeof(DragData)), true);
          else
          {
            DropIn(listBox, (DragData)args.Data.GetData(typeof(DragData)), true);
          }

        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"{ex.GetType().Name} in ListBoxBehavior.Drop: {ex.Message}");
      }
    }
  }
  #endregion

  #region ShowDrop methods

  private static void ShowDropAtItem(ListBox listBox, ListBoxItem listBoxItem, bool after)
  {
    var adornerLayer = AdornerLayer.GetAdornerLayer(listBox);
    if (adorner != null)
    {
      if (adorner.AdornedElement == listBoxItem)
      {
        adorner.Side = (after) ? Dock.Bottom : Dock.Top;
        return;
      }
      adornerLayer.Remove(adorner);
    }
    adorner = new BorderLine(listBoxItem) { Side = (after) ? Dock.Bottom : Dock.Top };
    adornerLayer.Add(adorner);
  }

  private static void ShowDropIn(ListBox listBox, bool after)
  {
    var adornerLayer = AdornerLayer.GetAdornerLayer(listBox);
    if (adorner != null)
    {
      if (adorner.AdornedElement == adornerLayer)
      {
        adorner.Side = (after) ? Dock.Bottom : Dock.Top;
        return;
      }
      adornerLayer.Remove(adorner);
    }
    adorner = new BorderLine(adornerLayer) { Side = (after) ? Dock.Bottom : Dock.Top };
    adornerLayer.Add(adorner);
  }

  private static void HideDropPlaceholder(ListBox listBox)
  {
    var adornerLayer = AdornerLayer.GetAdornerLayer(listBox);
    if (adorner != null)
    {
      adornerLayer.Remove(adorner);
      adorner = null;
    }
  }

  static BorderLine? adorner = null;

  #endregion

  #region Drop methods

  private static void DropAtItem(ListBox targetListBox, ListBoxItem targetItem, DragData dragData, bool after)
  {
    if (dragData == LocalDragData || dragData == null)
    {
      if (LocalDragData?.Source is ListBox sourceListBox)
        if (LocalDragData.Data is IList selectedItems)
        {
          if (selectedItems == null && LocalDragData.Data != null)
          {
            selectedItems = new List<object>();
            selectedItems.Add(LocalDragData.Data);
          }
          if (selectedItems != null && !selectedItems.Contains(targetItem))
          {
            var targetObject = targetItem.DataContext ?? targetItem;
            IList targetItems = targetListBox.ItemsSource as IList ?? targetListBox.Items;
            IList sourceItems = sourceListBox.ItemsSource as IList ?? sourceListBox.Items;
            var index = targetItems.IndexOf(targetObject);
            if (after)
              index++;
            MoveItems(selectedItems, sourceItems, targetItems, index);
          }
        }
    }
  }

  private static void DropIn(ListBox targetListBox, DragData dragData, bool after)
  {
    if (dragData == LocalDragData || dragData == null)
    {
      if (LocalDragData?.Source is ListBox sourceListBox)
      {
        var sourceItem = LocalDragData.Data;
        var selectedItems = sourceListBox.SelectedItems;
        IList targetItems = targetListBox.ItemsSource as IList ?? targetListBox.Items;
        IList sourceItems = sourceListBox.ItemsSource as IList ?? sourceListBox.Items;
        var index = (after) ? targetItems.Count : 0;
        MoveItems(selectedItems, sourceItems, targetItems, index);
      }
    }

  }

  private static void MoveItems(IList items, IList sourceItems, IList targetItems, int index)
  {
    //foreach (var obj in items)
    //Debug.WriteLine($"MoveItem {obj}");
    var selectedItems = items.Cast<object>().ToList();

    foreach (var item in selectedItems)
      sourceItems.Remove(item);

    if (index >= targetItems.Count)
    {
      foreach (var item in selectedItems)
        targetItems.Add(item);
    }
    else
    {
      selectedItems.Reverse();
      foreach (var item in selectedItems)
        targetItems.Insert(index, item);
    }
  }
  #endregion
}
