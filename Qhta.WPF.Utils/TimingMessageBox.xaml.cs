using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Qhta.WPF.Utils
{
  public partial class TimingMessageBox : Window
  {
    private TimingMessageBox()
    {
      InitializeComponent();
    }

    public static MessageBoxResult Show(string text, int timeout) => Show(text, null, MessageBoxButton.OK, timeout);

    public static MessageBoxResult Show(string text, Size size, int timeout) => Show(text, null, MessageBoxButton.OK, size, timeout);

    public static MessageBoxResult Show(string text, string caption, int timeout) => Show(text, caption, MessageBoxButton.OK, timeout);

    public static MessageBoxResult Show(string text, string caption, Size size, int timeout) => Show(text, caption, MessageBoxButton.OK, size, timeout);

    public static MessageBoxResult Show(string text, MessageBoxButton button, int timeout) => Show(text, null, button, timeout);

    public static MessageBoxResult Show(string text, MessageBoxButton button, Size size, int timeout) => Show(text, null, button, size, timeout);

    public static MessageBoxResult Show(string text, string caption, MessageBoxButton button, int timeout) => Show(text, caption, button, default(Size), timeout);

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

    private Timer PrepareTimer(int timeout)
    {
      return new System.Threading.Timer(OnTimerElapsed,
          null, timeout, System.Threading.Timeout.Infinite);
    }

    private void OnTimerElapsed(object state)
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
}
