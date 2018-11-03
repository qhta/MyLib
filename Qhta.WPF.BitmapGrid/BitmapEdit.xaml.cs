using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qhta.WindowsMedia.ColorUtils;
using PixelArray = Qhta.Drawing.ArrayGraphics.PixelArray;

namespace Qhta.WPF.Controls
{

  public partial class BitmapEdit : UserControl
  {
    public BitmapEdit()
    {
      InitializeComponent();
      SnapsToDevicePixels=true;
    }

    #region Source property
    /// <summary>
    /// Bitmapa jako źródło i miejsce przechowywania danych. Musi być typu <c>WriteableBitmap</c>
    /// </summary>
    public WriteableBitmap Source
    {
      get => (WriteableBitmap)GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    public static DependencyProperty SourceProperty = DependencyProperty.Register
      ("Source", typeof(WriteableBitmap), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Mask property
    /// <summary>
    /// Bitmapa jako źródło i miejsce przechowywania maski danych. Może być typu <c>WriteableBitmap</c>
    /// </summary>
    public BitmapSource Mask
    {
      get => (BitmapSource)GetValue(MaskProperty);
      set => SetValue(MaskProperty, value);
    }

    public static DependencyProperty MaskProperty = DependencyProperty.Register
      ("Mask", typeof(BitmapSource), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Scale property
    /// <summary>
    /// Skala wyświetlania - rozmiar kwadratu na piksel
    /// </summary>
    public int Scale
    {
      get => (int)GetValue(ScaleProperty);
      set => SetValue(ScaleProperty, value);
    }

    public static DependencyProperty ScaleProperty = DependencyProperty.Register
      ("Scale", typeof(int), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure));
    #endregion

    #region ShowRaster property
    /// <summary>
    /// Czy pokazywać linie siatki
    /// </summary>
    public bool ShowRaster
    {
      get => (bool)GetValue(ShowRasterProperty);
      set => SetValue(ShowRasterProperty, value);
    }

    public static DependencyProperty ShowRasterProperty = DependencyProperty.Register
      ("ShowRaster", typeof(bool), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region RasterThickness property
    /// <summary>
    /// Grubość linii rastra
    /// </summary>
    public double RasterThickness
    {
      get => (double)GetValue(RasterThicknessProperty);
      set => SetValue(RasterThicknessProperty, value);
    }

    public static DependencyProperty RasterThicknessProperty = DependencyProperty.Register
      ("RasterThickness", typeof(double), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender,
        ZoomPropertyChanged));
    #endregion

    #region SelectionFrameThickness property
    /// <summary>
    /// Grubość linii ramki wyboru
    /// </summary>
    public double SelectionFrameThickness
    {
      get => (double)GetValue(SelectionFrameThicknessProperty);
      set => SetValue(SelectionFrameThicknessProperty, value);
    }

    public static DependencyProperty SelectionFrameThicknessProperty = DependencyProperty.Register
      ("SelectionFrameThickness", typeof(double), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender,
        ZoomPropertyChanged));
    #endregion

    #region Zoom property
    /// <summary>
    /// Powiększenie wyświetlania -mnożnik skali
    /// </summary>
    public double Zoom
    {
      get => (double)GetValue(ZoomProperty);
      set => SetValue(ZoomProperty, value);
    }

    public static DependencyProperty ZoomProperty = DependencyProperty.Register
      ("Zoom", typeof(double), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender,
        ZoomPropertyChanged));


    private static void ZoomPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as BitmapEdit).ZoomChanged();
    }

    private void ZoomChanged()
    {
      var rasterThickness = RasterThickness/Zoom;
      if (Scale*Zoom>=5)
        Raster.RasterThickness = rasterThickness;
      else
        Raster.RasterThickness = 0;

      var lineThickness = SelectionFrameThickness/Zoom;
      if (lineThickness<0.5)
        lineThickness=0.5;
      if (lineThickness>5)
        lineThickness=5;
      SelectionOverlay.LineThickness = lineThickness;
    }
    #endregion

    #region Mode property
    /// <summary>
    /// Tryb, w którym znajduje się kontrolka. Domyślnie <c>Select</c>
    /// </summary>
    public BitmapEditMode Mode
    {
      get => (BitmapEditMode)GetValue(ModeProperty);
      set => SetValue(ModeProperty, value);
    }

    public static DependencyProperty ModeProperty = DependencyProperty.Register
      ("Mode", typeof(BitmapEditMode), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(BitmapEditMode.SetPoint, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region PrimaryColor property
    /// <summary>
    /// Kolor główny (odpowiadający wypełnieniu na lewy przycisk myszy)
    /// </summary>
    public Color PrimaryColor
    {
      get => (Color)GetValue(PrimaryColorProperty);
      set => SetValue(PrimaryColorProperty, value);
    }

    public static DependencyProperty PrimaryColorProperty = DependencyProperty.Register
      ("PrimaryColor", typeof(Color), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region SecondaryColor property
    /// <summary>
    /// Kolor drugorzędny (odpowiadający wypełnieniu na prawy przycisk myszy)
    /// </summary>
    public Color SecondaryColor
    {
      get => (Color)GetValue(SecondaryColorProperty);
      set => SetValue(SecondaryColorProperty, value);
    }

    public static DependencyProperty SecondaryColorProperty = DependencyProperty.Register
      ("SecondaryColor", typeof(Color), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region SelectionColor property
    /// <summary>
    /// Kolor prostokąta selekcji
    /// </summary>
    public Color SelectionColor
    {
      get => (Color)GetValue(SelectionColorProperty);
      set => SetValue(SelectionColorProperty, value);
    }

    public static DependencyProperty SelectionColorProperty = DependencyProperty.Register
      ("SelectionColor", typeof(Color), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region SelectionLightColor
    /// <summary>
    /// Kolor rozjaśnienia prostokąta selekcji.
    /// </summary>
    public Color SelectionLightColor
    {
      get => (Color)GetValue(SelectionLightColorProperty);
      set => SetValue(SelectionLightColorProperty, value);
    }

    public static DependencyProperty SelectionLightColorProperty = DependencyProperty.Register
      ("SelectionLightColor", typeof(Color), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(Colors.Yellow, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Selection property
    public static DependencyProperty SelectionProperty = DependencyProperty.Register
      ("Selection", typeof(Rect), typeof(BitmapEdit),
      new FrameworkPropertyMetadata(new Rect(), FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Skala wyświetlania - rozmiar kwadratu na piksel
    /// </summary>
    public Rect Selection
    {
      get => (Rect)GetValue(SelectionProperty);
      set => SetValue(SelectionProperty, value);
    }
    #endregion

    #region Bounds measure
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

    protected override Size MeasureOverride(Size availableSize)
    {
      if (Source!=null)
        return imageSize = new Size(Source.PixelWidth*Scale, Source.PixelHeight*Scale);
      return base.MeasureCore(availableSize);
    }
    /// <summary>
    /// Przychowywany rozmiar obrazu * skala
    /// </summary>
    Size imageSize = new Size(16, 16);
    #endregion

    #region Mouse events handling

    bool MouseStarted = false;
    MouseButton MouseButton;

    protected override void OnMouseDown(MouseButtonEventArgs args)
    {
      Debug.WriteLine($"OnMouseDown({args.ChangedButton})");
      if (MouseStarted)
        return;
      var pos = args.GetPosition(Raster);
      pos.X /=Scale;
      pos.Y /=Scale;
      int X = (int)pos.X;
      int Y = (int)pos.Y;
      switch (Mode)
      {
        case BitmapEditMode.Select:
          StartSelect(X, Y, args.ChangedButton);
          break;
        case BitmapEditMode.SetPoint:
          SaveStateForUndo();
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
      MouseStarted = true;
      MouseButton = args.ChangedButton;
    }

    protected override void OnMouseMove(MouseEventArgs args)
    {
      //Debug.WriteLine($"OnMouseMove(MouseStarted={MouseStarted})");
      if (MouseStarted)
      {
        var pos = args.GetPosition(Raster);
        if (!CanDetermineMousePos(pos, out int X, out int Y))
          return;
        switch (Mode)
        {
          //case BitmapEditMode.Select:
          //  StartSelect(X, Y, args.ChangedButton);
          //  break;
          case BitmapEditMode.SetPoint:
            SetPoint(X, Y, MouseButton);
            break;
          //case BitmapEditMode.GetPoint:
          //  GetPoint(X, Y, args.ChangedButton);
          //  break;
          //case BitmapEditMode.FloodFill:
          //  FloodFill(X, Y, args.ChangedButton);
          //  break;
          //case BitmapEditMode.FillAll:
          //  FillAll(X, Y, args.ChangedButton);
          //  break;
          //case BitmapEditMode.MagicWand:
          //  MagicWand(X, Y, args.ChangedButton);
          //  break;
        }
      }
    }

    protected override void OnMouseUp(MouseButtonEventArgs args)
    {
      Debug.WriteLine($"OnMouseUp({args.ChangedButton})");
      if (MouseStarted)
      {
        MouseStarted = false;
        //var pos = args.GetPosition(Raster);
        //pos.X /=Scale;
        //pos.Y /=Scale;
        //int X = (int)pos.X;
        //int Y = (int)pos.Y;
        //switch (Mode)
        //{
        //  case BitmapEditMode.Select:
        //    StartSelect(X, Y, args.ChangedButton);
        //    break;
        //  case BitmapEditMode.SetPoint:
        //    SetPoint(X, Y, args.ChangedButton);
        //    break;
        //  case BitmapEditMode.GetPoint:
        //    GetPoint(X, Y, args.ChangedButton);
        //    break;
        //  case BitmapEditMode.FloodFill:
        //    FloodFill(X, Y, args.ChangedButton);
        //    break;
        //  case BitmapEditMode.FillAll:
        //    FillAll(X, Y, args.ChangedButton);
        //    break;
        //  case BitmapEditMode.MagicWand:
        //    MagicWand(X, Y, args.ChangedButton);
        //    break;
        //}
      }
    }

    private bool CanDetermineMousePos(Point pos, out int X, out int Y)
    {
      pos.X /=Scale;
      pos.Y /=Scale;
      X = (int)pos.X;
      Y = (int)pos.Y;
      double xRest = Math.Abs(pos.X-X);
      double yRest = Math.Abs(pos.Y-Y);
      //Debug.WriteLine($"pos/Scale=({pos.X}, {pos.Y})->({X}, {Y})");
      return xRest>0.1 && yRest>0.1;
    }
    #endregion

    #region Selection handling
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
      SelectionOverlay.InvalidateVisual();
    }

    public void ClearSelection()
    {
      Selection = new Rect();
      InvalidateVisual();
    }
    #endregion selection

    #region Undo handling
    PixelArray undoState;

    private void SaveStateForUndo()
    {
      undoState = BitmapRaster.GetPixelArray(Source);
      CanUndo = Source is WriteableBitmap;
    }

    public bool CanUndo;

    public void Undo()
    {
      if (CanUndo)
      {
        BitmapRaster.SetPixelArray(Source as WriteableBitmap, undoState);
        Raster.InvalidateVisual();
      }
    }
    #endregion

    #region SetPoint handling
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
      var pixels = BitmapRaster.GetPixelArray(Source);
      pixels[X, Y] = color.ToDrawingColor();
      BitmapRaster.SetPixelArray(Source as WriteableBitmap, pixels);
      InvalidateVisual();
    }
    #endregion

    #region GetPoint handling
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
      var pixels = BitmapRaster.GetPixelArray(Source);
      return pixels[X, Y].ToMediaColor();
    }
    #endregion

    #region FloodFill handling
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
      var pixels = BitmapRaster.GetPixelArray(Source);
      pixels.FloodFill(X, Y, color.ToDrawingColor());
      BitmapRaster.SetPixelArray(Source as WriteableBitmap, pixels);
      InvalidateVisual();
    }
    #endregion

    #region FillAll handling
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
      var pixels = BitmapRaster.GetPixelArray(Source);
      pixels.FillAll(color.ToDrawingColor());
      BitmapRaster.SetPixelArray(Source as WriteableBitmap, pixels);
      InvalidateVisual();
    }
    #endregion

    #region MagicWand handling
    private void MagicWand(int X, int Y, MouseButton button)
    {
      if (X>=0 && X<Source.PixelWidth && Y>=0 && Y<Source.PixelHeight)
      {
        var maskColor = Color.FromArgb(128, 255, 0, 0);
        var unmaskColor = Colors.Transparent;
        if (button == MouseButton.Left)
          MagicWand(X, Y, maskColor, unmaskColor);
        if (button == MouseButton.Right)
          MagicWand(X, Y, unmaskColor, maskColor);
      }
    }

    public void MagicWand(int X, int Y, Color maskColor, Color unmaskColor)
    {
      var pixels = BitmapRaster.GetPixelArray(Source);
      var mask = pixels.WandMask(X, Y, maskColor.ToDrawingColor(), unmaskColor.ToDrawingColor());
      var maskBitmap = new WriteableBitmap(Source);
      BitmapRaster.SetPixelArray(maskBitmap, mask);
      Mask = maskBitmap;
    }
    #endregion

  }
}
