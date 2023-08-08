using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Qhta.WPF.Behaviors
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

    private static void BindingOperations_CollectionViewRegistering(object? sender, CollectionViewRegisteringEventArgs args)
    {
      //Debug.WriteLine($"CollectionViewRegistering({sender},{args.CollectionView})");
      CollectionView cv = args.CollectionView as CollectionView;
      if (cv != null)
      {
        //Debug.WriteLine($"AllowsCrossThreadChanges={cv.GetType().GetProperty("AllowsCrossThreadChanges", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(cv)}");
      }
    }

    private static void BindingOperations_CollectionRegistering(object? sender, CollectionRegisteringEventArgs args)
    {
      //Debug.WriteLine($"CollectionRegistering({sender},{args.Collection})");
      if (args.Collection is IList itemsCollection)
        EnableIListCollectionSynchronization(itemsCollection, true);
      else if (args.Collection is IEnumerable enumerable)
        EnableIEnumerableCollectionSynchronization(enumerable, true);
    }

    private static void ItemsControl_Loaded(object sender, RoutedEventArgs e)
    {
      //Debug.WriteLine($"ItemsControl_Loaded({sender})");
      //if (sender is ItemsControl itemsControl)
      //{
      //  var enable = GetEnableCollectionSynchronization(itemsControl);
      //  EnableCollectionSynchronization(itemsControl, enable);
      //}
      //else
      //  Debug.WriteLine($"  {sender} is not an ItemsControl");
    }

    //private static void EnableCollectionSynchronization(ItemsControl itemsControl, bool enable)
    //{
    //  BindingExpression binding = itemsControl.GetBindingExpression(ItemsControl.ItemsSourceProperty);
    //  if (binding != null)
    //  {
    //    var source = binding.ResolvedSource;
    //    //Debug.WriteLine($"EnableCollectionSynchronization({itemsControl}, {source}, {enable})");
    //    if (source is IList collection)
    //    {
    //      EnableCollectionSynchronization(collection, enable);
    //    }
    //    //else
    //      //Debug.WriteLine($"Could not enable CollectionSynchronization. Source does not implement interface IList");
    //  }
    //  else
    //    Debug.WriteLine($"  ItemsSourceProperty binding is null");
    //}

    private static void EnableIListCollectionSynchronization(IList itemsCollection, bool enable)
    {
      //Debug.WriteLine($"EnableIListCollectionSynchronization({itemsCollection},{enable})");
      if (enable && itemsCollection is INotifyCollectionChanged && itemsCollection.IsSynchronized)
      {
        //Debug.WriteLine($"  enable root={itemsCollection.SyncRoot}");
        BindingOperations.EnableCollectionSynchronization(itemsCollection, itemsCollection.SyncRoot);
      }
      else
      {
        //Debug.WriteLine($"  disable");
        BindingOperations.DisableCollectionSynchronization(itemsCollection);
      }
    }

    private static void EnableIEnumerableCollectionSynchronization(IEnumerable itemsCollection, bool enable)
    {
      //Debug.WriteLine($"EnableIEnumerableCollectionSynchronization({itemsCollection},{enable})");
      if (enable && itemsCollection is INotifyCollectionChanged)
      {
        //Debug.WriteLine($"  enable");
        BindingOperations.EnableCollectionSynchronization(itemsCollection, itemsCollection);
      }
      else
      {
        //Debug.WriteLine($"  disable");
        BindingOperations.DisableCollectionSynchronization(itemsCollection);
      }
    }
  }

}
