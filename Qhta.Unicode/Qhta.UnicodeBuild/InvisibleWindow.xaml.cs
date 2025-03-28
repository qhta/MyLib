using System.Diagnostics;
using System.Windows;

using Syncfusion.Windows.Tools.Controls;

namespace Qhta.UnicodeBuild
{
  public partial class InvisibleWindow : Window
  {
    public InvisibleWindow()
    {
      InitializeComponent();
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
      //Debug.WriteLine($"Invisible Window Drag Enter ");
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
      //Debug.WriteLine($"Invisible Window Drag Over ({dropPosition.X}, {dropPosition.Y}) -> {e.Effects}");
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
      Point dropPosition = e.GetPosition(this);
      if (e.Data.GetDataPresent(typeof(TabItemExt)))
      {
        if (e.Data.GetData(typeof(TabItemExt)) is TabItemExt tabItem)
        {
          e.Effects = DragDropEffects.Move;
          // Remove the TabItemExt from the original TabControlExt
          TabControlExt? originalTabControl = tabItem.Parent as TabControlExt;
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
