using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Qhta.SF.WPF.Tools.Views;
/// <summary>
/// Window for managing columns in a data grid.
/// </summary>
public partial class ColumnManagementWindow : Window, INotifyPropertyChanged
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ColumnManagementWindow"/> class.
  /// </summary>
  public ColumnManagementWindow()
  {
    InitializeComponent();
    Loaded += ColumnManagementWindow_OnLoaded;
  }

  private void ColumnManagementWindow_OnLoaded(object sender, RoutedEventArgs e)
  {
    Title = Qhta.SF.WPF.Tools.Resources.Strings.ColumnManagement;
    ColumnsListBox.SelectionChanged += ColumnsListBox_SelectionChanged;
    // Notify property changes after the ColumnsListBox is loaded
    NotifyPropertyChanged(nameof(IsMoveUpEnabled));
    NotifyPropertyChanged(nameof(IsMoveDownEnabled));

  }

  private void ColumnsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
  {
    NotifyPropertyChanged(nameof(IsMoveUpEnabled));
    NotifyPropertyChanged(nameof(IsMoveDownEnabled));
  }

  private void MoveUpButton_OnClick(object sender, RoutedEventArgs e)
  {
    var selectedItems = ColumnsListBox.SelectedItems.Cast<ColumnSelectionItem>().ToList();
    var itemsSource = ColumnsListBox.ItemsSource as ColumnSelectionCollection;
    if (itemsSource == null)
      return;
    foreach (var item in selectedItems)
    {
      var index = itemsSource.IndexOf(item);
      if (index > 0)
      {
        //Debug.WriteLine($"RemoveItem {item} at index {index}");
        itemsSource.RemoveAt(index);
        //Debug.WriteLine($"InsertItem {item} at index {index-1}");
        itemsSource.Insert(index - 1, item);
      }
    }
    ColumnsListBox.SelectedItems.Clear();
    foreach (var item in selectedItems)
    {
      ColumnsListBox.SelectedItems.Add(item);
    }
  }

  private void MoveDownButton_OnClick(object sender, RoutedEventArgs e)
  {
    var selectedItems = ColumnsListBox.SelectedItems.Cast<ColumnSelectionItem>().ToList();
    var itemsSource = ColumnsListBox.ItemsSource as ColumnSelectionCollection;
    if (itemsSource == null)
      return;
    for (int i = selectedItems.Count - 1; i >= 0; i--)
    {
      var item = selectedItems[i];
      var index = itemsSource.IndexOf(item);
      if (index < itemsSource.Count - 1)
      {
        //Debug.WriteLine($"RemoveItem {item} at index {index}");
        itemsSource.RemoveAt(index);
        //Debug.WriteLine($"InsertItem {item} at index {index-1}");
        itemsSource.Insert(index + 1, item);
      }
    }
    ColumnsListBox.SelectedItems.Clear();
    foreach (var item in selectedItems)
    {
      ColumnsListBox.SelectedItems.Add(item);
    }
  }

  #region Drag-and-drop reordering

  private Point _dragStartPoint;
  private AdornerLayer? _adornerLayer;
  private DropPositionAdorner? _dropPositionAdorner;

  /// <summary>
  /// Left mouse button down event handler only initiates drag-and-drop operation.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private void ColumnsListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (sender is ListBox listBox)
    {
      listBox.Focus();
      e.Handled = true;
    }
    // Store the drag start point
    _dragStartPoint = e.GetPosition(null);
  }

  /// <summary>
  /// Right mouse button up event handler is used to manage selection of items in the ListBox.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private void ColumnsListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    if (sender is ListBox listBox)
    {
      // Get the clicked item
      var clickedItem = GetItemAtPosition(listBox, e.GetPosition(listBox));
      if (clickedItem != null)
      {
        if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
          // If no modifier keys are pressed, select the clicked item
          listBox.SelectedItems.Clear();
          listBox.SelectedItems.Add(clickedItem);
        }
        else if (Keyboard.Modifiers == ModifierKeys.Shift && !listBox.SelectedItems.Contains(clickedItem))
        {
          var currentItemIndex = listBox.Items.IndexOf(clickedItem);
          int? lastPriorSelectedItemIndex = null;
          int? firstNextSelectedItemIndex = null;
          for (var i = 0; i < listBox.Items.Count; i++)
          {
            var item = listBox.Items[i];
            if (listBox.SelectedItems.Contains(item))
            {
              if (i < currentItemIndex) lastPriorSelectedItemIndex = i;
              else if (i > currentItemIndex) firstNextSelectedItemIndex = i;
            }
          }
          if (lastPriorSelectedItemIndex != null || firstNextSelectedItemIndex != null)
          {
            if (lastPriorSelectedItemIndex != null && (firstNextSelectedItemIndex == null || firstNextSelectedItemIndex - currentItemIndex >= currentItemIndex - lastPriorSelectedItemIndex))
              // Select all columns from last previous selected to current
              for (var i = lastPriorSelectedItemIndex.Value + 1; i <= currentItemIndex; i++)
              {
                var item = listBox.Items[i];
                listBox.SelectedItems.Add(item);
              }
            if (firstNextSelectedItemIndex != null && (lastPriorSelectedItemIndex == null || firstNextSelectedItemIndex - currentItemIndex >= currentItemIndex - lastPriorSelectedItemIndex))
              // Select all columns from current to first next selected
              for (var i = firstNextSelectedItemIndex.Value + 1; i >= currentItemIndex; i--)
              {
                var item = listBox.Items[i];
                listBox.SelectedItems.Add(item);
              }
          }
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control)
        {
          if (!listBox.SelectedItems.Contains(clickedItem))
            listBox.SelectedItems.Add(clickedItem);
          else
            listBox.SelectedItems.Remove(clickedItem);
        }
      }
      listBox.Focus();
      e.Handled = true;

    }
  }

  private void ColumnsListBox_PreviewMouseMove(object sender, MouseEventArgs e)
  {
    if (e.LeftButton == MouseButtonState.Pressed)
    {
      Point currentPosition = e.GetPosition(null);
      Vector diff = _dragStartPoint - currentPosition;

      if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
          Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
      {
        if (sender is ListBox listBox && listBox.SelectedItems.Count > 0)
        {
          var selectedItems = listBox.SelectedItems.Cast<ColumnSelectionItem>().ToList();
          Debug.WriteLine($"DoDragDrop({selectedItems.Count})");
          DragDrop.DoDragDrop(listBox, selectedItems, DragDropEffects.Move);
          Mouse.Capture(ColumnsListBox);
        }
      }
    }
  }

  private void ColumnsListBox_DragOver(object sender, DragEventArgs e)
  {
    if (sender is ListBox listBox && listBox.ItemsSource is ObservableCollection<ColumnSelectionItem> items)
    {
      Point position = e.GetPosition(listBox);
      var targetItem = GetItemAtPosition(listBox, position);

      int targetIndex = targetItem != null ? items.IndexOf(targetItem) : items.Count;

      if (_dropPositionAdorner == null)
      {
        _adornerLayer = AdornerLayer.GetAdornerLayer(listBox);
        _dropPositionAdorner = new DropPositionAdorner(listBox, targetIndex);
        _adornerLayer?.Add(_dropPositionAdorner);
      }
      else
      {
        _adornerLayer?.Remove(_dropPositionAdorner);
        _dropPositionAdorner = new DropPositionAdorner(listBox, targetIndex);
        _adornerLayer?.Add(_dropPositionAdorner);
      }
    }
  }
  private void ColumnsListBox_Drop(object sender, DragEventArgs e)
  {
    if (_adornerLayer != null && _dropPositionAdorner != null)
    {
      _adornerLayer.Remove(_dropPositionAdorner);
      _dropPositionAdorner = null;
    }
    Mouse.Capture(null);

    if (e.Data.GetDataPresent(typeof(List<ColumnSelectionItem>)) &&
        sender is ListBox listBox &&
        listBox.ItemsSource is ObservableCollection<ColumnSelectionItem> items)
    {

      var draggedItems = (List<ColumnSelectionItem>)e.Data.GetData(typeof(List<ColumnSelectionItem>))!;
      Point dropPosition = e.GetPosition(listBox);
      var targetItem = GetItemAtPosition(listBox, dropPosition);

      if (targetItem != null)
      {
        // Remove dragged items from the collection
        foreach (var draggedItem in draggedItems)
        {
          items.Remove(draggedItem);
        }

        int targetIndex = items.IndexOf(targetItem);

        // Insert dragged items at the target index
        foreach (var draggedItem in draggedItems)
        {
          items.Insert(targetIndex++, draggedItem);
        }

        // Update selection to reflect the new order
        listBox.SelectedItems.Clear();
        foreach (var draggedItem in draggedItems)
        {
          listBox.SelectedItems.Add(draggedItem);
        }
      }
    }
  }

  private ColumnSelectionItem? GetItemAtPosition(ListBox listBox, Point position)
  {
    var element = listBox.InputHitTest(position) as FrameworkElement;
    return element?.DataContext as ColumnSelectionItem;
    //return element != null ? ItemsControl.ContainerFromElement(listBox, element) as ColumnSelectionItem : null;
  }
  #endregion

  private void OkButton_OnClick(object sender, RoutedEventArgs e)
  {
    (DataContext as ColumnSelectionCollection)?.SetDataGridColumns();
    DialogResult = true;
    this.Close();
  }

  private void CancelButton_OnClick(object sender, RoutedEventArgs e)
  {
    DialogResult = false;
    this.Close();
  }

  #region INotifyPropertyChanged implementation
  /// <summary>
  /// Event for INotifyPropertyChanged implementation.
  /// </summary>
  public event PropertyChangedEventHandler? PropertyChanged;

  /// <summary>
  /// Method to notify that a property has changed.
  /// </summary>
  /// <param name="propertyName"></param>
  protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  #endregion

  #region IsMoveUpEnabled and IsMoveDownEnabled properties
  /// <summary>
  /// Checks if the first item in the collection is selected.
  /// </summary>
  public bool IsMoveUpEnabled
  {
    get
    {
      var selectedItem = ColumnsListBox.SelectedItems.Cast<ColumnSelectionItem>().FirstOrDefault();
      return selectedItem != null && selectedItem != ColumnsListBox.ItemsSource?.Cast<ColumnSelectionItem>().FirstOrDefault();
    }
  }

  /// <summary>
  /// Checks if the last item in the collection is selected.
  /// </summary>
  public bool IsMoveDownEnabled
  {
    get
    {
      var selectedItem = ColumnsListBox.SelectedItems.Cast<ColumnSelectionItem>().LastOrDefault();
      return selectedItem != null && selectedItem != ColumnsListBox.ItemsSource?.Cast<ColumnSelectionItem>().LastOrDefault();
    }
  }
  #endregion
}
