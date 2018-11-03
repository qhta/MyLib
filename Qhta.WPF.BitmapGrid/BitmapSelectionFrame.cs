using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Qhta.WPF.Controls
{
  public class BitmapSelectionFrame : FrameworkElement
  {
    #region Source property
    /// <summary>
    /// Bitmapa jako źródło i miejsce przechowywania danych. Może być typu <c>WriteableBitmap</c>
    /// </summary>
    public BitmapSource Source
    {
      get => (BitmapSource)GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    public static DependencyProperty SourceProperty = DependencyProperty.Register
      ("Source", typeof(BitmapSource), typeof(BitmapSelectionFrame),
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
      ("Scale", typeof(int), typeof(BitmapSelectionFrame),
      new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure));
    #endregion

    #region Selection property
    /// <summary>
    /// Prostokąt wyboru - we współrzędnych pikselowych
    /// </summary>
    public Rect Selection
    {
      get => (Rect)GetValue(SelectionProperty);
      set => SetValue(SelectionProperty, value);
    }

    public static DependencyProperty SelectionProperty = DependencyProperty.Register
      ("Selection", typeof(Rect), typeof(BitmapSelectionFrame),
      new FrameworkPropertyMetadata(new Rect(), FrameworkPropertyMetadataOptions.AffectsRender));
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
      ("SelectionColor", typeof(Color), typeof(BitmapSelectionFrame),
      new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region SelectionLightColor property
    /// <summary>
    /// Kolor rozjaśnienia prostokąta selekcji.
    /// </summary>
    public Color SelectionLightColor
    {
      get => (Color)GetValue(SelectionLightColorProperty);
      set => SetValue(SelectionLightColorProperty, value);
    }

    public static DependencyProperty SelectionLightColorProperty = DependencyProperty.Register
      ("SelectionLightColor", typeof(Color), typeof(BitmapSelectionFrame),
      new FrameworkPropertyMetadata(Colors.Yellow, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region LineThickness property
    /// <summary>
    /// Grubość linii ramki
    /// </summary>
    public double LineThickness
    {
      get => (double)GetValue(LineThicknessProperty);
      set => SetValue(LineThicknessProperty, value);
    }

    public static DependencyProperty LineThicknessProperty = DependencyProperty.Register
      ("LineThickness", typeof(double), typeof(BitmapSelectionFrame),
      new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
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
      return imageSize;
    }

    /// <summary>
    /// Przychowywany rozmiar obrazu * skala
    /// </summary>
    Size imageSize = new Size(0, 0);
    #endregion

    /// <summary>
    /// Metoda wyświetlenia
    /// </summary>
    /// <param name="drawingContext"></param>
    protected override void OnRender(DrawingContext drawingContext)
    {
      if (Source==null)
        return;
      var pixelWidth = Source.PixelWidth;
      var pixelHeight = Source.PixelHeight;
      var scale = Scale;
      var lineThickness = LineThickness;
      if (Selection.Width>0 && Selection.Height>0)
      {
        var selectionRect = new Rect(Selection.Left*scale, Selection.Top*scale, Selection.Width*scale, Selection.Height*scale);
        var selectionBrush = new SolidColorBrush(SelectionLightColor);
        var selectionPen = new Pen(selectionBrush, 3*lineThickness);
        drawingContext.DrawRectangle(null, selectionPen, selectionRect);
        selectionPen = new Pen(new SolidColorBrush(SelectionColor), lineThickness);
        drawingContext.DrawRectangle(null, selectionPen, selectionRect);
      }

    }
  }
}
