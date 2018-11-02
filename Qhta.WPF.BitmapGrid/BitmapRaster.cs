using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrawingColor = System.Drawing.Color;
using Qhta.Drawing.ArrayGraphics;
using Qhta.Drawing.ColorUtils;
using Qhta.WindowsMedia.ColorUtils;

namespace Qhta.WPF.Controls
{
  public class BitmapRaster : FrameworkElement
  {
    public static DependencyProperty SourceProperty = DependencyProperty.Register
      ("Source", typeof(BitmapSource), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Bitmapa jako źródło i miejsce przechowywania danych. Może być typu <c>WriteableBitmap</c>
    /// </summary>
    public BitmapSource Source
    {
      get => (BitmapSource)GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    public static DependencyProperty MaskProperty = DependencyProperty.Register
      ("Mask", typeof(BitmapSource), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Bitmapa jako źródło i miejsce przechowywania maski danych. Może być typu <c>WriteableBitmap</c>
    /// </summary>
    public BitmapSource Mask
    {
      get => (BitmapSource)GetValue(MaskProperty);
      set => SetValue(MaskProperty, value);
    }
    
    public static DependencyProperty ScaleProperty = DependencyProperty.Register
      ("Scale", typeof(int), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Skala wyświetlania - rozmiar kwadratu na piksel
    /// </summary>
    public int Scale
    {
      get => (int)GetValue(ScaleProperty);
      set => SetValue(ScaleProperty, value);
    }


    public static DependencyProperty ZoomProperty = DependencyProperty.Register
      ("Zoom", typeof(double), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(1.0, 
        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Powiększenie - mnożnik skali
    /// </summary>
    public double Zoom
    {
      get => (double)GetValue(ZoomProperty);
      set => SetValue(ZoomProperty, value);
    }

    public static DependencyProperty ShowRasterProperty = DependencyProperty.Register
      ("ShowRaster", typeof(bool), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Czy pokazywać linie siatki
    /// </summary>
    public bool ShowRaster
    {
      get => (bool)GetValue(ShowRasterProperty);
      set => SetValue(ShowRasterProperty, value);
    }

    public static DependencyProperty ModeProperty = DependencyProperty.Register
      ("Mode", typeof(BitmapEditMode), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(BitmapEditMode.Select, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Tryb, w którym znajduje się kontrolka. Domyślnie <c>Select</c>
    /// </summary>
    public BitmapEditMode Mode
    {
      get => (BitmapEditMode)GetValue(ModeProperty);
      set => SetValue(ModeProperty, value);
    }

    public static DependencyProperty PrimaryColorProperty = DependencyProperty.Register
      ("PrimaryColor", typeof(Color), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Kolor główny (odpowiadający wypełnieniu na lewy przycisk myszy)
    /// </summary>
    public Color PrimaryColor
    {
      get => (Color)GetValue(PrimaryColorProperty);
      set => SetValue(PrimaryColorProperty, value);
    }

    public static DependencyProperty SecondaryColorProperty = DependencyProperty.Register
      ("SecondaryColor", typeof(Color), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Kolor drugorzędny (odpowiadający wypełnieniu na prawy przycisk myszy)
    /// </summary>
    public Color SecondaryColor
    {
      get => (Color)GetValue(SecondaryColorProperty);
      set => SetValue(SecondaryColorProperty, value);
    }


    public static DependencyProperty SelectionColorProperty = DependencyProperty.Register
      ("SelectionColor", typeof(Color), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Kolor prostokąta selekcji
    /// </summary>
    public Color SelectionColor
    {
      get => (Color)GetValue(SelectionColorProperty);
      set => SetValue(SelectionColorProperty, value);
    }

    public static DependencyProperty SelectionLightColorProperty = DependencyProperty.Register
      ("SelectionLightColor", typeof(Color), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(Colors.Yellow, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Kolor rozjaśnienia prostokąta selekcji.
    /// </summary>
    public Color SelectionLightColor
    {
      get => (Color)GetValue(SelectionLightColorProperty);
      set => SetValue(SelectionLightColorProperty, value);
    }

    /// <summary>
    /// Rozmiar kontrolki obliczany w oparciu o rozmiar obrazu i skalę
    /// </summary>
    public Rect Bounds
    {
      get
      {
        return new Rect(0, 0, imageSize.Width, imageSize.Height);
      }
    }

    private Rect Selection;
    private int HandleSize = 6;


    protected override Size MeasureOverride(Size availableSize)
    {
      if (Source!=null)
        return imageSize = new Size(Source.PixelWidth*Scale, Source.PixelHeight*Scale);
      return base.MeasureCore(availableSize);
    }
    /// <summary>
    /// Przychowywany rozmiar obrazu * skala
    /// </summary>
    Size imageSize = new Size(16,16);

    /// <summary>
    /// Metoda wyświetlenia
    /// </summary>
    /// <param name="drawingContext"></param>
    protected override void OnRender(DrawingContext drawingContext)
    {
      if (Source == null)
        return;
      RenderBitmap(drawingContext);
    }

    PixelArray GetPixelArray(BitmapSource source)
    {
      PixelArray pixels;
      int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);
      byte[] data = new byte[stride * source.PixelHeight];
      source.CopyPixels(data, stride, 0);
      pixels = new PixelArray(source.PixelWidth, source.PixelHeight);
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
          pixels[x, y] = DrawingColor.FromArgb(A, R, G, B);
        }
      return pixels;
    }

    void SetPixelArray(WriteableBitmap target, PixelArray pixels)
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

    void SetPixelArray(WriteableBitmap target, PixelArray pixels, int scale)
    {
      int pixelWidth = (pixels.PixelWidth);
      int pixelHeight = (pixels.PixelHeight);
      int newPixelWidth = pixelWidth*scale;
      int newPixelHeight = pixelHeight*scale;
      var scaledPixels = new PixelArray(newPixelWidth, newPixelHeight);
      for (int y = 0; y < pixelHeight; y++)
        for (int x = 0; x < pixelWidth; x++)
          for (int i = 0; i<scale; i++)
            for (int j = 0; j<scale; j++)
              scaledPixels[x*scale+i, y*scale+j]=pixels[x, y];
      SetPixelArray(target, scaledPixels);
    }

    private double TransformScale()
    {
      double scale = Scale;
      var scaleTransform = RenderTransform as ScaleTransform;
      if (scaleTransform!=null)
      {
        scale *= (scaleTransform.ScaleX+scaleTransform.ScaleY)/2;
        this.RenderTransform=null;
      }
      else
      {
        var transformGroup = RenderTransform as TransformGroup;
        if (transformGroup!=null)
        {
          scaleTransform =  transformGroup.Children.LastOrDefault(item => item is ScaleTransform) as ScaleTransform;
          if (scaleTransform!=null)
          {
            scale *= (scaleTransform.ScaleX+scaleTransform.ScaleY)/2;
            var newTransform = new TransformGroup();
            foreach (var item in transformGroup.Children)
              if (item!=scaleTransform)
                newTransform.Children.Add(item);
            this.RenderTransform =  newTransform;
          }
        }
      }
      return scale;
    }

    protected virtual void RenderBitmap(DrawingContext drawingContext)
    {
      var pixelWidth = Source.PixelWidth;
      var pixelHeight = Source.PixelHeight;
      var transformSave = RenderTransform;
      if (transformSave!=null)
        Debug.WriteLine($"RenderBitmap.RenderTransform={transformSave}");
      int scale = (int)TransformScale();
      var pixels = GetPixelArray(Source);
      PixelArray mask = (Mask!=null) ? GetPixelArray(Mask) : null;
      bool showRaster = ShowRaster && Scale>1;
      for (int y = 0; y < pixelHeight; y++)
        for (int x = 0; x < pixelWidth; x++)
        {
          var pixel = pixels[x, y];
          var brush = new SolidColorBrush(pixel.ToMediaColor());
          var pixelRect = new Rect(x*scale, y*scale, scale, scale);
          drawingContext.DrawRectangle(brush, null, pixelRect);
          if (showRaster)
          {
            var lineColor = pixels[x, y].Inverse();
            var pen = new Pen(new SolidColorBrush(lineColor.ToMediaColor()), 1);
            drawingContext.DrawRectangle(null, pen, pixelRect);
          }
          if (mask!=null)
          {
            if (!mask[x, y].IsEqual(DrawingColor.White))
            {
              brush = new SolidColorBrush(Color.FromArgb((byte)(mask[x, y].A/2), 255, 0, 0));
              drawingContext.DrawRectangle(brush, null, pixelRect);
            }
          }
        }
      if (Selection!=null && Selection.Width>0 && Selection.Height>0)
      {
        var selectionRect = new Rect(Selection.Left*scale, Selection.Top*Scale, Selection.Width*scale, Selection.Height*scale);
        var selectionBrush = new SolidColorBrush(SelectionLightColor);
        var selectionPen = new Pen(selectionBrush, 3);
        drawingContext.DrawRectangle(null, selectionPen, selectionRect);
        selectionPen = new Pen(new SolidColorBrush(SelectionColor), 1);
        drawingContext.DrawRectangle(null, selectionPen, selectionRect);

        var handleRect = new Rect(selectionRect.Left-3, selectionRect.Top-3, HandleSize, HandleSize);
        drawingContext.DrawRectangle(selectionBrush, selectionPen, handleRect);
        handleRect = new Rect(selectionRect.Right-3, selectionRect.Top-3, HandleSize, HandleSize);
        drawingContext.DrawRectangle(selectionBrush, selectionPen, handleRect);
        handleRect = new Rect(selectionRect.Right-3, selectionRect.Bottom-3, HandleSize, HandleSize);
        drawingContext.DrawRectangle(selectionBrush, selectionPen, handleRect);
        handleRect = new Rect(selectionRect.Left-3, selectionRect.Bottom-3, HandleSize, HandleSize);
        drawingContext.DrawRectangle(selectionBrush, selectionPen, handleRect);
        handleRect = new Rect((selectionRect.Left+selectionRect.Right)/2-3, selectionRect.Top-3, HandleSize, HandleSize);
        drawingContext.DrawRectangle(selectionBrush, selectionPen, handleRect);
        handleRect = new Rect(selectionRect.Right-3, (selectionRect.Top+selectionRect.Bottom)/2-3, HandleSize, HandleSize);
        drawingContext.DrawRectangle(selectionBrush, selectionPen, handleRect);
        handleRect = new Rect((selectionRect.Left+selectionRect.Right)/2-3, selectionRect.Bottom-3, HandleSize, HandleSize);
        drawingContext.DrawRectangle(selectionBrush, selectionPen, handleRect);
        handleRect = new Rect(selectionRect.Left-3, (selectionRect.Top+selectionRect.Bottom)/2-3, HandleSize, HandleSize);
        drawingContext.DrawRectangle(selectionBrush, selectionPen, handleRect);
      }
      this.RenderTransform=transformSave;
    }

    protected override void OnMouseDown(MouseButtonEventArgs args)
    {
      var pos = args.GetPosition(this);
      pos.X /=Scale;
      pos.Y /=Scale;
      int X = (int)pos.X;
      int Y = (int)pos.Y;
      //Debug.WriteLine($"MouseDown({X}, {Y})");
      {
        switch (Mode)
        {
          case BitmapEditMode.Select:
            StartSelect(X, Y, args.ChangedButton);
            break;
          case BitmapEditMode.SetPoint:
            SetPoint(X, Y, args.ChangedButton);
            break;
          case BitmapEditMode.GetPoint:
            GetPoint(X, Y, args.ChangedButton);
            break;
          case BitmapEditMode.FloodFill:
            FloodFill(X, Y, args.ChangedButton);
            break;
          case BitmapEditMode.FillAll:
            FillAll(X, Y, args.ChangedButton);
            break;
          case BitmapEditMode.MagicWand:
            MagicWand(X, Y, args.ChangedButton);
            break;
        }
      }
    }

    protected override void OnPreviewMouseMove(MouseEventArgs args)
    {
      if (Mode.HasFlag(BitmapEditMode.Started))
      {

      }
    }

    #region selection
    private void StartSelect(int X, int Y, MouseButton button)
    {
      if (X>=0 && X<Source.PixelWidth && Y>=0 && Y<Source.PixelHeight)
      {
        if (button == MouseButton.Left)
          StartSelect(X, Y);
        if (button == MouseButton.Right)
          ClearSelection();
      }
    }

    public void StartSelect(int X, int Y)
    {
      Selection = new Rect(X, Y, 1, 1);
      InvalidateVisual();
    }
    public void ClearSelection()
    {
      Selection = new Rect();
      InvalidateVisual();
    }
    #endregion selection

    private void SetPoint(int X, int Y, MouseButton button)
    {
      if (X>=0 && X<Source.PixelWidth && Y>=0 && Y<Source.PixelHeight)
      {
        if (button == MouseButton.Left)
          SetPoint(X, Y, PrimaryColor);
        if (button == MouseButton.Right)
          SetPoint(X, Y, SecondaryColor);
      }
    }

    public void SetPoint(int X, int Y, Color color)
    {
      var pixels = GetPixelArray(Source);
      pixels[X, Y] = color.ToDrawingColor();
      SetPixelArray(Source as WriteableBitmap, pixels);
      InvalidateVisual();
    }

    private void GetPoint(int X, int Y, MouseButton button)
    {
      if (X>=0 && X<Source.PixelWidth && Y>=0 && Y<Source.PixelHeight)
      {
        if (button == MouseButton.Left)
          PrimaryColor = GetPoint(X, Y);
        if (button == MouseButton.Right)
          SecondaryColor = GetPoint(X, Y);
      }
    }

    public Color GetPoint(int X, int Y)
    {
      var pixels = GetPixelArray(Source);
      return pixels[X, Y].ToMediaColor();
    }

    private void FloodFill(int X, int Y, MouseButton button)
    {
      if (X>=0 && X<Source.PixelWidth && Y>=0 && Y<Source.PixelHeight)
      {
        if (button == MouseButton.Left)
          FloodFill(X, Y, PrimaryColor);
        if (button == MouseButton.Right)
          FloodFill(X, Y, SecondaryColor);
      }
    }

    public void FloodFill(int X, int Y, Color color)
    {
      var pixels = GetPixelArray(Source);
      pixels.FloodFill(X, Y, color.ToDrawingColor());
      SetPixelArray(Source as WriteableBitmap, pixels);
      InvalidateVisual();
    }

    private void FillAll(int X, int Y, MouseButton button)
    {
      if (X>=0 && X<Source.PixelWidth && Y>=0 && Y<Source.PixelHeight)
      {
        if (button == MouseButton.Left)
          FillAll(PrimaryColor);
        if (button == MouseButton.Right)
          FillAll(SecondaryColor);
      }
    }

    public void FillAll(Color color)
    {
      var pixels = GetPixelArray(Source);
      pixels.FillAll(color.ToDrawingColor());
      SetPixelArray(Source as WriteableBitmap, pixels);
      InvalidateVisual();
    }

    private void MagicWand(int X, int Y, MouseButton button)
    {
      if (X>=0 && X<Source.PixelWidth && Y>=0 && Y<Source.PixelHeight)
      {
        if (button == MouseButton.Left)
          MagicWand(X, Y, PrimaryColor);
        if (button == MouseButton.Right)
          MagicWand(X, Y, SecondaryColor);
      }
    }

    public void MagicWand(int X, int Y, Color color)
    {
      var pixels = GetPixelArray(Source);
      var mask = pixels.WandMask(X, Y, DrawingColor.Black, DrawingColor.White);
      Mask = new WriteableBitmap(Source);
      SetPixelArray(Mask as WriteableBitmap, mask);
      InvalidateVisual();
    }
  }
}
