using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Qhta.MVVM;
using Qhta.UndoManager;
using Qhta.UnicodeBuild.Commands;
using Qhta.UnicodeBuild.Helpers;
using SelectionChangedEventArgs = System.Windows.Controls.SelectionChangedEventArgs;

namespace Qhta.UnicodeBuild;

/// <summary>
/// Main window for the application.
/// </summary>
public partial class MainWindow : Window
{

  /// <summary>
  /// Initializes a new instance of the <see cref="MainWindow"/> class.
  /// </summary>
  public MainWindow()
  {
    InitializeComponent();
    this.KeyDown += MainWindow_KeyDown;
    _backgroundTimer = new Timer(TimerProc, null, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
    Closing += MainWindow_Closing;

  }

  private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
  {
    _backgroundTimer.Dispose();
  }

  private readonly Timer _backgroundTimer;

  private void TimerProc(object? state)
  {
    try
    {
      Dispatcher.Invoke(_Commander.NotifyCanExecuteChanged);
      Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
    }
    catch (System.Threading.Tasks.TaskCanceledException)
    {
      _backgroundTimer.Dispose();
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }



  /// <summary>
  /// This method is needed to override the default behavior of the Enter key in SfDataGrid.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private void MainWindow_KeyDown(object sender, KeyEventArgs e)
  {
    //Debug.WriteLine($"MainWindow_PreviewKeyDown: {e.Key} {Keyboard.Modifiers}");
    if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
    {
      if (UndoMgr.IsUndoAvailable)
      {
        UndoMgr.Undo();
        e.Handled = true;
        return;
      }
    }
    if (e.Key == Key.Y && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
    {
      if (UndoMgr.IsRedoAvailable)
      {
        UndoMgr.Redo();
        e.Handled = true;
        return;
      }
    }
    if (e.Key == Key.Return && Keyboard.Modifiers == ModifierKeys.Shift)
    {
      var focusedElement = Keyboard.FocusedElement;
      if (focusedElement is UIElement focusedControl)
      {
        var keyEventArgs = new KeyEventArgs(
          Keyboard.PrimaryDevice,
          PresentationSource.FromVisual(focusedControl)!,
          0,
          Key.Return)
        {
          RoutedEvent = Keyboard.KeyDownEvent
        };

        focusedControl.RaiseEvent(keyEventArgs);
        if (keyEventArgs.Handled)
        {
          e.Handled = true;
        }
      }
    }
  }

  private Point _startPoint;
  private DateTime _clickTime;
  private bool isDragging;
  private InvisibleWindow? _invisibleWindow;

  private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
  {
    //Debug.WriteLine($"MouseDown");
    _startPoint = e.GetPosition(null);
    _clickTime = DateTime.Now;
  }

  private void TabItem_MouseMove(object sender, MouseEventArgs e)
  {
    if (e.LeftButton == MouseButtonState.Pressed) return;

    var ms = (DateTime.Now - _clickTime!).TotalMilliseconds;
    if (ms < 1000) return;
    //Debug.WriteLine($"MouseMove ({ms})");
    Point mousePos = e.GetPosition(this);
    Vector diff = _startPoint - mousePos;

    if (e.LeftButton == MouseButtonState.Pressed &&
        (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
         Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
    {
      // Get the dragged TabItem
      if (sender is TabItem tabItem)
      {
        if (!isDragging)
        {
          if (tabItem.Parent is TabControl tabControl && tabControl.Items.Count > 1)
          {
            isDragging = true;
            //Debug.WriteLine($"Start Drag ({_startPoint})");

            // Create and show the invisible window
            _invisibleWindow = new InvisibleWindow();
            _invisibleWindow.Show();
            Mouse.Capture(this);

            // Initialize the drag-and-drop operation
            DataObject data = new DataObject(typeof(TabItem), tabItem);
            var result = DragDrop.DoDragDrop(tabItem, data, DragDropEffects.Move);

            //Debug.WriteLine($"DoDragDrop {result}");
            isDragging = false;

            // Close the invisible window
            _invisibleWindow.Close();
            _invisibleWindow = null;
          }
        }
      }
    }
  }

  private void Window_DragEnter(object sender, DragEventArgs e)
  {
    if (e.Data.GetDataPresent(typeof(TabItem)))
    {
      //Debug.WriteLine($"Drag Enter ");
      e.Effects = DragDropEffects.Move;
      e.Handled = true;
    }
  }

  private void Window_DragOver(object sender, DragEventArgs e)
  {
    if (e.Data.GetData(typeof(TabItem)) is TabItem tabItem)
    {
      Point dropPosition = e.GetPosition(this);
      e.Effects = DragDropEffects.Move;
      //Debug.WriteLine($"Drag Over ({dropPosition.X}, {dropPosition.Y}) {tabItem.Header} -> {e.Effects}");
      e.Handled = true;
    }
  }

  private void Window_Drop(object sender, DragEventArgs e)
  {
    if (e.Data.GetData(typeof(TabItem)) is TabItem tabItem)
    {
      Point dropPosition = e.GetPosition(this);
      e.Effects = DragDropEffects.Move;
      // Check if the drop position is outside the bounds of the MainWindow
      if (dropPosition.X < 0 || dropPosition.Y < 0 || dropPosition.X > this.ActualWidth || dropPosition.Y > this.ActualHeight)
      {
        // Remove the TabItem from the original TabControlExt
        TabControl? originalTabControl = tabItem.Parent as TabControl;
        originalTabControl?.Items.Remove(tabItem);

        // Create and show the new window
        NewWindow newWindow = new NewWindow();
        newWindow.NewTabControl.Items.Add(tabItem);
        newWindow.Show();
      }
      else
      {
        //Debug.WriteLine($"Drop ({dropPosition.X}, {dropPosition.Y}) {tabItem.Header} -> {e.Effects}");
        e.Handled = true;
      }
    }
  }

  private void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
  {
    Debug.WriteLine($"MainWindow_OnCanExecute({(e.Command as RoutedUICommand)?.Text ?? e.Command.ToString()})");
    _Commander.OnCanExecute(sender, e);
    Debug.WriteLine($"MainWindow_OnCanExecute({(e.Command as RoutedUICommand)?.Text ?? e.Command.ToString()})={e.CanExecute}");
  }

  private void OnExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    // Delegate to the active UserControl
    if (TabControl.SelectedContent is IRoutedCommandHandler handler)
    {
      handler.OnExecuted(sender, e);
    }
  }

  private void MainTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
  {
    //Debug.WriteLine($"MainTabControl_OnSelectionChanged");
    if (e.Source is TabControl tabControl && tabControl.SelectedContent is UIElement selectedElement)
    {
      //Debug.WriteLine($"{selectedElement}.SetFocus()");
      selectedElement.Focus(); // Set focus to the active UserControl
    }
  }
}