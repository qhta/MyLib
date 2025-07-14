using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Qhta.UnicodeBuild
{
  /// <summary>
  /// New window for displaying and managing TabItems.
  /// </summary>
  public partial class NewWindow : Window
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NewWindow"/> class.
    /// </summary>
    public NewWindow()
    {
      InitializeComponent();
    }
    private bool isDragging;
    private Point _startPoint;
    private InvisibleWindow? _invisibleWindow;

    private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
    {
      _startPoint = e.GetPosition(null);
    }

    private void TabItem_MouseMove(object sender, MouseEventArgs e)
    {
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
      //Debug.WriteLine($"Drag Enter ");
      if (e.Data.GetDataPresent(typeof(TabItem)))
      {
        e.Effects = DragDropEffects.Move;
      }
      else
      {
        e.Effects = DragDropEffects.None;
      }
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
      Point dropPosition = e.GetPosition(this);
      if (e.Data.GetDataPresent(typeof(TabItem)))
      {
        e.Effects = DragDropEffects.Move;
      }
      else
      {
        e.Effects = DragDropEffects.None;
      }
      //Debug.WriteLine($"Drag Over ({dropPosition.X}, {dropPosition.Y}) -> {e.Effects}");
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
      Point dropPosition = e.GetPosition(this);
      if (e.Data.GetDataPresent(typeof(TabItem)))
      {
        if (e.Data.GetData(typeof(TabItem)) is TabItem tabItem)
        {
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
        }
      }
      //Debug.WriteLine($"Drag Drop ({dropPosition.X}, {dropPosition.Y}) -> {e.Effects}");
    }
  }
}
