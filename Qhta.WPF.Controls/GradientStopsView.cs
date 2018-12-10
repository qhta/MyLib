using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Qhta.WPF.Controls
{
  public partial class GradientStopsView : ItemsControl
  {
    public GradientStopsView() : base()
    {
      Background=new SolidColorBrush(Color.FromArgb(1, 1, 1, 1));
      PreviewKeyDown+=GradientStopsView_PreviewKeyDown;
      PreviewKeyUp+=GradientStopsView_PreviewKeyUp;
      MouseDoubleClick+=GradientStopsView_MouseDoubleClick;
      MouseLeftButtonDown+=GradientStopsView_MouseLeftButtonDown;
    }

    private void GradientStopsView_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
      if (Keyboard.IsKeyDown(Key.LeftCtrl))
      {
        AddMarkerAtCursor(args);
      }
    }

    private void GradientStopsView_MouseDoubleClick(object sender, MouseButtonEventArgs args)
    {
      AddMarkerAtCursor(args);
    }

    private void GradientStopsView_PreviewKeyDown(object sender, KeyEventArgs args)
    {
      if (!isMouseDragStarted && Keyboard.IsKeyDown(Key.LeftCtrl))
      {
        var cursor = FindResource("ArrowPlus") as Cursor;
        if (cursor!=null)
        {
          this.Cursor = cursor;
          this.ForceCursor = true;
        }
      }
    }

    private void GradientStopsView_PreviewKeyUp(object sender, KeyEventArgs e)
    {
      if (!Keyboard.IsKeyDown(Key.LeftCtrl))
      {
        this.Cursor = Cursors.Arrow;
        this.ForceCursor = true;
      }
    }

    #region EditedBrush property
    public GradientBrush EditedBrush
    {
      get => (GradientBrush)GetValue(EditedBrushProperty);
      set => SetValue(EditedBrushProperty, value);
    }

    public static readonly DependencyProperty EditedBrushProperty = DependencyProperty.Register
      ("EditedBrush", typeof(GradientBrush), typeof(GradientStopsView),
        new FrameworkPropertyMetadata(null, EditedBrushPropertyChanged));

    private static void EditedBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as GradientStopsView).CopyGradientStopsFrom((GradientBrush)args.NewValue);
    }
    #endregion

    public event ValueChangedEventHandler<GradientStopCollection> GradientStopsChanged;

    #region GradientStops property
    public GradientStopCollection GradientStops
    {
      get => (GradientStopCollection)GetValue(GradientStopsProperty);
      set => SetValue(GradientStopsProperty, value);
    }

    public static readonly DependencyProperty GradientStopsProperty = DependencyProperty.Register
      ("GradientStops", typeof(GradientStopCollection), typeof(GradientStopsView),
        new FrameworkPropertyMetadata(null));
    #endregion

    public bool CopyGradientStopsDisabled;

    private void CopyGradientStopsFrom(GradientBrush brush)
    {
      if (CopyGradientStopsDisabled)
        return;
      if (isMouseDragStarted)
        return;
      Items.Clear();
      if (brush!=null)
      {
        GradientStops = new GradientStopCollection();
        foreach (var stop in brush.GradientStops)
        {
          GradientStop newStop = new GradientStop(stop.Color, stop.Offset);
          GradientStops.Add(newStop);
          AddMarker(new GradientStopMarker { DataContext=newStop, Color=stop.Color, Offset=stop.Offset });
        }
        var firstMarker = Markers.FirstOrDefault();
        if (firstMarker!=null)
          SelectMarker(firstMarker);
      }
    }

    private void AddMarker(GradientStopMarker marker)
    {
      Items.Add(marker);
      marker.MouseLeftButtonDown+=Marker_MouseLeftButtonDown;
      marker.PreviewMouseMove+=Marker_MouseMove;
      marker.PreviewMouseLeftButtonUp+=Marker_MouseLeftButtonUp;
    }


    private void AddMarkerAtCursor(MouseButtonEventArgs args)
    {
      var pos = args.GetPosition(this);
      var brush = EditedBrush;
      var w = (int)ActualWidth;
      var h = (int)ActualHeight;

      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      drawingContext.DrawRectangle(brush, null, new Rect(0,0,w,h));
      drawingContext.Close();

      var bmp = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(drawingVisual);

      int stride = (int)bmp.PixelWidth * (bmp.Format.BitsPerPixel / 8);
      byte[] pixels = new byte[(int)bmp.PixelHeight * stride];
      bmp.CopyPixels(pixels, stride, 0);
      var test = pixels.FirstOrDefault(b=>b!=0);
      var pi = (bmp.Format.BitsPerPixel / 8)*(int)pos.X;
      var color = Color.FromArgb(pixels[pi+3], pixels[pi+2], pixels[pi+1], pixels[pi]);

      var stop = new GradientStop(color, Math.Min(Math.Max(pos.X/this.ActualWidth, 0), 1));
      GradientStops.Add(stop);
      var marker = new GradientStopMarker { DataContext=stop, Color=stop.Color, Offset=stop.Offset};
      AddMarker(marker);
      if (brush!=null)
        UndoManagers.BrushUndoManager.SaveState(brush);
      CopyGradientStopsDisabled=true;
      GradientStopsChanged?.Invoke(this, new ValueChangedEventArgs<GradientStopCollection>(GradientStops));
      SelectMarker(marker);
      CopyGradientStopsDisabled=false;
    }

    public IEnumerable<GradientStopMarker> Markers => Items.Cast<GradientStopMarker>();
    public event EventHandler SelectionChanged;
    public GradientStopMarker SelectedMarker => Markers.FirstOrDefault(item => item.IsSelected);

    private void SelectMarker(GradientStopMarker marker)
    {
      foreach (var item in Items.Cast<GradientStopMarker>())
      {
        item.IsSelected = item==marker;
      }
      // Bring marker to top of the ItemsCollection
      Items.Remove(marker);
      Items.Add(marker);
      SelectionChanged?.Invoke(this, new EventArgs());
    }

    Point mouseStartPos;
    bool isMouseLeftButtonDown;
    bool isMouseDragStarted;

    private void Marker_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
      var marker = sender as GradientStopMarker;
      marker.CaptureMouse();
      mouseStartPos = args.GetPosition(this);
      isMouseDragStarted=false;
      isMouseLeftButtonDown = true;
      SelectMarker(marker);
      args.Handled=true;
    }

    private void Marker_MouseMove(object sender, MouseEventArgs args)
    {
      if (!isMouseLeftButtonDown)
        return;
      args.Handled=true;
      var marker = sender as GradientStopMarker;
      var pos = args.GetPosition(this);
      if (!isMouseDragStarted && (Math.Abs(pos.X-mouseStartPos.X)>3 || Math.Abs(pos.Y-mouseStartPos.Y)>3))
      {
        var brush = EditedBrush;
        if (brush!=null)
          UndoManagers.BrushUndoManager.SaveState(brush);
        isMouseDragStarted=true;
        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
          Mouse.Capture(null);
          var stop = new GradientStop(marker.Color, Math.Min(Math.Max(pos.X/this.ActualWidth, 0), 1));
          GradientStops.Add(stop);
          marker = new GradientStopMarker { DataContext=stop, Color=stop.Color, Offset=stop.Offset};
          AddMarker(marker);
          SelectMarker(marker);
          Mouse.Capture(marker);
          args.Handled=true;
        }
      }
      if (isMouseDragStarted)
      {
        this.Cursor=Cursors.Arrow;
        (marker.DataContext as GradientStop).Offset = marker.Offset = Math.Min(Math.Max(pos.X/this.ActualWidth, 0), 1);
        GradientStopsChanged?.Invoke(this, new ValueChangedEventArgs<GradientStopCollection>(GradientStops));
      }
    }

    private void Marker_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
    {
      if (!isMouseLeftButtonDown)
        return;
      isMouseLeftButtonDown=false;
      var marker = sender as GradientStopMarker;
      Mouse.Capture(null);
      if (isMouseDragStarted)
      {
        isMouseDragStarted=false;
      }
    }

  }
}

