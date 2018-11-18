using System.ComponentModel;
using System.Windows;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Windows.Media;

namespace Qhta.WPF.IconDefinition
{
  [KnownType(typeof(Rectangle))]
  public abstract class DrawingItem: DependencyObject, INotifyPropertyChanged
  {

    #region INotifyPropertyChanged interface support
    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected static void DependencyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as DrawingItem).NotifyPropertyChanged(args.Property.Name);
    }
    #endregion

    #region Left property
    public double Left
    {
      get => (double)GetValue(LeftProperty);
      set => SetValue(LeftProperty, value);
    }
    public static DependencyProperty LeftProperty = DependencyProperty.Register
      ("Left", typeof(double), typeof(DrawingItem),
       new PropertyMetadata(0.0, DependencyPropertyChanged));
    #endregion

    #region Top property
    public double Top
    {
      get => (double)GetValue(TopProperty);
      set => SetValue(TopProperty, value);
    }
    public static DependencyProperty TopProperty = DependencyProperty.Register
      ("Top", typeof(double), typeof(DrawingItem),
       new PropertyMetadata(0.0, DependencyPropertyChanged));
    #endregion

    #region Width property
    public double Width
    {
      get => (double)GetValue(WidthProperty);
      set => SetValue(WidthProperty, value);
    }
    public static DependencyProperty WidthProperty = DependencyProperty.Register
      ("Width", typeof(double), typeof(DrawingItem),
       new PropertyMetadata(0.0, DependencyPropertyChanged));
    #endregion

    #region Height property
    public double Height
    {
      get => (double)GetValue(HeightProperty);
      set => SetValue(HeightProperty, value);
    }
    public static DependencyProperty HeightProperty = DependencyProperty.Register
      ("Height", typeof(double), typeof(DrawingItem),
       new PropertyMetadata(0.0, DependencyPropertyChanged));

    #endregion

    public virtual void InvalidateBindings()
    {
      BindingOperations.GetBindingExpressionBase(this, DrawingItem.LeftProperty)?.UpdateTarget();
      BindingOperations.GetBindingExpressionBase(this, DrawingItem.TopProperty)?.UpdateTarget();
      BindingOperations.GetBindingExpressionBase(this, DrawingItem.WidthProperty)?.UpdateTarget();
      BindingOperations.GetBindingExpressionBase(this, DrawingItem.HeightProperty)?.UpdateTarget();
    }

    public abstract void Draw(Qhta.Drawing.DrawingContext context);

    public abstract void Draw(System.Windows.Media.DrawingContext context);


    public abstract Geometry GetFillGeometry();

    public abstract Geometry GetOutlineGeometry();

    public abstract bool Contains(Point point);
  }
}
