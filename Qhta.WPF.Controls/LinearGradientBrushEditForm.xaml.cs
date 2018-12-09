using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{
  public partial class LinearGradientBrushEditForm : UserControl
  {
    public LinearGradientBrushEditForm()
    {
      InitializeComponent();
      UndoManagers.BrushUndoManager.StartProtection();
      PreviewKeyDown+=LinearGradientBrushEditForm_PreviewKeyDown;
    }

    public override void OnApplyTemplate()
    {
      GradientSlider.GradientStopsChanged+=GradientSlider_GradientStopsChanged;
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(LinearGradientBrushEditForm),
       new FrameworkPropertyMetadata(Colors.Transparent, SelectedColorPropertyChanged));

    public static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as LinearGradientBrushEditForm).SelectedColorChanged(args);
    }
    public void SelectedColorChanged(DependencyPropertyChangedEventArgs args)
    {
      var brush = EditedBrush;
      if (brush==null)
      {
        var startColor = (Color)args.NewValue;
        var hsv = startColor.ToDrawingColor().ToAhsv();
        hsv.S=0;
        hsv.V=1;
        startColor = hsv.ToColor().ToMediaColor();
        hsv.S=1;
        hsv.V=0;
        var endColor = hsv.ToColor().ToMediaColor();
        brush = new LinearGradientBrush(startColor, endColor, 0);
        if (EditedBrush!=null)
          UndoManagers.BrushUndoManager.SaveState(EditedBrush);
        EditedBrush=brush;
      }
    }
    #endregion

    #region EditedBrush property
    public LinearGradientBrush EditedBrush
    {
      get => (LinearGradientBrush)GetValue(EditedBrushProperty);
      set => SetValue(EditedBrushProperty, value);
    }

    public static readonly DependencyProperty EditedBrushProperty = DependencyProperty.Register
      ("EditedBrush", typeof(LinearGradientBrush), typeof(LinearGradientBrushEditForm),
       new FrameworkPropertyMetadata(null));

    #endregion

    private void GradientSlider_GradientStopsChanged(object sender, ValueChangedEventArgs<GradientStopCollection> args)
    {
      var brush = new LinearGradientBrush(args.NewValue);
      EditedBrush = brush;
      GradientSlider.EditedBrush = brush;
    }

    private void LinearGradientBrushEditForm_PreviewKeyDown(object sender, KeyEventArgs args)
    {
      if (Keyboard.IsKeyDown(Key.LeftCtrl) && args.Key==Key.Z)
      {
        if (UndoManagers.BrushUndoManager.CanUndo)
        {
          var brush = EditedBrush;
          //Debug.WriteLine($"EditedBrush=[{string.Join(", ", brush.GradientStops.Select(item => item.Offset.ToString()))}]");
          brush = (LinearGradientBrush)UndoManagers.BrushUndoManager.UndoChanges(brush);
          //Debug.WriteLine($"UndoBrush=[{string.Join(", ", brush.GradientStops.Select(item=>item.Offset.ToString()))}]");
          EditedBrush = brush;
          GradientSlider.EditedBrush = brush;
        }
      }
      else if (Keyboard.IsKeyDown(Key.LeftCtrl) && args.Key==Key.Y)
      {
        if (UndoManagers.BrushUndoManager.CanRedo)
        {
          var brush = (LinearGradientBrush)UndoManagers.BrushUndoManager.RedoChanges();
          //Debug.WriteLine($"RedoBrush[{string.Join(", ", brush.GradientStops.Select(item=>item.Offset.ToString()))}]");
          EditedBrush = brush;
          GradientSlider.EditedBrush = brush;
        }
      }
    }


  }
}
