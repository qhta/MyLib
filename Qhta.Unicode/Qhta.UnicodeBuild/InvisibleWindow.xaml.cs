using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace Qhta.UnicodeBuild
{
  /// <summary>
  /// A simple invisible window used to handle drag-and-drop operations for TabItems.
  /// </summary>
  public partial class InvisibleWindow : Window
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InvisibleWindow"/> class.
    /// </summary>
    public InvisibleWindow()
    {
      InitializeComponent();
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
      //Debug.WriteLine($"Invisible Window Drag Enter ");
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
      //Debug.WriteLine($"Invisible Window Drag Over ({dropPosition.X}, {dropPosition.Y}) -> {e.Effects}");
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
      Point dropPosition = e.GetPosition(this);
      if (e.Data.GetDataPresent(typeof(TabItem)))
      {
        if (e.Data.GetData(typeof(TabItem)) is TabItem tabItem)
        {
          e.Effects = DragDropEffects.Move;
          // Remove the TabItem from the original TabControlExt
          TabControl? originalTabControl = tabItem.Parent as TabControl;
          originalTabControl?.Items.Remove(tabItem);

          // Create and show the new window
          NewWindow newWindow = new NewWindow();
          newWindow.NewTabControl.Items.Add(tabItem);
          newWindow.Left = dropPosition.X-100;
          newWindow.Top = dropPosition.Y-10;
          newWindow.Show();
        }
      }
      //Debug.WriteLine($"Invisible Window Drag Drop ({dropPosition.X}, {dropPosition.Y}) -> {e.Effects}");
      this.Close();
    }
  }
}
