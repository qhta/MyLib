using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Qhta.WPF.Utils;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

/// <summary>
/// This control allows resizing of rows in a <see cref="SfDataGrid"/>.
/// It is typically used at the bottom of the row header of a grid to adjust the height of rows.
/// </summary>
public class RowResizer : Thumb
{

  #region Constructors

  static RowResizer()
  {
    DefaultStyleKeyProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(typeof(RowResizer)));

    FocusableProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(false));
    HorizontalAlignmentProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
    VerticalAlignmentProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(VerticalAlignment.Bottom));
    HeightProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(5.0));
    CursorProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(Cursors.SizeNS));

    EventManager.RegisterClassHandler(typeof(RowResizer), Thumb.PreviewMouseDownEvent, new MouseButtonEventHandler(RowResizer.OnMouseLeftButtonDown));
    EventManager.RegisterClassHandler(typeof(RowResizer), Thumb.PreviewMouseMoveEvent, new MouseEventHandler(RowResizer.OnMouseMove));
    EventManager.RegisterClassHandler(typeof(RowResizer), Thumb.PreviewMouseUpEvent, new MouseButtonEventHandler(RowResizer.OnMouseLeftButtonUp));
  }

  /// <summary>
  /// Instantiates a new instance of a RowResizer.
  /// </summary>
  public RowResizer()
  {
  }

  #endregion

  #region Properties

  /// <summary>
  /// DependencyProperty for the <see cref="MinRowHeight"/> property.
  /// Default value is 24.0
  /// </summary>
  public static readonly DependencyProperty MinRowHeightProperty =
    DependencyProperty.Register(
      nameof(MinRowHeight),
      typeof(double),
      typeof(RowResizer),
      new FrameworkPropertyMetadata(24.0));


  /// <summary>
  ///     Minimum height of the Row being resized.
  /// </summary>
  public double MinRowHeight
  {
    get => (double)GetValue(MinRowHeightProperty);
    set => SetValue(MinRowHeightProperty, value);
  }

  /// <summary>
  /// DependencyProperty for the <see cref="MaxRowHeight"/> property.
  /// Default value is 120.0
  /// </summary>
  public static readonly DependencyProperty MaxRowHeightProperty =
    DependencyProperty.Register(
      nameof(MaxRowHeight),
      typeof(double),
      typeof(RowResizer),
      new FrameworkPropertyMetadata(120.0));

  /// <summary>
  ///   Maximum height of the Row being resized.
  /// </summary>
  public double MaxRowHeight
  {
    get => (double)GetValue(MaxRowHeightProperty);
    set => SetValue(MaxRowHeightProperty, value);
  }

  #endregion

  #region Mouse Event Handlers
  /// <summary>
  /// Handles the mouse left button down event on the <see cref="RowResizer"/> to start resizing the row height.
  /// Resizer is typically used at the bottom of the row header cell in a <see cref="SfDataGrid"/>.
  /// This method checks if the data grid allows row resizing and if the click is near the bottom edge of the row header cell.
  /// If so, it sets the state of the grid to indicate that row resizing is in progress.
  /// Resizing a specific row requires the <see cref="IRowHeightProvider"/> interface to be implemented by the data context of the row header cell.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (e.ChangedButton != MouseButton.Left)
      return;

    if (sender is RowResizer rowResizer)
    {
      var rowHeaderCell = rowResizer.FindParent<GridRowHeaderCell>();
      if (rowHeaderCell == null)
        return;
      var dataGrid = rowHeaderCell.FindParent<SfDataGrid>();
      if (dataGrid == null) return;

      if (rowResizer.DataContext is not IRowHeightProvider rowHeightProvider)
        return;

      var position = e.GetPosition(rowResizer);
      var actualHeight = rowResizer.ActualHeight;

      if (SfDataGridBehavior.GetAllowRowResizing(dataGrid))
      {
        if (!SfDataGridBehavior.GetIsRowResizing(dataGrid))
        {
          // If the grid allows row resizing and is not currently resizing rows, set the state to resizing
          SfDataGridBehavior.SetStartOffset(dataGrid, actualHeight - position.Y);
          SfDataGridBehavior.SetIsRowResizing(dataGrid, true);
          Mouse.Capture(rowResizer);
          e.Handled = true;
        }
      }
    }
  }

  /// <summary>
  /// Handles the mouse move event on the <see cref="RowResizer"/> to resize the row height.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private static void OnMouseMove(object sender, MouseEventArgs e)
  {
    if (sender is RowResizer rowResizer)
    {
      var rowHeaderCell = rowResizer.FindParent<GridRowHeaderCell>();
      if (rowHeaderCell == null)
        return;
      var dataGrid = rowHeaderCell.FindParent<SfDataGrid>();
      if (dataGrid == null) return;

      if (rowHeaderCell.DataContext is not IRowHeightProvider rowHeightProvider)
        return;

      if (SfDataGridBehavior.GetIsRowResizing(dataGrid))
      {
        var position = e.GetPosition(rowHeaderCell);
        var actualHeight = rowHeaderCell.ActualHeight;
        var requestedHeight = position.Y + SfDataGridBehavior.GetStartOffset(dataGrid);
        if (!double.IsNaN(rowResizer.MinRowHeight) && requestedHeight < rowResizer.MinRowHeight)
          requestedHeight = rowResizer.MinRowHeight;
        if (!double.IsNaN(rowResizer.MaxRowHeight) && requestedHeight > rowResizer.MaxRowHeight)
          requestedHeight = rowResizer.MaxRowHeight;

        rowHeightProvider.RowHeight = requestedHeight;
        //Debug.WriteLine($"Resized to {requestedHeight}");
        e.Handled = true;
        dataGrid.View.Refresh();
      }
    }
  }

  /// <summary>
  /// Handles the mouse left button up event on the <see cref="RowResizer"/> to stop resizing the row height.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
  {
    if (e.ChangedButton != MouseButton.Left)
      return;
    if (sender is RowResizer rowResizer)
    {
      var rowHeaderCell = rowResizer.FindParent<GridRowHeaderCell>();
      if (rowHeaderCell == null)
        return;
      var grid = rowHeaderCell.FindParent<SfDataGrid>();
      if (grid == null) return;

      if (SfDataGridBehavior.GetIsRowResizing(grid))
      {
        SfDataGridBehavior.SetIsRowResizing(grid, false);
        Mouse.Capture(null);
        e.Handled = true;
      }
    }
  }
  #endregion
}