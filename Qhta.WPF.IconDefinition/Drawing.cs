using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;


namespace Qhta.WPF.IconDefinition
{
  [ContentProperty("Items")]
  public class Drawing: FrameworkElement
  {
    public Drawing()
    {
      Items = new DrawingItemsCollection();
    }

    #region Items property
    public DrawingItemsCollection Items
    {
      get => (DrawingItemsCollection)GetValue(ItemsProperty);
      set => SetValue(ItemsProperty, value);
    }

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register
      ("Items", typeof(DrawingItemsCollection), typeof(Drawing),
       new FrameworkPropertyMetadata(null,
         FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsRender,
         ItemsPropertyChanged)
      );

    private static void ItemsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as Drawing).ItemsChanged();
    }

    private void ItemsChanged()
    {
      if (Items!=null)
        Items.CollectionChanged+=Items_CollectionChanged;
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      InvalidateBindings();
      InvalidateVisual();
      if (args.Action == NotifyCollectionChangedAction.Add)
      {
        foreach (var drawingItem in args.NewItems.Cast<DrawingItem>())
          drawingItem.PropertyChanged+=DrawingItem_PropertyChanged;
      }
    }

    private void DrawingItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      this.InvalidateVisual();
      this.Invalidated?.Invoke(this, new EventArgs());
    }
    #endregion

    public void InvalidateBindings()
    {
      if (Items!=null)
        foreach (var item in Items)
          item.InvalidateBindings();
      if (Invalidated!=null)
        Invalidated.Invoke(this, new EventArgs());
    }

    public event EventHandler Invalidated;

    public void Draw(Qhta.Drawing.DrawingContext context)
    {
      var newContext = new Qhta.Drawing.DrawingContext
      {
        Graphics=context.Graphics,
        ScaleX = context.ScaleX,
        ScaleY = context.ScaleY,
      };
      foreach (var item in Items)
        item.Draw(newContext);
    }

    public void Draw(DrawingContext context)
    {
      foreach (var item in Items)
        item.Draw(context);
    }

    public Geometry GetFillGeometry()
    {
      var group = new GeometryGroup();
      foreach (var item in Items)
        group.Children.Add(item.GetFillGeometry());
      return group;
    }

    public Geometry GetOutlineGeometry()
    {
      var group = new GeometryGroup();
      foreach (var item in Items)
        group.Children.Add(item.GetOutlineGeometry());
      return group;
    }
  }
}
