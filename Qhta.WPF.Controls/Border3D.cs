using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Qhta.WPF.Controls
{
  /// <summary>
  /// Kontrolka symulująca 3-wymiarowe obramowanie
  /// </summary>
  [TemplateVisualState(Name = "Embossed", GroupName = "LayoutStates")]
  [TemplateVisualState(Name = "Engraved", GroupName = "LayoutStates")]
  public class Border3D : ContentControl, INotifyPropertyChanged
  {
    /// <summary>
    /// Zdarzenie implementujące interfejs <c>INotifyPropertyChanged</c>
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
    
    /// <summary>
    /// Konstruktor domyślny
    /// </summary>
    public Border3D ()
    {
      this.DefaultStyleKey = typeof(Border3D);
    }

    /// <summary>
    /// Overrides the OnApplyTemplate method.
    /// </summary>
    public override void OnApplyTemplate ()
    {
      base.OnApplyTemplate();
      ChangeVisualState();
    }

    /// <summary>
    /// Przy zmianie rozmiaru następuje przeliczenie wielokątów
    /// </summary>
    /// <param name="constraint"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size constraint)
    {
      Size result = base.MeasureOverride(constraint);
      if (Double.IsInfinity(constraint.Width))
        constraint.Width = result.Width;
      if (Double.IsInfinity(constraint.Height))
        constraint.Height = result.Height;
      ResetPolygons(constraint);
      ChangeVisualState();
      return result;
    }


    private void ResetPolygons(Size size)
    {
      ResetPolygon(_LeftTopPoints, size, new Point(0, 0));
      ResetPolygon(_RightBottomPoints, size, new Point(size.Width, size.Height));
    }

    private void ResetPolygon(ObservableCollection<Point> points, Size size, Point start)
    {
      points.Clear();
      points.Add(start);
      points.Add(new Point(size.Width, 0));
      if (size.Width >= size.Height)
      {
        points.Add(new Point(size.Width - size.Height / 2, size.Height / 2));
        points.Add(new Point(size.Height / 2, size.Height / 2));
      }
      else
      {
        points.Add(new Point(size.Width / 2, size.Width / 2));
        points.Add(new Point(size.Width / 2, size.Height - size.Width / 2));
      }
      points.Add(new Point(0, size.Height));
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs("LeftTopPoints"));
        PropertyChanged(this, new PropertyChangedEventArgs("RightBottomPoints"));
      }
    }

    /// <summary>
    /// Wielokąt dla lewego, górnego wierzchołka
    /// </summary>
    public ObservableCollection<Point> LeftTopPoints { get { return _LeftTopPoints; } }
    private ObservableCollection<Point> _LeftTopPoints = new ObservableCollection<Point>();
    /// <summary>
    /// Wielokąt dla prawego, dolnego wierzchołka
    /// </summary>    
    public ObservableCollection<Point> RightBottomPoints { get { return _RightBottomPoints; } }
    private ObservableCollection<Point> _RightBottomPoints = new ObservableCollection<Point>();

    /// <summary>
    /// Changes the control's visual state(s).
    /// </summary>
    protected virtual void ChangeVisualState ()
    {
      VisualStateManager.GoToState(this, Layout3D==Border3DLayout.Embossed ? "Embossed" : "Engraved", false);
    }

    /// <summary>
    /// Sposób prezentacji wytłoczenie/wyrzeźbienie
    /// </summary>
    public Border3DLayout Layout3D
    {
      get { return (Border3DLayout)GetValue(Layout3DProperty); }
      set { SetValue(Layout3DProperty, value); }
    }

    /// <summary>
    /// Właściwość zależna dla <see cref="Layout3D"/>
    /// </summary>
    public static readonly DependencyProperty Layout3DProperty = DependencyProperty.Register(
        "Layout3D",
        typeof(Border3DLayout),
        typeof(Border3D),
        new PropertyMetadata(Border3DLayout.Embossed, new PropertyChangedCallback(OnLayout3DChanged)));

    /// <summary>
    /// Obsługa zmiany właściwości zależnej <see cref="Layout3D"/>
    /// </summary>
    private static void OnLayout3DChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as Border3D).ChangeVisualState();
    }


  }

  /// <summary>
  /// Wybór sposobu prezentacji obramowania
  /// </summary>
  public enum Border3DLayout
  {
    /// <summary>
    /// Wytłoczone (światło z góry i z lewej)
    /// </summary>
    Embossed,
    /// <summary>
    /// Wyrzeźbione (światło z dołu i z prawej)
    /// </summary>
    Engraved
  }
}
