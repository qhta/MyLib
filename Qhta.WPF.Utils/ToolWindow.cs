namespace Qhta.WPF.Utils;
/// <summary>
/// Tool window hides Close button.
/// </summary>
public partial class ToolWindow : Window
{
  // Prep stuff needed to remove close button on window
  private const int GWL_STYLE = -16;
  private const int WS_SYSMENU = 0x80000;
  [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
  private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
  [System.Runtime.InteropServices.DllImport("user32.dll")]
  private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

  /// <summary>
  /// Default constructor
  /// </summary>
  public ToolWindow()
  {
    Loaded += ToolWindow_Loaded;
  }

  //static ToolWindow()
  //{
  //  DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolWindow), new FrameworkPropertyMetadata("ToolWindowStyle"));
  //}

  void ToolWindow_Loaded(object sender, RoutedEventArgs e)
  {
    // Code to remove close box from window
    var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
  }
}