using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public static class CollectionViewBehavior
  {
    static CollectionViewBehavior()
    {
      BindingOperations.CollectionRegistering += BindingOperations_CollectionRegistering;
      BindingOperations.CollectionViewRegistering += BindingOperations_CollectionViewRegistering;
    }

    public static bool GetEnableCollectionSynchronization(DependencyObject obj)
    {
      return (bool)obj.GetValue(EnableCollectionSynchronizationProperty);
    }
    public static void SetEnableCollectionSynchronization(DependencyObject obj, bool value)
    {
      obj.SetValue(EnableCollectionSynchronizationProperty, value);
    }

    public static readonly DependencyProperty EnableCollectionSynchronizationProperty =
        DependencyProperty.RegisterAttached("EnableCollectionSynchronization", typeof(bool), typeof(CollectionViewBehavior),
          new UIPropertyMetadata(false, EnableCollectionSynchronizationPropertyChangedCallback));

    public static void EnableCollectionSynchronizationPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      //Debug.WriteLine($"EnableCollectionSynchronizationPropertyChangedCallback({obj}, {args.NewValue})");
      if (obj is ItemsControl itemsControl)
      {
        itemsControl.Loaded += ItemsControl_Loaded;
      }
    }

    private static void BindingOperations_CollectionViewRegistering(object sender, CollectionViewRegisteringEventArgs args)
    {
      //Debug.WriteLine($"CollectionViewRegistering({sender},{args.CollectionView})");
      //ListCollectionView cv = args.CollectionView as ListCollectionView;
      //if (cv != null)
      //{
        //Debug.WriteLine($"AllowsCrossThreadChanges={cv.GetType().GetProperty("AllowsCrossThreadChanges", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(cv)}");
      //}
    }

    private static void BindingOperations_CollectionRegistering(object sender, CollectionRegisteringEventArgs args)
    {
      //Debug.WriteLine($"CollectionRegistering({sender},{args.Collection})");
      if (args.Collection is IList itemsCollection)
        EnableCollectionSynchronization(itemsCollection, true);
    }

    private static void ItemsControl_Loaded(object sender, RoutedEventArgs e)
    {
      //Debug.WriteLine($"ItemsControl_Loaded({sender})");
      if (sender is ItemsControl itemsControl)
      {
        var enable = GetEnableCollectionSynchronization(itemsControl);
        EnableCollectionSynchronization(itemsControl, enable);
      }
    }

    private static void EnableCollectionSynchronization(ItemsControl itemsControl, bool enable)
    {
      var binding = itemsControl.GetBindingExpression(ItemsControl.ItemsSourceProperty);
      var source = binding.ResolvedSource;
      //Debug.WriteLine($"EnableCollectionSynchronization({itemsControl}, {source}, {enable})");
      if (source is IList collection)
      {
        EnableCollectionSynchronization(collection, enable);
      }
    }

    private static void EnableCollectionSynchronization(IList itemsCollection, bool enable)
    {
      //Debug.WriteLine($"EnableCollectionSynchronization({itemsCollection},{enable})");
      if (enable && itemsCollection.IsSynchronized)
        BindingOperations.EnableCollectionSynchronization(itemsCollection, itemsCollection.SyncRoot);
      else
        BindingOperations.DisableCollectionSynchronization(itemsCollection);
    }
  }

}
