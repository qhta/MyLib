using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public class TreeViewBehavior
  {
    private static Dictionary<DependencyObject, TreeViewSelectedItemBehavior> behaviors = new Dictionary<DependencyObject, TreeViewSelectedItemBehavior>();

    public static object GetSelectedItem(DependencyObject obj)
    {
      return obj.GetValue(SelectedItemProperty);
    }

    public static void SetSelectedItem(DependencyObject obj, object value)
    {
      obj.SetValue(SelectedItemProperty, value);
    }

    // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached
      ("SelectedItem", typeof(object), typeof(TreeViewBehavior),
          new UIPropertyMetadata(null, SelectedItemChanged));

    private static void SelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      if (!(obj is TreeView))
        return;

      if (!behaviors.ContainsKey(obj))
        behaviors.Add(obj, new TreeViewSelectedItemBehavior(obj as TreeView));
      //Debug.WriteLine($"TreeViewBehavior.SelectedItemChanged({e.NewValue})");
      TreeViewSelectedItemBehavior behavior = behaviors[obj];
      behavior.ChangeSelectedItem(e.NewValue);
    }

    private class TreeViewSelectedItemBehavior
    {
      private TreeView TreeView;

      public TreeViewSelectedItemBehavior(TreeView treeView)
      {
        this.TreeView = treeView;
        treeView.SelectedItemChanged += (sender, args) => SetSelectedItem(treeView, args.NewValue);

      }

      /// <summary>
      ///   Recursively search for an item in this subtree.
      /// </summary>
      /// <param name="container">
      ///   The parent ItemsControl. This can be a TreeView or a TreeViewItem.
      /// </param>
      /// <param name="obj">
      ///   The object to search item for.
      /// </param>
      /// <returns>
      ///   The TreeViewItem that DataContext is the specified object.
      /// </returns>
      internal TreeViewItem GetTreeViewItem(ItemsControl container, object obj)
      {
        if (container != null)
        {
          if (container.DataContext == obj)
          {
            return container as TreeViewItem;
          }

          // Expand the current container
          if (container is TreeViewItem && !((TreeViewItem)container).IsExpanded)
          {
            container.SetValue(TreeViewItem.IsExpandedProperty, true);
          }

          // Try to generate the ItemsPresenter and the ItemsPanel.
          // by calling ApplyTemplate.  Note that in the
          // virtualizing case even if the item is marked
          // expanded we still need to do this step in order to
          // regenerate the visuals because they may have been virtualized away.

          container.ApplyTemplate();
          ItemsPresenter itemsPresenter =
              (ItemsPresenter)container.Template.FindName("ItemsHost", container);
          if (itemsPresenter != null)
          {
            itemsPresenter.ApplyTemplate();
          }
          else
          {
            // The Tree template has not named the ItemsPresenter,
            // so walk the descendents and find the child.
            itemsPresenter = VisualTreeHelperExt.FindInVisualTreeDown<ItemsPresenter>(container);
            if (itemsPresenter == null)
            {
              container.UpdateLayout();

              itemsPresenter = VisualTreeHelperExt.FindInVisualTreeDown<ItemsPresenter>(container);
            }
          }

          Panel itemsHostPanel = (Panel)VisualTreeHelper.GetChild(itemsPresenter, 0);

          // Ensure that the generator for this panel has been created.
          UIElementCollection children = itemsHostPanel.Children;

          VirtualizingStackPanel virtualizingPanel =
              itemsHostPanel as VirtualizingStackPanel;

          for (int i = 0, count = container.Items.Count; i < count; i++)
          {
            TreeViewItem subContainer;
            if (virtualizingPanel != null)
            {
              // Bring the item into view so
              // that the container will be generated.
              // We must invoke virtualizingPanel.BringIndexIntoView(i)
              // through type reflection as this method is protected.
              var method = virtualizingPanel.GetType().GetMethod("BringIndexIntoView",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
              method.Invoke(virtualizingPanel, new Object[] { i });


              subContainer =
                  (TreeViewItem)container.ItemContainerGenerator.
                  ContainerFromIndex(i);
            }
            else
            {
              subContainer =
                  (TreeViewItem)container.ItemContainerGenerator.
                  ContainerFromIndex(i);

              // Bring the item into view to maintain the
              // same behavior as with a virtualizing panel.
              subContainer.BringIntoView();
            }

            if (subContainer != null)
            {
              // Search the next level for the object.
              TreeViewItem resultContainer = GetTreeViewItem(subContainer, obj);
              if (resultContainer != null)
              {
                return resultContainer;
              }
              else
              {
                // The object is not under this TreeViewItem
                // so collapse it.
                subContainer.IsExpanded = false;
              }
            }
          }
        }
        return null;
      }

      internal void ChangeSelectedItem(object obj)
      {
        //Debug.WriteLine($"TreeViewBehavior.ChangeSelectedItem({obj})");
        var item = this.GetTreeViewItem(this.TreeView, obj);
        if (item != null)
        {
          //Debug.WriteLine($"  selectedItem = {item}");
          item.IsSelected = true;
        }
      }
    }

  }
}
