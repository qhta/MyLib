namespace Qhta.WPF.Utils;

/// <summary>
/// Helper class for tree view.
/// </summary>
public static class TreeViewHelper
{

  /// <summary>
  /// Finds a tree view item as a container of the specific data.
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="data"></param>
  /// <returns></returns>
  public static TreeViewItem? ContainerFromItem(this TreeView treeView, object data)
  {
    //Debug.WriteLine($"TreeView.ContainerFromItem({data}) start");
    var mainItem = treeView.ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;
    if (mainItem != null)
    {
      //Debug.WriteLine($"TreeView.ContainerFromItem({data}) return {mainItem}");
      return mainItem;
    }
    foreach (var dataItem in treeView.Items)
    {
      //Debug.WriteLine($"TreeView.ContainerFromItem({data}) loop item = {dataItem}");
      //dataItem.GetType().GetProperty("IsExpanded")?.SetValue(dataItem, true);
      mainItem = treeView.ItemContainerGenerator.ContainerFromItem(dataItem) as TreeViewItem;
      var subItem = mainItem?.ContainerFromItem(data);
      if (subItem != null)
      {
        //Debug.WriteLine($"TreeView.ContainerFromItem({data}) return {subItem}");
        return subItem;
      }
    }
    //Debug.WriteLine($"TreeView.ContainerFromItem({data}) return null");
    return null;
  }

  /// <summary>
  /// Finds a tree view item as a container of the specific data starting from specified item.
  /// </summary>
  /// <param name="thisItem"></param>
  /// <param name="data"></param>
  /// <returns></returns>
  public static TreeViewItem? ContainerFromItem(this TreeViewItem thisItem, object data)
  {
    //Debug.WriteLine($"TreeViewItem.ContainerFromItem({thisItem}, {data}) start");
    if (thisItem.DataContext == data)
    {
      //Debug.WriteLine($"TreeViewItem.ContainerFromItem({thisItem}, {data}) return thisItem={thisItem}");
      return thisItem;
    }
    var mainItem = thisItem.ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;
    if (mainItem != null)
    {
      //Debug.WriteLine($"TreeViewItem.ContainerFromItem({thisItem}, {data}) return mainItem={mainItem}");
      return mainItem;
    }
    foreach (var dataItem in thisItem.Items)
    {
      //Debug.WriteLine($"TreeViewItem.ContainerFromItem({thisItem}, {data}) loop item = {dataItem}");
      //dataItem.GetType().GetProperty("IsExpanded")?.SetValue(dataItem, true);
      mainItem = thisItem.ItemContainerGenerator.ContainerFromItem(dataItem) as TreeViewItem;
      var subItem = mainItem?.ContainerFromItem(data);
      if (subItem != null)
      {
        //Debug.WriteLine($"TreeViewItem.ContainerFromItem({thisItem}, {data}) return {subItem}");
        return subItem;
      }
    }
    //Debug.WriteLine($"TreeViewItem.ContainerFromItem({thisItem}, {data}) return null");
    return null;
  }

  // ******************************************************************
  /// <summary>
  /// Delegate for TreeViewItem Visible event handler.
  /// </summary>
  /// <param name="tvi"></param>
  public delegate void OnTreeViewVisible(TreeViewItem tvi);

  /// <summary>
  /// Delegate for TreeViewItem expanded event handler.
  /// </summary>
  /// <param name="tvi"></param>
  /// <param name="item"></param>
  public delegate void OnItemExpanded(TreeViewItem tvi, object item);

  /// <summary>
  /// Delegate for AllItemsExpanded event.
  /// </summary>
  public delegate void OnAllItemExpanded();

  // ******************************************************************
  private static void SetItemHierarchyVisible(ItemContainerGenerator icg, IList listOfRootToNodeItemPath, OnTreeViewVisible? onTreeViewVisible = null)
  {
    Debug.Assert(icg != null);

    if (icg != null)
    {
      if (listOfRootToNodeItemPath.Count == 0) // nothing to do
        return;

      TreeViewItem? tvi = icg.ContainerFromItem(listOfRootToNodeItemPath[0]) as TreeViewItem;
      if (tvi != null) // Due to threading, always better to verify
      {
        listOfRootToNodeItemPath.RemoveAt(0);

        if (listOfRootToNodeItemPath.Count == 0)
        {
          if (onTreeViewVisible != null)
            onTreeViewVisible(tvi);
        }
        else
        {
          if (!tvi.IsExpanded)
            tvi.IsExpanded = true;

          SetItemHierarchyVisible(tvi.ItemContainerGenerator, listOfRootToNodeItemPath, onTreeViewVisible);
        }
      }
      else
      {
        ActionHolder actionHolder = new ActionHolder();
        EventHandler itemCreated = delegate (object? sender, EventArgs eventArgs)
        {
          if (sender is ItemContainerGenerator icgSender)
          {
            tvi = icgSender.ContainerFromItem(listOfRootToNodeItemPath[0]) as TreeViewItem;
            if (tvi != null) // Due to threading, it is always better to verify
            {
              SetItemHierarchyVisible(icg, listOfRootToNodeItemPath, onTreeViewVisible);

              actionHolder.Execute();
            }
          }
        };

        actionHolder.Action = new Action(() => icg.StatusChanged -= itemCreated);
        icg.StatusChanged += itemCreated;
        return;
      }
    }
  }

  // ******************************************************************
  /// <summary>
  /// You cannot rely on this method to be synchronous. If you have any action that depend on the TreeViewItem 
  /// (last item of collectionOfRootToNodePath) to be visible, you should set it in the 'onTreeViewItemVisible' method.
  /// This method should work for Virtualized and non virtualized tree.
  /// The difference with ExpandItem is that this one open up the tree up to the target but will not expand the target itself,
  /// while ExpandItem expand the target itself.
  /// </summary>
  /// <param name="treeView">TreeView where  an item has to be set visible</param>
  /// <param name="listOfRootToNodePath">Any collectionic List. The collection should have every objet of the path to the targeted item from the root
  /// to the target. For example for an apple tree: AppleTree (index 0), Branch4, SubBranch3, Leaf2 (index 3)</param>
  /// <param name="onTreeViewVisible">Optionnal</param>
  public static void SetItemHierarchyVisible(this TreeView treeView, IEnumerable<object> listOfRootToNodePath, OnTreeViewVisible? onTreeViewVisible = null)
  {
    ItemContainerGenerator icg = treeView.ItemContainerGenerator;
    if (icg == null)
      return; // Is tree loaded and initialized ???

    SetItemHierarchyVisible(icg, new List<object>(listOfRootToNodePath), onTreeViewVisible);
  }

  // ******************************************************************
  private static void ExpandItem(ItemContainerGenerator icg, IList listOfRootToNodePath, OnTreeViewVisible? onTreeViewVisible = null)
  {
    Debug.Assert(icg != null);

    if (icg != null)
    {
      if (listOfRootToNodePath.Count == 0) // nothing to do
        return;

      TreeViewItem? tvi = icg.ContainerFromItem(listOfRootToNodePath[0]) as TreeViewItem;
      if (tvi != null) // Due to threading, always better to verify
      {
        listOfRootToNodePath.RemoveAt(0);

        if (!tvi.IsExpanded)
          tvi.IsExpanded = true;

        if (listOfRootToNodePath.Count == 0)
        {
          if (onTreeViewVisible != null)
            onTreeViewVisible(tvi);
        }
        else
        {
          SetItemHierarchyVisible(tvi.ItemContainerGenerator, listOfRootToNodePath, onTreeViewVisible);
        }
      }
      else
      {
        ActionHolder actionHolder = new ActionHolder();
        EventHandler itemCreated = delegate (object? sender, EventArgs eventArgs)
        {
          if (sender is ItemContainerGenerator icgSender)
          {
            tvi = icgSender.ContainerFromItem(listOfRootToNodePath[0]) as TreeViewItem;
            if (tvi != null) // Due to threading, it is always better to verify
            {
              SetItemHierarchyVisible(icg, listOfRootToNodePath, onTreeViewVisible);

              actionHolder.Execute();
            }
          }
        };

        actionHolder.Action = new Action(() => icg.StatusChanged -= itemCreated);
        icg.StatusChanged += itemCreated;
        return;
      }
    }
  }

  // ******************************************************************
  /// <summary>
  /// You cannot rely on this method to be synchronous. If you have any action that depend on the TreeViewItem 
  /// (last item of collectionOfRootToNodePath) to be visible, you should set it in the 'onTreeViewItemVisible' method.
  /// This method should work for Virtualized and non virtualized tree.
  /// The difference with SetItemHierarchyVisible is that this one open the target while SetItemHierarchyVisible does not try to expand the target.
  /// (SetItemHierarchyVisible just ensure the target will be visible)
  /// </summary>
  /// <param name="treeView">TreeView where  an item has to be set visible</param>
  /// <param name="listOfRootToNodePath">The collection should have every objet of the path, from the root to the targeted item.
  /// For example for an apple tree: AppleTree (index 0), Branch4, SubBranch3, Leaf2</param>
  /// <param name="onTreeViewVisible">Optionnal</param>
  public static void ExpandItem(this TreeView treeView, IEnumerable<object> listOfRootToNodePath, OnTreeViewVisible? onTreeViewVisible = null)
  {
    ItemContainerGenerator icg = treeView.ItemContainerGenerator;
    if (icg == null)
      return; // Is tree loaded and initialized ???

    ExpandItem(icg, new List<object>(listOfRootToNodePath), onTreeViewVisible);
  }

  // ******************************************************************
  private static void ExpandSubWithContainersGenerated(ItemsControl ic, Action<TreeViewItem, object> actionItemExpanded, ReferenceCounterTracker referenceCounterTracker)
  {
    ItemContainerGenerator icg = ic.ItemContainerGenerator;
    foreach (object item in ic.Items)
    {
      if (icg.ContainerFromItem(item) is TreeViewItem tvi)
      {
        actionItemExpanded(tvi, item);
        tvi.IsExpanded = true;
        ExpandSubContainers(tvi, actionItemExpanded, referenceCounterTracker);
      }
    }
  }

  // ******************************************************************
  /// <summary>
  /// Expand any ItemsControl (TreeView, TreeViewItem, ListBox, ComboBox, ...) and their childs if any (TreeView)
  /// </summary>
  /// <param name="ic"></param>
  /// <param name="actionItemExpanded"></param>
  /// <param name="referenceCounterTracker"></param>
  public static void ExpandSubContainers(ItemsControl ic, Action<TreeViewItem, object> actionItemExpanded, ReferenceCounterTracker referenceCounterTracker)
  {
    ItemContainerGenerator icg = ic.ItemContainerGenerator;
    {
      if (icg.Status == GeneratorStatus.ContainersGenerated)
      {
        ExpandSubWithContainersGenerated(ic, actionItemExpanded, referenceCounterTracker);
      }
      else if (icg.Status == GeneratorStatus.NotStarted)
      {
        ActionHolder actionHolder = new ActionHolder();
        EventHandler itemCreated = delegate (object? sender, EventArgs eventArgs)
        {
          if (sender is ItemContainerGenerator icgSender)
          {
            if (icgSender.Status == GeneratorStatus.ContainersGenerated)
            {
              ExpandSubWithContainersGenerated(ic, actionItemExpanded, referenceCounterTracker);

              // Never use the following method in BeginInvoke due to ICG recycling. The same icg could be 
              // used and will keep more than one subscribers which is far from being intended
              //  ic.Dispatcher.BeginInvoke(actionHolder.Action, DispatcherPriority.Background);

              // Very important to unsubscribe as soon we've done due to ICG recycling.
              actionHolder.Execute();

              referenceCounterTracker.ReleaseRef();
            }
          }
        };

        referenceCounterTracker.AddRef();
        actionHolder.Action = new Action(() => icg.StatusChanged -= itemCreated);
        icg.StatusChanged += itemCreated;

        // Next block is only intended to protect against any race condition (I don't know if it is possible ? How Microsoft implemented it)
        // I mean the status changed before I subscribe to StatusChanged but after I made the check about its state.
        if (icg.Status == GeneratorStatus.ContainersGenerated)
        {
          ExpandSubWithContainersGenerated(ic, actionItemExpanded, referenceCounterTracker);
        }
      }
    }
  }

  // ******************************************************************
  /// <summary>
  /// This method is asynchronous.
  /// Expand all items and subs recursively if any. Does support virtualization (item recycling).
  /// But honestly, make you a favor, make your life easier en create a model view around your hierarchy with
  /// a IsExpanded property for each node level and bind it to each TreeView node level.
  /// </summary>
  /// <param name="treeView"></param>
  /// <param name="actionItemExpanded"></param>
  /// <param name="actionAllItemExpanded"></param>
  public static void ExpandAll(this TreeView treeView, Action<TreeViewItem, object>? actionItemExpanded = null, Action? actionAllItemExpanded = null)
  {
    //var referenceCounterTracker = new ReferenceCounterTracker(actionAllItemExpanded);
    //referenceCounterTracker.AddRef();
    //treeView.Dispatcher.BeginInvoke(new Action(() => ExpandSubContainers(treeView, actionItemExpanded, referenceCounterTracker)), DispatcherPriority.Background);
    //referenceCounterTracker.ReleaseRef();
  }

  /// <summary>
  /// Delegate to Count to zero action.
  /// </summary>
  public delegate void CountToZeroAction();

  /// <summary>
  /// Tracker for reference counter.
  /// </summary>
  public class ReferenceCounterTracker
  {
    private Action? _actionOnCountReachZero = null;
    private int _count = 0;

    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="actionOnCountReachZero"></param>
    public ReferenceCounterTracker(Action actionOnCountReachZero)
    {
      _actionOnCountReachZero = actionOnCountReachZero;
    }

    /// <summary>
    /// Increments reference counter.
    /// </summary>
    public void AddRef()
    {
      Interlocked.Increment(ref _count);
    }

    /// <summary>
    /// Decrements reference counter.
    /// </summary>
    public void ReleaseRef()
    {
      int count = Interlocked.Decrement(ref _count);
      if (count == 0)
      {
        if (_actionOnCountReachZero != null)
        {
          _actionOnCountReachZero();
        }
      }
    }
  }
}
