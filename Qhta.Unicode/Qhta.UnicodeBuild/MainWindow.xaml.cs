using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using Syncfusion.Windows.Tools.Controls;

namespace Qhta.UnicodeBuild
{
  public partial class MainWindow : Window
  {
    public MainWindow()
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
        // Get the dragged TabItemExt
        if (sender is TabItemExt tabItem)
        {
          if (!isDragging)
          {
            if (tabItem.Parent is TabControlExt tabControl && tabControl.Items.Count > 1)
            {
              isDragging = true;
              //Debug.WriteLine($"Start Drag ({_startPoint})");

              // Create and show the invisible window
              _invisibleWindow = new InvisibleWindow();
              _invisibleWindow.Show();
              Mouse.Capture(this);

              // Initialize the drag-and-drop operation
              DataObject data = new DataObject(typeof(TabItemExt), tabItem);
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
      if (e.Data.GetDataPresent(typeof(TabItemExt)))
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
      if (e.Data.GetDataPresent(typeof(TabItemExt)))
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
      if (e.Data.GetDataPresent(typeof(TabItemExt)))
      {
        if (e.Data.GetData(typeof(TabItemExt)) is TabItemExt tabItem)
        {
          e.Effects = DragDropEffects.Move;
          // Check if the drop position is outside the bounds of the MainWindow
          if (dropPosition.X < 0 || dropPosition.Y < 0 || dropPosition.X > this.ActualWidth || dropPosition.Y > this.ActualHeight)
          {
            // Remove the TabItemExt from the original TabControlExt
            TabControlExt? originalTabControl = tabItem.Parent as TabControlExt;
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
