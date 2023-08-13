namespace Qhta.WPF.Utils;

public partial class TimingMessageBox : Window
{
  private TimingMessageBox()
  {
    InitializeComponent();
  }

  /// <summary>
  /// Shows message box with empty caption, OK button, using timeout.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="timeout"></param>
  /// <returns></returns>
  public static MessageBoxResult Show(string text, int timeout) => Show(text, "", MessageBoxButton.OK, timeout);

  /// <summary>
  /// Shows message box with empty caption, OK button, using timeout.
  /// Uses specific window size.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="size"></param>
  /// <param name="timeout"></param>
  /// <returns></returns>
  public static MessageBoxResult Show(string text, Size size, int timeout) => Show(text, "", MessageBoxButton.OK, size, timeout);

  /// <summary>
  /// Shows message box with specific caption, OK button, using timeout.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="caption"></param>
  /// <param name="timeout"></param>
  /// <returns></returns>
  public static MessageBoxResult Show(string text, string caption, int timeout) => Show(text, caption, MessageBoxButton.OK, timeout);

  /// <summary>
  /// Shows message box with specific caption, OK button, using timeout.
  /// Uses specific window size.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="caption"></param>
  /// <param name="size"></param>
  /// <param name="timeout"></param>
  /// <returns></returns>
  public static MessageBoxResult Show(string text, string caption, Size size, int timeout) => Show(text, caption, MessageBoxButton.OK, size, timeout);

  /// <summary>
  /// Shows message box with empty caption, specified button, using timeout.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="button"></param>
  /// <param name="timeout"></param>
  /// <returns></returns>
  public static MessageBoxResult Show(string text, MessageBoxButton button, int timeout) => Show(text, "", button, timeout);

  /// <summary>
  /// Shows message box with empty caption, specified button, using timeout.
  /// Uses specific window size.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="button"></param>
  /// <param name="size"></param>
  /// <param name="timeout"></param>
  /// <returns></returns>
  public static MessageBoxResult Show(string text, MessageBoxButton button, Size size, int timeout) => Show(text, "", button, size, timeout);

  /// <summary>
  /// Shows message box with specified caption, specified button, using timeout.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="caption"></param>
  /// <param name="button"></param>
  /// <param name="timeout"></param>
  /// <returns></returns>
  public static MessageBoxResult Show(string text, string caption, MessageBoxButton button, int timeout) => Show(text, caption, button, default(Size), timeout);

  /// <summary>
  /// Shows message box with specified caption, specified button, using timeout.
  /// Uses specific window size.
  /// </summary>
  /// <param name="text"></param>
  /// <param name="caption"></param>
  /// <param name="button"></param>
  /// <param name="size"></param>
  /// <param name="timeout"></param>
  /// <returns></returns>
  public static MessageBoxResult Show(string text, string caption, MessageBoxButton button, Size size, int timeout)
  {
    var window = new TimingMessageBox();
    window.Title = caption ?? "";
    window.textBlock.Text = text ?? "";
    window.YesButton.Visibility = (button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel) ? Visibility.Visible : Visibility.Collapsed;
    window.NoButton.Visibility = (button == MessageBoxButton.YesNo || button == MessageBoxButton.YesNoCancel) ? Visibility.Visible : Visibility.Collapsed;
    window.OKButton.Visibility = (button == MessageBoxButton.OK || button == MessageBoxButton.OKCancel) ? Visibility.Visible : Visibility.Collapsed;
    window.CancelButton.Visibility = (button == MessageBoxButton.OKCancel || button == MessageBoxButton.YesNoCancel) ? Visibility.Visible : Visibility.Collapsed;
    if (size != default(Size))
    {
      window.Height = size.Height;
      window.Width = size.Width;
    }

    using (window.PrepareTimer(timeout))
    {
      window.ShowDialog();
      return window.result;
    }
  }
  private MessageBoxResult result = MessageBoxResult.None;

  private System.Threading.Timer PrepareTimer(int timeout)
  {
    return new System.Threading.Timer(OnTimerElapsed,
        null, timeout, System.Threading.Timeout.Infinite);
  }

  private void OnTimerElapsed(object? state)
  {
    this.Dispatcher.Invoke(Close);
  }

  private void YesButton_Click(object sender, RoutedEventArgs e)
  {
    result = MessageBoxResult.Yes;
    this.Dispatcher.Invoke(Close);
  }

  private void NosButton_Click(object sender, RoutedEventArgs e)
  {
    result = MessageBoxResult.No;
    this.Dispatcher.Invoke(Close);
  }

  private void OKButton_Click(object sender, RoutedEventArgs e)
  {
    result = MessageBoxResult.OK;
    this.Dispatcher.Invoke(Close);
  }

  private void CancelButton_Click(object sender, RoutedEventArgs e)
  {
    result = MessageBoxResult.Cancel;
    this.Dispatcher.Invoke(Close);
  }

}
