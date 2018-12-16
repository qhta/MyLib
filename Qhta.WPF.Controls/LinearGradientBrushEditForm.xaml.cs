using System;
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


    #region SelectedBrush property
    /// <summary>
    /// A brush set to edition and returned edited
    /// </summary>
    public Brush SelectedBrush
    {
      get => (Brush)GetValue(SelectedBrushProperty);
      set => SetValue(SelectedBrushProperty, value);
    }

    public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register
      ("SelectedBrush", typeof(Brush), typeof(LinearGradientBrushEditForm),
       new FrameworkPropertyMetadata(null, SelectedBrushPropertyChanged));

    public static void SelectedBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as LinearGradientBrushEditForm).SelectedBrushChanged(args);
    }

    public void SelectedBrushChanged(DependencyPropertyChangedEventArgs args)
    {
      if (SelectedBrush is LinearGradientBrush linearGradientBrush)
        EditedBrush = linearGradientBrush;
      else 
      if (SelectedBrush is SolidColorBrush solidColorBrush)
      {
        var startColor = solidColorBrush.Color;
        var hsv = startColor.ToDrawingColor().ToAhsv();
        hsv.S=0;
        hsv.V=1;
        startColor = hsv.ToColor().ToMediaColor();
        hsv.S=1;
        hsv.V=0;
        var endColor = hsv.ToColor().ToMediaColor();
        var brush = new LinearGradientBrush(startColor, endColor, 0);// new Point(0,0), new Point(1,0));
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
       new FrameworkPropertyMetadata(null, EditedBrushPropertyChanged));

    private static void EditedBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      //(sender as LinearGradientBrushEditForm).UpdateAngleDataViews();
    }

    #endregion

    private void GradientSlider_GradientStopsChanged(object sender, ValueChangedEventArgs<GradientStopCollection> args)
    {
      var oldBrush = EditedBrush;
      var brush = new LinearGradientBrush(args.NewValue, oldBrush.StartPoint, oldBrush.EndPoint);
      EditedBrush = brush;
      //GradientSlider.EditedBrush = brush;
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
          //Debug.WriteLine($"UndonedBrush=[{string.Join(", ", brush.GradientStops.Select(item => $"({item.Offset.ToString()}, {item.Color.ToString()})"))}]");
          EditedBrush = brush;
        }
      }
      else if (Keyboard.IsKeyDown(Key.LeftCtrl) && args.Key==Key.Y)
      {
        if (UndoManagers.BrushUndoManager.CanRedo)
        {
          var brush = (LinearGradientBrush)UndoManagers.BrushUndoManager.RedoChanges();
          //Debug.WriteLine($"RedoBrush[{string.Join(", ", brush.GradientStops.Select(item=>item.Offset.ToString()))}]");
          EditedBrush = brush;
        }
      }
    }
    public event ValueChangedEventHandler<Brush> BrushSelected;
    public event EventHandler CloseFormRequest;

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
      SelectedBrush = EditedBrush;
      BrushSelected?.Invoke(this, new ValueChangedEventArgs<Brush>(SelectedBrush));
      CloseFormRequest(this, new EventArgs());
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      CloseFormRequest(this, new EventArgs());
    }

  }
}
