using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Qhta.WPF.Controls
{
  /// <summary>
  /// Kontrolka do edycji bitmapy w powiększeniu. Kontekst danych musi być typu <c>WriteableBitmap</c>
  /// </summary>
  public partial class BitmapGrid : UserControl
  {
    /// <summary>
    /// Konstruktor inicjujący
    /// </summary>
    public BitmapGrid ()
    {
      InitializeComponent();
      DataContextChanged += BitmapGrid_DataContextChanged;
    }

    /// <summary>
    /// Edytowalna bitmapa. Utożsamiana z kontekstem danych.
    /// </summary>
    public WriteableBitmap Source
    {
      get { return DataContext as WriteableBitmap; }
      set { DataContext = value; }
    }

    /// <summary>
    /// Szerokość bitmapy w pikselach
    /// </summary>
    public int PixelWidth
    {
      get { return (int)GetValue(PixelWidthProperty); }
      set { SetValue(PixelWidthProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="PixelWidth"/>
    /// </summary>
    public static DependencyProperty PixelWidthProperty = DependencyProperty.Register
      ("PixelWidthProperty", typeof(int), typeof(BitmapGrid),
      new PropertyMetadata(1, ResolutionPropertiesChanged)
      );

    /// <summary>
    /// Wysokość bitmapy w pikselach
    /// </summary>
    public int PixelHeight
    {
      get { return (int)GetValue(PixelHeightProperty); }
      set { SetValue(PixelHeightProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla właściwości <see cref="PixelHeight"/>
    /// </summary>
    public static DependencyProperty PixelHeightProperty = DependencyProperty.Register
      ("PixelHeightProperty", typeof(int), typeof(BitmapGrid),
      new PropertyMetadata(1, ResolutionPropertiesChanged)
      );
    
    private static void ResolutionPropertiesChanged (object obj, DependencyPropertyChangedEventArgs args)
    {
      (obj as BitmapGrid).ResolutionChanged();
    }

    private void ResolutionChanged ()
    {
      //DrawingBrush backgroundBrush = ThisGrid.Background as DrawingBrush;
      //backgroundBrush.Viewport = new Rect(0, 0, 1.0 / PixelWidth, 1.0 / PixelHeight);
    }

    private Color[,] Pixels { get; set; }

    Color[,] GetPixelArray (BitmapSource source)
    {
      Color[,] pixels;
      int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);
      byte[] data = new byte[stride * source.PixelHeight];
      source.CopyPixels(data, stride, 0);
      pixels = new Color[source.PixelWidth, source.PixelHeight];
      for (int y = 0; y < source.PixelHeight; y++)
        for (int x = 0; x < source.PixelWidth; x++)
        {
          int pixelAddress = y*stride+x*source.Format.BitsPerPixel/8;
          byte B = data[pixelAddress++];
          byte G = data[pixelAddress++];
          byte R = data[pixelAddress++];
          byte A = 0xFF;
          if (source.Format.BitsPerPixel == 32)
            A = data[pixelAddress++];
          pixels[x, y] = Color.FromArgb(A, R, G, B);
        }
      return pixels;
    }

    void SetPixelArray(WriteableBitmap target, Color[,] pixels)
    {
      int stride = target.PixelWidth * (target.Format.BitsPerPixel / 8);
      byte[] data = new byte[stride * target.PixelHeight];
      for (int y = 0; y < target.PixelHeight; y++)
        for (int x = 0; x < target.PixelWidth; x++)
        {
          int pixelAddress = y*stride+x*target.Format.BitsPerPixel/8;
          var pixel = pixels[x, y];
          data[pixelAddress++]=pixel.B;
          data[pixelAddress++]=pixel.G;
          data[pixelAddress++]=pixel.R;
          data[pixelAddress++]=pixel.A;
        }
      Int32Rect rect = new Int32Rect(0, 0, (int)target.PixelWidth, (int)target.PixelHeight);
      target.WritePixels(rect, data, stride, 0);
    }

    void SetPixelArray(WriteableBitmap target, Color[,] pixels, int scale)
    {
      int pixelWidth= (pixels.GetUpperBound(0)+1);
      int pixelHeight = (pixels.GetUpperBound(1)+1);
      int newPixelWidth = pixelWidth*scale;
      int newPixelHeight = pixelHeight*scale;
      var scaledPixels = new Color[newPixelWidth, newPixelHeight];
      for (int y = 0; y < pixelHeight; y++)
        for (int x = 0; x < pixelWidth; x++)
          for (int i = 0; i<scale; i++)
            for (int j = 0; j<scale; j++)
              scaledPixels[x*scale+i, y*scale+j]=pixels[x, y];
      SetPixelArray(target, scaledPixels);
    }

    /// <summary>
    /// W reakcji na zmianę kontekstu danych następuje ustawienie gridu.
    /// </summary>
    void BitmapGrid_DataContextChanged (object sender, DependencyPropertyChangedEventArgs e)
    {
      //ThisGrid.Children.Clear();
      //ThisGrid.ColumnDefinitions.Clear();
      //ThisGrid.RowDefinitions.Clear();
      Pixels = null;
      if (Source == null)
        return;
      PixelWidth = Source.PixelWidth;
      PixelHeight = Source.PixelHeight;
      int scale = 1;

      Pixels = GetPixelArray(this.Source);
      var bitmap = new WriteableBitmap(PixelWidth*scale,PixelHeight*scale,Source.DpiX,Source.DpiY, PixelFormats.Bgra32, Source.Palette);
      //for (int x = 0; x < Source.PixelWidth; x++)
      //  for (int y = 0; y < Source.PixelHeight; y++)
      //  {
      //    bitmap.WritePixels();
      //  }
      SetPixelArray(bitmap, Pixels, scale);
      BitmapView.Width=MainGrid.Width=bitmap.PixelWidth;
      BitmapView.Height=MainGrid.Height=bitmap.PixelHeight;
      BitmapView.Source = bitmap;
      //for (int x = 0; x < Source.PixelWidth; x++)
      //  ThisGrid.ColumnDefinitions.Add(new ColumnDefinition());
      //for (int y = 0; y < Source.PixelHeight; y++)
      //  ThisGrid.RowDefinitions.Add(new RowDefinition());
      //for (int x = 0; x < Source.PixelWidth; x++)
      //  for (int y = 0; y < Source.PixelHeight; y++)
      //  {
      //    Rectangle pixelRect = new Rectangle 
      //    { 
      //      Fill = this.Resources["DiamondBrush"] as Brush
      //    };
      //    Grid.SetColumn(pixelRect, x);
      //    Grid.SetRow(pixelRect, y);
      //    ThisGrid.Children.Add(pixelRect);
      //  }

      //for (int x = 0; x < Source.PixelWidth; x++)
      //  for (int y = 0; y < Source.PixelHeight; y++)
      //  {
      //    Rectangle pixelRect = new Rectangle
      //    {
      //      Fill = new SolidColorBrush(Pixels[x, y]),
      //    };
      //    Grid.SetColumn(pixelRect, x);
      //    Grid.SetRow(pixelRect, y);
      //    ThisGrid.Children.Add(pixelRect);
      //  }

    }

  }
}
