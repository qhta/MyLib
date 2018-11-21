using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{
  public partial class CustomColorForm : UserControl
  {
    public CustomColorForm()
    {
      InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
      //image.Source = new BitmapImage { UriSource=new Uri("/Resources/colorChart.png", UriKind.RelativeOrAbsolute) };
      var bitmapSource = image.Source as BitmapSource;
      colorMap = ArrayGraphics.GetPixelArray(bitmapSource);
      AlphaTextBox.LostFocus += new RoutedEventHandler(AlphaTextBox_TextChanged);
      RedTextBox.LostFocus += new RoutedEventHandler(RedTextBox_TextChanged);
      GreenTextBox.LostFocus += new RoutedEventHandler(GreenTextBox_TextChanged);
      BlueTextBox.LostFocus += new RoutedEventHandler(BlueTextBox_TextChanged);
      HexTextBox.LostFocus += new RoutedEventHandler(HexTextBox_TextChanged);
      AlphaTextBox.KeyDown += new KeyEventHandler(AlphaTextBox_KeyDown);
      RedTextBox.KeyDown += new KeyEventHandler(RedTextBox_KeyDown);
      GreenTextBox.KeyDown += new KeyEventHandler(GreenTextBox_KeyDown);
      BlueTextBox.KeyDown += new KeyEventHandler(BlueTextBox_KeyDown);
      HexTextBox.KeyDown += new KeyEventHandler(HexTextBox_KeyDown);
      CanColor.MouseLeftButtonDown += new MouseButtonEventHandler(CanColor_MouseLeftButtonDown);
      CanColor.MouseLeftButtonUp += new MouseButtonEventHandler(CanColor_MouseLeftButtonUp);
      EpPointer.MouseMove += new MouseEventHandler(EpPointer_MouseMove);
      EpPointer.MouseLeftButtonDown += new MouseButtonEventHandler(EpPointer_MouseLeftButtonDown);
      EpPointer.MouseLeftButtonUp += new MouseButtonEventHandler(EpPointer_MouseLeftButtonUp);
      isTemplateApplied=true;
      UpdatePreview();
    }

    bool isTemplateApplied;

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(CustomColorForm),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
         SelectedColorPropertyChanged));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as CustomColorForm).UpdatePreview();
    }
    #endregion

    private PixelArray colorMap;
    private bool isMouseDownOverEllipse = false;
    private int lastMouseX;
    private int lastMouseY;
    private bool _shift = false;

    void EpPointer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      isMouseDownOverEllipse = false;
      EpPointer.ReleaseMouseCapture();
    }

    void CanColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    void CanColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      int x = (int)Mouse.GetPosition(CanColor).X;
      int y = (int)Mouse.GetPosition(CanColor).Y;
      ChangeColor(x, y);
      lastMouseX=x;
      lastMouseY=y;
      e.Handled = true;
    }

    void EpPointer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      isMouseDownOverEllipse = true;
      EpPointer.CaptureMouse();
    }

    void EpPointer_MouseMove(object sender, MouseEventArgs e)
    {
      if (isMouseDownOverEllipse)
      {
        int x = (int)Mouse.GetPosition(CanColor).X;
        int y = (int)Mouse.GetPosition(CanColor).Y;
        if (x!=lastMouseX || y!=lastMouseY)
        {
          ChangeColor(x, y);
          lastMouseX = x;
          lastMouseY = y;
        }
      }
      e.Handled = true;
    }

    void HexTextBox_KeyDown(object sender, KeyEventArgs e)
    {

      if (e.Key == Key.Enter)
      {
        try
        {
          if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
          SelectedColor = MakeColorFromHex(sender);
          Reposition();
        }
        catch(Exception ex)
        {
          Debug.WriteLine(ex.Message);
        }
      }
      else if (e.Key == Key.Tab)
      {
        AlphaTextBox.Focus();
      }

      string input = e.Key.ToString().Substring(1);
      if (string.IsNullOrEmpty(input))
      {
        input = e.Key.ToString();
      }
      if (input == "3" && _shift == true)
      {
        input = "#";
      }

      if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
      {
        _shift = true;
      }
      else
      {
        _shift = false;
      }

      if (!(input == "#" || (input[0] >= 'A' && input[0] <= 'F') || (input[0] >= 'a' && input[0] <= 'F') || (input[0] >= '0' && input[0] <= '9')))
        e.Handled = true;
      if (input.Length > 1)
        e.Handled = true;
    }

    void AlphaTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    void BlueTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      NumericValidation(e);
      NumericValidation(e);
      if (e.Key == Key.Tab)
      {
        HexTextBox.Focus();
      }

    }

    void GreenTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      NumericValidation(e);
      if (e.Key == Key.Tab)
      {
        BlueTextBox.Focus();
      }
    }

    void RedTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      NumericValidation(e);
      if (e.Key == Key.Tab)
      {
        GreenTextBox.Focus();
      }
    }

    void AlphaTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      NumericValidation(e);

      if (e.Key == Key.Tab)
      {
        RedTextBox.Focus();
      }
    }

    void HexTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
      try
      {
        if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
        SelectedColor = MakeColorFromHex(sender);
        Reposition();
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }

    }

    private Color MakeColorFromHex(object sender)
    {
      try
      {
        ColorConverter cc = new ColorConverter();
        return (Color)cc.ConvertFrom(((TextBox)sender).Text);
      }
      catch
      {
        string alphaHex = SelectedColor.A.ToString("X").PadLeft(2, '0');
        string redHex = SelectedColor.R.ToString("X").PadLeft(2, '0');
        string greenHex = SelectedColor.G.ToString("X").PadLeft(2, '0');
        string blueHex = SelectedColor.B.ToString("X").PadLeft(2, '0');
        HexTextBox.Text = String.Format("#{0}{1}{2}{3}",
        alphaHex, redHex,
        greenHex, blueHex);


      }
      return SelectedColor;
    }

    void BlueTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
      try
      {
        if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
        int val = Convert.ToInt32(((TextBox)sender).Text);
        if (val > 255)

          ((TextBox)sender).Text = "255";
        else
        {
          byte byteValue = Convert.ToByte(((TextBox)sender).Text);
          SelectedColor = MakeColorFromRGB();
          Reposition();
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }

    }

    private Color MakeColorFromRGB()
    {
      byte abyteValue = Convert.ToByte(AlphaTextBox.Value);
      byte rbyteValue = Convert.ToByte(RedTextBox.Text);
      byte gbyteValue = Convert.ToByte(GreenTextBox.Text);
      byte bbyteValue = Convert.ToByte(BlueTextBox.Text);
      Color rgbColor =
           Color.FromArgb(
               abyteValue,
               rbyteValue,
               gbyteValue,
               bbyteValue);
      return rgbColor;
    }

    void GreenTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
      try
      {
        if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
        int val = Convert.ToInt32(((TextBox)sender).Text);
        if (val > 255)

          ((TextBox)sender).Text = "255";
        else
        {
          byte byteValue = Convert.ToByte(((TextBox)sender).Text);
          SelectedColor =
             Color.FromArgb(
                  SelectedColor.A,
                 SelectedColor.R,
                byteValue,
                 SelectedColor.B);
          Reposition();

        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }
    }

    void RedTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
      try
      {

        if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
        int val = Convert.ToInt32(((TextBox)sender).Text);
        if (val > 255)

          ((TextBox)sender).Text = "255";
        else
        {
          byte byteValue = Convert.ToByte(((TextBox)sender).Text);
          SelectedColor =
             Color.FromArgb(
                  SelectedColor.A,
                 byteValue,
                 SelectedColor.G,
                 SelectedColor.B);
          Reposition();
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }
    }

    void AlphaTextBox_TextChanged(object sender, RoutedEventArgs e)
    {
      try
      {
        if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
        int val = Convert.ToInt32(((TextBox)sender).Text);
        if (val > 255)

          ((TextBox)sender).Text = "255";
        else
        {
          byte byteValue = Convert.ToByte(((TextBox)sender).Text);
          SelectedColor =
             Color.FromArgb(
                  byteValue,
                 SelectedColor.R,
                 SelectedColor.G,
                 SelectedColor.B);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }
    }

    private void NumericValidation(System.Windows.Input.KeyEventArgs e)
    {
      string input = e.Key.ToString().Substring(1);
      try
      {
        if (e.Key == Key.Enter)
        {
          SelectedColor = MakeColorFromRGB();
          Reposition();
        }
        int inputDigit = Int32.Parse(input);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        e.Handled = true;
      }
    }

    //public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
    //{
    //  return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
    //      System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
    //}

    private void ChangeColor(int x, int y)
    {
      try
      {
        SelectedColor = GetColorFromImage(x, y);
        MovePointer(x,y);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }
    }

    private void Reposition()
    {
      for (int i = 0; i < CanColor.ActualWidth; i++)
      {
        bool flag = false;
        for (int j = 0; j < CanColor.ActualHeight; j++)
        {

          try
          {
            Color Colorfromimagepoint = GetColorFromImage(i, j);
            if (SimilarColor(Colorfromimagepoint, SelectedColor))
            {
              MovePointerDuringReposition(i, j);
              flag = true;
              break;
            }

          }
          catch (Exception ex)
          {
            Debug.WriteLine(ex.Message);
          }

        }
        if (flag) break;

      }


    }
    // <summary>
    // 1*1 pixel copy is based on an article by Lee Brimelow    
    // http://thewpfblog.com/?p=62
    // </summary>

    private Color GetColorFromImage(int i, int j)
    {
      i = Math.Max(Math.Min((int)(i*image.Source.Width/image.Width), (int)image.Source.Width-1),0);
      j = Math.Max(Math.Min((int)(j*image.Source.Height/image.Height), (int)image.Source.Height-1),0);
      var pixel = colorMap[i, j];
      Color Colorfromimagepoint = Color.FromArgb((byte)AlphaTextBox.Value, pixel.R, pixel.G, pixel.B);
      //CroppedBitmap cb = new CroppedBitmap(image.Source as BitmapSource,
      //    new Int32Rect(i,
      //        j, 1, 1));
      //byte[] color = new byte[4];
      //cb.CopyPixels(color, 4, 0);
      //Color Colorfromimagepoint = Color.FromArgb((byte)AlphaSlider.Value, color[0], color[1], color[2]);
      return Colorfromimagepoint;
    }

    private void MovePointerDuringReposition(int x, int y)
    {
      //EpPointer.SetValue(Canvas.LeftProperty, (double)(x - 3));
      //EpPointer.SetValue(Canvas.TopProperty, (double)(y - 3));
      ////EpPointer.InvalidateVisual();
      ////CanColor.InvalidateVisual();
      MovePointer(x, y);
    }
    private void MovePointer(int x, int y)
    {
      x-=3;
      y-=3;
      x = Math.Max(Math.Min(x, (int)image.Width-5),-2);
      y = Math.Max(Math.Min(y, (int)image.Height-5), -2);
      EpPointer.SetValue(Canvas.LeftProperty, (double)x);
      EpPointer.SetValue(Canvas.TopProperty, (double)y);
      //CanColor.InvalidateVisual();
    }

    private bool SimilarColor(Color pointColor, Color selectedColor)
    {
      int diff = Math.Abs(pointColor.R - selectedColor.R) + Math.Abs(pointColor.G - selectedColor.G) + Math.Abs(pointColor.B - selectedColor.B);
      if (diff < 20) return true;
      else
        return false;
    }

    private void UpdatePreview()
    {
      //Debug.WriteLine($"CustomColorForm SelectedColor={SelectedColor}");
      if (!isTemplateApplied)
        return;
      PreviewRectangle.Fill = new SolidColorBrush(SelectedColor);
      AlphaTextBox.Value = SelectedColor.A;
      string alphaHex = SelectedColor.A.ToString("X").PadLeft(2, '0');
      RedTextBox.Text = SelectedColor.R.ToString();
      string redHex = SelectedColor.R.ToString("X").PadLeft(2, '0');
      GreenTextBox.Text = SelectedColor.G.ToString();
      string greenHex = SelectedColor.G.ToString("X").PadLeft(2, '0');
      BlueTextBox.Text = SelectedColor.B.ToString();
      string blueHex = SelectedColor.B.ToString("X").PadLeft(2, '0');
      HexTextBox.Text = String.Format("#{0}{1}{2}{3}",
      alphaHex, redHex,
      greenHex, blueHex);
    }

    private void AlphaTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      SelectedColor = Color.FromArgb((byte)AlphaTextBox.Value, SelectedColor.R, SelectedColor.G, SelectedColor.B);
    }
  }
}
