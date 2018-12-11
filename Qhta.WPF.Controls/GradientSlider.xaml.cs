using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Qhta.WPF.Utils;

namespace Qhta.WPF.Controls
{
  public partial class GradientSlider : UserControl
  {
    public GradientSlider()
    {
      InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
      GradientStopsView.GradientStopsChanged+=GradientStopsView_GradientStopsChanged;
    }

    #region EditedBrush property
    public GradientBrush EditedBrush
    {
      get => (GradientBrush)GetValue(EditedBrushProperty);
      set => SetValue(EditedBrushProperty, value);
    }

    public static readonly DependencyProperty EditedBrushProperty = DependencyProperty.Register
      ("EditedBrush", typeof(GradientBrush), typeof(GradientSlider),
       new FrameworkPropertyMetadata(null, EditedBrushPropertyChanged));

    public static void EditedBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as GradientSlider).EditedBrushChanged(args);
    }

    public void EditedBrushChanged(DependencyPropertyChangedEventArgs args)
    {
      var brush = args.NewValue as GradientBrush;
      if (brush!=null)
      {
        if (brush.GradientStops.Count==0)
          brush.GradientStops.Add(new GradientStop(Colors.White, 0));
        if (brush.GradientStops.Count==1)
          brush.GradientStops.Add(new GradientStop(Colors.Black, 1));
      }
      ShownBrush=brush.Clone();
    }
    #endregion

    #region ShownBrush property
    public GradientBrush ShownBrush
    {
      get => (GradientBrush)GetValue(ShownBrushProperty);
      set => SetValue(ShownBrushProperty, value);
    }

    public static readonly DependencyProperty ShownBrushProperty = DependencyProperty.Register
      ("ShownBrush", typeof(GradientBrush), typeof(GradientSlider),
       new FrameworkPropertyMetadata(null));
    #endregion

    public event ValueChangedEventHandler<GradientStopCollection> GradientStopsChanged;
    //public event PropertyChangedEventHandler PropertyChanged;

    private void GradientStopsView_GradientStopsChanged(object sender, ValueChangedEventArgs<GradientStopCollection> args)
    {
      GradientStopsChanged?.Invoke(sender, args);
      GradientStopsView_SelectionChanged(this, args);
    }

    private GradientStopMarker SelectedMarker;

    private void GradientStopsView_SelectionChanged(object sender, EventArgs e)
    {
      var marker = GradientStopsView.SelectedMarker;
      // Setting SelectedMarker to null is required to bypass
      // OffsetNumBox_ValueChanged method
      SelectedMarker=null;
      if (marker!=null)
      {
        //BindingOperations.SetBinding(OffsetNumBox, NumericEditBox.ValueProperty,
        //  new Binding("Offset") { Source=marker, Mode=BindingMode.OneWay, Converter=multiplyingConverter, ConverterParameter=100.0 });
        //BindingOperations.SetBinding(ColorPicker, ColorPickerDropDown.SelectedColorProperty, 
        //  new Binding("Color") { Source=marker, Mode=BindingMode.OneWay });
        OffsetNumBox.Value = (decimal)Math.Round(marker.Offset*100);
        ColorPicker.SelectedColor = marker.Color;
      }
      SelectedMarker=marker;
    }

    private static MultiplyingConverter multiplyingConverter = new MultiplyingConverter();

    private void OffsetNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (SelectedMarker!=null)
      {
        UndoManagers.BrushUndoManager.SaveState(EditedBrush);
        GradientStopsView.CopyGradientStopsDisabled = true;
        var marker = SelectedMarker;
        var offset = (double)OffsetNumBox.Value/100.0;
        (marker.DataContext as GradientStop).Offset = marker.Offset = offset;
        GradientStopsChanged?.Invoke(this, new ValueChangedEventArgs<GradientStopCollection>(GradientStopsView.GradientStops));
        GradientStopsView.CopyGradientStopsDisabled = false;
      }
    }

    private void ColorPicker_SelectedColorChanged(Color obj)
    {
      if (SelectedMarker!=null)
      {
        var brush = (GradientBrush)EditedBrush;
        //Debug.WriteLine($"SavedBrush=[{string.Join(", ", brush.GradientStops.Select(item => $"({item.Offset.ToString()}, {item.Color.ToString()})"))}]");
        UndoManagers.BrushUndoManager.SaveState(brush);
        GradientStopsView.CopyGradientStopsDisabled = true;
        var marker = SelectedMarker;
        var color = ColorPicker.SelectedColor;
        (marker.DataContext as GradientStop).Color = marker.Color = color;
        GradientStopsChanged?.Invoke(this, new ValueChangedEventArgs<GradientStopCollection>(GradientStopsView.GradientStops));
        GradientStopsView.CopyGradientStopsDisabled = false;
      }
    }
  }
}
